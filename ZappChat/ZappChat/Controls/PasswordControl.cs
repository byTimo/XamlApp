using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZappChat.Controls
{
    [TemplatePart(Name = "ShowPasswordButton", Type = typeof(ToggleButton)),
     TemplatePart(Name = "Password", Type = typeof(PasswordBox)),
     TemplatePart(Name = "Text", Type = typeof(TextBox))]
    public class PasswordControl : Control
    {
        private PasswordBox _password;
        private TextBox _text;
        private ToggleButton _showButton;
        private string _secret;
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
            typeof (CornerRadius), typeof (PasswordControl));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty ViewPasswordProperty = DependencyProperty.Register("ViewPassword", typeof (bool),
            typeof (PasswordControl), new FrameworkPropertyMetadata(false));

        public bool ViewPassword
        {
            get { return (bool) GetValue(ViewPasswordProperty); }
            set { SetValue(ViewPasswordProperty, value); }
        }
        static PasswordControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (PasswordControl),
                new FrameworkPropertyMetadata(typeof (PasswordControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _password = GetTemplateChild("Password") as PasswordBox;
            _showButton = GetTemplateChild("ShowPasswordButton") as ToggleButton;
            _text = GetTemplateChild("Text") as TextBox;
            _text.GotKeyboardFocus += (sender, args) =>
            {
                if (!ViewPassword)
                {
                    _text.Text = "Пароль";
                    _text.Visibility = Visibility.Hidden;
                    Keyboard.Focus(_password);
                }
            };
            _text.TextChanged += (sender, args) => { _secret = _text.Text; };
            _password.LostKeyboardFocus += (sender, args) =>
            {
                if (_password.Password == string.Empty)
                {
                    _text.Visibility = Visibility.Visible;
                }
            };
            _password.PasswordChanged += (sender, args) => { _secret = _password.Password; };
            _showButton.Checked += (sender, args) =>
            {
                ViewPassword = true;
                _text.Visibility = Visibility.Visible;
                _password.Visibility = Visibility.Collapsed;
                _text.Text = _secret;
            };
            _showButton.Unchecked += (sender, args) =>
            {
                ViewPassword = false;
                _text.Visibility = Visibility.Collapsed;
                _password.Visibility = Visibility.Visible;
                _password.Password = _secret;
            };
        }
    }
}
