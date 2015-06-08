using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using ZappChat.Controls;
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
        public const double IntervalBetweenReshowNotificationInSecond = 300.0;
        public const double InterbalBetweenUpdateTryInSeconds = 10.0;

#if DEBUG
        public const string WebSocketUrl = "ws://zappchat.spyric.ru:7778";
        public const string UpdateFeedUrl = "http://zappchat.spyric.ru/program/feed.xml";
        public const string HelpUrl = "http://ekaterinburg.zappchat.spyric.ru/lost-password";
#else
        public const string WebSocketUrl = "ws://zappchat.ru:8888";
        public const string UpdateFeedUrl = "http://zappchat.ru/program/feed.xml";
        public const string HelpUrl = "http://ekaterinburg.zappchat.ru/lost-password";
#endif
        

        public static ConnectionStatus ConnectionStatus { get; set; }
        public static ulong LastLogId { get; set; }

        public static double RightMonitorBorder = SystemParameters.WorkArea.Right;
        public static double BottomMoniorBorder = SystemParameters.WorkArea.Bottom;

        private static MainWindow main;
        private static LoginWindow login;
        public static TaskbarIcon NotifyIcon { get; private set; }
        private static OpenedWindow currentWindow;

        private static DispatcherTimer reconnectionTimer;
        private static DispatcherTimer fileMonitore;
        private static DispatcherTimer updateTryTimer;
        private static DispatcherTimer updateCountersDispatcherTimer;

        public static Dictionary<ulong, string> DialoguesStatuses;

        private static INotification currentNotification;
        private static Queue<INotification> notifications;

        enum OpenedWindow
        {
            Login,
            Chat
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LastLogId = 0;
            FileDispetcher.InitializeFileDispetcher();
            DialoguesStatuses = FileDispetcher.ReadAllCollection(FileDispetcher.FullPathToDialogueInformation);
            login = new LoginWindow();
            main = new MainWindow();

#if !DEBUG
            AppUpdateManager.SetUrlRemoteServer(UpdateFeedUrl);
#endif
            var socketOpener = new BackgroundWorker();
            socketOpener.DoWork += (o, args) => AppWebSocketEventManager.OpenWebSocket();
            login.Show();
            currentWindow = OpenedWindow.Login;
            updateTryTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(InterbalBetweenUpdateTryInSeconds) };
            updateTryTimer.Tick += (o, args) => Update(socketOpener.RunWorkerAsync);
#if !DEBUG
            Update(socketOpener.RunWorkerAsync);
#else
            socketOpener.RunWorkerAsync();
#endif


            AppEventManager.Connect += o =>
            {
                ConnectionStatus = ConnectionStatus.Connect;
                reconnectionTimer.Stop();
            };
            AppEventManager.Disconnect += o =>
            {
                ConnectionStatus = ConnectionStatus.Disconnect;
                updateCountersDispatcherTimer.Stop();
                reconnectionTimer.Start();
            };
            AppEventManager.AuthorizationSuccess += AuthorizationSuccess;
            AppEventManager.AuthorizationFail += AuthorizationFail;
            AppEventManager.OpenDialogue += AppEventManagerOnOpenDialogue;
            AppEventManager.CloseNotification += AppEventManager_CloseNotification;
            AppEventManager.SetCarInfo += AppEventManager_SetCarInfo;

            InicializeNotyfication();
            notifications = new Queue<INotification>();

            reconnectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(IntervalBetweenConnectionInSeconds) };
            reconnectionTimer.Tick += (o, args) => AppWebSocketEventManager.OpenWebSocket();
            reconnectionTimer.Stop();

            fileMonitore = new DispatcherTimer { Interval = TimeSpan.FromSeconds(CheckingFilesIntervalInSeconds) };
            fileMonitore.Tick += (o, args) => Dispatcher.Invoke(new Action(FileDispetcher.CheckExistsFiles));
            fileMonitore.Start();

            updateCountersDispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(UpdateControlTimeIntervalInMinutes) };
            updateCountersDispatcherTimer.Tick += (o, args) => AppEventManager.UpdateCounterEvent();
        }

        private void AuthorizationSuccess(object sender, AuthorizationType type)
        {
            if (currentWindow == OpenedWindow.Login)
                SwitchWindow(OpenedWindow.Chat);
            updateCountersDispatcherTimer.Start();
        }

        private void AuthorizationFail(object sender, AuthorizationType type)
        {
            if (currentWindow == OpenedWindow.Chat)
                SwitchWindow(OpenedWindow.Login);
            updateCountersDispatcherTimer.Stop();
        }
        private static void ShowNotification(Window notificationWindow)
        {
            notificationWindow.Show();
        }
        private static void CloseNotification()
        {
            (currentNotification as Window).Close();
        }
        private static void CreateNotification(INotification newNotification)
        {
            if (currentNotification == null)
            {
                currentNotification = newNotification;
                ShowNotification(newNotification as Window);
            }
            else
            {
                if (currentNotification.Dialogue.Equals(newNotification.Dialogue))
                {
                    currentNotification = newNotification;
                    CloseNotification();
                    ShowNotification(newNotification as Window);
                    return;
                }
                if (!notifications.Contains(newNotification, Support.DialogueComparer))
                    notifications.Enqueue(newNotification);
                else
                {
                    var oldNotification =
                        notifications.First(x => Equals(newNotification.Dialogue.RoomId, x.Dialogue.RoomId));
                    notifications = Change(notifications, oldNotification, newNotification);
                }
            }
        }

        private void AppEventManager_SetCarInfo(ulong arg1, string arg2, string arg3, string arg4, string arg5)
        {
            var notify = notifications.FirstOrDefault(x => x.Dialogue.CarId == arg1);
            if (notify != null)
            {
                notify.SetCarInfo(arg2, arg3, arg4, arg5);
                notifications = Change(notifications, notify, notify);
                return;
            }
            if (currentNotification != null)
            {
                if (currentNotification.Dialogue.CarId == arg1)
                {
                    currentNotification.SetCarInfo(arg2, arg3, arg4, arg5);
                }
            }

        }

        public static void CreateQueryNotification(Dialogue dialogue)
        {
            var newNotification = new QueryNotificationWindow(dialogue, RightMonitorBorder, BottomMoniorBorder);
            CreateNotification(newNotification);
        }

        public static void CreateMessageNotification(Dialogue dialogue)
        {
            var newNotification = new MessageNotificationWindow(dialogue, RightMonitorBorder, BottomMoniorBorder);
            CreateNotification(newNotification);
        }

        private void AppEventManagerOnOpenDialogue(ulong id, List<Message> messages)
        {
            if (currentNotification != null)
                if (currentNotification.Dialogue.RoomId == id)
                    currentNotification.CloseNotify();
            if (notifications.Count != 0)
            {
                var notification = notifications.FirstOrDefault(x => x.Dialogue.RoomId == id);
                if (notification != null)
                    notifications = Remove(notifications, notification);
            }

        }
        private static Queue<INotification> Remove(Queue<INotification> queue, INotification removable)
        {
            if (removable == null) throw new NullReferenceException("Ссылка не ссылается на объект");
            if (!queue.Contains(removable, Support.DialogueComparer)) throw new ArgumentException("Переданного объекта нет в очереди");
            return new Queue<INotification>(queue.Where(notification => !notification.Dialogue.Equals(removable.Dialogue)));
        }

        private static Queue<INotification> Change(Queue<INotification> queue, INotification changing, INotification changed)
        {
            if (changed == null) throw new NullReferenceException("Ссылка не ссылается на объект");
            if (!queue.Contains(changing, Support.DialogueComparer)) throw new ArgumentException("Переданного объекта нет в очереди");
            var newQueue = new Queue<INotification>(queue.Count);
            foreach (var notification in queue)
            {
                newQueue.Enqueue(notification.Equals(changing) ? changed : notification);
            }
            return newQueue;
        }

        private void AppEventManager_CloseNotification()
        {
            CloseNotification();
            if (notifications.Count != 0)
            {
                currentNotification = notifications.Dequeue();
                ShowNotification(currentNotification as Window);
            }
            else
            {
                currentNotification = null;
            }
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
            if (currentWindow == window) return;
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
            if (currentWindow == OpenedWindow.Chat)
            {
                main.Show();
                main.WindowState = WindowState.Normal;
                main.Activate();
            }
            else
            {
                login.Show();
                login.WindowState = WindowState.Normal;
                login.Activate();
            }
        }

        private static void Update(Action callback)
        {
            login.Show();
            login.Activate();
            try
            {
                AppUpdateManager.StartupCheckAndPrepareUpdateFeeds(b =>
                {
                    if (b)
                    {
                        callback.Invoke();
                        updateTryTimer.Stop();
                        return;
                    }
                    MessageBox.Show("Ошибка в обновлении!");
                    login.Hide();
                    updateTryTimer.Start();
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Ошибка в обновлении!\n{0}", exception.Message));
                login.Hide();
                updateTryTimer.Start();
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
            if (!DialoguesStatuses.ContainsKey(roomId))
                DialoguesStatuses.Add(roomId, status);
            else
                DialoguesStatuses[roomId] = status;
            FileDispetcher.SynchronizeDialogueStatuses(DialoguesStatuses);
        }
    }
}
