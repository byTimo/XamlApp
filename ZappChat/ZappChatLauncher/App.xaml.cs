using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ZappChatLauncher
{
    public partial class App : Application
    {
        public static bool IsLastVersion { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IsLastVersion = false;
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            var clientAssambly = Path.Combine(Environment.CurrentDirectory, "ZappChat.exe");
            if (!File.Exists(clientAssambly))
            {
                MessageBox.Show("Невозможно найти ZappChat.exe", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Process.Start(clientAssambly, IsLastVersion.ToString());
        }
    }
}
