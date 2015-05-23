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
        Outgoing,
        Incoming,
        System
    }

    public enum DialogueStatus
    {
        Created,
        Answered,
        Missed
    }

    public enum AnswerType
    {
        Selling,
        OnOrder,
        NoSelling
    }
}