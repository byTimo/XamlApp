using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ZappChat.Controls;
using ZappChat.Core.Socket;

namespace ZappChat
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool test = false;
        private bool loginEmpty = true;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimazeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!loginEmpty) return;
            (sender as TextBox).Text = "";
            loginEmpty = false;
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = "E-mail";
                loginEmpty = true;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            test = !test;
            (sender as LoginButton).SwapState(test);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(ExecuteAfterWindowRender),
                DispatcherPriority.ContextIdle, null);
        }

        private void ExecuteAfterWindowRender()
        {
            if (ZappChatSocketEventManager.OpenWebSocket())
            {
                Loading.Visibility = Visibility.Collapsed;
                Authorization.Visibility = Visibility.Visible;
            }
        }
    }
}
