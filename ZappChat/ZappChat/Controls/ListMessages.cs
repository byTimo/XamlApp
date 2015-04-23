using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            //AppEventManager.TakeMessage += AddNewMessage;
            //AppEventManager.DeleteDialogue += DelMessage;
            //AppEventManager.TakeQuery += TakeQuery;
        }

        public void AddNewMessageToList(int dialogueId, Message message)
        {
            var thisDialogueInListQuery = DialogueWithQuery.Any(x => x.Dialogue.Id == dialogueId);

            if (thisDialogueInListQuery)
            {
                var thisDialogue = DialogueWithQuery.First(x => x.Dialogue.Id == dialogueId);
                thisDialogue.Dialogue.AddMessage(message);
                var indexThisDialogue = DialogueWithQuery.IndexOf(thisDialogue);
                DialogueWithQuery.Move(indexThisDialogue, 0);
                var thisDialogueWithoutQuery = DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.Id == dialogueId);
                if(thisDialogueWithoutQuery == null)
                    DialogueWithoutQuery.Insert(0, thisDialogue);

                thisDialogue.UpdateControl();
            }
            else
            {
                var thisDialogue = DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.Id == dialogueId);
                if (thisDialogue == null)
                {
                    DialogueWithoutQuery.Insert(0,
                        new MessageControl(new Dialogue(dialogueId, message.Author, new List<Message> {message})));
                }
                else
                {
                    thisDialogue.Dialogue.AddMessage(message);
                    var indexThisDialogue = DialogueWithoutQuery.IndexOf(thisDialogue);
                    DialogueWithoutQuery.Move(indexThisDialogue, 0);

                    var thisDialogueWithQuery = DialogueWithQuery.FirstOrDefault(x => x.Dialogue.Id == dialogueId);
                    if (thisDialogueWithQuery != null)
                    {
                        indexThisDialogue = DialogueWithQuery.IndexOf(thisDialogueWithQuery);
                        DialogueWithQuery.Move(indexThisDialogue, 0);
                    }

                    thisDialogue.UpdateControl();
                }
            }
        }

        public void RemoveDialogueFromLists(int dialogueId)
        {
            var delDialogue = DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.Id == dialogueId);
            if (delDialogue != null)
                DialogueWithoutQuery.Remove(delDialogue);
            delDialogue = DialogueWithQuery.FirstOrDefault(x => x.Dialogue.Id == dialogueId);
            if (delDialogue != null)
                DialogueWithQuery.Remove(delDialogue);

        }

        public void TakeQuery(int dialogueId, string interlocutor, string query, DateTime time)
        {
            var thisDialogue = DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.Id == dialogueId);
            if (thisDialogue != null)
            {
                thisDialogue.Dialogue.Query = query;
                var indexThisDialogue = DialogueWithoutQuery.IndexOf(thisDialogue);
                DialogueWithoutQuery.Move(indexThisDialogue, 0);
                // Добавление диалога с запросом и сообщениями в список диалогов с запросами - нужно ли так делать?
                DialogueWithQuery.Insert(0,thisDialogue);

                thisDialogue.UpdateControl();
            }
            thisDialogue = DialogueWithQuery.FirstOrDefault(x => x.Dialogue.Id == dialogueId);
            if (thisDialogue == null)
            {
                DialogueWithQuery.Insert(0, new MessageControl(new Dialogue(dialogueId,interlocutor,query, time)));
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
                DialogueWithoutQuery.FirstOrDefault(control => control.Dialogue.Id == changedDialogue.Id);
            if (openedDialogue != null)
                foreach (var message in openedDialogue.Dialogue.Messages)
                {
                    message.Status = MessageStatus.Read;
                }
            openedDialogue =
                DialogueWithQuery.FirstOrDefault(control => control.Dialogue.Id == changedDialogue.Id);
            if (openedDialogue != null)
                foreach (var message in openedDialogue.Dialogue.Messages)
                {
                    message.Status = MessageStatus.Read;
                }
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
