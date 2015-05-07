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

namespace ZappChat.Controls
{
    [TemplateVisualState(Name = "Static", GroupName = "LoadingView"),
     TemplateVisualState(Name = "Loading", GroupName = "LoadingView")]
    public class LoginButton : Button
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
            typeof (CornerRadius), typeof (LoginButton));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string),
            typeof (LoginButton), new FrameworkPropertyMetadata("Text"));

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty LoginTryProperty = DependencyProperty.Register("LoginTry",
            typeof(bool), typeof(LoginButton),
            new FrameworkPropertyMetadata(false));

        public bool LoginTry
        {
            get { return (bool)GetValue(LoginTryProperty); }
            private set { SetValue(LoginTryProperty, value); }
        }


        static LoginButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LoginButton),
                new FrameworkPropertyMetadata(typeof (LoginButton)));
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VisualStateManager.GoToState(this, "Static", false);
        }

        public void SwapState(bool isLogin)
        {
            VisualStateManager.GoToState(this, isLogin ? "Loading" : "Static", true);
            LoginTry = isLogin;
        }
    }
}
