using SNet;
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
        }
    }
}
