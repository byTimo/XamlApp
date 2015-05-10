using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
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
        private BackgroundWorker _Ping;

        public MainWindow()
        {
            InitializeComponent();
            AppEventManager.Connection += (s, e) => { statusButton.Status = e.ConnectionStatus; };
            AppEventManager.TakeMessage += TakeMessage;
            AppEventManager.DeleteConfirmationDialogue += DeleteConfirmationDialogue;
            AppEventManager.DeleteDialogue += DeleteDialogue;
            AppEventManager.OpenDialogue += OpenDialogue;
            AppEventManager.CloseDialogue += () => ShowDialogue(false);
            AppEventManager.TakeQuery += TakeQuery;
            AppEventManager.SendMessage += (s, e) => chat.SendMessage(e.Message);
            //@TODO разобраться с пингом
            _Ping = new BackgroundWorker();
            _Ping.DoWork += (o, args) =>
            {
                while (true)
                {
                    App.PingRequestSuccses = false;
                    var pingRequest = new PingRequest();
                    var pingRequestToJson = JsonConvert.SerializeObject(pingRequest);
                    App.StartTimerToPingRequest();
                    AppWebSocketEventManager.SendObject(pingRequestToJson);
                    Thread.Sleep(App.PingInterval);
                }
            };
        }

        private void TakeMessage(object sender, MessagingEventArgs e)
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
            if (message.Status != MessageStatus.Read
                &&
                IsControlHaveUnreadMessageForTakeMessage(
                    Dialogues.DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.Id == e.DialogueId)))
                messageButton.MessagesCount++;
            //Списка диалогов
            Dialogues.AddNewMessageToList(e.DialogueId, message);
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
                        Dialogues.DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.Id == e.DeletedDialogue.Id)))
                    messageButton.MessagesCount--;
                //Кнопка запросов:
                if (IsControlHaveUnreadMessage(
                        Dialogues.DialogueWithQuery.FirstOrDefault(x => x.Dialogue.Id == e.DeletedDialogue.Id)))
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
        }
        private void OpenDialogue(object sender, DialogueOpenEventArgs e)
        {
            ShowDialogue(true);
            //Реагирование на открытие диалога:
            //Кнопки сообщения:
            if (
                IsControlHaveUnreadMessage(
                    Dialogues.DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.Id == e.OpenedDialogue.Id)))
                messageButton.MessagesCount--;
            //Кнопка запросов:
            if (IsControlHaveUnreadMessage(
                    Dialogues.DialogueWithQuery.FirstOrDefault(x => x.Dialogue.Id == e.OpenedDialogue.Id)))
                myQuaryButton.MessagesCount--;
            //Список диалогов:
            Dialogues.ChangeMessageStatus(e.OpenedDialogue);
            //Чата
            chat.OpenDialogue(e.OpenedDialogue);

        }

        private void TakeQuery(object sender, TakeQueryEventArgs e)
        {

            //Реагирование на получение запроса:
            //Кнопки запросов
            if (chat.CurrentDialogue.Id != e.DialogueId)
                myQuaryButton.MessagesCount++;
            //Списка диалогов
            Dialogues.TakeQuery(e.DialogueId, e.Interlocutor, e.Query, e.Time);
            //Чата
            if (chat.CurrentDialogue.Id == e.DialogueId)
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             _Ping.RunWorkerAsync();
        }

    }
}
