namespace ZappChat.Core
{
    public enum AppStatus
    {
        Connect,
        Disconnect
    }

    public enum MessageStatus
    {
        Sent,
        Delivered,
        Read,
        Error
    }

    public enum MessageType
    {
        User,
        Interlocutor,
        System
    }
}