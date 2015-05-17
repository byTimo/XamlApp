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
using ZappChat.Core;

namespace ZappChat.Controls
{
    public class ChatMessage : Control
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type",
            typeof (MessageType), typeof (ChatMessage), new FrameworkPropertyMetadata(MessageType.Outgoing));

        public MessageType Type
        {
            get { return (MessageType) GetValue(TypeProperty); }
            set { SetValue(TypeProperty,value); }
        }

        public static readonly DependencyProperty AuthorMessageProperty = DependencyProperty.Register("AuthorMessage",
            typeof(string), typeof(ChatMessage), new FrameworkPropertyMetadata(""));
        public string AuthorMessage
        {
            get { return GetValue(AuthorMessageProperty) as string; }
            set { SetValue(AuthorMessageProperty, value); }
        }
        public static readonly DependencyProperty TextMessageProperty = DependencyProperty.Register("TextMessage",
            typeof(string), typeof(ChatMessage), new FrameworkPropertyMetadata("Text"));
        public string TextMessage
        {
            get { return GetValue(TextMessageProperty) as string; }
            set { SetValue(TextMessageProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment",
            typeof (TextAlignment), typeof (ChatMessage),
            new FrameworkPropertyMetadata(TextAlignment.Left));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment) GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        public static readonly DependencyProperty AuthorMarginProperty = DependencyProperty.Register("AuthorMargin",
            typeof(Thickness), typeof(ChatMessage));
        public Thickness AuthorMargin
        {
            get { return (Thickness)GetValue(AuthorMarginProperty); }
            set { SetValue(AuthorMarginProperty, value);  }
        }
        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin",
        typeof(Thickness), typeof(ChatMessage));
        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }
        static ChatMessage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatMessage), new FrameworkPropertyMetadata(typeof(ChatMessage)));
        }

        public ChatMessage() { }
        public ChatMessage(Message mes)
        {
            switch (mes.Type)
            {
                case MessageType.Outgoing:
                    TextMessage = mes.Text;
                    TextAlignment = TextAlignment.Right;
                    HorizontalContentAlignment = HorizontalAlignment.Right;
                    AuthorMargin = new Thickness(100, 5, 0, 10);
                    TextMargin = new Thickness(100, 0, 0, 5);
                    break;
                case MessageType.Incoming:
                    AuthorMessage = mes.Author;
                    TextMessage = mes.Text;
                    HorizontalContentAlignment = HorizontalAlignment.Left;
                    AuthorMargin = new Thickness(0, 5, 100, 10);
                    TextMargin = new Thickness(0, 0, 100, 5);
                    break;
            }
        }
    }
}
