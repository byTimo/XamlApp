using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;

namespace ZappChat.Controls
{
    /// <summary>
    /// Логика взаимодействия для QueryNotification.xaml
    /// </summary>
    public partial class QueryNotification : UserControl, INotification
    {
        public Dialogue Dialogue { get; set; }

        public static readonly DependencyProperty NotificationTextProperty = DependencyProperty.Register(
            "NotificationText", typeof (string), typeof (QueryNotification), new FrameworkPropertyMetadata("Title"));

        public static readonly DependencyProperty NotificationVinProperty =
            DependencyProperty.Register("NotificationVin", typeof (string), typeof (QueryNotification),
                new FrameworkPropertyMetadata(""));

        public static readonly DependencyProperty NotificationYearProperty =
            DependencyProperty.Register("NotificationYear", typeof(string), typeof(QueryNotification),
                new FrameworkPropertyMetadata(""));

        public string NotificationText
        {
            get { return GetValue(NotificationTextProperty) as string; }
            set { SetValue(NotificationTextProperty, value); }

        }

        public string NotificationVin
        {
            get { return GetValue(NotificationVinProperty) as string; }
            set { SetValue(NotificationVinProperty, value); }
        }
        public string NotificationYear
        {
            get { return GetValue(NotificationYearProperty) as string; }
            set { SetValue(NotificationYearProperty, value); }
        }

        public QueryNotification()
        {
            this.InitializeComponent();
        }

        public QueryNotification(Dialogue dialogue) : this()
        {
            Dialogue = dialogue;
            NotificationText = dialogue.GetTitleMessage();
            NotificationVin = dialogue.VIN ?? "";
            NotificationYear = dialogue.Year ?? "";
            Label.GotKeyboardFocus += (sender, args) =>
            {
                Keyboard.Focus(UserInput);
            };
            UserInput.GotKeyboardFocus += (sender, args) =>
            {
                Label.Visibility = Visibility.Hidden;
            };
            UserInput.LostKeyboardFocus += (sender, args) =>
            {
                if (UserInput.Text.Trim().Equals(string.Empty))
                    Label.Visibility = Visibility.Visible;
            };
        }

        public void SetCarInfo(string brand, string model, string vin, string year)
        {
            Dialogue.SetCarInformation(brand, model, vin, year);
            if (vin == null)
                NotificationVin = "";
            else
                NotificationVin = "VIN: " + vin;
            NotificationYear = year ?? "";
        }

        public void CloseNotify()
        {
            AppEventManager.CloseNotificationEvent();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseNotify();
        }


        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.ConnectionStatus == ConnectionStatus.Connect)
            {
                var historyRequest = new HistoryRequest
                {
                    @from = null,
                    to = null,
                    chat_room_id = Dialogue.RoomId
                };
                var historyRequestToJson = JsonConvert.SerializeObject(historyRequest);
                AppWebSocketEventManager.SendObject(historyRequestToJson);
                App.ShowCurrentWindow();
            }
            CloseNotify();
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenButton_Click(null, null);
        }

        private void FirstButton_OnClick(object sender, RoutedEventArgs e)
        {
            SendAnswer(AnswerType.Selling);
        }
        private void SecondButton_OnClick(object sender, RoutedEventArgs e)
        {
            SendAnswer(AnswerType.OnOrder);
        }

        private void ThirdButton_OnClick(object sender, RoutedEventArgs e)
        {
            SendAnswer(AnswerType.NoSelling);
        }

        private void SendAnswer(AnswerType type)
        {
            if (App.ConnectionStatus != ConnectionStatus.Connect) return;
            var selling = type != AnswerType.NoSelling;
            var onStock = type == AnswerType.Selling;

            SendAnswerRequest(selling, onStock);

            var text = (selling && onStock)
                ? "Запчасть имеется в наличии"
                : (selling || onStock)
                    ? "К сожалению, вашей запчасти нет в наличии, но мы готовы привести ее под заказ"
                    : "Нет в продаже";
            SendAnswerMessageRequest(text);

            var userInput = UserInput.Text.Trim();
            if(!userInput.Equals(string.Empty))
                SendAnswerMessageRequest(userInput);

            AppEventManager.NotificationAnswerEvent(Dialogue.RoomId);
            AppEventManager.CloseNotificationEvent();

        }
        private void SendAnswerRequest(bool selling, bool onStock)
        {
            var answerRequest = new AnswerRequest
            {
                id = Dialogue.QueryId,
                selling = selling,
                on_stock = onStock
            };
            var answerRequestToJson = JsonConvert.SerializeObject(answerRequest);
            AppWebSocketEventManager.SendObject(answerRequestToJson);
        }

        private void SendAnswerMessageRequest(string text)
        {
            var newMessage = new Message(0, text, "outgoing", Guid.NewGuid().ToString(),
               DateTime.Now.ToString(CultureInfo.InvariantCulture), "") { IsSuccessfully = false };
            var sendMessageRequest = new SendMessageRequest
            {
                room_id = Dialogue.RoomId,
                message = newMessage.Text,
                hash = newMessage.Hash,
                system = true
            };
            var sendMessageRequestToJson = JsonConvert.SerializeObject(sendMessageRequest);
            AppWebSocketEventManager.SendObject(sendMessageRequestToJson);
        }
    }
}