using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using ZappChat.Core;
using ZappChat.Core.Socket;

namespace ZappChat
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App : Application
    {
        public const double IntervalBetweenConnectionInSeconds = 1.5;
        public const double CheckingFilesIntervalInSeconds = 2.0;
        public const double UpdateControlTimeIntervalInMinutes = 2.0;
        public const double NotificationCloseTimeInSeconds = 5.0;

        public static ConnectionStatus ConnectionStatus { get; set; }
        public static ulong LastLogId { get; set; }
        
        private static MainWindow main;
        private static LoginWindow login;
        public static TaskbarIcon NotifyIcon { get; private set; }
        private static OpenedWindow currentWindow;

        private static DispatcherTimer reconnectionTimer;
        private static DispatcherTimer fileMonitore;
        private static DispatcherTimer updateCountersDispatcherTimer;

        public static Dictionary<ulong, string> DialoguesStatuses;

        enum OpenedWindow
        {
            Login,
            Chat
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LastLogId = 0;
            FileDispetcher.InitializeFileDispetcher();
            DialoguesStatuses = FileDispetcher.ReadAllCollection(FileDispetcher.FullPasthToDialogueInformation);
            login = new LoginWindow();
            main = new MainWindow();

            AppEventManager.Connect += o =>
            {
                ConnectionStatus = ConnectionStatus.Connect;
                reconnectionTimer.Stop();
            };
            AppEventManager.Disconnect += o =>
            {
                ConnectionStatus = ConnectionStatus.Disconnect;
                reconnectionTimer.Start();
            };
            AppEventManager.AuthorizationSuccess += AuthorizationSuccess;
            AppEventManager.AuthorizationFail += AuthorizationFail;

            AppWebSocketEventManager.MainWindow = main;
            AppWebSocketEventManager.Login = login;
            InicializeNotyfication();

            reconnectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(IntervalBetweenConnectionInSeconds) };
            reconnectionTimer.Tick += (o, args) => AppWebSocketEventManager.OpenWebSocket();
            reconnectionTimer.Stop();

            fileMonitore = new DispatcherTimer { Interval = TimeSpan.FromSeconds(CheckingFilesIntervalInSeconds) };
            fileMonitore.Tick += (o, args) => Dispatcher.Invoke(FileDispetcher.CheckExistsFiles);
            fileMonitore.Start();

            updateCountersDispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromMinutes(UpdateControlTimeIntervalInMinutes)};
            updateCountersDispatcherTimer.Tick += (o, args) => AppEventManager.UpdateCounterEvent();
            updateCountersDispatcherTimer.Start();

            var socketOpener = new BackgroundWorker();
            socketOpener.DoWork += (o, args) => AppWebSocketEventManager.OpenWebSocket();
            login.Show();
            currentWindow = OpenedWindow.Login;
            socketOpener.RunWorkerAsync();
        }

        private void AuthorizationSuccess(object sender, AuthorizationType type)
        {
            if(currentWindow == OpenedWindow.Login)
                SwitchWindow(OpenedWindow.Chat);
        }

        private void AuthorizationFail(object sender, AuthorizationType type)
        {
            if(currentWindow == OpenedWindow.Chat)
                SwitchWindow(OpenedWindow.Login);
        }

        public static void CreateNotification(Dialogue dialogue)
        {
            NotifyIcon.CloseBalloon();
            var balloon = new Notification(dialogue);
            NotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Fade, null);
        }

        private void InicializeNotyfication()
        {
            var showCommand = new ShowWindowCommand();
            var appCloseCommand = new CloseApplicationCommand();
            var iconUri = new Uri("pack://application:,,,/Images/icon.ico", UriKind.RelativeOrAbsolute);
            var icon = BitmapFrame.Create(iconUri);
            NotifyIcon = new TaskbarIcon
            {
                IconSource = icon,
                ToolTipText = "ZappChat",
                DoubleClickCommand = showCommand,
                ContextMenu = new ContextMenu
                {
                    Items =
                    {
                        new MenuItem {Header = "Показать", Command = showCommand},
                        new MenuItem {Header = "Закрыть", Command = appCloseCommand}
                    }
                }
            };
        }
        private void SwitchWindow(OpenedWindow window)
        {
            if(currentWindow == window) return;
            switch (window)
            {
                case OpenedWindow.Login:
                    main.Hide();
                    login.Show();
                    currentWindow = OpenedWindow.Login;
                    break;
                case OpenedWindow.Chat:
                    login.Hide();
                    main.Show();
                    currentWindow = OpenedWindow.Chat;
                    break;

            }
        }

        public static void ShowCurrentWindow()
        {
//@TODO ---------------- что нибудь по адекватней, разворачивается там, на передний план и тд -------------------
            if (currentWindow == OpenedWindow.Chat)
            {
                main.Show();
                main.Activate();
            }
            else
            {
                login.Show();
            }
        }

        public static bool IsCurrentWindowVisible()
        {
            return currentWindow == OpenedWindow.Chat ? main.IsVisible : login.IsVisible;
        }
        public static bool IsThisDialogueDeleted(ulong roomId)
        {
            return DialoguesStatuses.Any(x => x.Key == roomId && x.Value == "d");
        }

        public static bool IsThisUnreadMessage(ulong roomId, ulong messageId)
        {
            if (!DialoguesStatuses.ContainsKey(roomId)) return true;
            if (!char.IsDigit(DialoguesStatuses[roomId][0])) return false;
            return ulong.Parse(DialoguesStatuses[roomId]) < messageId;
        }

        public static void ChangeDialogueStatus(ulong roomId, string status)
        {
            if(!DialoguesStatuses.ContainsKey(roomId))
                DialoguesStatuses.Add(roomId, status);
            else
                DialoguesStatuses[roomId] = status;
            FileDispetcher.SynchronizeDialogueStatuses(DialoguesStatuses);
        }
    }
}
