using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace AndroidSockets.Sockets
{
    /// <summary>
    /// Сервер TCP
    /// </summary>
    public class SNetServer
    {
        private Socket _socket;
        private List<Socket> _clients;
        private byte[] _buffer = new byte[1024];

        /// <summary>
        /// Событие при получении сообщения
        /// </summary>
        public event SocketEventHandler OnRecieve;

        /// <summary>
        /// Событие при подключении клиента
        /// </summary>
        public event SocketEventHandler OnClientConnect;

        /// <summary>
        /// Событие при отключении клиента
        /// </summary>
        public event SocketEventHandler OnClientDisconnect;

        /// <summary>
        /// Сервер TCP
        /// </summary>
        public SNetServer()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<Socket>();
        }

        /// <summary>
        /// Ассоциация сокета с портом
        /// </summary>
        /// <param name="port">Номер порта</param>
        public void Bind(int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// Перевод сокета в режум слушания
        /// </summary>
        /// <param name="backlog">Максимальная очередь подключений </param>
        public void Listen(int backlog)
        {
            _socket.Listen(backlog);
        }

        /// <summary>
        /// Ожидание получения данных
        /// </summary>
        public void Accept()
        {
            _socket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult result)
        {
            Socket clientSocket = _socket.EndAccept(result);
            _clients.Add(clientSocket);

            //_buffer = new byte[1024];
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
                Buffer.BlockCopy(_buffer, 0, packet, 0, bufferSize);
                if (OnRecieve != null)
                {
                    OnRecieve(this, new SocketEventArgs(packet));
                }
                //_buffer = new byte[1024];
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

        /// <summary>
        /// Посылка сообщения всем активным клиентам
        /// </summary>
        /// <param name="sendBuf">Буфер сообщения</param>
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
