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
            typeof (MessageType), typeof (ChatMessage), new FrameworkPropertyMetadata(MessageType.User));

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
        static ChatMessage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatMessage), new FrameworkPropertyMetadata(typeof(ChatMessage)));
        }

        public ChatMessage() { }
        public ChatMessage(Message mes)
        {
            Type = mes.Author == "" ? MessageType.User : MessageType.Interlocutor;
            switch (Type)
            {
                case MessageType.User:
                    TextMessage = mes.Text;
                    TextAlignment = TextAlignment.Right;
                    HorizontalContentAlignment = HorizontalAlignment.Right;
                    break;
                case MessageType.Interlocutor:
                    AuthorMessage = mes.Author;
                    TextMessage = mes.Text;
                    HorizontalContentAlignment = HorizontalAlignment.Left;
                    break;
            }
        }
    }
}
