using System;
using System.Collections.Generic;
using System.Text;
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
using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;

namespace ZappChat
{
    /// <summary>
    /// Логика взаимодействия для Notification.xaml
    /// </summary>
    public partial class Notification : UserControl
    {
        public Dialogue Dialogue { get; set; }
        private readonly Brush _startBrush;
        private DispatcherTimer closeTimer;

        public static readonly DependencyProperty NotificationTextProperty = DependencyProperty.Register(
            "NotificationText", typeof (string), typeof (Notification), new FrameworkPropertyMetadata("Title"));

        public string NotificationText
        {
            get { return GetValue(NotificationTextProperty) as string; }
            set { SetValue(NotificationTextProperty, value); }

        }

        public Notification()
        {
            this.InitializeComponent();
        }

        public Notification(Dialogue dialogue) : this()
        {
            Dialogue = dialogue;
            NotificationText = dialogue.GetTitleMessage();
            _startBrush = MainBorder.Background;
            closeTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(App.NotificationCloseTimeInSeconds)};
            closeTimer.Tick += (sender, args) => CloseNotify();
            if (App.IsCurrentWindowVisible())
                closeTimer.Start();

        }

        private void CloseNotify()
        {
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            if(taskbarIcon != null)
                taskbarIcon.CloseBalloon();
        }

    private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            MainBorder.Background = Brushes.Black;
        }

        private void MainBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            MainBorder.Background = _startBrush;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseNotify();
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
            }
            CloseNotify();
        }
	}
}