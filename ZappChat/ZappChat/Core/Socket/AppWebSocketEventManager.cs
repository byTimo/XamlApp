using System;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace ZappChat.Core.Socket
{
    static class AppWebSocketEventManager
    {
        private const int MaxConnectAttemptCount = 10;
        private const int WaitTimeBetweenConnectAttemptInMillisecond = 500;

        private static readonly WebSocket _webSocket;
        public static LoginWindow Login { get; set; }
        public static MainWindow MainWindow { get; set; }

        public static ConnectionStatus Connection { get; private set; }
        public static bool IsChat { get; set; }


        static AppWebSocketEventManager()
        {
            _webSocket = new WebSocket("ws://zappchat.ru:7778");
            _webSocket.Opened += OpenedConnection;
            _webSocket.Closed += ClosedConnection;
            _webSocket.Error += SocketErrorEvent;
            _webSocket.MessageReceived += MessageReceivedEvent;
            _webSocket.AutoSendPingInterval = 120;

            Connection = ConnectionStatus.Disconnect;
        }
        private static void MessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var responseJson = (JObject)JsonConvert.DeserializeObject(e.Message);
                if(responseJson["action"] == null)
                    throw new Exception("JSON object don't conaint action field!");
                switch ((string)responseJson["action"])
                {
                    case "client/auth":
                        CrossThreadOperationWithOneString(Login.Dispatcher, Login.AuthorizationResult,
                            (string) responseJson["status"]);
                        break;
                        //@TODO
                }
            }
            catch (Exception exception)
            {
                //@TODO
                throw exception;
            }
        }

        private static void SocketErrorEvent(object sender, ErrorEventArgs e)
        {
        //    MessageBox.Show(e.Exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Connection = ConnectionStatus.Error;
        }

        private static void ClosedConnection(object sender, EventArgs e)
        {
            _webSocket.Close();
            //AppEventManager.ConnectionEvent(_webSocket, ConnectionStatus.Disconnect);
        }

        private static void OpenedConnection(object sender, EventArgs e)
        {
            Connection = ConnectionStatus.Connect;
        }

        public static bool OpenWebSocket()
        {
            Connection = ConnectionStatus.Disconnect;
            var connectAttemptConunt = 0;
            try
            {
               // Thread.Sleep(2000);
                if (_webSocket.State == WebSocketState.Closed || _webSocket.State == WebSocketState.None)
                    _webSocket.Open();
                while (Connection == ConnectionStatus.Disconnect)
                {
                    Thread.Sleep(WaitTimeBetweenConnectAttemptInMillisecond);
                    if(connectAttemptConunt++ >= MaxConnectAttemptCount)
                        Connection = ConnectionStatus.Error;
                }
                return _webSocket.State == WebSocketState.Open;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool SendObject(string jsonString)
        {
            _webSocket.Send(jsonString);
            return _webSocket.State == WebSocketState.Open;
        }

        private static void CrossThreadOperationWithoutParams(Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.Invoke(action);
        }

        private static void CrossThreadOperationWithOneString(Dispatcher dispatcher, Action<string> action, string firstParam)
        {
            if (dispatcher.CheckAccess())
                action(firstParam);
            else
                dispatcher.Invoke(action, firstParam);
        }
        
    }
}
