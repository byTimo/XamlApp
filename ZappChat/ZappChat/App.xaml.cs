﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
#if DEBUG
        public const double IntervalBetweenReshowNotificationInSecond = 5.0;
#else
        public const double IntervalBetweenReshowNotificationInSecond = 300.0;
#endif
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


        public static MainWindow MainWin;
        public static LoginWindow LoginWin;
        public static TaskbarIcon Taskbar { get; private set; }
        private static OpenedWindow currentWindow;

        private static DispatcherTimer reconnectionTimer;
        private static DispatcherTimer fileMonitore;
        private static DispatcherTimer updateTryTimer;
        private static DispatcherTimer updateCountersDispatcherTimer;

        public static Dictionary<long, string> DialoguesStatuses;

        enum OpenedWindow
        {
            Login,
            Chat
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FileDispetcher.InitializeFileDispetcher();
            var settingValueToString = FileDispetcher.GetSetting("logId");
            LastLogId = settingValueToString == null ? 0 : ulong.Parse(settingValueToString);
            DialoguesStatuses = FileDispetcher.ReadAllCollection(FileDispetcher.FullPathToDialogueInformation);
            LoginWin = new LoginWindow();
            MainWin = new MainWindow();

#if !DEBUG
            AppUpdateManager.SetUrlRemoteServer(UpdateFeedUrl);
#endif
            var socketOpener = new BackgroundWorker();
            socketOpener.DoWork += SocketOpenerOnDoWork;
            LoginWin.Show();
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
            AppEventManager.AnswerOnQuery += AppEventManagerOnAnswerOnQuery;
            AppEventManager.DeleteDialogue +=
                (o, args) => AppEventManager.CloseNotificationEvent(args.DeletedDialogue.RoomId, false);

            InicializeTaskbar();

            reconnectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(IntervalBetweenConnectionInSeconds) };
            reconnectionTimer.Tick += (o, args) => AppWebSocketEventManager.OpenWebSocket();
            reconnectionTimer.Stop();

            fileMonitore = new DispatcherTimer { Interval = TimeSpan.FromSeconds(CheckingFilesIntervalInSeconds) };
            fileMonitore.Tick += (o, args) => Dispatcher.Invoke(new Action(FileDispetcher.CheckExistsFiles));
            fileMonitore.Start();

            updateCountersDispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(UpdateControlTimeIntervalInMinutes) };
            updateCountersDispatcherTimer.Tick += (o, args) => AppEventManager.UpdateCounterEvent();
        }

        private void AppEventManagerOnAnswerOnQuery(long roomId)
        {
            var dialogue = MainWin.Dialogues.DialogueWithQuery.FirstOrDefault(d => d.Dialogue.RoomId == roomId);
            if (dialogue != null)
                AppEventManager.CloseNotificationEvent(dialogue.Dialogue.RoomId, false);
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

        private void AppEventManagerOnOpenDialogue(long id, List<Message> messages)
        {
            var notification = AppNotificationManager.GetNotificationOnRoomId(id);
            if(notification == null) return;

            AppEventManager.CloseNotificationEvent(id, notification.Type != NotificationType.Message);
        }

        private void AppEventManager_CloseNotification(long id, bool flag)
        {
            if (flag)
                AppNotificationManager.CloseNotificationWithReshow(id);
            else
                AppNotificationManager.CloseNotificationWithoutReshow(id);
        }

        private void SocketOpenerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            DialogueStore.LoadDbInformation();
            AppWebSocketEventManager.OpenWebSocket();
        }

        private void InicializeTaskbar()
        {
            var showCommand = new ShowWindowCommand();
            var appCloseCommand = new CloseApplicationCommand();
            var iconUri = new Uri("pack://application:,,,/Images/icon.ico", UriKind.RelativeOrAbsolute);
            var icon = BitmapFrame.Create(iconUri);
            Taskbar = new TaskbarIcon
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
                    MainWin.Hide();
                    LoginWin.Show();
                    currentWindow = OpenedWindow.Login;
                    break;
                case OpenedWindow.Chat:
                    LoginWin.Hide();
                    MainWin.Show();
                    currentWindow = OpenedWindow.Chat;
                    break;

            }
        }
        public static void ShowCurrentWindow()
        {
            if (currentWindow == OpenedWindow.Chat)
            {
                MainWin.Show();
                MainWin.WindowState = WindowState.Normal;
                MainWin.Activate();
            }
            else
            {
                LoginWin.Show();
                LoginWin.WindowState = WindowState.Normal;
                LoginWin.Activate();
            }
        }

        private static void Update(Action callback)
        {
            LoginWin.Show();
            LoginWin.Activate();
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
                    LoginWin.Hide();
                    updateTryTimer.Start();
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Ошибка в обновлении!\n{0}", exception.Message));
                LoginWin.Hide();
                updateTryTimer.Start();
            }
        }
        public static bool IsThisDialogueDeleted(long roomId)
        {
            return DialoguesStatuses.Any(x => x.Key == roomId && x.Value == "d");
        }

        public static bool IsThisUnreadMessage(long roomId, long messageId)
        {
            if (!DialoguesStatuses.ContainsKey(roomId)) return true;
            if (!char.IsDigit(DialoguesStatuses[roomId][0])) return false;
            return long.Parse(DialoguesStatuses[roomId]) < messageId;
        }

        public static void ChangeDialogueStatus(long roomId, string status)
        {
            if (!DialoguesStatuses.ContainsKey(roomId))
                DialoguesStatuses.Add(roomId, status);
            else
                DialoguesStatuses[roomId] = status;
            FileDispetcher.SynchronizeDialogueStatuses(DialoguesStatuses);
        }
    }
}
