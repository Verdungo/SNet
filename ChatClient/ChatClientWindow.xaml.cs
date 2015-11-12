using SNet.Messages;
using SNet.Sockets;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatClientWindow : Window
    {
        private SNetClient _sNetClient;
        private SNetServer _listenSocket;

        private Socket _udpSocket;
        private DispatcherTimer _awaitConnectionTimer;
        private const string _seekMessage = "Looking for SNet!";
        byte[] _buffer = new byte[1024];
        bool _connected = false;
        IPEndPoint _ipep = new IPEndPoint(IPAddress.Any, 50002);
        private string _serverHostName = string.Empty;
        private string _role = string.Empty;



        public string Nickname
        {
            get { return (string)GetValue(NicknameProperty); }
            set { SetValue(NicknameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Nickname.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NicknameProperty =
            DependencyProperty.Register("Nickname", typeof(string), typeof(ChatClientWindow), new PropertyMetadata("<Nickname>"));



        public ChatClientWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TryFindServer();

            _awaitConnectionTimer = new DispatcherTimer();
            _awaitConnectionTimer.Interval = new TimeSpan(0, 0, 3);
            _awaitConnectionTimer.Tick += AwaitConnectionTimer_Tick;
            _awaitConnectionTimer.Start();
        }

        private void AwaitConnectionTimer_Tick(object sender, EventArgs e)
        {
            if (!_connected)
            {
                LogMessage("Server not found. I am server now!");

                _awaitConnectionTimer.Stop();
                _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _udpSocket.Bind(_ipep);
                EndPoint ep = (EndPoint)_ipep;
                _udpSocket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref ep, new AsyncCallback(ListenerRecieveCallback), _udpSocket);

                TcpServerInit();
            }
        }

        private void ListenerRecieveCallback(IAsyncResult result)
        {
            EndPoint ep = _ipep;
            int length = _udpSocket.EndReceiveFrom(result, ref ep);
            if (Encoding.UTF8.GetString(_buffer.Take(length).ToArray()) == _seekMessage)
            {
                byte[] sendBuffer = Encoding.UTF8.GetBytes(Dns.GetHostName());
                _udpSocket.BeginSendTo(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, ep, new AsyncCallback(ListenerSendCallback), _udpSocket);
            }
            _udpSocket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref ep, new AsyncCallback(ListenerRecieveCallback), _udpSocket);
        }

        private void ListenerSendCallback(IAsyncResult ar)
        {
            (ar.AsyncState as Socket).EndSendTo(ar);
        }

        private void TryFindServer()
        {
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udpSocket.EnableBroadcast = true;
            byte[] packet = Encoding.UTF8.GetBytes(_seekMessage);

            _udpSocket.SendTo(packet, new IPEndPoint(IPAddress.Broadcast, 50002));

            LogMessage("Looking for server...");
            _udpSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(BroadcasterRecieveCallback), _udpSocket);
        }

        private void BroadcasterRecieveCallback(IAsyncResult result)
        {
            int recieveLength = _udpSocket.EndReceive(result);
            byte[] data = new byte[recieveLength];
            Buffer.BlockCopy(_buffer, 0, data, 0, recieveLength);
            _serverHostName = Encoding.UTF8.GetString(data);
            _connected = true;
            _awaitConnectionTimer.Stop();

            TcpClientConnect();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void Message_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Enter:
                    SendMessage();
                    break;
            }
        }

        private void SendMessage()
        {
            TextMessage msg = new TextMessage(String.Format("{0}: {1}", Nickname, Message.Text));

            switch (_role)
            {
                case "Server":
                    LogMessage("{0}", Message.Text);
                    _listenSocket.SendToAllClients(msg.Buffer);
                    break;
                case "Client":
                    _sNetClient?.Send(msg.Buffer);
                    break;
                default:
                    break;
            }
            Message.Text = "";
        }

        private void LogMessage(string message, params object[] args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatListView.Items.Add(String.Format(message, args));
            });
        }

        #region TCP CLIENT
        private void TcpClientConnect()
        {
            _role = "Client";
            _sNetClient = new SNetClient();
            _sNetClient.Connect(_serverHostName, 50001);
            _sNetClient.OnConnect += SNetClient_OnConnect;
            _sNetClient.OnRecieve += SNetClient_OnRecieve;
            _sNetClient.OnDisconnect += SNetClient_OnDisconnect;
        }

        private void SNetClient_OnConnect(object sender, SocketEventArgs e)
        {
            LogMessage("Connected to server {0}!", _serverHostName);
        }

        private void SNetClient_OnDisconnect(object sender, SocketEventArgs e)
        {
            // TODO: Сами становимся сервером, или ищем новый ?
            LogMessage("Server shuted down");
            _sNetClient = null;

        }

        private void SNetClient_OnRecieve(object sender, SocketEventArgs e)
        {
            switch (e.Message.Type)
            {
                case MessageType.TextMessage:
                    LogMessage("{0}", TextMessage.FromMessageBase(e.Message).Text);
                    break;
                default:
                    break;
            }
         
        }
        #endregion

        #region TCP SERVER
        private void TcpServerInit()
        {
            _role = "Server";
            _listenSocket = new SNetServer();
            _listenSocket.Bind(50001);
            _listenSocket.Listen(500);
            _listenSocket.Accept();

            _listenSocket.OnRecieve += ListenSocket_OnRecieve;
            _listenSocket.OnClientConnect += ListenSocket_OnClientConnect;
            _listenSocket.OnClientDisconnect += ListenSocket_OnClientDisconnect;
        }

        private void ListenSocket_OnClientConnect(object sender, SocketEventArgs e)
        {
            LogMessage("Client connected! {0}", sender);
        }

        private void ListenSocket_OnRecieve(object sender, SocketEventArgs e)
        {
            switch (e.Message.Type)
            {
                case MessageType.TextMessage:
                    LogMessage("{0}", TextMessage.FromMessageBase(e.Message).Text);
                    break;
                default:
                    break;
            }

            (sender as SNetServer).SendToAllClients(e.Message.Buffer);
        }

        private void ListenSocket_OnClientDisconnect(object sender, SocketEventArgs e)
        {
            LogMessage("Client disconnected! {0}", sender);
        }
        #endregion
    }
}
