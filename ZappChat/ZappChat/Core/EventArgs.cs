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

    public class MessagingEventArgs : EventArgs
    {
        public readonly Message Message;

        public MessagingEventArgs(Message message)
        {
            Message = message;
        }
    }

    public class DeletingEventArgs : EventArgs
    {
        public readonly Dialogue DeletedDialogue;
        public readonly bool IsConfirmed;

        public DeletingEventArgs(Dialogue deleteDialogue, bool isConfirmed)
        {
            DeletedDialogue = deleteDialogue;
            IsConfirmed = isConfirmed;
        }
    }
}
