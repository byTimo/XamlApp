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
        public readonly int DialogueId;
        public readonly Message Message;

        public MessagingEventArgs(int id, Message message)
        {
            DialogueId = id;
            Message = message;
        }
    }

    public class SentQueryEventArgs : EventArgs
    {
        public readonly int DialogueId;
        public readonly string Query;

        public SentQueryEventArgs(int id, string query)
        {
            DialogueId = id;
            Query = query;
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
