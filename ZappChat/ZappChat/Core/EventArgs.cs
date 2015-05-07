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

    public class TakeQueryEventArgs : EventArgs
    {
        public readonly int DialogueId;
        public readonly string Interlocutor;
        public readonly string Query;
        public readonly DateTime Time;

        public TakeQueryEventArgs(int id, string interlocutor, string query, DateTime time)
        {
            DialogueId = id;
            Interlocutor = interlocutor;
            Query = query;
            Time = time;
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

    public class DialogueOpenEventArgs : EventArgs
    {
        public readonly Dialogue OpenedDialogue;

        public DialogueOpenEventArgs(Dialogue dialogue)
        {
            OpenedDialogue = dialogue;
        }
    }

    public class AuthorizationEventArgs : EventArgs
    {
        public string JsonObject;

        public AuthorizationEventArgs(string jsonObject)
        {
            JsonObject = jsonObject;

        }
    }

    public class SwitchWindowEventArgs : EventArgs
    {
        public readonly string WindowName;

        public SwitchWindowEventArgs(string windowName)
        {
            WindowName = windowName;
        }
    }
}
