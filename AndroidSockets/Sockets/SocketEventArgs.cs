using AndroidSockets.Messages;
using System;

namespace AndroidSockets.Sockets
{
    /// <summary>
    /// Аргументы события сокетаы
    /// </summary>
    public class SocketEventArgs : EventArgs
    {
        /// <summary>
        /// Содержимое буфера для передачи обработчику
        /// </summary>
        public MessageBase Message;

        /// <summary>
        /// Пустой аргумент
        /// </summary>
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

    /// <summary>
    /// Событие сокета
    /// </summary>
    /// <param name="sender">Объект, вызавающий событие</param>
    /// <param name="e">Аргументы события сокета</param>
    public delegate void SocketEventHandler(object sender, SocketEventArgs e);
}
