using SNet;
using System;
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

        private void ListenSocket_OnClientConnect(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                ChatListView.Items.Add(string.Format("Client connected! {0}", sender));
            }));
        }

        private void ListenSocket_OnRecieve(object sender, System.EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                ChatListView.Items.Add(string.Format("Получено сообщение от {0}", sender));
            }));
        }

        private void ListenSocket_OnClientDisconnect(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                ChatListView.Items.Add(string.Format("Client disconnected! {0}", sender));
            }));
        }
    }
}
