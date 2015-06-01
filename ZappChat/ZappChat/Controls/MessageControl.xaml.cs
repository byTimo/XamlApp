using System;
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
        private bool _dialogueOpened;

	    readonly Brush _unreadBackground = new SolidColorBrush(Color.FromRgb(206,244,253));
	    readonly Brush _readBackground = Brushes.White;

	    private readonly Brush _unreadGradient = new LinearGradientBrush(new GradientStopCollection(new[]
	    {
	        new GradientStop(Color.FromRgb(231, 250, 255), 0.8),
	        new GradientStop(Color.FromArgb(0, 231, 250, 255), 0.0)
	    }));

	    private readonly Brush _readGradient = new LinearGradientBrush(new GradientStopCollection(new[]
	    {
	        new GradientStop(Colors.White, 0.8),
	        new GradientStop(Color.FromArgb(0, 255, 255, 255), 0.0)
	    }));

	    private void SetColors(Brush background, Brush gradient)
	    {
	        LayoutRoot.Background = background;
	        GradientRectangle.Fill = gradient;
	    }

	    public bool DialogueOpened
	    {
	        get { return _dialogueOpened; }
	        set
	        {
	            if (value)
	            {
	                SetColors(!_containtUnreeadMessage ? _readBackground : _unreadBackground,
	                    !_containtUnreeadMessage ? _readGradient : _unreadGradient);
	            }
	            else
	            {
	                if (Dialogue.Query != null || _containtUnreeadMessage)
	                    SetColors(_unreadBackground, _unreadGradient);
	                else
	                    SetColors(_readBackground, _readGradient);
	            }
	            _dialogueOpened = value;
	        }
	    }

	    private bool _containtUnreeadMessage;

	    public bool ContaintUnreadMessages
	    {
	        get { return _containtUnreeadMessage; }
	        set
	        {
	            if (_dialogueOpened)
	            {
                    SetColors(!value ? _readBackground : _unreadBackground,
                        !_containtUnreeadMessage ? _readGradient : _unreadGradient);

	            }
	            else
	            {
	                if (Dialogue.Query != null || value)
                        SetColors(_unreadBackground, _unreadGradient);
	                else
                        SetColors(_readBackground, _readGradient);
	            }
	            _containtUnreeadMessage = value;
	        }
	    }

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