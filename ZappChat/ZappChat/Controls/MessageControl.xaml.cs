using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZappChat.Core;

namespace ZappChat.Controls
{
	/// <summary>
	/// Логика взаимодействия для MessageControl.xaml
	/// </summary>
	public partial class MessageControl : UserControl
	{
        public Dialogue Dialogue { get; set; }

	    public static readonly DependencyProperty MessageTextProperty =
	        DependencyProperty.Register("MessageText", typeof (string), typeof (MessageControl));

	    public static readonly DependencyProperty MessageDateProperty =
	        DependencyProperty.Register("MessageDate", typeof (string), typeof (MessageControl));

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

		public MessageControl()
		{
			InitializeComponent();
            Trashcan.Click += Trashcan_Click;
		}

	    public MessageControl(Dialogue dialogue) : this()
	    {
	        Dialogue = dialogue;
            UpdateControl();
	    }

	    public void UpdateControl()
	    {
            MessageText = Dialogue.GetLatMessage().Text;
            MessageDate = Dialogue.GetLatMessage().Date;
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