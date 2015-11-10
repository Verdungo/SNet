using System;
using System.Text;

namespace SNet.Messages
{
    /// <summary>
    /// Текстовое сообщение
    /// </summary>
    public class TextMessage : MessageBase
    {
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public override string Text => Encoding.UTF8.GetString(Body);

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
