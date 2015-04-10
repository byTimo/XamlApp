using System;

namespace ZappChat.Core
{
    public class ConnectionEventArgs : EventArgs
    {
        public readonly AppStatus ConnectionStatus;

        public ConnectionEventArgs(AppStatus connectionStatus)
        {
            ConnectionStatus = connectionStatus;
        }
    }

    public class SendMessageEventArgs : EventArgs
    {
        public readonly Message Message;

        public SendMessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}
