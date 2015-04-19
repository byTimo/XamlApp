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

        public ListMessages()
        {
            DialogueWithQuery = new ObservableCollection<MessageControl>();
            DialogueWithoutQuery = new ObservableCollection<MessageControl>();
            AppEventManager.TakeMessage += AddNewMessage;
            AppEventManager.DeleteDialogue += DelMessage;
        }

        public void AddNewMessage(object sender, MessagingEventArgs e)
        {
            var thisDialogue = DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.Id == e.DialogueId);
            if (thisDialogue == null)
            {
                DialogueWithoutQuery.Insert(0, new MessageControl(new Dialogue(e.DialogueId, e.Message.Author, new List<Message> { e.Message })));
            }
            else
            {
                thisDialogue.Dialogue.AddMessage(e.Message);
                thisDialogue.UpdateControl();
                var indexThisDialogue = DialogueWithoutQuery.IndexOf(thisDialogue);
                DialogueWithoutQuery.Move(indexThisDialogue,0);
            }

            thisDialogue = DialogueWithQuery.FirstOrDefault(x => x.Dialogue.Id == e.DialogueId);
            if (thisDialogue != null)
            {
                thisDialogue.Dialogue.AddMessage(e.Message);
                thisDialogue.UpdateControl();
                var indexThisDialogue = DialogueWithQuery.IndexOf(thisDialogue);
                DialogueWithQuery.Move(indexThisDialogue, 0);
            }
        }

        public void DelMessage(object sender, DeletingEventArgs e)
        {
            if (e.IsConfirmed)
            {
                var delDialogue = DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.Id == e.DeletedDialogue.Id);
                if (delDialogue != null)
                    DialogueWithoutQuery.Remove(delDialogue);
                delDialogue = DialogueWithQuery.FirstOrDefault(x => x.Dialogue.Id == e.DeletedDialogue.Id);
                if (delDialogue != null)
                    DialogueWithQuery.Remove(delDialogue);
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
