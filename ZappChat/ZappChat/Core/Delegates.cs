namespace ZappChat.Core
{
    public delegate void AuthorizationEventHandler(object sender, AuthorizationType type);

    public delegate void ReceivingDataEventHandler(object sender, Dialogue dialogue);

    public delegate void DeletingDialogueEventHandler(object sender, DeletingEventArgs e);

    public delegate void CloseDialogueEventHandler();
}
