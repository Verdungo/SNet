using System;
using System.Net;
using System.Net.Sockets;

namespace SNet.Sockets
{
    public class SNetClient
    {
        private Socket _socket;
        byte[] _buffer;

        public event SocketEventHandler OnConnect;
        public event SocketEventHandler OnDisconnect;
        public event SocketEventHandler OnSend;
        public event SocketEventHandler OnRecieve;

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
                    OnConnect(this, new SocketEventArgs(_buffer));
                } 
            }
            else
            {
                _socket.Close();
                _socket.Dispose();
                if (OnDisconnect != null)
                {
                    OnDisconnect(this, SocketEventArgs.Empty);
                }
            }
        }

        private void RecieveCallback(IAsyncResult result)
        {
            try
            {
                int bufferSize = _socket.EndReceive(result);
                byte[] packet = new byte[bufferSize];
                Array.Copy(_buffer, packet, packet.Length);
                if (OnRecieve != null) 
                {
                    // TODO: EventArgs - create my class
                    OnRecieve(this, new SocketEventArgs(packet));
                }

                _buffer = new byte[1024];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, null);
            }
            catch (Exception)
            {
                _socket.Close();
                _socket.Dispose();
                if (OnDisconnect != null)
                {
                    OnDisconnect(this, SocketEventArgs.Empty);
                }
            }
        }

        public void Send(byte[] sendBuf)
        {
            if (_socket.Connected) _socket.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, SendCallback, _socket);
        }

        private void SendCallback(IAsyncResult result)
        {
            _socket.EndSend(result);
        }
    }
}
