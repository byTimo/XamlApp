using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZappChat.Core;

namespace ZappChat.Controls
{
	/// <summary>
	/// Логика взаимодействия для MessageControl.xaml
	/// </summary>
	public partial class MessageControl : UserControl
	{
        public Dialogue Dialogue { get; set; }
        public bool DialogueOpened { get; set; }
        public bool ContaintUnreadMessages { get; set; }

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
		    DialogueOpened = false;
		    ContaintUnreadMessages = false;
            Trashcan.Click += Trashcan_Click;
		}

	    public MessageControl(Dialogue dialogue) : this()
	    {
	        Dialogue = dialogue;
	        DialogueOpened = false;
	        ContaintUnreadMessages = false;
            UpdateControl();
	        AppEventManager.UpdateCounter += UpdateControl;
	    }

	    private double OpacityChange(DateTime time)
	    {
            var dTime = DateTime.Now.Subtract(time);
            if (dTime.Days > 0) return 0.5;
            return dTime.Hours * 0.0208 + dTime.Minutes * 0.0003;

	    }

	    public void UpdateControl()
	    {
            Timer.Opacity = 1.0 - OpacityChange(Dialogue.LastDateTime);
	        MessageText = Dialogue.GetTitleMessage();
	        MessageDate = Dialogue.LastMessageDate;
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
	        AppEventManager.DeleteConfirmationDialogueEvent(this, Dialogue, false);
	    }
	}
}