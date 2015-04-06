using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ZappChat.Controls
{
	/// <summary>
	/// Логика взаимодействия для MessageControl.xaml
	/// </summary>
	public partial class MessageControl : UserControl
	{
	    public static readonly RoutedEvent DeleteMessageEvent =
	        EventManager.RegisterRoutedEvent("DeleteMessage", RoutingStrategy.Bubble,
	            typeof (RoutedEvent), typeof (MessageControl));

	    public static readonly DependencyProperty MessageTextProperty;
	    public static readonly DependencyProperty MessageDateProperty;
	 /*   private static void OnMessageDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	    {
	        var messageControl = sender as MessageControl;
	        if (e.Property == MessageTextProperty)
	            messageControl.MessageText = (string) e.NewValue;
	        else
	            messageControl.MessageDate = (string) e.NewValue;
	    }*/

	    static MessageControl ()
	    {
	        MessageTextProperty = DependencyProperty.Register("MessageText",typeof(string),typeof(MessageControl));
	        MessageDateProperty = DependencyProperty.Register("MessageDate", typeof (string), typeof (MessageControl));
	    }
	    public string MessageText
	    {
            get { return (string) GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
	    }

	    public string MessageDate
	    {
            get { return (string) GetValue(MessageDateProperty); }
            set { SetValue(MessageDateProperty, value); }
	    }

        public event RoutedEventHandler DeleteMessage
        {
            add { AddHandler(DeleteMessageEvent, value); }
            remove { RemoveHandler(DeleteMessageEvent, value); }
        }
		public MessageControl()
		{
			InitializeComponent();
            Trashcan.Click += Trashcan_Click;
		}

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Trashcan.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Trashcan.Visibility = Visibility.Collapsed;
        }

	    private void Trashcan_Click(object sender, RoutedEventArgs e)
	    {
	        Trashcan.Background = Brushes.Black;
	    }
	}
}