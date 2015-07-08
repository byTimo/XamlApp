using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;

namespace ZappChat.Controls
{

    public class ListMessages : ListBox
    {
        public ObservableCollection<MessageControl> DialogueWithoutQuery { get; set; }
        public ObservableCollection<MessageControl> DialogueWithQuery { get; set; } 

        static ListMessages()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListMessages), new FrameworkPropertyMetadata(typeof(ListMessages)));
        }
        public void InitListMessages()
        {
            if (DialogueWithQuery == null)
                DialogueWithQuery = new ObservableCollection<MessageControl>();
            if(DialogueWithoutQuery == null)
                DialogueWithoutQuery = new ObservableCollection<MessageControl>();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitListMessages();
            SelectionChanged += OpenDialogue;
            SelectWithQuery();
        }

        public void AddDialogueIntoDataBase(Dialogue dialogue)
        {
            var messageControl = new MessageControl(dialogue)
            {
                DialogueOpened = dialogue.Status != DialogueStatus.Created,
                ContaintUnreadMessages = dialogue.Messages.Any(m => m.IsUnread)
            };
            if (dialogue.QueryId > 0)
            {
                DialogueWithQuery.Insert(0, messageControl);
            }
            if (dialogue.Messages.Count > 0)
            {
                DialogueWithoutQuery.Insert(0, messageControl);
            }
        }

        public void AddNewMessageToList(Dialogue dialogue)
        {
            var thisDialogueInListQuery = DialogueWithQuery.Any(x => Equals(x.Dialogue, dialogue));

            if (thisDialogueInListQuery)
            {
                var thisDialogue = DialogueWithQuery.First(x => Equals(x.Dialogue, dialogue));
                var indexThisDialogue = DialogueWithQuery.IndexOf(thisDialogue);
                DialogueWithQuery.Move(indexThisDialogue, 0);
                var thisDialogueWithoutQuery = DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
                if(thisDialogueWithoutQuery == null)
                    DialogueWithoutQuery.Insert(0, thisDialogue);

                thisDialogue.UpdateControl();
            }
            else
            {
                var thisDialogue = DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
                if (thisDialogue == null)
                {
                    DialogueWithoutQuery.Insert(0, new MessageControl(dialogue));
                }
                else
                {
                    var indexThisDialogue = DialogueWithoutQuery.IndexOf(thisDialogue);
                    DialogueWithoutQuery.Move(indexThisDialogue, 0);

                    var thisDialogueWithQuery = DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
                    if (thisDialogueWithQuery != null)
                    {
                        indexThisDialogue = DialogueWithQuery.IndexOf(thisDialogueWithQuery);
                        DialogueWithQuery.Move(indexThisDialogue, 0);
                    }

                    thisDialogue.UpdateControl();
                }
            }
        }

        public void RemoveDialogueFromLists(long dialogueId)
        {
            var delDialogue = DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.RoomId == dialogueId);
            if (delDialogue != null)
                DialogueWithoutQuery.Remove(delDialogue);
            delDialogue = DialogueWithQuery.FirstOrDefault(x => x.Dialogue.RoomId == dialogueId);
            if (delDialogue != null)
                DialogueWithQuery.Remove(delDialogue);

        }

        public void TakeQuery(Dialogue dialogue)
        {

            var thisDialogue = DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
            if (thisDialogue != null)
            {
                thisDialogue.Dialogue.Query = dialogue.Query;
                var indexThisDialogue = DialogueWithoutQuery.IndexOf(thisDialogue);
                DialogueWithoutQuery.Move(indexThisDialogue, 0);
                DialogueWithQuery.Insert(0,thisDialogue);

                thisDialogue.UpdateControl();
            }
            thisDialogue = DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
            if (thisDialogue == null)
            {
                var isOpenedDialogue = !App.IsThisUnreadMessage(dialogue.RoomId, 0);

                var newDialogueWithQuery = new MessageControl(dialogue) {DialogueOpened = isOpenedDialogue};
                DialogueWithQuery.Insert(0, newDialogueWithQuery);
            }
        }

        public void OpenDialogue(object sender, SelectionChangedEventArgs e)
        {
            if(App.ConnectionStatus != ConnectionStatus.Connect) return;

            var selectedMessageControl = (SelectedItem as MessageControl);
            if(selectedMessageControl == null) return;            
            SelectedIndex = -1;
            AppEventManager.PreopenDialogueEvent(selectedMessageControl.Dialogue.RoomId, null, null);
        }
        public Dialogue ChangeMessageStatus(long roomId, List<Message> messages)
        {
            var controlWithoutQuery =
                DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.RoomId == roomId);
            if (controlWithoutQuery != null)
                controlWithoutQuery.Dialogue.Messages = messages;
            var controlWithQuery =
                DialogueWithQuery.FirstOrDefault(x => x.Dialogue.RoomId == roomId);
            if (controlWithQuery != null)
                controlWithQuery.Dialogue.Messages = messages;
            return controlWithoutQuery != null ? controlWithoutQuery.Dialogue : controlWithQuery.Dialogue;
        }

        public void TakeNewDialogue(Dialogue newDialogue)
        {
            DialogueWithQuery.Insert(0, new MessageControl(newDialogue));
        }

        public void SelectWithoutQuery()
        {
            ItemsSource = DialogueWithoutQuery;
        }

        public void SelectWithQuery()
        {
            ItemsSource = DialogueWithQuery;
        }


    }
}
