using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZappChat
{
	/// <summary>
	/// Логика взаимодействия для MessageControl.xaml
	/// </summary>
	public partial class MessageControl : UserControl
	{
	    public static readonly RoutedEvent DeleteMessageEvent =
	        EventManager.RegisterRoutedEvent("DeleteMessage", RoutingStrategy.Bubble,
	            typeof (RoutedEvent), typeof (MessageControl));

	    public static DependencyProperty MessageTextProperty =
	        DependencyProperty.Register("MessageText", typeof (string), typeof (MessageControl),
	            new FrameworkPropertyMetadata("Сообщение"));
	    public static DependencyProperty MessageDateProperty =
	        DependencyProperty.Register("MessageText", typeof (string), typeof (MessageControl));

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
			this.InitializeComponent();
		}
	}
}