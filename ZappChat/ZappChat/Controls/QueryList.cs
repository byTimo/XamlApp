using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using ZappChat.Core;
using ZappChat.Core.Socket;
using ZappChat.Core.Socket.Requests;

namespace ZappChat.Controls
{
    public class QueryList : ListBox
    {
        public ObservableCollection<QueryControl> Queries { get; set; }
        static QueryList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QueryList), new FrameworkPropertyMetadata(typeof(QueryList)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitQueryList();
            SelectionChanged += OpenDialogue;
            SelectedIndex = -1;
        }

        public void InitQueryList()
        {
            if (Queries == null)
            {
                Queries = new ObservableCollection<QueryControl>();
            }
            ItemsSource = Queries;
        }

        private void OpenDialogue(object sender, SelectionChangedEventArgs e)
        {
            if (App.ConnectionStatus != ConnectionStatus.Connect) return;

            var selectedQueryControl = (SelectedItem as QueryControl);
            if (selectedQueryControl == null) return;
//            var historyRequest = new HistoryRequest
//            {
//                from = null,
//                to = null,
//                chat_room_id = selectedQueryControl.Dialogue.RoomId
//            };
//            var historyRequestToJson = JsonConvert.SerializeObject(historyRequest);
//            AppWebSocketEventManager.SendObject(historyRequestToJson);
            SelectedIndex = -1;
            AppEventManager.PreopenDialogueEvent(selectedQueryControl.Dialogue.RoomId, null, null);
        }
    }
}
