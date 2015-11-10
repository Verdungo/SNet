using SNet.Messages;
using SNet.Sockets;
using System;
using System.Text;
using System.Windows;

namespace ServerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatServer : Window
    {
        private SNetServer _listenSocket;

        public ChatServer()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _listenSocket = new SNetServer();
            _listenSocket.Bind(50001);
            _listenSocket.Listen(500);
            _listenSocket.Accept();

            _listenSocket.OnRecieve += ListenSocket_OnRecieve;
            _listenSocket.OnClientConnect += ListenSocket_OnClientConnect;
            _listenSocket.OnClientDisconnect += ListenSocket_OnClientDisconnect;

            StartButton.IsEnabled = false;
        }

        private void ListenSocket_OnClientConnect(object sender, SocketEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatListView.Items.Add(string.Format("Client connected! {0}", sender));
            });
        }

        private void ListenSocket_OnRecieve(object sender, SocketEventArgs e)
        {
            TextMessage inMessage = (TextMessage)e.Message;
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (e.Message.Type)
                {
                    case MessageType.MessageText:
                        ChatListView.Items.Add(string.Format("{0}", (e.Message as TextMessage).Text));
                        break;
                    default:
                        break;
                }

                (sender as SNetServer).SendToAllClients(e.Message.Buffer);
            });
        }

        private void ListenSocket_OnClientDisconnect(object sender, SocketEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                ChatListView.Items.Add(string.Format("Client disconnected! {0}", sender));
            }));
        }
    }
}
