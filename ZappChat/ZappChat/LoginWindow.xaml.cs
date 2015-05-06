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
using Newtonsoft.Json;
using ZappChat.Controls;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;

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
            AppEventManager.Authorization += AuthorizationResult;
        }

        private void AuthorizationResult(object sender, AuthorizationEventArgs e)
        {
            switch (e.Status)
            {
                case AuthorizationStatus.Ok:
                    //@TODO
                    break;
                case AuthorizationStatus.Fail:
                    //@TODO
                    break;
                case AuthorizationStatus.Error:
                    //@TODO
                    break;
            }
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
            var button = sender as LoginButton;
            if(LoginTextBox.Text == "" || PasswordBox.GetPassword() == "" || button == null) return;
            if (button.LoginTry) return;
            button.SwapState(true);
            var request = new AuthorizationLoginAndPasswordRequest
            {
                email = LoginTextBox.Text,
                password = Support.Base64FromString(
                Support.XorEncoder(PasswordBox.GetPassword()))
            };
            var jsonString = JsonConvert.SerializeObject(request);
            ZappChatSocketEventManager.SendObject(jsonString);
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
