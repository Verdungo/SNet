using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SNet.Sockets
{
    public class SNetServer
    {
        private Socket _socket;
        private List<Socket> _clients;
        private byte[] _buffer;

        public event SocketEventHandler OnRecieve;
        public event SocketEventHandler OnClientConnect;
        public event SocketEventHandler OnClientDisconnect;

        public SNetServer()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<Socket>();
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
            _clients.Add(clientSocket);

            _buffer = new byte[1024];
            clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, clientSocket);
            if (OnClientConnect != null)
            {
                OnClientConnect(this, SocketEventArgs.Empty);
            }
            Accept();
        }

        private void RecieveCallback(IAsyncResult result)
        {
            Socket clientSocket = result.AsyncState as Socket;

            try
            {
                int bufferSize = clientSocket.EndReceive(result);
                byte[] packet = new byte[bufferSize];
                Array.Copy(_buffer, packet, packet.Length);
                if (OnRecieve != null)
                {
                    OnRecieve(this, new SocketEventArgs(packet));
                }
                _buffer = new byte[1024];
                clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, RecieveCallback, clientSocket);

            }
            catch (Exception)
            {
                clientSocket.Close();
                clientSocket.Dispose();
                _clients.Remove(clientSocket);

                if (OnClientDisconnect != null)
                {
                    OnClientDisconnect(this, SocketEventArgs.Empty);
                }
            }
        }

        public void SendToAllClients(byte[] sendBuf)
        {
            foreach (var client in _clients)
            {
                client.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, SendCallback, client);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            Socket clientSocket = result.AsyncState as Socket;
            clientSocket.EndSend(result);
        }
    }
}
