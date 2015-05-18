using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using ZappChat.Controls;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;

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
            AppEventManager.Connect += Connection; //Активация элементов управления
            AppEventManager.Disconnect += Disconnect; //Деактивация элементов управления
            AppEventManager.AuthorizationSuccess += AuthorizationSucces;

            AppEventManager.DeleteConfirmationDialogue += DeleteConfirmationDialogue;
            AppEventManager.DeleteDialogue += DeleteDialogue;
            AppEventManager.OpenDialogue += OpenDialogue;
            AppEventManager.CloseDialogue += () => ShowDialogue(false);
            AppEventManager.ReceiveMessage += ReceivingMessage;
            AppEventManager.ReceiveQuery += ReceivingQuery;
            //AppEventManager.SendMessage += (s, e) => chat.SendMessage(e.Message);
        }

        private void Connection(object sender)
        {
            statusButton.ChangeStatusOnButton(App.ConnectionStatus);
            AppInfo.Background = border.Background;
            VersionText.Visibility = Visibility.Visible;
            NoConnectionImage.Visibility = Visibility.Collapsed;
            NoConnectionText.Visibility = Visibility.Collapsed;
        }

        private void Disconnect(object sender)
        {
            statusButton.ChangeStatusOnButton(App.ConnectionStatus);
            AppInfo.Background = new SolidColorBrush(Color.FromRgb(72,73,73));
            VersionText.Visibility = Visibility.Collapsed;
            NoConnectionImage.Visibility = Visibility.Visible;
            NoConnectionText.Visibility = Visibility.Visible;
        }

        private void AuthorizationSucces(object sender, AuthorizationType type)
        {
            var auditRequest = new AuditRequest
            {
                log_id = App.LastLogId
            };
            var auditRequestToJson = JsonConvert.SerializeObject(auditRequest);
            AppWebSocketEventManager.SendObject(auditRequestToJson);
        }

        private void ReceivingMessage(object sender, Dialogue dialogue)
        {
            var message = dialogue.Messages[0];
            message.Status = MessageStatus.Delivered;
            //Реагирование на приход нового сообщения:
            //Чата
            if (Equals(dialogue, chat.CurrentDialogue))
            {
                message.Status = MessageStatus.Read;
                chat.AddNewMessageToChat(dialogue);
            }
            //Списка диалогов
            Dialogues.AddNewMessageToList(dialogue);
            //Кнопки "Сообщения"
            var control = Dialogues.DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
            if (control != null && !Equals(chat.CurrentDialogue, dialogue)
                && !control.ContaintUnreadMessages)
            {
                messageButton.MessagesCount++;
                control.ContaintUnreadMessages = true;
            }

        }
        private void DeleteConfirmationDialogue(object sender, DeletingEventArgs e)
        {
            //Реагирование по запросу на удаление:
            //Блокера:
            ControlBlocker.Visibility = Visibility.Visible;
            //Синего меню:
            BlueMenu.DeleteDialgoueQery(e.DeletedDialogue);
        }
        private void DeleteDialogue(object sender, DeletingEventArgs e)
        {
            //Реагирование по запросу на удаление:
            if (e.IsConfirmed)
            {
                //Чата
                if (Equals(chat.CurrentDialogue, e.DeletedDialogue))
                {
                    chat.CloseDialogue();
                    ShowDialogue(false);
                }
                //Кнопки сообщения:
                var control = Dialogues.DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, e.DeletedDialogue));
                if (control != null && control.ContaintUnreadMessages)
                    messageButton.MessagesCount--;
                //Кнопка запросов:
                control = Dialogues.DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, e.DeletedDialogue));
                if (control != null && !control.DialogueOpened)
                    myQuaryButton.MessagesCount--;
                //Списка диалогов
                Dialogues.RemoveDialogueFromLists(e.DeletedDialogue.RoomId);
                
            }
            //Блокера:
            ControlBlocker.Visibility = Visibility.Collapsed;
        }
        private void OpenDialogue(object sender, DialogueOpenEventArgs e)
        {
            ShowDialogue(true);
            //Реагирование на открытие диалога:            
            //Список диалогов:
            Dialogues.ChangeMessageStatus(e.OpenedDialogue);
            //Чата
            chat.OpenDialogue(e.OpenedDialogue);
            //Кнопки сообщения:
            var control = Dialogues.DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, e.OpenedDialogue));
            if (control != null && control.ContaintUnreadMessages)
            {
                messageButton.MessagesCount--;
                control.ContaintUnreadMessages = false;
            }
            //Кнопка запросов:
            control = Dialogues.DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, e.OpenedDialogue));
            if (control != null && !control.DialogueOpened)
            {
                myQuaryButton.MessagesCount--;
                control.DialogueOpened = true;
            }
        }

        private void ReceivingQuery(object sender, Dialogue dialogue)
        {

            //Реагирование на получение запроса:
            //Списка диалогов
            Dialogues.TakeQuery(dialogue);
            //Чата
            if (Equals(chat.CurrentDialogue, dialogue))
                chat.DialogueTitle = chat.CurrentDialogue.GetTitleMessage();
            //Кнопки запросов
            if (!Equals(chat.CurrentDialogue, dialogue))
            {
                myQuaryButton.MessagesCount++;
                var control = Dialogues.DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
                if (control != null) control.DialogueOpened = false;
            }
        }

        private void ShowDialogue(bool solution)
        {
            tabs.Visibility = solution ? Visibility.Hidden : Visibility.Visible;
            chat.Visibility = solution ? Visibility.Visible : Visibility.Hidden;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
