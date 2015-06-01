using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;

namespace ZappChat.Controls
{

    public class ListMessages : ListBox
    {
        public ObservableCollection<MessageControl> DialogueWithoutQuery { get; set; }
        public ObservableCollection<MessageControl> DialogueWithQuery { get; set; } 

        static ListMessages()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListMessages), new FrameworkPropertyMetadata(typeof(ListMessages)));
        }

        public Dialogue CheckUnreadedDialogues()
        {
            foreach (var messageControl in DialogueWithQuery)
            {
                if (App.IsThisUnreadMessage(messageControl.Dialogue.RoomId, 0))
                    return messageControl.Dialogue;
            }
            foreach (var messageControl in DialogueWithoutQuery)
            {
                if (App.IsThisUnreadMessage(messageControl.Dialogue.RoomId, messageControl.Dialogue.GetLastMessage().Id))
                    return messageControl.Dialogue;
            
            }
            return null;

        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DialogueWithQuery = new ObservableCollection<MessageControl>();
            DialogueWithoutQuery = new ObservableCollection<MessageControl>();
            SelectionChanged += OpenDialogue;
            SelectWithQuery();
        }

        public void AddNewMessageToList(Dialogue dialogue)
        {
            var thisDialogueInListQuery = DialogueWithQuery.Any(x => Equals(x.Dialogue, dialogue));

            if (thisDialogueInListQuery)
            {
                var thisDialogue = DialogueWithQuery.First(x => Equals(x.Dialogue, dialogue));
                thisDialogue.Dialogue.AddMessage(dialogue.Messages[0]);
                var indexThisDialogue = DialogueWithQuery.IndexOf(thisDialogue);
                DialogueWithQuery.Move(indexThisDialogue, 0);
                var thisDialogueWithoutQuery = DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
                if(thisDialogueWithoutQuery == null)
                    DialogueWithoutQuery.Insert(0, thisDialogue);

                thisDialogue.UpdateControl();
            }
            else
            {
                var thisDialogue = DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
                if (thisDialogue == null)
                {
                    DialogueWithoutQuery.Insert(0, new MessageControl(dialogue));
                }
                else
                {
                    thisDialogue.Dialogue.AddMessage(dialogue.Messages[0]);
                    var indexThisDialogue = DialogueWithoutQuery.IndexOf(thisDialogue);
                    DialogueWithoutQuery.Move(indexThisDialogue, 0);

                    var thisDialogueWithQuery = DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
                    if (thisDialogueWithQuery != null)
                    {
                        indexThisDialogue = DialogueWithQuery.IndexOf(thisDialogueWithQuery);
                        DialogueWithQuery.Move(indexThisDialogue, 0);
                    }

                    thisDialogue.UpdateControl();
                }
            }
        }

        public void RemoveDialogueFromLists(ulong dialogueId)
        {
            var delDialogue = DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.RoomId == dialogueId);
            if (delDialogue != null)
                DialogueWithoutQuery.Remove(delDialogue);
            delDialogue = DialogueWithQuery.FirstOrDefault(x => x.Dialogue.RoomId == dialogueId);
            if (delDialogue != null)
                DialogueWithQuery.Remove(delDialogue);

        }

        public void TakeQuery(Dialogue dialogue)
        {

            var thisDialogue = DialogueWithoutQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
            if (thisDialogue != null)
            {
                thisDialogue.Dialogue.Query = dialogue.Query;
                var indexThisDialogue = DialogueWithoutQuery.IndexOf(thisDialogue);
                DialogueWithoutQuery.Move(indexThisDialogue, 0);
                // Добавление диалога с запросом и сообщениями в список диалогов с запросами - нужно ли так делать?
                DialogueWithQuery.Insert(0,thisDialogue);

                thisDialogue.UpdateControl();
            }
            thisDialogue = DialogueWithQuery.FirstOrDefault(x => Equals(x.Dialogue, dialogue));
            if (thisDialogue == null)
            {
                var isOpenedDialogue = !App.IsThisUnreadMessage(dialogue.RoomId, 0);

                var newDialogueWithQuery = new MessageControl(dialogue) {DialogueOpened = isOpenedDialogue};
                DialogueWithQuery.Insert(0, newDialogueWithQuery);
            }
        }

        public void OpenDialogue(object sender, SelectionChangedEventArgs e)
        {
            if(App.ConnectionStatus != ConnectionStatus.Connect) return;

            var selectedMessageControl = (SelectedItem as MessageControl);
            if(selectedMessageControl == null) return;
            
            var historyRequest = new HistoryRequest
            {
                from = null,
                to = null,
                chat_room_id = selectedMessageControl.Dialogue.RoomId
            };
            var historyRequestToJson = JsonConvert.SerializeObject(historyRequest);
            AppWebSocketEventManager.SendObject(historyRequestToJson);
            SelectedIndex = -1;
        }
        public Dialogue ChangeMessageStatus(ulong roomId, List<Message> messages)
        {
            var controlWithoutQuery =
                DialogueWithoutQuery.FirstOrDefault(x => x.Dialogue.RoomId == roomId);
            if (controlWithoutQuery != null)
                controlWithoutQuery.Dialogue.Messages = messages;
            var controlWithQuery =
                DialogueWithQuery.FirstOrDefault(x => x.Dialogue.RoomId == roomId);
            if (controlWithQuery != null)
                controlWithQuery.Dialogue.Messages = messages;
            return controlWithoutQuery != null ? controlWithoutQuery.Dialogue : controlWithQuery.Dialogue;
        }

        public void TakeNewDialogue(Dialogue newDialogue)
        {
            DialogueWithQuery.Insert(0, new MessageControl(newDialogue));
        }

        public void SelectWithoutQuery()
        {
            ItemsSource = DialogueWithoutQuery;
        }

        public void SelectWithQuery()
        {
            ItemsSource = DialogueWithQuery;
        }


    }
}
