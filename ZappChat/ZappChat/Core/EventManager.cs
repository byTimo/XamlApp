namespace ZappChat.Core
{
    static class AppEventManager
    {
        public static event Connection ConnectionEventHandler;
        public static void ConnectionEvent(object sender, AppStatus status)
        {
            ConnectionEventHandler(sender, new ConnectionEventArgs(status));
        }

        public static event SendMessage SendMessageEventHandler;

        public static void SendMessageEvent(object sender, Message message)
        {
            SendMessageEventHandler(sender, new SendMessageEventArgs(message));
        }

        
        
    }
}
