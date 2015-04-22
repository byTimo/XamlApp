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
using ZappChat.Controls;
using ZappChat.Core;

namespace ZappChat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppEventManager.Connection += (s, e) => { statusButton.Status = e.ConnectionStatus; };

            AppEventManager.TakeMessage += (s, e) =>
            {
                var message = e.Message;
                message.Status = MessageStatus.Delivered;
                //Реагирование на приход нового сообщения:
                //Чата
                if (e.DialogueId == chat.CurrentDialogue.Id)
                {
                    message.Status = MessageStatus.Read;
                    chat.AddNewMessageToChat(e.DialogueId, message);
                }
                //Кнопки "Сообщения"
                var haveNotReadMessage = FindNotReadMessageToMessageControls(Dialogues.DialogueWithoutQuery,
                    e.DialogueId);
                if (message.Status != MessageStatus.Read && !haveNotReadMessage)
                    messageButton.MessagesCount++;
                //Списка диалогов
                Dialogues.AddNewMessageToList(e.DialogueId,message);
            };

            AppEventManager.DeleteConfirmationDialogue += (s, e) =>
            {
                //Реагирование по запросу на удаление:
                //Блокера:
                ControlBlocker.Visibility = Visibility.Visible;
                //Синего меню:
                BlueMenu.DeleteDialgoueQery(e.DeletedDialogue);
            };
            AppEventManager.DeleteDialogue += (s, e) =>
            {
                //Реагирование по запросу на удаление:
                if (e.IsConfirmed)
                {
                    //Кнопки сообщения:
                    if (FindNotReadMessageToMessageControls(Dialogues.DialogueWithoutQuery, e.DeletedDialogue.Id))
                        messageButton.MessagesCount--;
                    //Кнопка запросов:
                    if (FindNotReadMessageToMessageControls(Dialogues.DialogueWithQuery, e.DeletedDialogue.Id))
                        myQuaryButton.MessagesCount--;
                    //Чата
                    if (chat.CurrentDialogue.Id == e.DeletedDialogue.Id)
                    {
                        chat.CloseDialogue();
                        ShowDialogue(false);
                    }
                    //Списка диалогов
                    Dialogues.RemoveDialogueFromLists(e.DeletedDialogue.Id);
                }
                //Блокера:
                ControlBlocker.Visibility = Visibility.Collapsed;
            };

            AppEventManager.OpenDialogue += (s, e) =>
            {
                ShowDialogue(true);
                //Реагирование на открытие диалога:
                //Кнопки сообщений
                if (FindNotReadMessageToMessageControls(Dialogues.DialogueWithoutQuery, e.OpenedDialogue.Id))
                    messageButton.MessagesCount--;
                //Кнопка запросов:
                if (FindNotReadMessageToMessageControls(Dialogues.DialogueWithQuery, e.OpenedDialogue.Id))
                    myQuaryButton.MessagesCount--;
                //Список диалогов:
                Dialogues.ChangeMessageStatus(e.OpenedDialogue);
                //Чата
                chat.OpenDialogue(e.OpenedDialogue);
            };
            AppEventManager.CloseDialogue += () => ShowDialogue(false);
        }

        private bool FindNotReadMessageToMessageControls(ObservableCollection<MessageControl> collection, int dialogueId)
        {
            if (collection.Count(x => x.Dialogue.Id == dialogueId) == 0) return false;
            return collection.Where(c => c.Dialogue.Id == dialogueId)
                    .Any(d => d.Dialogue.Messages.All(m => m.Status == MessageStatus.Delivered));
        }

        private void ShowDialogue(bool solution)
        {
            tabs.Visibility = solution ? Visibility.Hidden : Visibility.Visible;
            chat.Visibility = solution ? Visibility.Visible : Visibility.Hidden;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HideButton_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void myQuaryButton_Checked(object sender, RoutedEventArgs e)
        {
            Dialogues.SelectWithQuery();
        }

        private void messageButton_Checked(object sender, RoutedEventArgs e)
        {
            Dialogues.SelectWithoutQuery();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
        }

    }
}
