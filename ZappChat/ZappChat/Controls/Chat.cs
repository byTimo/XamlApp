using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
    [TemplatePart(Name = "Selling", Type = typeof(CornerRadiusButton))]
    [TemplatePart(Name = "OnOrder", Type = typeof(CornerRadiusButton))]
    [TemplatePart(Name = "NoSelling", Type = typeof(CornerRadiusButton))]
    [TemplatePart(Name = "Blocker", Type = typeof(Rectangle))]
    [TemplatePart(Name = "TitleTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "CarTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "VinTextBox", Type = typeof(TextBox))]

    public class Chat : Control
    {
        public Dialogue CurrentDialogue { get; set; }

        private CornerRadiusButton _selling;
        private CornerRadiusButton _onOrder;
        private CornerRadiusButton _noSelling;
        private TextBox _userInput;
        private TextBox _titleTextBox;
        private TextBox _carTextBox;
        private TextBox _vinTextBox;
        private Rectangle _blocker;

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
            set
            {
                FontSize = FontSizeChange(28.0, 563, value.Length, 15.0);
                SetValue(DialogueTitleProperty, value);
            }
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
            if (opendedDialogue.Status != DialogueStatus.Created || opendedDialogue.QueryId == 0)
            {
                _selling.Visibility = Visibility.Collapsed;
                _noSelling.Visibility = Visibility.Collapsed;
                _onOrder.Visibility = Visibility.Collapsed;
            }
            else
            {
                _selling.Visibility = Visibility.Visible;
                _noSelling.Visibility = Visibility.Visible;
                _onOrder.Visibility = Visibility.Visible;
            }
            if (!Equals(CurrentDialogue, opendedDialogue))
                _userInput.Text = "";
            CurrentDialogue = opendedDialogue;
            DialogueTitle = CurrentDialogue.GetTitleMessage();

            ChatMessages = new ObservableCollection<ChatMessage>();
            foreach (var message in CurrentDialogue.Messages)
            {
                ChatMessages.Add(new ChatMessage(message));
            }
            var carLabel = "" + (opendedDialogue.CarBrand ?? "")+ " " + (opendedDialogue.CarModel ?? "");
            Car = carLabel.Trim() != "" ? carLabel : "Автомобиль";
            Vin = opendedDialogue.VIN ?? "Vin";
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
            if (chat.Items.Count != 0)
                chat.ScrollIntoView(chat.Items[chat.Items.Count - 1]);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _selling = GetTemplateChild("Selling") as CornerRadiusButton;
            _selling.Click += (sender, args) => AnsweredToQuery(AnswerType.Selling);
            _onOrder = GetTemplateChild("OnOrder") as CornerRadiusButton;
            _onOrder.Click += (sender, args) => AnsweredToQuery(AnswerType.OnOrder);
            _noSelling = GetTemplateChild("NoSelling") as CornerRadiusButton;
            _noSelling.Click += (sender, args) => AnsweredToQuery(AnswerType.NoSelling);
            _userInput = GetTemplateChild("UserInput") as TextBox;
            if (_userInput == null) throw new NullReferenceException("Не определил TextBox в чате!");
            _userInput.KeyDown += UserInputOnKeyDown;
            _blocker = GetTemplateChild("Blocker") as Rectangle;
            if (_blocker == null) throw new NullReferenceException("Не определеил Blocker");
            _titleTextBox = GetTemplateChild("TitleTextBox") as TextBox;
            if (_titleTextBox == null) throw new NullReferenceException("Не определил TextBox в чате!");
            _titleTextBox.GotMouseCapture += TextBoxGotMouseCapture;
            _carTextBox = GetTemplateChild("CarTextBox") as TextBox;
            if (_carTextBox == null) throw new NullReferenceException("Не определил TextBox в чате!");
            _carTextBox.GotMouseCapture += TextBoxGotMouseCapture;
            _vinTextBox = GetTemplateChild("VinTextBox") as TextBox;
            if (_vinTextBox == null) throw new NullReferenceException("Не определил TextBox в чате!");
            _vinTextBox.GotMouseCapture += TextBoxGotMouseCapture;

            ChatMessages = new ObservableCollection<ChatMessage>();
            CurrentDialogue = new Dialogue();
            var backButton = GetTemplateChild("Back") as CornerRadiusButton;
            backButton.Click += (s, e) =>
            {
                CloseDialogue();
                AppEventManager.CloseDialogueEvent();
            };
            var sendButton = GetTemplateChild("Send") as CornerRadiusButton;
            sendButton.Click += SendMessageThroughtButtonClick;

        }

        private void TextBoxGotMouseCapture(object sender, MouseEventArgs mouseEventArgs)
        {
            ((TextBox)sender).SelectAll();
        }

        private void UserInputOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if(keyEventArgs.Key == Key.Enter)
                SendMessage();
        }

        private void SendMessageThroughtButtonClick(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (App.ConnectionStatus != ConnectionStatus.Connect) return;
            var userMessage = _userInput.Text.Trim();
            if (userMessage == "") return;
            _userInput.Text = "";
            var newMessage = new Message(0, userMessage, "outgoing", Guid.NewGuid().ToString(),
                DateTime.Now.ToString(CultureInfo.InvariantCulture), "") { IsSuccessfully = false };
            var sendMessageRequest = new SendMessageRequest
            {
                room_id = CurrentDialogue.RoomId,
                message = newMessage.Text,
                hash = newMessage.Hash,
                system = false
            };
            var sendMessageRequestToJson = JsonConvert.SerializeObject(sendMessageRequest);

            ChatMessages.Add(new ChatMessage(newMessage));
            AppWebSocketEventManager.SendObject(sendMessageRequestToJson);

            var chat = GetTemplateChild("MessageChat") as ListBox;
            if (chat == null) throw new NullReferenceException("Не определил ListBox в чате");
            if (chat.Items.Count != 0)
                chat.ScrollIntoView(chat.Items[chat.Items.Count - 1]);
        }

        public void SendMessageSuccess(ulong roomId, ulong messageId, string hash)
        {
            if(CurrentDialogue.RoomId != roomId) return;
            var control = ChatMessages.FirstOrDefault(c => c.Message.Hash == hash);
            if(control == null) return;
            control.SetMessageId(messageId);
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

        public void SetCarInfoAdapter(string brand, string model, string vin, string year)
        {
            CurrentDialogue.SetCarInformation(brand, model, vin, year);
            var carLabel = "" + (brand ?? "")+ " " + (model ?? "");
            Car = carLabel.Trim() != "" ? carLabel : "Автомобиль";
            Vin = vin ?? "Vin";
        }

        public void BlockingChat(bool block)
        {
            if(_blocker == null) return;
            _blocker.Visibility = block ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AnsweredToQuery(AnswerType type)
        {
            if(App.ConnectionStatus != ConnectionStatus.Connect) return;
           
            var selling = type != AnswerType.NoSelling;
            var onStock = type == AnswerType.Selling;
            var answerRequest = new AnswerRequest
            {
                id = CurrentDialogue.QueryId,
                selling = selling,
                on_stock = onStock
            };
            var answerRequestToJson = JsonConvert.SerializeObject(answerRequest);
            AppWebSocketEventManager.SendObject(answerRequestToJson);

            var text = (selling && onStock)
                ? "Запчасть имеется в наличии"
                : (selling || onStock)
                    ? "К сожалению, вашей запчасти нет в наличии, но мы готовы привести ее под заказ"
                    : "Нет в продаже";

            var newMessage = new Message(0, text, "outgoing", Guid.NewGuid().ToString(),
                DateTime.Now.ToString(CultureInfo.InvariantCulture), "") { IsSuccessfully = false };
            var sendMessageRequest = new SendMessageRequest
            {
                room_id = CurrentDialogue.RoomId,
                message = newMessage.Text,
                hash = newMessage.Hash,
                system = true
            };
            var sendMessageRequestToJson = JsonConvert.SerializeObject(sendMessageRequest);

            ChatMessages.Add(new ChatMessage(newMessage));
            AppWebSocketEventManager.SendObject(sendMessageRequestToJson);

            var chat = GetTemplateChild("MessageChat") as ListBox;
            if (chat == null) throw new NullReferenceException("Не определил ListBox в чате");
            if (chat.Items.Count != 0)
                chat.ScrollIntoView(chat.Items[chat.Items.Count - 1]);
        }

        public void ChangeDialogueStatus()
        {
            CurrentDialogue.Status = DialogueStatus.Answered;
            _selling.Visibility = Visibility.Collapsed;
            _noSelling.Visibility = Visibility.Collapsed;
            _onOrder.Visibility = Visibility.Collapsed;
        }

        private double FontSizeChange(double defaultFontSize, int maxLength, int stringLength, double minFontSize = 20)
        {
            var minSymbolChangeCount = maxLength/defaultFontSize;
            if (stringLength <= minSymbolChangeCount) return defaultFontSize;
            var changedFontSize = maxLength/stringLength;
            return changedFontSize >= minFontSize ? changedFontSize : minFontSize;
        }
    }
}
