using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZappChatLauncher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker _updater;
        public MainWindow()
        {
            InitializeComponent();
            _updater = new BackgroundWorker();
            _updater.DoWork += (sender, args) => BegingUpdateClient(UpdateCallBack);
            _updater.RunWorkerAsync();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimazeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BegingUpdateClient(Action callBack)
        {
            //@TODO предавать URL feed-а на сервере
            AppUpdateManager.SetUrlRemoteServer("URL сервера");
            AppUpdateManager.StartupCheckAndPrepareUpdateFeeds(callBack);
        }

        private void UpdateCallBack()
        {
            App.IsLastVersion = true;
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
    }
}
