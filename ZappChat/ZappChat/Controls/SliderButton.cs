using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ZappChat.Controls
{
    [TemplateVisualState(Name = "NoUnreadMessages", GroupName = "MessageView"),
     TemplateVisualState(Name = "UnreadMessages", GroupName = "MessageView")]
    public class SliderButton : RadioButton
    {
        public static readonly DependencyProperty MessagesCountProperty =
            DependencyProperty.Register("MessagesCount", typeof (string), typeof (SliderButton),
                new FrameworkPropertyMetadata("0"));

        public int MessagesCount
        {
            get { return ParseProperatyValue(GetValue(MessagesCountProperty) as string); }
            set { SetValue(MessagesCountProperty, SetMessagesCount(value)); }
        }

        static SliderButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (SliderButton),
                new FrameworkPropertyMetadata(typeof (SliderButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetMessagesCount(0);
        }

        public string SetMessagesCount(int number)
        {
            if (number <= 0)
            {
                VisualStateManager.GoToState(this, "NoUnreadMessages", true);
                return "";
            }
            if (number >= 100)
            {
                VisualStateManager.GoToState(this, "UnreadMessages", true);
                //TODO <- Раньше здесь было 99+, но для этого нужно менять get у MessageCount
                return number.ToString(CultureInfo.InvariantCulture);
            }
            VisualStateManager.GoToState(this, "UnreadMessages", true);
            return number.ToString(CultureInfo.InvariantCulture);
        }

        private static int ParseProperatyValue(string value)
        {
            return value == "" ? 0 : int.Parse(value);
        }
    }
}
