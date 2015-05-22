using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using WebSocket4Net;
using ZappChat.Core.Socket.Requests;

namespace ZappChat.Core.Socket
{
    static class AppWebSocketEventManager
    {
        private static readonly WebSocket _webSocket;
        public static LoginWindow Login { get; set; }
        public static MainWindow MainWindow { get; set; }

        /// <summary>
        /// Переменная, которая не даёт закрывать уже закрытый сокет, который пытается открыться.
        /// </summary>
        private static bool socketAlreadyClosed;

        static AppWebSocketEventManager()
        {
            _webSocket = new WebSocket("ws://zappchat.ru:7778");
            _webSocket.Opened += OpenedConnection;
            _webSocket.Closed += ClosedConnection;
            _webSocket.Error += SocketErrorEvent;
            _webSocket.MessageReceived += MessageReceivedEvent;
        }

        private static void OpenedConnection(object sender, EventArgs e)
        {
            socketAlreadyClosed = false;
            App.ConnectionStatus = ConnectionStatus.Connect;
            Application.Current.Dispatcher.Invoke(() => AppEventManager.Connection(_webSocket));
            string token;
            try
            {
                token = Application.Current.Dispatcher.Invoke<string>(FileDispetcher.GetToken);
            }
            catch
            {
                token = null;
            }
            if (token == null)
            {
                Application.Current.Dispatcher.Invoke(() => AppEventManager.AuthorizationFailEvent(_webSocket, AuthorizationType.Token));
                return;
            }
            var requestToken = new AuthorizationTokenRequest
            {
                token = token
            };
            var requestTokenToJson = JsonConvert.SerializeObject(requestToken);
            SendObject(requestTokenToJson);
        }

        private static void ClosedConnection(object sender, EventArgs e)
        {
            socketAlreadyClosed = true;
            App.ConnectionStatus = ConnectionStatus.Disconnect;
            Application.Current.Dispatcher.Invoke(() => AppEventManager.Disconnection(_webSocket));

        }

        public static void SendObject(string jsonString)
        {
            _webSocket.Send(jsonString);
        }

        private static void MessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var responseJson = (JObject)JsonConvert.DeserializeObject(e.Message);
                if(responseJson["action"] == null)
                    throw new Exception("JSON object don't containt action field!");
                switch ((string)responseJson["action"])
                {
                    case "client/auth":
                        HandlingAuthorizationResponce(responseJson);
                        break;
                    case "client/token":
                        HandlingTokenResponce(responseJson);
                        break;
                    case "client/pong":
                        HandlingPongResponce(responseJson);
                        break;
                    case "request/list":
                        HandlingListResponce(responseJson);
                        break;
                    case "client/audit":
                        HndlingAuditResponce(responseJson);
                        break;
                    case "chat/message":
                        HandlingChatMessage(responseJson);
                        break;
                    case "request/new":
                        HandlingNewRequest(responseJson);
                        break;
                    case "chat/history":
                        HandlingChatHistory(responseJson);
                        break;
                    case "request/car":
                        HandkingCarInfoRequest(responseJson);
                        break;
                        //@TODO
                }
            }
            catch (Exception exception)
            {
                //@TODO
                throw exception;
            }
        }

        private static void SocketErrorEvent(object sender, ErrorEventArgs e)
        {
            if(!socketAlreadyClosed)
                _webSocket.Close();
        }

        public static void OpenWebSocket()
        {
            try
            {
                if (_webSocket.State == WebSocketState.Closed || _webSocket.State == WebSocketState.None)
                    _webSocket.Open();
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void HandlingTokenResponce(JObject json)
        {
            switch ((string)json["status"])
            {
                case "ok":
                    Application.Current.Dispatcher.Invoke(new Action<object, AuthorizationType>(AppEventManager.AuthorizationSuccessEvent), _webSocket, AuthorizationType.Token);
                    break;
                case "fail":
                    try
                    {
                        Application.Current.Dispatcher.Invoke(new Action<string>(FileDispetcher.DeleteSetting), "token");
                    }
                    finally 
                    {
                        Application.Current.Dispatcher.Invoke(new Action<object, AuthorizationType>(AppEventManager.AuthorizationFailEvent), _webSocket, AuthorizationType.Token);
                    }
                    break;
                case "error":
                    break;
                default:
                    throw new Exception("Ошибка на сервере! Обратитесь в службу поддержки.");
            }
        }

        private static void HandlingAuthorizationResponce(JObject json)
        {
            switch ((string)json["status"])
            {
                case "ok":
                    if ((string)json["token"] != null)
                        Application.Current.Dispatcher.Invoke(() => FileDispetcher.SaveSettings("token", (string)json["token"]));
                    Application.Current.Dispatcher.Invoke(new Action<object, AuthorizationType>(AppEventManager.AuthorizationSuccessEvent), _webSocket, AuthorizationType.Login);
                    break;
                case "fail":
                    Application.Current.Dispatcher.Invoke(new Action<object, AuthorizationType>(AppEventManager.AuthorizationFailEvent), _webSocket, AuthorizationType.Login);
                    break;
                case "error":
                    break;
                default:
                    throw new Exception("Ошибка на сервере! Обратитесь в службу поддержки.");
            }
        }

        private static void HandlingNewRequest(JObject responseJson)
        {
            if ((string) responseJson["log_id"] != null)
                App.LastLogId = ulong.Parse((string) responseJson["log_id"]);
            var request = responseJson["request"];
            var roomId = ulong.Parse((string)request["chat_room_id"]);
            var lastupdata = (string)request["updated_at"];
            var queryId = ulong.Parse((string)request["id"]);
            var query = (string)request["query"];
            var carId = ulong.Parse((string)request["car_id"] ?? "0");
            var dialogue = new Dialogue(roomId, query, queryId, lastupdata, carId);
            Application.Current.Dispatcher.Invoke(() => AppEventManager.ReceiveQueryEvent(_webSocket, dialogue));
            if(carId == 0) return;
            var carInfoRequest = new CarInfoRequest {request_id = queryId};
            var carInfoRequestToJson = JsonConvert.SerializeObject(carInfoRequest);
            SendObject(carInfoRequestToJson);
        }

        private static void HandlingPongResponce(JObject responseJson)
        {
            return;
        }

        private static void HndlingAuditResponce(JObject responseJson)
        {
            if ((string) responseJson["status"] == "ok")
                App.LastLogId = ulong.Parse((string) responseJson["log_id"]);
        }

        private static void HandlingChatMessage(JObject responseJson)
        {
            var responceType = (string) responseJson["type"];
            switch (responceType)
            {
                case "receive":
                    var roomId = ulong.Parse((string) responseJson["room_id"]);
                    var logId = (string) responseJson["log_id"];
                    if (logId != null)
                        App.LastLogId = ulong.Parse(logId);
                    var mes = responseJson["message"];
                    var mesId = ulong.Parse((string) mes["id"]);
                    var hash = (string) mes["hash"];
                    var type = (string) mes["type"];
                    var text = (string) mes["message"];
                    var author = (string) mes["user_name"];
                    var lastUpdata = (string) mes["created_at"];
                    var message = new Message(mesId, text, type, hash, lastUpdata, author);
                    var dialogue = new Dialogue(roomId, message);
                    Application.Current.Dispatcher.Invoke(
                        () => AppEventManager.ReceiveMessageEvent(_webSocket, dialogue));
                    break;
                case "send":
                    var roomIdSend = ulong.Parse((string) responseJson["room_id"]);
                    var hashSend = (string) responseJson["hash"];
                    var mesIdSend = ulong.Parse((string) responseJson["id"]);
                    Application.Current.Dispatcher.Invoke(
                        () => AppEventManager.SendMessageSuccessEvent(roomIdSend, mesIdSend, hashSend));
                    break;
            }
        }

        private static void HandlingChatHistory(JObject responseJson)
        {
            if (responseJson["messages"] == null) return;
            var roomIdReceive = ulong.Parse((string) responseJson["room_id"]);
            var messages = (from message in responseJson["messages"]
                let mesId = ulong.Parse((string) message["id"])
                let text = (string) message["message"]
                let hashReceive = (string) message["hash"]
                let createTime = (string) message["created_at"]
                let author = (string) message["user_name"]
                let type = (string) message["type"]
                select new Message(mesId, text, type, hashReceive, createTime, author) {Status = MessageStatus.Read})
                .ToList();
            Application.Current.Dispatcher.Invoke(() => AppEventManager.OpenDialogueEvent(roomIdReceive, messages));
        }

        private static void HandkingCarInfoRequest(JObject responseJson)
        {
            if((string)responseJson["status"] != "ok") return;
            var car = responseJson["car"];
            var carId = ulong.Parse((string)car["id"]);
            var brand = (string) car["manufacturer"];
            var model = (string) car["model"];
            var vin = (string) car["vin"];
            var year = (string) car["year"];
            Application.Current.Dispatcher.Invoke(() => AppEventManager.SetCarInfoEvent(carId, brand, model, vin, year));
        }

        private static void HandlingListResponce(JObject responseJson)
        {
            //@TODO - В этом нет необходимости, возмжно, пока

            //if((string)responseJson["status"] != "ok") throw new Exception((string)responseJson["reason"]);
            //foreach (var dialogue in responseJson["list"])
            //{
            //    var id = int.Parse((string)dialogue["id"]);
            //    var query = (string)dialogue["query"];
            //    var newDialog = new Dialogue(id, query);
            //    var action = new Action<object, Dialogue>(AppEventManager.TakeNewDialogueEvent);
            //    MainWindow.Dispatcher.Invoke(action, _webSocket, newDialog);
            //}

        }
    }
}
