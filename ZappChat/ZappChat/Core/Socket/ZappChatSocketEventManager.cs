using System;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace ZappChat.Core.Socket
{
    static class ZappChatSocketEventManager
    {
        private static readonly WebSocket _webSocket;


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
                dynamic responseJson = JsonConvert.DeserializeObject(e.Message);
                if(responseJson.action == null)
                    throw new Exception("Неверно пришёл Json");
                switch ((string)responseJson.action.Value)
                {
                    case "client/auth":
                        var status = responseJson.status.Value == "ok"
                            ?  AuthorizationStatus.Ok 
                            : responseJson.status.Value == "fail"
                                ? AuthorizationStatus.Fail
                                : AuthorizationStatus.Error;
                        AppEventManager.AuthorizationEvent(_webSocket, status);
                        break;
                        //@TODO
                }
            }
            catch (Exception exception)
            {
                //@TODO
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
    }
}
