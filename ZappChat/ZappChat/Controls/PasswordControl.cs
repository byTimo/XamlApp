using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

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

        public static readonly RoutedEvent EnterPressEvent = EventManager.RegisterRoutedEvent("EnterPress",
            RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (PasswordControl));

        public event RoutedEventHandler EnterPress
        {
            add { AddHandler(EnterPressEvent, value); }
            remove { RemoveHandler(EnterPressEvent, value); }
        }

        void RaiseEnterPress()
        {
            var newEventArgs  = new RoutedEventArgs(EnterPressEvent);
            RaiseEvent(newEventArgs);
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
                if (ViewPassword)
                {
                    Keyboard.Focus(_text);
                }
                else
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
            _text.GotKeyboardFocus += (sender, args) =>
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
                ViewPassword = true;
                _text.Text = _password.Password;
                _password.Visibility = Visibility.Collapsed;
                _text.Visibility = Visibility.Visible;
                _label.Visibility = Visibility.Collapsed;
            };
            _showButton.Unchecked += (sender, args) =>
            {
                ViewPassword = false;
                _password.Password = _text.Text;
                _password.Visibility = Visibility.Visible;
                _text.Visibility = Visibility.Collapsed;
                if (_password.Password == string.Empty)
                    _label.Visibility = Visibility.Visible;
            };
            _password.KeyDown += ControlKeyDown;
            _text.KeyDown += ControlKeyDown;
        }

        void ControlKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                RaiseEnterPress();
        }

        public void AuthorizationFailReaction(string messageText, Brush messageBrush)
        {
            if (Equals(Keyboard.FocusedElement, _password))
            {
                ViewPassword = false;
            }
            else if (Equals(Keyboard.FocusedElement, _text))
            {
                Keyboard.Focus(_password);
                ViewPassword = false;
            }
            else
            {
                _label.Visibility = Visibility.Visible;
                _label.Text = messageText;
                _label.Foreground = messageBrush;
            }
            _text.Text = "";
            _password.Password = "";

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
