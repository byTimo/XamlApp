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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZappChat.Controls;

namespace ZappChat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HideButton_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BlueMenu.IsDeleteDialog = !BlueMenu.IsDeleteDialog;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Items.Add(new MessageControl {MessageText = "asdasdadasdadaивапвпвпвпвапвапвапвапвапвапвапвапвап", MessageDate = "161621621"});
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            messageButton.MessagesCount = text.Text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BlueMenu.SetStartupState();
            messageButton.SetStartupState();
        }
    }
}
