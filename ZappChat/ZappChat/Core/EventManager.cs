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

        public static event TakeNewDialogueEventHandler TakeNewDialgoue;

        public static void TakeNewDialogueEvent(object sender, Dialogue dialogue)
        {
            TakeNewDialgoue.Invoke(sender,new TakeNewDialogueEventArgs(dialogue));
        }

        public static event MessagingEventHandler TakeMessage;

        public static void TakeMessageEvent(object sender,int dialogueId, Message message)
        {
            TakeMessage(sender, new MessagingEventArgs(dialogueId, message));
        }

        public static event TakeQueryEventHandler TakeQuery;

        public static void TakeQueryEvent(object sender, int dialogueId, string interlocutor, string query, DateTime time)
        {
            TakeQuery(sender, new TakeQueryEventArgs(dialogueId, interlocutor, query, time));
        }

        public static event MessagingEventHandler SendMessage;

        public static void SendMessageEvent(object sender, int dialogueId, Message message)
        {
            SendMessage(sender, new MessagingEventArgs(dialogueId, message));
        }

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

        public static event SwitchWindowEventHandler SwitchWindows;

        public static void SwitchWindowEvent(object sender, string windwoName)
        {
            SwitchWindows(sender, new SwitchWindowEventArgs(windwoName));
        }
    }
}
