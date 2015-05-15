namespace ZappChat.Core
{
    public enum AuthorizationType
    {
        Token,
        Login
    }
    public enum ConnectionStatus
    {
        Connect,
        Disconnect,
        Error
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