using System;
using System.Collections.Generic;

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

        public static event Action UpdateCounter;

        public static void UpdateCounterEvent()
        {
            if(UpdateCounter != null)
                UpdateCounter.Invoke();
        }
//        public static event OpenDialogueEventHandler OpenDialogue;
//
//        public static void OpenDialogueEvent(object sender, Dialogue dialogue)
//        {
//            OpenDialogue(sender, new DialogueOpenEventArgs(dialogue));
//        }
        public static event Action<ulong, List<Message>> OpenDialogue;

        public static void OpenDialogueEvent(ulong roomId, List<Message> messages)
        {
            if (OpenDialogue != null)
                OpenDialogue.Invoke(roomId, messages);
        }

        public static event CloseDialogueEventHandler CloseDialogue;

        public static void CloseDialogueEvent()
        {
            CloseDialogue();
        }

    }
}
