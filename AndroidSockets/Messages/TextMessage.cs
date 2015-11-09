using System;
using System.Text;

namespace AndroidSockets.Messages
{
    /// <summary>
    /// Текстовое сообщение
    /// </summary>
    public class TextMessage : MessageBase
    {
        /// <summary>
        /// Текстовое сообщение
        /// </summary>
        public TextMessage(string msg)
            : base((ushort)(4 + Encoding.UTF8.GetByteCount(msg)), MessageType.MessageText)
        {

            byte[] msgBuf = Encoding.UTF8.GetBytes(msg);
            System.Buffer.BlockCopy(msgBuf, 0, Buffer, 4, msgBuf.Length);
        }
    }
}
