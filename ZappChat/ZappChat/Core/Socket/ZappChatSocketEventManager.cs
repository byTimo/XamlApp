using System;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace ZappChat.Core.Socket
{
    
    class ZappChatSocketEventManager
    {
        private WebSocket _webSocket;

        public ZappChatSocketEventManager()
        {
            _webSocket = new WebSocket("ws://ws.zappchat.ru/");
            _webSocket.Opened += OpenedConnection;
            _webSocket.Closed += ClosedConnection;
            _webSocket.Error += SocketErrorEvent;
            _webSocket.MessageReceived += MessageReceivedEvent;
            _webSocket.AutoSendPingInterval = 120;
        }

        private void MessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SocketErrorEvent(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ClosedConnection(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenedConnection(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
