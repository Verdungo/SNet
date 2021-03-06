﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SNet.Sockets
{
    /// <summary>
    /// Клиент TCP
    /// </summary>
    public class SNetClient
    {
        private Socket _socket;
        byte[] _buffer = new byte[1024];

        /// <summary>
        /// Событие при подключении
        /// </summary>
        public event SocketEventHandler OnConnect;
        
        /// <summary>
        /// Событие при отключении
        /// </summary>
        public event SocketEventHandler OnDisconnect;

        /// <summary>
        /// Событие при посылке сообщения
        /// </summary>
        public event SocketEventHandler OnSend;

        /// <summary>
        /// Событие при получении сообщения
        /// </summary>
        public event SocketEventHandler OnRecieve;

        /// <summary>
        /// Клиент TCP
        /// </summary>
        public SNetClient()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Подключение к удаленному хосту
        /// </summary>
        /// <param name="host">IP адрес хоста</param>
        /// <param name="port">Номер порта</param>
        public void Connect(string host, int port)
        {
            // TODO: вместо IPV4 в явном виде использовать конструкцию ниже
            
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = Array.Find(ipHost.AddressList, ip => ip.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 50001);
            
            try
            {
                _socket.BeginConnect(ipEndPoint, ConnectCallback, null);
            }
            catch (SocketException ex)
            {
                throw new SocketException(ex.ErrorCode);
            }
            
        }

        private void ConnectCallback(IAsyncResult result)
        {
            if (_socket.Connected)
            {
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), null);
                if (OnConnect != null)
                {
                    OnConnect(this, SocketEventArgs.Empty);
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
                Buffer.BlockCopy(_buffer, 0, packet, 0, bufferSize);
                //Array.Copy(_buffer, packet, packet.Length);
                if (OnRecieve != null) 
                {
                    OnRecieve(this, new SocketEventArgs(packet));
                }

                //_buffer = new byte[1024];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), null);
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

        /// <summary>
        /// Посылка сообщения
        /// </summary>
        /// <param name="sendBuf">Буфер сообщения</param>
        public void Send(byte[] sendBuf)
        {
            if (_socket.Connected)
            {
                _socket.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, new AsyncCallback(SendCallback), _socket);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            _socket.EndSend(result);
        }
    }
}
