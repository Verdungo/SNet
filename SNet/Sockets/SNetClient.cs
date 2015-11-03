using System;
using System.Net;
using System.Net.Sockets;

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
            _buffer = new byte[1024];
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, null);
        }

        private void RecieveCallback(IAsyncResult result)
        {
            int bufferSize = _socket.EndReceive(result);
            byte[] packet = new byte[bufferSize];
            Array.Copy(_buffer, packet, packet.Length);

            _buffer = new byte[1024];
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, null);
        }
    }
}
