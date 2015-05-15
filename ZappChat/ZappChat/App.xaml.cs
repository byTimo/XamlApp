using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ZappChat.Core;
using ZappChat.Core.Socket;

namespace ZappChat
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        private static MainWindow main;
        private static LoginWindow login;
        private const double IntervalBetweenConnection = 1.5;
        private static DispatcherTimer reconnectionTimer;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            login = new LoginWindow();
            main = new MainWindow();
            AppEventManager.Connect += (o, args) => reconnectionTimer.Stop();
            AppEventManager.Authorization += SwitchWindow;
            AppEventManager.Disconnect += (o, args) => reconnectionTimer.Start();
            AppEventManager.Reauthorization += SwitchWindow;

            AppWebSocketEventManager.MainWindow = main;
            AppWebSocketEventManager.Login = login;
            reconnectionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(IntervalBetweenConnection)
            };
            reconnectionTimer.Tick += (o, args) => AppWebSocketEventManager.OpenWebSocket();
            reconnectionTimer.Start();

            login.Show();
        }
        private void SwitchWindow(object sender, WebSocketEventArgs e)
        {
            //@TODO
            if (!AppWebSocketEventManager.IsChat)
            {
                login.Visibility = Visibility.Collapsed;
                main.Show();
                AppWebSocketEventManager.IsChat = true;
            }
            else
            {
                main.Visibility = Visibility.Collapsed;

                login.Show();
                AppWebSocketEventManager.IsChat = false;
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
