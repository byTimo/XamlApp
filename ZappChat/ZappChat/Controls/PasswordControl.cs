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
    [TemplatePart(Name = "ShowPasswordButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "Password", Type = typeof(PasswordBox))]
    [TemplatePart(Name = "Text", Type = typeof(TextBox))]
    [TemplatePart(Name = "Label", Type = typeof(TextBox))]
    public class PasswordControl : Control
    {
        private PasswordBox _password;
        private TextBox _text;
        private ToggleButton _showButton;
        private TextBox _label;
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

        public static readonly DependencyProperty SecretProperty = DependencyProperty.Register("Secret", typeof (string),
            typeof (PasswordControl), new FrameworkPropertyMetadata(""));

        private string Secret
        {
            get { return GetValue(SecretProperty) as string; }
            set { SetValue(SecretProperty, value); }
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
            _label = GetTemplateChild("Label") as TextBox;
            _label.GotKeyboardFocus += (sender, args) =>
            {
                if (!ViewPassword)
                {
                    Keyboard.Focus(_password);
                }
            };
            _password.GotKeyboardFocus += (sender, args) =>
            {
                _label.Text = "Пароль";
                _label.Foreground = new SolidColorBrush(Color.FromRgb(141, 141, 141));
                _label.Visibility = Visibility.Collapsed;
            };
            _password.LostKeyboardFocus += (sender, args) =>
            {
                if (_password.Password == string.Empty)
                {
                    _label.Visibility = Visibility.Visible;
                }
            };
            _showButton.Checked += (sender, args) =>
            {
                _text.Text = _password.Password;
                _password.Visibility =Visibility.Collapsed;
                _text.Visibility = Visibility.Visible;
                _label.Visibility = Visibility.Collapsed;
            };
            _showButton.Unchecked += (sender, args) =>
            {
                _password.Password = _text.Text;
                _password.Visibility = Visibility.Visible;
                _text.Visibility =Visibility.Collapsed;
                if(_password.Password == string.Empty)
                    _label.Visibility = Visibility.Visible;
            };
        }

        public void AuthorizationFailReaction(string messageText, Brush messageBrush)
        {
            ResetValues();
            _label.Text = messageText;
            _label.Foreground = messageBrush;
        }

        public void ResetValues()
        {
            _password.Visibility = Visibility.Visible;
            _label.Visibility = Visibility.Visible;
            _text.Visibility = Visibility.Collapsed;
            ViewPassword = false;
            _text.Text = "";
            _password.Password = "";
            _label.Text = "Пароль";
            _label.Foreground = new SolidColorBrush(Color.FromRgb(141, 141, 141));
        }
        public string GetPassword()
        {
            return ViewPassword ? _text.Text : _password.Password;
        }
    }
}
