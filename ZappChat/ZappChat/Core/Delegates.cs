namespace ZappChat.Core
{
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);

    public delegate void MessagingEventHandler(object sender, MessagingEventArgs e);

    public delegate void TakeQueryEventHandler(object sender, TakeQueryEventArgs e);

    public delegate void DeletingDialogueEventHandler(object sender, DeletingEventArgs e);

    public delegate void OpenDialogueEventHandler(object sender, DialogueOpenEventArgs e);

}
