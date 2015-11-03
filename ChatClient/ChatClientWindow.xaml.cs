using SNet.Sockets;
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
        }
    }
}
