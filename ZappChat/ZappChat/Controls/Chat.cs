using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
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
using Newtonsoft.Json;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;

namespace ZappChat.Controls
{
    [TemplatePart(Name = "Back", Type = typeof(CornerRadiusButton))]
    [TemplatePart(Name = "Send", Type = typeof(CornerRadiusButton))]
    [TemplatePart(Name = "UserInput", Type = typeof(TextBox))]
    [TemplatePart(Name = "MessageChat", Type = typeof(ListBox))]
    public class Chat : Control
    {
        public Dialogue CurrentDialogue { get; set; }

        public static readonly DependencyProperty ChatMessagesProperty = DependencyProperty.Register("ChatMessages",
            typeof (ObservableCollection<ChatMessage>), typeof (Chat),
            new FrameworkPropertyMetadata(new ObservableCollection<ChatMessage>()));

        public ObservableCollection<ChatMessage> ChatMessages
        {
            get { return GetValue(ChatMessagesProperty) as ObservableCollection<ChatMessage>; }
            set { SetValue(ChatMessagesProperty, value); }
        }
        public static readonly DependencyProperty DialogueTitleProperty = DependencyProperty.Register("DialogueTitle",
            typeof (string), typeof (Chat),
            new FrameworkPropertyMetadata("Title"));

        public string DialogueTitle
        {
            get { return GetValue(DialogueTitleProperty) as string; }
            set { SetValue(DialogueTitleProperty, value); }
        }

        public static readonly DependencyProperty CarProperty = DependencyProperty.Register("Car", typeof (string),
            typeof (Chat),
            new FrameworkPropertyMetadata("Автомобиль"));

        public string Car
        {
            get { return GetValue(CarProperty) as string; }
            set { SetValue(CarProperty, value); }
        }

        public static readonly DependencyProperty VinProperty = DependencyProperty.Register("Vin", typeof (string),
            typeof (Chat),
            new FrameworkPropertyMetadata("VIN"));
        public string Vin
        {
            get { return GetValue(VinProperty) as string; }
            set { SetValue(VinProperty, value); }
        }
        static Chat()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chat), new FrameworkPropertyMetadata(typeof(Chat)));
        }

        public void OpenDialogue(Dialogue opendedDialogue)
        {
            var userInput = GetTemplateChild("UserInput") as TextBox;
            if (userInput == null) throw new NullReferenceException("Не определил TextBox в чате!");
            if (!Equals(CurrentDialogue, opendedDialogue))
                userInput.Text = "";
            CurrentDialogue = opendedDialogue;
            DialogueTitle = CurrentDialogue.GetTitleMessage();

            foreach (var message in CurrentDialogue.Messages)
            {
                message.Status = MessageStatus.Read;
            }

            ChatMessages = new ObservableCollection<ChatMessage>();
            foreach (var message in CurrentDialogue.Messages)
            {
                ChatMessages.Add(new ChatMessage(message));
            }
            var chat = GetTemplateChild("MessageChat") as ListBox;
            if(chat == null) throw new NullReferenceException("Не определил ListBox в чате");
            if(chat.Items.Count != 0)
                chat.ScrollIntoView(chat.Items[chat.Items.Count-1]);
        }

        public void AddNewMessageToChat(Dialogue dialogue)
        {
            ChatMessages.Add(new ChatMessage(dialogue.Messages[0]));
            DialogueTitle = CurrentDialogue.GetTitleMessage();
            var chat = GetTemplateChild("MessageChat") as ListBox;
            if (chat == null) throw new NullReferenceException("Не определил ListBox в чате");
            chat.ScrollIntoView(chat.Items[chat.Items.Count - 1]);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ChatMessages = new ObservableCollection<ChatMessage>();
            CurrentDialogue = new Dialogue();
            var backButton = GetTemplateChild("Back") as CornerRadiusButton;
            backButton.Click += (s, e) =>
            {
                CloseDialogue();
                AppEventManager.CloseDialogueEvent();
            };
            var sendButton = GetTemplateChild("Send") as CornerRadiusButton;
            sendButton.Click += SendUserMessage;
//            {
//                var userInput = GetTemplateChild("UserInput") as TextBox;
//                if (userInput != null && userInput.Text != "")
//                {
//                    //AppEventManager.SendMessageEvent(this, CurrentDialogue.RoomId,
//                    //    new Message(CurrentDialogue.RoomId, "", userInput.Text, MessageStatus.Read));
//                    userInput.Text = "";
//                }
//            };
        }

        private void SendUserMessage(object sender, RoutedEventArgs e)
        {
            var userInput = GetTemplateChild("UserInput") as TextBox;
            if(userInput == null) throw new NullReferenceException("Не определил TextBox в чате!");
            var userMessage = userInput.Text.Trim();
            if(userMessage == "") return;
            var sendMessageRequest = new SendMessageRequest
            {
                room_id = CurrentDialogue.RoomId,
                text = userMessage,
                hash = Guid.NewGuid().ToString(),
                system = false
            };
            var sendMessageRequestToJson = JsonConvert.SerializeObject(sendMessageRequest);
//@TODO------------- После реализации получения ответа ----------------------
            //AppWebSocketEventManager.SendObject(sendMessageRequestToJson);


        }
        public void CloseDialogue()
        {
            CurrentDialogue = new Dialogue();
            ChatMessages = new ObservableCollection<ChatMessage>();
        }

        public void SendMessage(Message message)
        {
            CurrentDialogue.AddMessage(message);
            ChatMessages.Add(new ChatMessage(message));
        }
    }
}
