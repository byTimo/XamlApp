using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            login = new LoginWindow();
            main = new MainWindow();
            AppEventManager.Authorization += SwitchWindow;
            AppSocketEventManager.MainWindow = main;
            AppSocketEventManager.Login = login;

            login.ShowDialog();
        }

        private void SwitchWindow(object sender, WebSocketEventArgs e)
        {
            //@TODO
            if (!AppSocketEventManager.IsChat)
            {
                login.Close();
                main.ShowDialog();
                AppSocketEventManager.IsChat = false;
            }
            else
            {
                main.Close();
                login.ShowDialog();
                AppSocketEventManager.IsChat = false;
            }
        }
    }
}
