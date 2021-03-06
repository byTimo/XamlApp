﻿using System;
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
        private static readonly WebSocket WebSocket;

        /// <summary>
        /// Переменная, которая не даёт закрывать уже закрытый сокет, который пытается открыться.
        /// </summary>
        private static bool socketAlreadyClosed;

        static AppWebSocketEventManager()
        {
            WebSocket = new WebSocket(App.WebSocketUrl);
            WebSocket.Opened += OpenedConnection;
            WebSocket.Closed += ClosedConnection;
            WebSocket.Error += SocketErrorEvent;
            WebSocket.MessageReceived += MessageReceivedEvent;
        }

        private static void OpenedConnection(object sender, EventArgs e)
        {
            socketAlreadyClosed = false;
            App.ConnectionStatus = ConnectionStatus.Connect;
            Application.Current.Dispatcher.Invoke(new Action(() => AppEventManager.Connection(WebSocket)));
            string token;
            try
            {
                token = Application.Current.Dispatcher.Invoke(new Func<string>(() => FileDispetcher.GetSetting("token"))) as string;
            }
            catch
            {
                token = null;
            }
            if (token == null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => AppEventManager.AuthorizationFailEvent(WebSocket, AuthorizationType.Token)));
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
            Application.Current.Dispatcher.Invoke(new Action(() => AppEventManager.Disconnection(WebSocket)));

        }

        public static void SendObject(string jsonString)
        {
            WebSocket.Send(jsonString);
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
                    case "request/answer":
                        HandlingAnswerRequest(responseJson);
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
                WebSocket.Close();
        }

        public static void OpenWebSocket()
        {
            try
            {
                if (WebSocket.State == WebSocketState.Closed || WebSocket.State == WebSocketState.None)
                    WebSocket.Open();
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
                    Application.Current.Dispatcher.Invoke(new Action<object, AuthorizationType>(AppEventManager.AuthorizationSuccessEvent), WebSocket, AuthorizationType.Token);
                    break;
                case "fail":
                    try
                    {
                        Application.Current.Dispatcher.Invoke(new Action<string>(FileDispetcher.DeleteSetting), "token");
                    }
                    finally 
                    {
                        Application.Current.Dispatcher.Invoke(new Action<object, AuthorizationType>(AppEventManager.AuthorizationFailEvent), WebSocket, AuthorizationType.Token);
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
                        Application.Current.Dispatcher.Invoke(new Func<bool>(() => FileDispetcher.SaveSettings("token", (string)json["token"])));
                    Application.Current.Dispatcher.Invoke(new Action<object, AuthorizationType>(AppEventManager.AuthorizationSuccessEvent), WebSocket, AuthorizationType.Login);
                    break;
                case "fail":
                    Application.Current.Dispatcher.Invoke(new Action<object, AuthorizationType>(AppEventManager.AuthorizationFailEvent), WebSocket, AuthorizationType.Login);
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
            {
                App.LastLogId = ulong.Parse((string) responseJson["log_id"]);
                Application.Current.Dispatcher.Invoke(
                    new Action(() => FileDispetcher.SaveSettings("logId", App.LastLogId.ToString())));
            }
            var request = responseJson["request"];
            var status = (string)request["status"] == "created"
               ? DialogueStatus.Created
               : (string)request["status"] == "answered" ? DialogueStatus.Answered : DialogueStatus.Missed;
            var roomId = long.Parse((string)request["chat_room_id"]);
            var lastupdata = (string)request["updated_at"];
            var queryId = long.Parse((string)request["id"]);
            var query = (string)request["query"];
            var carId = long.Parse((string)request["car_id"] ?? "0");
            var dialogue = new Dialogue(roomId, query, queryId, lastupdata, carId, status);
            DialogueStore.AddDialogue(dialogue);
            Application.Current.Dispatcher.Invoke(new Action(() => AppEventManager.ReceiveQueryEvent(WebSocket, dialogue)));
            if(carId == 0) return;
            var carInfoRequest = new CarInfoRequest {request_id = queryId};
            var carInfoRequestToJson = JsonConvert.SerializeObject(carInfoRequest);
            SendObject(carInfoRequestToJson);
        }

        private static void HandlingChatMessage(JObject responseJson)
        {
            var responceType = (string) responseJson["type"];
            switch (responceType)
            {
                case "receive":
                    var roomId = long.Parse((string) responseJson["room_id"]);
                    var logId = (string) responseJson["log_id"];
                    if (logId != null)
                    {
                        App.LastLogId = ulong.Parse((string)responseJson["log_id"]);
                        Application.Current.Dispatcher.Invoke(
                            new Action(() => FileDispetcher.SaveSettings("logId", App.LastLogId.ToString())));
                    }
                    var mes = responseJson["message"];
                    var mesId = long.Parse((string) mes["id"]);
                    var hash = (string) mes["hash"];
                    var type = (string) mes["type"];
                    var text = (string) mes["message"];
                    var author = (string) mes["user_name"];
                    var lastUpdata = (string) mes["created_at"];
                    var isUnread = (string)mes["unread"] == "1";
                    var message = new Message(mesId, text, type, hash, lastUpdata, author, isUnread, roomId);
                    var dialogue = DialogueStore.GetDialogueOnRoomId(roomId);
                    if (dialogue != null)
                    {
                        dialogue.AddMessage(message);
                        DialogueStore.SaveChanges();
                    }
                    else
                    {
                        dialogue = new Dialogue(roomId , message);
                        DialogueStore.AddDialogue(dialogue);
                    }
                    Application.Current.Dispatcher.Invoke(
                        new Action(() => AppEventManager.ReceiveMessageEvent(WebSocket, dialogue)));
                    break;
                case "send":
                    var roomIdSend = long.Parse((string) responseJson["room_id"]);
                    var hashSend = (string) responseJson["hash"];
                    var mesIdSend = long.Parse((string) responseJson["id"]);
                    Application.Current.Dispatcher.Invoke(
                        new Action(() => AppEventManager.SendMessageSuccessEvent(roomIdSend, mesIdSend, hashSend)));
                    break;
            }
        }

        private static void HandlingPongResponce(JObject responseJson)
        {
            return;
        }

        private static void HndlingAuditResponce(JObject responseJson)
        {
            ulong logId = 0;
            if ((string) responseJson["status"] == "ok")
                logId = ulong.Parse((string) responseJson["log_id"]);
            if (logId > App.LastLogId)
            {
                App.LastLogId = logId;
                Application.Current.Dispatcher.Invoke(
                    new Action(() => FileDispetcher.SaveSettings("logId", App.LastLogId.ToString())));
            }
        }

        private static void HandlingChatHistory(JObject responseJson)
        {
            if (responseJson["messages"] == null) return;
            var roomIdReceive = long.Parse((string) responseJson["room_id"]);
            var messages = (from message in responseJson["messages"]
                let mesId = long.Parse((string) message["id"])
                let text = (string) message["message"]
                let hashReceive = (string) message["hash"]
                let createTime = (string) message["created_at"]
                let author = (string) message["user_name"]
                let type = (string) message["type"]
                select new Message(mesId, text, type, hashReceive, createTime, author, roomIdReceive))
                .ToList();
            Application.Current.Dispatcher.Invoke(new Action(() => AppEventManager.OpenDialogueEvent(roomIdReceive, messages)));
        }

        private static void HandkingCarInfoRequest(JObject responseJson)
        {
            if((string)responseJson["status"] != "ok") return;
            var car = responseJson["car"];
            var carId = long.Parse((string)car["id"]);
            var brand = (string) car["manufacturer"];
            var model = (string) car["model"];
            var vin = (string) car["vin"];
            var year = (string) car["year"];
            foreach(var dialogue in DialogueStore.Instance.Dialogues.Where(d => d.CarId == carId))
            {
                dialogue.SetCarInformation(brand, model, vin, year);
                DialogueStore.SaveChanges();
            }
            Application.Current.Dispatcher.Invoke(new Action(() => AppEventManager.SetCarInfoEvent(carId, brand, model, vin, year)));
        }

        private static void HandlingAnswerRequest(JObject responseJson)
        {
            if ((string)responseJson["status"] != "ok") return;
            var request = responseJson["request"];
            var id = long.Parse((string)request["chat_room_id"]);
            Application.Current.Dispatcher.Invoke(new Action(() => AppEventManager.AnswerOnQueryEvent(id)));
        }

        private static void HandlingListResponce(JObject responseJson)
        {
            //@TODO - В этом нет необходимости, возмжно, пока
        }
    }
}
