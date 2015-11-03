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

                //debug
                _socket.Send(Encoding.UTF8.GetBytes("Hello, Net!"));
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

                _buffer = new byte[1024];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, null);
            }
            else
            {
                // TODO: Handle disconnect
            }
        }
    }
}
