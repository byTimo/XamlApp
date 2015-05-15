using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public LoginWindow()
        {
            InitializeComponent();
            AppEventManager.Connect += Connection;
            AppEventManager.Disconnect += Disconnection;
            LoginLabel.GotKeyboardFocus += (sender, args) =>  Keyboard.Focus(LoginTextBox);

        }

        private void Connection(object sender, ConnectionEventArgs e)
        {
            ConnectionView.Height = 0;
        }

        private void Disconnection(object sender, ConnectionEventArgs e)
        {
            ConnectionView.Height = 55;
        }

        public void AuthorizationResult(string status)
        {
            if(loginButton.LoginTry) loginButton.SwapState(false);
            switch (status)
            {
                case "ok":
                    //@TODO
                    AppEventManager.AuthorizationEvent(this, status);
                    break;
                case "fail":
                    AuthrizationFail();
                    break;
                case "error":
                    //@TODO <- сигнал о том, что беда на сервере :(
                    break;
                default:
                    //@TODO сделать свой Exception
                    throw new Exception("Неверный параметр поля JSon");
            }
        }

        private void AuthrizationFail()
        {
            LoginTextBox.Text = "";
            LoginLabel.Text = "Логин неверный";
            LoginLabel.Foreground = new SolidColorBrush(Color.FromRgb(239, 46, 46));
            LoginLabel.Visibility = Visibility.Visible;
            PasswordBox.AuthorizationFailReaction("Логин неверный", new SolidColorBrush(Color.FromRgb(239, 46, 46)));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void LoginTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            LoginLabel.Text = "E-mail";
            LoginLabel.Foreground = Brushes.Black;
            LoginLabel.Visibility = Visibility.Collapsed;
        }

        private void LoginTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (LoginTextBox.Text == String.Empty)
                LoginLabel.Visibility = Visibility.Visible;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as LoginButton;
            if (LoginTextBox.Text == "" || PasswordBox.GetPassword() == "" || button == null)
            {
                AuthrizationFail();
                return;
            }
            if (button.LoginTry) return;
            button.SwapState(true);
            var request = new AuthorizationLoginAndPasswordRequest
            {
                email = LoginTextBox.Text,
                password = Support.Base64FromString(
                    Support.XorEncoder(PasswordBox.GetPassword()))
            };
            var jsonString = JsonConvert.SerializeObject(request);
            AppWebSocketEventManager.SendObject(jsonString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var back = new BackgroundWorker();
            back.DoWork += ExecuteAfterWindowRender;
            back.RunWorkerAsync();
        }

        private void ExecuteAfterWindowRender(object sender, DoWorkEventArgs e)
        {
           AppWebSocketEventManager.OpenWebSocket();
           
           Dispatcher.Invoke(() =>
           {
               Loading.Visibility = Visibility.Collapsed;
               Authorization.Visibility = Visibility.Visible;
           });
           
        }
    }
}
