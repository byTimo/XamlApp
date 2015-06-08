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
    /// Логика взаимодействия для MessageNotificationWindow.xaml
    /// </summary>
    public partial class MessageNotificationWindow : Window, INotification
    {

        private DispatcherTimer reshowNotification;
        public Dialogue Dialogue { get; set; }

        public NotificationType Type { get; set; }

        public static readonly DependencyProperty NotificationTextProperty = DependencyProperty.Register(
            "NotificationText", typeof(string), typeof(MessageNotificationWindow), new FrameworkPropertyMetadata("Title"));

        public string NotificationText
        {
            get { return GetValue(NotificationTextProperty) as string; }
            set { SetValue(NotificationTextProperty, value); }

        }
        public MessageNotificationWindow()
        {
            InitializeComponent();
        }
        public MessageNotificationWindow(Dialogue dialogue, double rightMonitorBorder, double bottomMonitorBorders)
            : this()
        {
            Dialogue = dialogue;
            Type = NotificationType.Message;
            NotificationText = dialogue.GetTitleMessage();
            Left = rightMonitorBorder - Width - 10;
            Top = bottomMonitorBorders - Height - 10;
            reshowNotification = new DispatcherTimer { Interval = TimeSpan.FromSeconds(App.IntervalBetweenReshowNotificationInSecond) };
            reshowNotification.Tick += (s, e) =>
            {
                reshowNotification.Stop();
                AppEventManager.ReshowNotificationEvent(Dialogue, Type);
            };
            reshowNotification.Stop();
        }

        public void CloseNotify(bool isOpened)
        {
            if(!isOpened)
                reshowNotification.Start();
            AppEventManager.CloseNotificationEvent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseNotify(false);
        }

        private void CornerRadiusButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.ConnectionStatus == ConnectionStatus.Connect)
            {
                var historyRequest = new HistoryRequest
                {
                    from = null,
                    to = null,
                    chat_room_id = Dialogue.RoomId
                };
                var historyRequestToJson = JsonConvert.SerializeObject(historyRequest);
                AppWebSocketEventManager.SendObject(historyRequestToJson);
                App.ShowCurrentWindow();
            }
            CloseNotify(true);
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CornerRadiusButton_Click(null, null);
        }

        public void SetCarInfo(string brand, string model, string vin, string year)
        {
            Dialogue.SetCarInformation(brand, model, vin, year);
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}