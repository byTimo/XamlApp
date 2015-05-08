using System;

namespace ZappChat.Core
{
    static class AppEventManager
    {
        public static event ConnectionEventHandler Connection;
        public static void ConnectionEvent(object sender, ConnectionStatus status)
        {
            Connection(sender, new ConnectionEventArgs(status));
        }

        public static event AuthorizationEventHandler Authorization;

        public static void AuthorizationEvent(object sender, string json)
        {
            Authorization(sender, new WebSocketEventArgs(json));
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
        //TODO SendMessageEvent and SendMessageEventHadler
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
