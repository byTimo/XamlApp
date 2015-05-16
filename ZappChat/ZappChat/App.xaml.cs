using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
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
        private const double IntervalBetweenConnectionInSeconds = 1.5;
        private const double CheckingFilesIntervalInSeconds = 2.0;

        public static ConnectionStatus ConnectionStatus { get; set; }
        
        private static MainWindow main;
        private static LoginWindow login;
        private static OpenedWindow currentWindow;

        private static DispatcherTimer reconnectionTimer;
        private static DispatcherTimer fileMonitore;

        enum OpenedWindow
        {
            Login,
            Chat
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FileDispetcher.InitializeFileDispetcher();
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

            reconnectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(IntervalBetweenConnectionInSeconds) };
            reconnectionTimer.Tick += (o, args) => AppWebSocketEventManager.OpenWebSocket();
            reconnectionTimer.Stop();

            fileMonitore = new DispatcherTimer { Interval = TimeSpan.FromSeconds(CheckingFilesIntervalInSeconds) };
            fileMonitore.Tick += (o, args) => Dispatcher.Invoke(FileDispetcher.CheckExistsFiles);
            fileMonitore.Start();

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

        //public static void StartTimerToPingRequest()
        //{
        //    var time = new TimeSpan();
        //    pingRequestTime.Interval = TimeSpan.FromMilliseconds(50);
        //    pingRequestTime.Tick += (sender, args) =>
        //    {
        //        if (PingRequestSuccses)
        //        {
        //            pingRequestTime.Stop();
        //            AppEventManager.ConnectionEvent(pingRequestTime, ConnectionStatus.Connect);
        //        }
        //        if(!PingRequestSuccses && time.Seconds == 2)
        //            AppEventManager.ConnectionEvent(pingRequestTime,ConnectionStatus.Disconnect);
        //        time += PingInterval;
        //    };
        //    pingRequestTime.Start();
        //}
    }
}
