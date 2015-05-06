using System;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace ZappChat.Core.Socket
{
    static class ZappChatSocketEventManager
    {
        private static readonly WebSocket _webSocket;
        private static string _email;
        private static string _password;


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
            throw new NotImplementedException();
        }

        private static void SocketErrorEvent(object sender, ErrorEventArgs e)
        {
            throw new Exception("Ошибка инициализации класса подкючения.");
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
    }
}
