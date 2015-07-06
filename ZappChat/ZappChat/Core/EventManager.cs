using System;
using System.Collections.Generic;

namespace ZappChat.Core
{
    static class AppEventManager
    {
        public static event Action<object> Connect;

        public static void Connection(object sender)
        {
            if (Connect != null) Connect.Invoke(sender);
        }

        public static event Action<object> Disconnect;

        public static void Disconnection(object sender)
        {
            if (Disconnect != null) Disconnect.Invoke(sender);
        }

        public static event AuthorizationEventHandler AuthorizationSuccess;

        public static void AuthorizationSuccessEvent(object sender, AuthorizationType type)
        {
            if (AuthorizationSuccess != null) AuthorizationSuccess.Invoke(sender, type);
        }

        public static event AuthorizationEventHandler AuthorizationFail;

        public static void AuthorizationFailEvent(object sender, AuthorizationType type)
        {
            if (AuthorizationFail != null) AuthorizationFail.Invoke(sender, type);
        }

        public static event ReceivingDataEventHandler ReceiveMessage;

        public static void ReceiveMessageEvent(object sender, Dialogue dialogue)
        {
            if (ReceiveMessage != null) ReceiveMessage.Invoke(sender, dialogue);
        }

        public static event ReceivingDataEventHandler ReceiveQuery;

        public static void ReceiveQueryEvent(object sender, Dialogue dialogue)
        {
            if (ReceiveQuery != null) ReceiveQuery.Invoke(sender, dialogue);
        }

        public static event Action<long, long, string> SendMessageSuccess;

        public static void SendMessageSuccessEvent(long roomId, long id, string hash)
        {
            if (SendMessageSuccess != null)
                SendMessageSuccess.Invoke(roomId, id, hash);
        }

        public static event DeletingDialogueEventHandler DeleteConfirmationDialogue;

        public static void DeleteConfirmationDialogueEvent(object sender, Dialogue dialogue, bool isConfirmed)
        {
            if (DeleteConfirmationDialogue != null)
                DeleteConfirmationDialogue(sender, new DeletingEventArgs(dialogue, isConfirmed));
        }

        public static event DeletingDialogueEventHandler DeleteDialogue;

        public static void DeleteDialogueEvent(object sender, Dialogue dialogue, bool isConfirmed)
        {
            if (DeleteDialogue != null) DeleteDialogue(sender, new DeletingEventArgs(dialogue, isConfirmed));
        }

        public static event Action UpdateCounter;

        public static void UpdateCounterEvent()
        {
            if(UpdateCounter != null)
                UpdateCounter.Invoke();
        }

        public static event Action<long, string, string> PreopenDialogue;

        public static void PreopenDialogueEvent(long roomId, string from, string to)
        {
            if (PreopenDialogue != null)
                PreopenDialogue.Invoke(roomId, from, to);
        }

        public static event Action<long, List<Message>> OpenDialogue;

        public static void OpenDialogueEvent(long roomId, List<Message> messages)
        {
            if (OpenDialogue != null)
                OpenDialogue.Invoke(roomId, messages);
        }

        public static event Action AfteropenDialogue;

        public static void AfteropenDialogueEvent()
        {
            if(AfteropenDialogue != null)
                AfteropenDialogue.Invoke();
        }

        public static event CloseDialogueEventHandler CloseDialogue;

        public static void CloseDialogueEvent()
        {
            if (CloseDialogue != null) CloseDialogue();
        }

        public static event Action<long, string, string, string, string> SetCarInfo;

        public static void SetCarInfoEvent(long id, string brand, string model, string vin, string year)
        {
            if (SetCarInfo != null) SetCarInfo.Invoke(id, brand, model, vin, year);
        }

        public static event Action<long> AnswerOnQuery;

        public static void AnswerOnQueryEvent(long queryId)
        {
            var handler = AnswerOnQuery;
            if (handler != null) handler(queryId);
        }

        public static event Action<long,bool> CloseNotification;

        public static void CloseNotificationEvent(long roomId, bool reshowNotificationInFuture)
        {
            if(CloseNotification != null)
                CloseNotification.Invoke(roomId, reshowNotificationInFuture);
        }

        public static event Action<long> NotificationAnswer;

        public static void NotificationAnswerEvent(long roomId)
        {
            if(NotificationAnswer != null)
                NotificationAnswer.Invoke(roomId);
        }
        public static event Action<long> ReshowNotification;
        public static void ReshowNotificationEvent(long roomId)
        {
            if (ReshowNotification != null)
                ReshowNotification.Invoke(roomId);
        }
    }
}
