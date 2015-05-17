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
            if (dialogue.RoomId == chat.CurrentDialogue.RoomId)
            {
                message.Status = MessageStatus.Read;
                chat.AddNewMessageToChat(dialogue);
            }
            //Кнопки "Сообщения"
            if (message.Status != MessageStatus.Read
                &&
                IsControlHaveUnreadMessageForTakeMessage(
                    Dialogues.DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue))))
                messageButton.MessagesCount++;
            //Списка диалогов
            Dialogues.AddNewMessageToList(dialogue);
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
                //Кнопки сообщения:
                if (
                    IsControlHaveUnreadMessage(
                        Dialogues.DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, e.DeletedDialogue))))
                    messageButton.MessagesCount--;
                //Кнопка запросов:
                if (IsControlHaveUnreadMessage(
                        Dialogues.DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, e.DeletedDialogue))))
                    myQuaryButton.MessagesCount--;
                //Чата
                if (chat.CurrentDialogue.RoomId == e.DeletedDialogue.RoomId)
                {
                    chat.CloseDialogue();
                    ShowDialogue(false);
                }
                //Списка диалогов
                Dialogues.RemoveDialogueFromLists(e.DeletedDialogue.RoomId);
            }
            //Блокера:
            ControlBlocker.Visibility = Visibility.Collapsed;
        }
//@TODO ------------------- Переделать условие, которое проверяет нужно ли уменьшать цифру на счётчике! --------------------------
        private void OpenDialogue(object sender, DialogueOpenEventArgs e)
        {
            ShowDialogue(true);
            //Реагирование на открытие диалога:
            //Кнопки сообщения:
            if (
                IsControlHaveUnreadMessage(
                    Dialogues.DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, e.OpenedDialogue))))
                messageButton.MessagesCount--;
            //Кнопка запросов:
            if (IsControlHaveUnreadMessage(
                    Dialogues.DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, e.OpenedDialogue))))
                myQuaryButton.MessagesCount--;
            //Список диалогов:
            Dialogues.ChangeMessageStatus(e.OpenedDialogue);
            //Чата
            chat.OpenDialogue(e.OpenedDialogue);

        }

        private void ReceivingQuery(object sender, Dialogue dialogue)
        {

            //Реагирование на получение запроса:
            //Кнопки запросов
            if (!Equals(chat.CurrentDialogue, dialogue))
                myQuaryButton.MessagesCount++;
            //Списка диалогов
            Dialogues.TakeQuery(dialogue);
            //Чата
            if (Equals(chat.CurrentDialogue, dialogue))
                chat.DialogueTitle = chat.CurrentDialogue.GetTitleMessage();
        }

        private bool IsControlHaveUnreadMessageForTakeMessage(MessageControl control)
        {
            if (control == null) return true;
            return control.Dialogue.Messages.All(mes => mes.Status == MessageStatus.Read);
        }
        private bool IsControlHaveUnreadMessage(MessageControl control)
        {
            if (control == null) return false;
            if (control.Dialogue.Messages.Count == 0) return true;
            return control.Dialogue.Messages.Any(mes => mes.Status != MessageStatus.Read);
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
