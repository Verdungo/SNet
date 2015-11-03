using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SNet.Sockets
{
    public class SNetClient
    {
        private Socket _socket;
        byte[] _buffer;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnSend;
        public event EventHandler OnRecieve;

        public SNetClient()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string host, int port)
        {
            // TODO: вместо IPV4 в явном виде использовать конструкцию ниже
            /*
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
            */
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(host), port), ConnectCallback, null);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            if (_socket.Connected)
            {
                _buffer = new byte[1024];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, null);
                if (OnConnect != null)
                {
                    OnConnect(this, EventArgs.Empty);
                } 
            }
            else
            {
                // TODO: Handle disconnect
            }
        }

        private void RecieveCallback(IAsyncResult result)
        {
            if (_socket.Connected)
            {
                int bufferSize = _socket.EndReceive(result);
                byte[] packet = new byte[bufferSize];
                Array.Copy(_buffer, packet, packet.Length);
                if (OnRecieve != null) 
                {
                    // TODO: EventArgs - create my class
                    OnRecieve(this, EventArgs.Empty);
                }

                _buffer = new byte[1024];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, null);
            }
            else
            {
                if (OnDisconnect != null)
                {
                    OnDisconnect(this, EventArgs.Empty);
                }
            }
        }

        public void Send(string message)
        {
            byte[] sendBuf = Encoding.UTF8.GetBytes(message);
            _socket.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, SendCallback, _socket);
        }

        private void SendCallback(IAsyncResult result)
        {
            _socket.EndSend(result);
        }
    }
}
