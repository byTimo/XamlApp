using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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

namespace ZappChat.Controls
{
    [TemplateVisualState(Name = "Static", GroupName = "InputView"),
     TemplateVisualState(Name = "Input", GroupName = "InputView"),
     TemplatePart(Name = "ShowPasswordButton", Type = typeof(Button)),
     TemplatePart(Name = "Password", Type = typeof(PasswordBox))]
    public class PasswordControl : Control
    {
        private PasswordBox password = null;
        private Button showButton = null;
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
            typeof (CornerRadius), typeof (PasswordControl));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty IsInputProperty = DependencyProperty.Register("IsInput", typeof (bool),
            typeof (PasswordControl), new FrameworkPropertyMetadata(false));

        public bool IsInput
        {
            get { return (bool) GetValue(IsInputProperty); }
            set { SetValue(IsInputProperty, value); }
        }
        static PasswordControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (PasswordControl),
                new FrameworkPropertyMetadata(typeof (PasswordControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            password = GetTemplateChild("Password") as PasswordBox;
            showButton = GetTemplateChild("ShowPasswordButton") as Button;
            showButton.Click += ViewPassword;
        }

        public void ViewPassword(object sender, RoutedEventArgs e)
        {
            password.PasswordChar = '8';
        }
        private void SwapState(bool isInput)
        {
            VisualStateManager.GoToState(this, isInput ? "Input" : "Static", false);
        }
    }
}
