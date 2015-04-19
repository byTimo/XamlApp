namespace ZappChat.Core
{
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);

    public delegate void MessagingEventHandler(object sender, MessagingEventArgs e);

    public delegate void SentQueryEventHandler(object sender, SentQueryEventArgs e);

    public delegate void DeletingDialogueEventHandler(object sender, DeletingEventArgs e);

}
