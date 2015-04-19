namespace ZappChat.Core
{
    static class AppEventManager
    {
        public static event ConnectionEventHandler Connection;
        public static void ConnectionEvent(object sender, AppStatus status)
        {
            Connection(sender, new ConnectionEventArgs(status));
        }

        public static event MessagingEventHandler TakeMessage;

        public static void TakeMessageEvent(object sender,int dialogueId, Message message)
        {
            TakeMessage(sender, new MessagingEventArgs(dialogueId, message));
        }

        public static event SentQueryEventHandler SentQuery;

        public static void SentQueryEvent(object sender, int dialogueId, string query)
        {
            SentQuery(sender, new SentQueryEventArgs(dialogueId, query));
        }
        //TODO SendMessageEvent and SendMessageEventHadler

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
    }
}
