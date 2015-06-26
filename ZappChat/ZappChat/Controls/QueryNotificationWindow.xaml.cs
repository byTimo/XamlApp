using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;
using System.Globalization;
using System.Windows.Threading;

namespace ZappChat.Controls
{
    /// <summary>
    /// Логика взаимодействия для QueryNotificationWindow.xaml
    /// </summary>
    public partial class QueryNotificationWindow : Window, INotification
    {
        public DispatcherTimer ReshowTimer { get; set; }
        public Dialogue Dialogue { get; set; }
        public NotificationType Type { get; set; }

        public static readonly DependencyProperty NotificationTextProperty = DependencyProperty.Register(
            "NotificationText", typeof(string), typeof(QueryNotificationWindow), new FrameworkPropertyMetadata("Title"));

        public static readonly DependencyProperty NotificationVinProperty =
            DependencyProperty.Register("NotificationVin", typeof(string), typeof(QueryNotificationWindow),
                new FrameworkPropertyMetadata(""));

        public static readonly DependencyProperty NotificationYearProperty =
            DependencyProperty.Register("NotificationYear", typeof(string), typeof(QueryNotificationWindow),
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

        public QueryNotificationWindow()
        {
            InitializeComponent();
        }
        public QueryNotificationWindow(Dialogue dialogue, double rightMonitorBorder, double bottomMonitorBorders)
            : this()
        {
            Dialogue = dialogue;
            Type = NotificationType.Query;
            Left = rightMonitorBorder - Width - 10;
            Top = bottomMonitorBorders - Height - 10;
            NotificationText = dialogue.GetTitleMessage();
            NotificationText += " " + (dialogue.CarBrand ?? "");
            NotificationText += " " + (dialogue.CarModel ?? "");

            NotificationText = NotificationText.Trim();

            NotificationVin = dialogue.VIN ?? "";
            NotificationYear = dialogue.Year ?? "";
            
            Label.MouseDown += (sender, args) =>
            {
                Keyboard.Focus(Label);
            };
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
            ReshowTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(App.IntervalBetweenReshowNotificationInSecond) };
            ReshowTimer.Tick += (s, e) =>
            {
                if(!App.MainWin.IsEqualChatDialogue(Dialogue))
                {
                    ReshowTimer.Stop();
                    AppEventManager.ReshowNotificationEvent(Dialogue.RoomId);
                }
                if (!App.IsThisUnreadMessage(Dialogue.RoomId, 0))
                    ReshowTimer.Stop();
            };
        }

        public void SetCarInfo(string brand, string model, string vin, string year)
        {
            Dialogue.SetCarInformation(brand, model, vin, year);
            string carBrandModel = brand + " " + model;
            if (NotificationText.IndexOf(carBrandModel) == -1)
                NotificationText += " " + carBrandModel;

            if (vin == null)
                NotificationVin = "";
            else
                NotificationVin = "VIN: " + vin;
            NotificationYear = year ?? "";
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            AppEventManager.CloseNotificationEvent(Dialogue.RoomId, true);
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
            AppEventManager.CloseNotificationEvent(Dialogue.RoomId, true);
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
            if (!userInput.Equals(string.Empty))
                SendAnswerMessageRequest(userInput);

            AppEventManager.NotificationAnswerEvent(Dialogue.RoomId);
            AppEventManager.CloseNotificationEvent(Dialogue.RoomId, false);

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
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
        private void TextBoxGotMouseCapture(object sender, MouseEventArgs mouseEventArgs)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
