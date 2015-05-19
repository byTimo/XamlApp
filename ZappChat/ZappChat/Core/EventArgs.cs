using System;

namespace ZappChat.Core
{
    public class DeletingEventArgs : EventArgs
    {
        public readonly Dialogue DeletedDialogue;
        public readonly bool IsConfirmed;

        public DeletingEventArgs(Dialogue deleteDialogue, bool isConfirmed)
        {
            DeletedDialogue = deleteDialogue;
            IsConfirmed = isConfirmed;
        }
    }
}
