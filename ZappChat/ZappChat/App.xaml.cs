﻿using System;
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
    }
}
