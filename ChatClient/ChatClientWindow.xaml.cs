using SNet.Sockets;
using System;
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

        private void SNetClient_OnDisconnect(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                ChatListView.Items.Add(string.Format("Server shuted down"));
            }));
            ConnectButton.IsEnabled = true;
        }

        private void SNetClient_OnRecieve(object sender, System.EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                ChatListView.Items.Add(string.Format("Получено сообщение"));
            }));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            _sNetClient.Send(Message.Text);
            Message.Text = "";
        }
    }
}
