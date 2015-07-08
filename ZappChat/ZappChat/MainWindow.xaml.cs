using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool _databaseLoaded;
        public MainWindow()
        {
            InitializeComponent();
            AppEventManager.Connect += Connection;
            AppEventManager.Disconnect += Disconnect;
            AppEventManager.AuthorizationSuccess += AuthorizationSucces;

            AppEventManager.DeleteConfirmationDialogue += DeleteConfirmationDialogue;
            AppEventManager.DeleteDialogue += DeleteDialogue;
            AppEventManager.OpenDialogue += OpenDialogue;
            AppEventManager.CloseDialogue += () => ShowDialogue(false);
            AppEventManager.ReceiveMessage += ReceivingMessage;
            AppEventManager.ReceiveQuery += ReceivingQuery;
            AppEventManager.SendMessageSuccess += chat.SendMessageSuccess;
            AppEventManager.SetCarInfo += SetCarInfo;
            AppEventManager.AnswerOnQuery += AnswerOnQuery;
            AppEventManager.NotificationAnswer +=AppEventManager_NotificationAnswer;
            AppEventManager.PreopenDialogue += AppEventManagerOnPreopenDialogue;
            AppEventManager.AfteropenDialogue += AppEventManagerOnAfteropenDialogue;
            TabNow.Queries = new ObservableCollection<QueryControl>();
            TabYesterday.Queries = new ObservableCollection<QueryControl>();
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetDialoguesIntoDatabase();
        }
        private void AuthorizationSucces(object sender, AuthorizationType type)
        {
            if(!_databaseLoaded) return;
            var auditRequest = new AuditRequest
            {
                log_id = App.LastLogId
            };
            var auditRequestToJson = JsonConvert.SerializeObject(auditRequest);
            AppWebSocketEventManager.SendObject(auditRequestToJson);
        }

        private void GetDialoguesIntoDatabase()
        {
            Dialogues.InitListMessages();
            TabNow.InitQueryList();
            TabYesterday.InitQueryList();
            foreach (var dialogue in DialogueStore.GetAllDialogues().OrderBy(d => d.LastDateTime))
            {
                if(App.IsThisDialogueDeleted(dialogue.RoomId)) continue;

                Dialogues.AddDialogueIntoDataBase(dialogue);
                if(dialogue.QueryId > 0)
                    TabControlReceiveQuery(dialogue);
                if (dialogue.Messages.Any(d => d.IsUnread))
                {
                    messageButton.MessagesCount++;
                    AppNotificationManager.CreateMessageNotification(dialogue);
                }
                if (dialogue.Status == DialogueStatus.Created && dialogue.QueryId > 0)
                {
                    myQuaryButton.MessagesCount++;
                    AppNotificationManager.CreateQueryNotification(dialogue);
                }
            }
            var auditRequest = new AuditRequest
            {
                log_id = App.LastLogId
            };
            var auditRequestToJson = JsonConvert.SerializeObject(auditRequest);
            AppWebSocketEventManager.SendObject(auditRequestToJson);
            _databaseLoaded = true;
        }

        private void ReceivingQuery(object sender, Dialogue dialogue)
        {
            if(App.IsThisDialogueDeleted(dialogue.RoomId)) return;

            if (dialogue.Status != DialogueStatus.Created && App.IsThisUnreadMessage(dialogue.RoomId,0))
                App.ChangeDialogueStatus(dialogue.RoomId, "0");
            //Реагирование на получение запроса:
            //Списка диалогов
            Dialogues.TakeQuery(dialogue);
            //TabControl
            TabControlReceiveQuery(dialogue);
            //Чата
            if (Equals(chat.CurrentDialogue, dialogue))
                chat.DialogueTitle = chat.CurrentDialogue.GetTitleMessage();
            //Кнопки запросов
            if (/*!Equals(chat.CurrentDialogue, dialogue) && App.IsThisUnreadMessage(dialogue.RoomId,0)*/
                dialogue.Status == DialogueStatus.Created && dialogue.QueryId > 0)
            {
                myQuaryButton.MessagesCount++;
                var control = Dialogues.DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
                if (control != null) control.DialogueOpened = false;
                if(!Equals(chat.CurrentDialogue, dialogue))
                    AppNotificationManager.CreateQueryNotification(dialogue);
            }
            if(Equals(chat.CurrentDialogue, dialogue) && !IsActive)
            {
                AppNotificationManager.CreateQueryNotification(dialogue);
            }
        }

        private void ReceivingMessage(object sender, Dialogue dialogue)
        {
            if (App.IsThisDialogueDeleted(dialogue.RoomId)) return;

            var message = dialogue.GetLastMessage();

            if (!message.IsUnread && App.IsThisUnreadMessage(dialogue.RoomId, message.MessageId))
                App.ChangeDialogueStatus(dialogue.RoomId, message.MessageId.ToString());
            //Реагирование на приход нового сообщения:
            //Списка диалогов
            Dialogues.AddNewMessageToList(dialogue);
            //Чата
            if (Equals(dialogue, chat.CurrentDialogue))
            {
                var lastMessageCurrent = dialogue.GetLastMessage();
                if(lastMessageCurrent != null) App.ChangeDialogueStatus(dialogue.RoomId, lastMessageCurrent.MessageId.ToString());
                chat.AddNewMessageToChat(dialogue);

                var readType = new ReadRoomRequest { room_id = dialogue.RoomId };
                var readTypeToJson = JsonConvert.SerializeObject(readType);
                AppWebSocketEventManager.SendObject(readTypeToJson);
            }
            //Кнопки "Сообщения"
            var control = Dialogues.DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
            var lastMessage = dialogue.GetLastMessage();
            if (control != null && !Equals(chat.CurrentDialogue, dialogue) &&
                App.IsThisUnreadMessage(dialogue.RoomId, lastMessage != null ? lastMessage.MessageId : 0)
                && !control.ContaintUnreadMessages)
            {
                messageButton.MessagesCount++;
                control.ContaintUnreadMessages = true;
            }
            if (App.IsThisUnreadMessage(dialogue.RoomId, lastMessage != null ? lastMessage.MessageId : 0))
                AppNotificationManager.CreateMessageNotification(dialogue);
            if (Equals(chat.CurrentDialogue, dialogue) && !IsActive)
            {
                AppNotificationManager.CreateMessageNotification(dialogue);
            }

        }

        private void DeleteConfirmationDialogue(object sender, DeletingEventArgs e)
        {
            //Реагирование по запросу на удаление:
            //Блокера:
            ControlBlocker.Visibility = Visibility.Visible;
            //Чат-блокер
            if(chat != null)
                chat.BlockingChat(true);
            //Синего меню:
            BlueMenu.DeleteDialgoueQery(e.DeletedDialogue);
        }

        private void DeleteDialogue(object sender, DeletingEventArgs e)
        {
            if(App.ConnectionStatus != ConnectionStatus.Connect) return;

            //Реагирование по запросу на удаление:
            if (e.IsConfirmed)
            {
                //Чата
                if (Equals(chat.CurrentDialogue, e.DeletedDialogue))
                {
                    chat.CloseDialogue();
                    ShowDialogue(false);
                }
                //Файла статусов диалогов
                App.ChangeDialogueStatus(e.DeletedDialogue.RoomId, "d");
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
                //TabControl
                var nowQueryControl = TabNow.Queries.FirstOrDefault(x => Equals(x.Dialogue, e.DeletedDialogue));
                if (nowQueryControl != null) TabNow.Queries.Remove(nowQueryControl);
                var yestQueryControl = TabYesterday.Queries.FirstOrDefault(x => Equals(x.Dialogue, e.DeletedDialogue));
                if (yestQueryControl != null) TabYesterday.Queries.Remove(yestQueryControl);

            }
            //Блокера:
            ControlBlocker.Visibility = Visibility.Collapsed;
            //Чат-блокер
            if (chat != null)
                chat.BlockingChat(false);
        }

        private void OpenDialogue(long roomId, List<Message> messages)
        {
            ShowDialogue(true);
            var openedDialogue = DialogueStore.GetDialogueOnRoomId(roomId);
            foreach (var message in messages)
            {
                var messageInDb = openedDialogue.Messages.FirstOrDefault(m => m.MessageId == message.MessageId);
                if (messageInDb != null)
                {
                    messageInDb.IsUnread = false;
                }
                else
                {
                    openedDialogue.AddMessage(message);
                }
            }
            //Реагирование на открытие диалога:
            //Файла статусов диалогов
            var lastMessage = openedDialogue.GetLastMessage();
            if (lastMessage != null)
                App.ChangeDialogueStatus(roomId, lastMessage.MessageId.ToString());
            //Чата
            chat.OpenDialogue(openedDialogue);
            //Кнопки сообщения:
            var control = Dialogues.DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.RoomId == roomId);
            if (control != null && control.ContaintUnreadMessages)
            {
                messageButton.MessagesCount--;
                control.ContaintUnreadMessages = false;
            }

            DialogueStore.SaveChanges();
            var readType = new ReadRoomRequest {room_id = roomId};
            var readTypeToJson = JsonConvert.SerializeObject(readType);
            AppWebSocketEventManager.SendObject(readTypeToJson);
        }

        private void AppEventManager_NotificationAnswer(long obj)
        {
            var control = Dialogues.DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.RoomId == obj);
            if (control != null && control.ContaintUnreadMessages)
            {
                messageButton.MessagesCount--;
                control.ContaintUnreadMessages = false;
            }
            //Кнопка запросов:
            control = Dialogues.DialogueWithQuery.FirstOrDefault(x => x.Dialogue.RoomId == obj);
            if (control != null && !control.DialogueOpened)
            {
                myQuaryButton.MessagesCount--;
                control.DialogueOpened = true;
            }
        }

        private void SetCarInfo(long id, string brand, string model, string vin, string year)
        {
            //Реагирование на получение информации об автомобиле
            //Список диалогов
            foreach (var source in Dialogues.DialogueWithQuery.Where(x => x.Dialogue.CarId == id))
            {
                source.Dialogue.SetCarInformation(brand, model, vin, year);
            }
            //Списка запросов
            foreach (var source in TabNow.Queries.Where(x => x.Dialogue.CarId == id))
            {
                source.SetCarInfoAdapter(brand,model,vin,year);
            }
            foreach (var source in TabYesterday.Queries.Where(x => x.Dialogue.CarId == id))
            {
                source.SetCarInfoAdapter(brand, model, vin, year);
            }
            //Чата
            if (chat.CurrentDialogue.CarId == id)
                chat.SetCarInfoAdapter(brand, model, vin, year);
        }

        private void AnswerOnQuery(long roomId)
        {
            var dialogue = DialogueStore.GetDialogueOnRoomId(roomId);
            dialogue.Status = DialogueStatus.Answered;
            myQuaryButton.MessagesCount--;
            var control = Dialogues.DialogueWithQuery.FirstOrDefault(x => x.Dialogue.RoomId == roomId);
            if (control != null)
            {
                control.DialogueOpened = true;
            }
            if (chat.CurrentDialogue.RoomId == roomId)
            {
                chat.ChangeDialogueStatus();
            }
            DialogueStore.SaveChanges();
        }

        private void AppEventManagerOnPreopenDialogue(long roomId, string f, string t)
        {           
            ControlBlocker.Visibility = Visibility.Visible;
            ShowDialogue(true);
            chat.BeginMessageLoad();
            var historyRequest = new HistoryRequest
            {
                @from = f,
                to = t,
                chat_room_id = roomId
            };
            var historyRequestToJson = JsonConvert.SerializeObject(historyRequest);
            AppWebSocketEventManager.SendObject(historyRequestToJson);
        }

        private void AppEventManagerOnAfteropenDialogue()
        {
            ControlBlocker.Visibility = Visibility.Collapsed;
            chat.EndMessageLoad();
        }

        private void TabControlReceiveQuery(Dialogue dialogue)
        {
            var dateNow = DateTime.Now;
            if (dateNow.Year == dialogue.LastDateTime.Year && dateNow.DayOfYear == dialogue.LastDateTime.DayOfYear)
            {
                TabNow.Queries.Insert(0, new QueryControl(dialogue));
                return;
            }
            var dateYeastr = dateNow.Subtract(new TimeSpan(1,0,0,0));
            if (dateYeastr.Year == dialogue.LastDateTime.Year && dateYeastr.DayOfYear == dialogue.LastDateTime.DayOfYear)
                TabYesterday.Queries.Insert(0, new QueryControl(dialogue));
        }

        private void ShowDialogue(bool solution)
        {
            tabs.Visibility = solution ? Visibility.Hidden : Visibility.Visible;
            chat.Visibility = solution ? Visibility.Visible : Visibility.Hidden;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
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

        public bool IsEqualChatDialogue(Dialogue dialogue)
        {
            return chat.CurrentDialogue.Equals(dialogue);
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
