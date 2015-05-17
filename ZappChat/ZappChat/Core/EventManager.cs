using System;

namespace ZappChat.Core
{
    static class AppEventManager
    {
        public static event Action<object> Connect;

        public static void Connection(object sender)
        {
            Connect.Invoke(sender);
        }

        public static event Action<object> Disconnect;

        public static void Disconnection(object sender)
        {
            Disconnect.Invoke(sender);
        }

        public static event AuthorizationEventHandler AuthorizationSuccess;

        public static void AuthorizationSuccessEvent(object sender, AuthorizationType type)
        {
            AuthorizationSuccess.Invoke(sender, type);
        }
        public static event AuthorizationEventHandler AuthorizationFail;

        public static void AuthorizationFailEvent(object sender, AuthorizationType type)
        {
            AuthorizationFail.Invoke(sender, type);
        }

        public static event ReceivingDataEventHandler ReceiveMessage;

        public static void ReceiveMessageEvent(object sender, Dialogue dialogue)
        {
            ReceiveMessage.Invoke(sender, dialogue);
        }

        public static event ReceivingDataEventHandler ReceiveQuery;

        public static void ReceiveQueryEvent(object sender, Dialogue dialogue)
        {
            ReceiveQuery.Invoke(sender, dialogue);
        }

        //public static event MessagingEventHandler SendMessage;

        //public static void SendMessageEvent(object sender, int dialogueId, Message message)
        //{
        //    SendMessage(sender, new MessagingEventArgs(dialogueId, message));
        //}

        public static event DeletingDialogueEventHandler DeleteConfirmationDialogue;

        public static void DeleteConfirmationDialogueEvent(object sender, Dialogue dialogue, bool isConfirmed)
        {
            DeleteConfirmationDialogue(sender, new DeletingEventArgs(dialogue, isConfirmed));
        }

        public static event DeletingDialogueEventHandler DeleteDialogue;

        public static void DeleteDialogueEvent(object sender, Dialogue dialogue, bool isConfirmed)
        {
            DeleteDialogue(sender, new DeletingEventArgs(dialogue, isConfirmed));
        }

        public static event OpenDialogueEventHandler OpenDialogue;

        public static void OpenDialogueEvent(object sender, Dialogue dialogue)
        {
            OpenDialogue(sender, new DialogueOpenEventArgs(dialogue));
        }

        public static event CloseDialogueEventHandler CloseDialogue;

        public static void CloseDialogueEvent()
        {
            CloseDialogue();
        }

    }
}
