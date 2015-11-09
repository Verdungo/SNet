using SNet.Messages;
using SNet.Sockets;
using System;
using System.Text;
using System.Windows;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatClientWindow : Window
    {
        private SNetClient _sNetClient;
        public ChatClientWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            _sNetClient = new SNetClient();
            _sNetClient.Connect("127.0.0.1", 50001);
            _sNetClient.OnRecieve += SNetClient_OnRecieve;
            _sNetClient.OnDisconnect += SNetClient_OnDisconnect;

            ConnectButton.IsEnabled = false;
        }

        private void SNetClient_OnDisconnect(object sender, SocketEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                ChatListView.Items.Add(string.Format("Server shuted down"));
                _sNetClient = null;
                ConnectButton.IsEnabled = true;
            }));
        }

        private void SNetClient_OnRecieve(object sender, SocketEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (e.Message.Type)
                {
                    case MessageType.MessageText:
                        ChatListView.Items.Add(string.Format("{0}", e.Message.Body));      
                        break;
                    default:
                        break;
                }
            });
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            TextMessage msg = new TextMessage(Message.Text);

            _sNetClient?.Send(msg.Buffer);
            Message.Text = "";
        }
    }
}
