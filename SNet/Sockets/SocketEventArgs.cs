using SNet.Messages;
using System;

namespace SNet.Sockets
{
    public class SocketEventArgs : EventArgs
    {
        /// <summary>
        /// Содержимое буфера для передачи обработчику
        /// </summary>
        public MessageBase Message;

        public static readonly new SocketEventArgs Empty;

        /// <summary>
        /// Аргумент события сокета
        /// </summary>
        public SocketEventArgs()
        { }

        /// <summary>
        /// Аргумент события сокета
        /// </summary>
        /// <param name="buffer">Содержимое буфера для передачи обработчику</param>
        public SocketEventArgs(byte[] buffer)
        {
            Message = new MessageBase(buffer);    
        }
    }

    public delegate void SocketEventHandler(object o, SocketEventArgs e);
}
