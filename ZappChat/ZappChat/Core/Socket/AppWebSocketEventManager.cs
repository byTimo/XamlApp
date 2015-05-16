using System;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
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
            CrossThreadOperationWithoutParams(Application.Current.Dispatcher,
                () => AppEventManager.Disconnection(_webSocket));

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

        private static void HandlingPongResponce(JObject responseJson)
        {
            throw new NotImplementedException();
        }

        private static void HandlingListResponce(JObject responseJson)
        {
            if((string)responseJson["status"] != "ok") throw new Exception((string)responseJson["reason"]);
            foreach (var dialogue in responseJson["list"])
            {
                var id = int.Parse((string)dialogue["id"]);
                var query = (string)dialogue["query"];
                var newDialog = new Dialogue(id, query);
                var action = new Action<object, Dialogue>(AppEventManager.TakeNewDialogueEvent);
                MainWindow.Dispatcher.Invoke(action, _webSocket, newDialog);
            }

        }

        private static void CrossThreadOperationWithoutParams(Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.Invoke(action);
        }

        private static void CrossThreadOperationWithOneString(Dispatcher dispatcher, Action<string> action, string firstParam)
        {
            if (dispatcher.CheckAccess())
                action(firstParam);
            else
                dispatcher.Invoke(action, firstParam);
        }
        private static void CrossThreadOperationWithEventArrgs(Dispatcher dispatcher, Action<object,string> action,object sender, string firstParam)
        {
            if (dispatcher.CheckAccess())
                action(sender,firstParam);
            else
                dispatcher.Invoke(action, sender, firstParam);
        }
        
    }
}
