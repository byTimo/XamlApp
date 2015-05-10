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
        private static DispatcherTimer pingRequestTime;

        public static bool PingRequestSuccses { get; set; }
        public static readonly TimeSpan PingInterval = new TimeSpan(0,0,0,5);


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            login = new LoginWindow();
            main = new MainWindow();
            pingRequestTime = new DispatcherTimer();
            AppEventManager.Authorization += SwitchWindow;
            AppWebSocketEventManager.MainWindow = main;
            AppWebSocketEventManager.Login = login;

            login.ShowDialog();
        }

        private void SwitchWindow(object sender, WebSocketEventArgs e)
        {
            //@TODO
            if (!AppWebSocketEventManager.IsChat)
            {
                login.Close();
                main.ShowDialog();
                AppWebSocketEventManager.IsChat = false;
            }
            else
            {
                main.Close();
                login.ShowDialog();
                AppWebSocketEventManager.IsChat = true;
            }
        }

        public static void StartTimerToPingRequest()
        {
            var time = new TimeSpan();
            pingRequestTime.Interval = TimeSpan.FromMilliseconds(50);
            pingRequestTime.Tick += (sender, args) =>
            {
                if (PingRequestSuccses)
                {
                    pingRequestTime.Stop();
                    AppEventManager.ConnectionEvent(pingRequestTime, ConnectionStatus.Connect);
                }
                if(!PingRequestSuccses && time.Seconds == 2)
                    AppEventManager.ConnectionEvent(pingRequestTime,ConnectionStatus.Disconnect);
                time += PingInterval;
            };
            pingRequestTime.Start();

        }
    }
}
