namespace ZappChat.Core
{
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);

    //Переделал для теста
    public delegate void AuthorizationEventHandler(object sender, WebSocketEventArgs e);

    public delegate void MessagingEventHandler(object sender, MessagingEventArgs e);

    public delegate void TakeQueryEventHandler(object sender, TakeQueryEventArgs e);

    public delegate void DeletingDialogueEventHandler(object sender, DeletingEventArgs e);

    public delegate void OpenDialogueEventHandler(object sender, DialogueOpenEventArgs e);

    public delegate void CloseDialogueEventHandler();

    public delegate void SwitchWindowEventHandler(object sender, SwitchWindowEventArgs e);
}
