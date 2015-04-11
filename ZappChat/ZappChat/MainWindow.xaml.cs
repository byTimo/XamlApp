using System;
using System.Collections.Generic;
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
using ZappChat.Controls;
using ZappChat.Core;

namespace ZappChat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Dialogue> activeDialogues = new List<Dialogue>();
        public MainWindow()
        {
            InitializeComponent();
            AppEventManager.Connection += (s, e) => { statusButton.Status = e.ConnectionStatus; };
            AppEventManager.TakeMessage += TakeMessage;
            AppEventManager.DeleteConfirmationDialogue += (s, e) => { ControlBlocker.Visibility = Visibility.Visible; };
            AppEventManager.DeleteDialogue += DeleteDialogue;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HideButton_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TakeMessage(object sender, MessagingEventArgs e)
        {
            //TODO Возможно не имеет смысла делать именно так, лучше попробовать сделать через лист<MessageBox>
            //Проверю после коммита!!!

            var activeDialogue = activeDialogues.FirstOrDefault(x => x.Interlocutor == e.Message.Author);
            if (activeDialogue == null)
            {
                var newDialogue = new Dialogue(e.Message.Author, new List<Message> {e.Message});
                activeDialogues.Add(newDialogue);
                MessageBox.Items.Add(new MessageControl(newDialogue));
            }
            else
            {
                var indexActiveDialogue = activeDialogues.FindIndex(x => x.Equals(activeDialogue));
                var mesControlActiveDialogue = MessageBox.Items.GetItemAt(indexActiveDialogue) as MessageControl;
                mesControlActiveDialogue.Dialogue.AddMessage(e.Message);
                mesControlActiveDialogue.UpdateControl();
                activeDialogue.AddMessage(e.Message);
            }
            messageButton.MessagesCount++;
        }

        private void DeleteDialogue(object sender, DeletingEventArgs e)
        {
            if (e.IsConfirmed)
            {
                var indexActiveDialogue = activeDialogues.FindIndex(x => x.Equals(e.DeletedDialogue));
                MessageBox.Items.RemoveAt(indexActiveDialogue);
                activeDialogues.RemoveAt(indexActiveDialogue);
            }
            ControlBlocker.Visibility = Visibility.Collapsed;
        }
    }
}
