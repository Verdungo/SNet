using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UDPBroadcastTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Socket _listener;
        //private Socket _broadcaster;
        private Socket _udpSocket;
        private bool _connected = false;
        byte[] _buffer = new byte[1024];
        IPEndPoint _ipep = new IPEndPoint(IPAddress.Any, 50001);
        private DispatcherTimer _awaitConnectionTimer;

        public MainWindow()
        {
            InitializeComponent();

            LogMessage("Applisation started.");
            TryFindServer();

            LogMessage("Starting awaiting connection timer.");
            _awaitConnectionTimer = new DispatcherTimer();
            _awaitConnectionTimer.Interval = new TimeSpan(0, 0, 3);
            _awaitConnectionTimer.Tick += AwaitConnectionTimer_Tick;
            _awaitConnectionTimer.Start();
        }

        private void AwaitConnectionTimer_Tick(object sender, EventArgs e)
        {
            LogMessage("Connection timer elapsed.");

            _awaitConnectionTimer.Stop();
            // TODO: а нам это надо?
            if (!_connected)
            {
                _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _udpSocket.Bind(_ipep);
                EndPoint ep = (EndPoint)_ipep;
                _udpSocket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref ep, new AsyncCallback(ListenerRecieveCallback), _udpSocket);
                LogMessage("Waiting for clients...");
            }
        }

        private void ListenerRecieveCallback(IAsyncResult result)
        {
            EndPoint ep = _ipep;
            int length = _udpSocket.EndReceiveFrom(result, ref ep);
            byte[] sendBuffer = Encoding.UTF8.GetBytes(Dns.GetHostName());

            LogMessage("Recieved: \"{0}\" from {1}", Encoding.UTF8.GetString(_buffer.Take(length).ToArray()), ep.ToString());

            _udpSocket.BeginSendTo(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, ep, new AsyncCallback(ListenerSendCallback), _udpSocket);
            LogMessage("Sending from server message \"{0}\" to {1}", Encoding.UTF8.GetString(sendBuffer), ep.ToString());
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
            byte[] packet = Encoding.UTF8.GetBytes("1234");
            LogMessage("Broadcast string \"1234\"");

            _udpSocket.SendTo(packet, new IPEndPoint(IPAddress.Broadcast, 50001));

            LogMessage("Waiting for broadcast callback...");
            _udpSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(BroadcasterRecieveCallback), _udpSocket);
            
        }

        private void BroadcasterRecieveCallback(IAsyncResult result)
        {
            int recieveLength = _udpSocket.EndReceive(result);
            byte[] data = new byte[recieveLength];
            Buffer.BlockCopy(_buffer, 0, data, 0, recieveLength);
            LogMessage("Broadcater recieved callback: \"{0}\"", Encoding.UTF8.GetString(data));

            _connected = true;
            _awaitConnectionTimer.Stop();
        }

        private void LogMessage(string message, params object[] args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Log.Items.Add(String.Format(message, args));
            });
        }
    }
}
