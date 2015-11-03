using System;
using System.Net;
using System.Net.Sockets;

namespace SNet
{
    public class SnetServer
    {
        private Socket _socket;
        private byte[] _buffer;

        public SnetServer()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Bind(int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public void Listen(int backlog)
        {
            _socket.Listen(backlog);
        }

        public void Accept()
        {
            _socket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult result)
        {
            Socket clientSocket = _socket.EndAccept(result);
            _buffer = new byte[1024];
            clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, clientSocket);
        }

        private void RecieveCallback(IAsyncResult result)
        {
            Socket clientSocket = result.AsyncState as Socket;
            int bufferSize = clientSocket.EndReceive(result);
            byte[] packet = new byte[bufferSize];
            Array.Copy(_buffer, packet, packet.Length);

            _buffer = new byte[1024];
            clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, clientSocket);
        }
    }
}
