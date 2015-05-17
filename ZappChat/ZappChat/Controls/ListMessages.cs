using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZappChat.Core;

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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DialogueWithQuery = new ObservableCollection<MessageControl>();
            DialogueWithoutQuery = new ObservableCollection<MessageControl>();
            SelectionChanged += OpenDialogue;
        }

        public void AddNewMessageToList(Dialogue dialogue)
        {
            var thisDialogueInListQuery = DialogueWithQuery.Any(x => Equals(x.Dialogue, dialogue));

            if (thisDialogueInListQuery)
            {
                var thisDialogue = DialogueWithQuery.First(x => Equals(x.Dialogue, dialogue));
                thisDialogue.Dialogue.AddMessage(dialogue.Messages[0]);
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
                    thisDialogue.Dialogue.AddMessage(dialogue.Messages[0]);
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

        public void RemoveDialogueFromLists(uint dialogueId)
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
                // Добавление диалога с запросом и сообщениями в список диалогов с запросами - нужно ли так делать?
                DialogueWithQuery.Insert(0,thisDialogue);

                thisDialogue.UpdateControl();
            }
            thisDialogue = DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
            if (thisDialogue == null)
            {
                DialogueWithQuery.Insert(0, new MessageControl(dialogue));
            }
        }

        public void OpenDialogue(object sender, SelectionChangedEventArgs e)
        {
            var selectedDialogue = (SelectedItem as MessageControl);
            if(selectedDialogue == null) return;

            AppEventManager.OpenDialogueEvent(selectedDialogue, selectedDialogue.Dialogue);
            SelectedIndex = -1;//TODO test
        }
        public void ChangeMessageStatus(Dialogue changedDialogue)
        {
            var openedDialogue =
                DialogueWithoutQuery.FirstOrDefault(control => control.Dialogue.RoomId == changedDialogue.RoomId);
            if (openedDialogue != null)
                foreach (var message in openedDialogue.Dialogue.Messages)
                {
                    message.Status = MessageStatus.Read;
                }
            openedDialogue =
                DialogueWithQuery.FirstOrDefault(control => control.Dialogue.RoomId == changedDialogue.RoomId);
            if (openedDialogue != null)
                foreach (var message in openedDialogue.Dialogue.Messages)
                {
                    message.Status = MessageStatus.Read;
                }
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
