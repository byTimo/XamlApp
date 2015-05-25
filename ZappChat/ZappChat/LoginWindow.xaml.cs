using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
            AppEventManager.Connect += Connection; //Активация элементов управления или событий внутри окна
            AppEventManager.Disconnect += Disconnection; //Деактивация элементов управления
            AppEventManager.AuthorizationSuccess += ReactionOnAuthorization;
            AppEventManager.AuthorizationFail += ReactionOnFailAuthorization;
            LoginLabel.GotKeyboardFocus += (sender, args) =>  Keyboard.Focus(LoginTextBox);

        }

        public void ChangeState(bool isLoading)
        {
            Authorization.Visibility = !isLoading ? Visibility.Visible : Visibility.Hidden;
            Loading.Visibility = isLoading ? Visibility.Hidden : Visibility.Visible;
        }

        private void Connection(object sender)
        {
            ConnectionView.Height = 0;
        }

        private void Disconnection(object sender)
        {
            ConnectionView.Height = 55;
            ChangeState(false);
        }

        private void ReactionOnAuthorization(object sender, AuthorizationType type)
        {
            LoginTextBox.Text = "";
            LoginLabel.Text = "E-mail";
            LoginLabel.Visibility = Visibility.Visible;
            PasswordBox.ResetValues();
            loginButton.SwapState(false);
        }

        private void ReactionOnFailAuthorization(object sender, AuthorizationType type)
        {
            if (type == AuthorizationType.Login)
                AuthorizationFail();
            if(type == AuthorizationType.Token)
                ChangeState(false);
        }

        private void AuthorizationFail()
        {
            LoginTextBox.Text = "";
            LoginLabel.Text = "Логин неверный";
            LoginLabel.Foreground = new SolidColorBrush(Color.FromRgb(239, 46, 46));
            LoginLabel.Visibility = Visibility.Visible;
            PasswordBox.AuthorizationFailReaction("Логин неверный", new SolidColorBrush(Color.FromRgb(239, 46, 46)));
            if(loginButton.LoginTry) loginButton.SwapState(false);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
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
           SendLoginAndPassword();
        }

        public void SendLoginAndPassword()
        {
            if (App.ConnectionStatus != ConnectionStatus.Connect) return;
            if (LoginTextBox.Text == "" || PasswordBox.GetPassword() == "" )
            {
                AuthorizationFail();
                return;
            }
            if (loginButton.LoginTry) return;
            loginButton.SwapState(true);
            var request = new AuthorizationLoginAndPasswordRequest
            {
                email = LoginTextBox.Text,
                password = Support.Base64FromString(
                    Support.XorEncoder(PasswordBox.GetPassword()))
            };
            var jsonString = JsonConvert.SerializeObject(request);
            AppWebSocketEventManager.SendObject(jsonString); 
        }

        private void PasswordBox_OnEnterPress(object sender, RoutedEventArgs e)
        {
            SendLoginAndPassword();
        }
    }
}
