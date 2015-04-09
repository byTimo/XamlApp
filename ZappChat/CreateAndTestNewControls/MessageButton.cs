using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace CreateAndTestNewControls
{
    [TemplateVisualState(Name = "NoUnreadMessages", GroupName = "MessageView"),
    TemplateVisualState(Name = "UnreadMessages", GroupName = "MessageView")]
    public class MessageButton : Button
    {
        public static readonly DependencyProperty MessagesCountProperty =
            DependencyProperty.Register("MessagesCount", typeof (string), typeof (MessageButton));

        public string MessagesCount
        {
            get { return GetValue(MessagesCountProperty) as string; }
            set { SetValue(MessagesCountProperty, SetMessagesCount(int.Parse(value))); }
        }
        static MessageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageButton), new FrameworkPropertyMetadata(typeof(MessageButton)));
        }

        public void SetStartupState()
        {
            SetMessagesCount(0);
        }

        public string SetMessagesCount(int number)
        {
            if (number <= 0)
            {
                VisualStateManager.GoToState(this, "NoUnreadMessages", true);
                return "";
            }
            if(number >= 100)
            {
                VisualStateManager.GoToState(this, "UnreadMessages", true);
                return "99+";
            }
            VisualStateManager.GoToState(this, "UnreadMessages", true);
            return number.ToString(CultureInfo.InvariantCulture);
        }
    }
}
