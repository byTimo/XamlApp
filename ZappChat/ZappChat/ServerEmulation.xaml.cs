using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ZappChat.Core;

namespace ZappChat
{
    /// <summary>
    /// Логика взаимодействия для ServerEmulation.xaml
    /// </summary>
    public partial class ServerEmulation : Window
    {
        public ServerEmulation()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AppEventManager.ConnectionEvent(this,AppStatus.Connect);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AppEventManager.ConnectionEvent(this,AppStatus.Disconnect);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var m = new Message(author.Text, message.Text);
            AppEventManager.SendMessageEvent(this, m);
        }
    }
}
