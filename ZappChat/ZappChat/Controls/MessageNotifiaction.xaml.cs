using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;

namespace ZappChat.Controls
{
    /// <summary>
    /// Логика взаимодействия для MessageNotification.xaml
    /// </summary>
    public partial class MessageNotification : UserControl, INotification
    {
        public Dialogue Dialogue { get; set; }

        public static readonly DependencyProperty NotificationTextProperty = DependencyProperty.Register(
            "NotificationText", typeof(string), typeof(MessageNotification), new FrameworkPropertyMetadata("Title"));

        public string NotificationText
        {
            get { return GetValue(NotificationTextProperty) as string; }
            set { SetValue(NotificationTextProperty, value); }

        }

        public MessageNotification()
        {
            this.InitializeComponent();
        }

        public MessageNotification(Dialogue dialogue)
            : this()
        {
            Dialogue = dialogue;
            NotificationText = dialogue.GetTitleMessage();
        }

        public void CloseNotify()
        {
//            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
//            if (taskbarIcon != null)
//                taskbarIcon.CloseBalloon();
            AppEventManager.CloseNotificationEvent();
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
                App.ShowCurrentWindow();
            }
            CloseNotify();
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CornerRadiusButton_Click(null, null);
        }

        public void SetCarInfo(string brand, string model, string vin, string year)
        {
            Dialogue.SetCarInformation(brand, model, vin, year);
        }
    }
}