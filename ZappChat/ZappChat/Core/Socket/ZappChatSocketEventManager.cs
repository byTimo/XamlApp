using System;
using System.Windows.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace ZappChat.Core.Socket
{
    static class ZappChatSocketEventManager
    {
        private static readonly WebSocket _webSocket;
        public static LoginWindow Login { get; set; }
        public static MainWindow MainWindow { get; set; }


        static ZappChatSocketEventManager()
        {
            _webSocket = new WebSocket("ws://zappchat.ru:7778");
            _webSocket.Opened += OpenedConnection;
            _webSocket.Closed += ClosedConnection;
            _webSocket.Error += SocketErrorEvent;
            _webSocket.MessageReceived += MessageReceivedEvent;
            _webSocket.AutoSendPingInterval = 120;
        }
        private static void MessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var responseJson = (JObject)JsonConvert.DeserializeObject(e.Message);
                if(responseJson["action"] == null)
                    throw new Exception("Неверно пришёл Json");
                switch ((string)responseJson["action"])
                {
                    case "client/auth":
                        AuthorizationEvent(e.Message);
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
            throw e.Exception;
        }

        private static void ClosedConnection(object sender, EventArgs e)
        {
            _webSocket.Close();
            AppEventManager.ConnectionEvent(_webSocket, AppStatus.Disconnect);
        }

        private static void OpenedConnection(object sender, EventArgs e)
        {
            //AppEventManager.ConnectionEvent(_webSocket, AppStatus.Connect);
        }

        public static bool OpenWebSocket()
        {
            if (_webSocket.State == WebSocketState.Closed || _webSocket.State == WebSocketState.None)
                _webSocket.Open();
            while (_webSocket.State == WebSocketState.Connecting)
            {
            }
            return _webSocket.State == WebSocketState.Open;
        }

        public static bool SendObject(string jsonString)
        {
            _webSocket.Send(jsonString);
            return _webSocket.State == WebSocketState.Open;
        }

        private static void AuthorizationEvent(string json)
        {
            AuthorizationStatus status = AuthorizationStatus.Error;
            var responceJson = (JObject) JsonConvert.DeserializeObject(json);

            switch ((string)responceJson.GetValue("status"))
            {
                case "ok":
                    status = AuthorizationStatus.Ok;
                    break;
                case "fail":
                    status = AuthorizationStatus.Fail;
                    break;
                case "error":
                    status = AuthorizationStatus.Error;
                    break;
            }
            Login.AuthorizationResult(status);
        }
    }
}
