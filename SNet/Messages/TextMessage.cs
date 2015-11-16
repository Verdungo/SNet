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
        public string Text => Encoding.UTF8.GetString(Body);

        /// <summary>
        /// Текстовое сообщение
        /// </summary>
        public TextMessage()
            : base(4, MessageType.TextMessage)
        {

        }

        /// <summary>
        /// Текстовое сообщение
        /// </summary>
        public TextMessage(byte[] buffer)
            : base(buffer)
        {

        }

        /// <summary>
        /// Текстовое сообщение
        /// </summary>
        public TextMessage(string msg) 
            : base((ushort)(4 + Encoding.UTF8.GetByteCount(msg)), MessageType.TextMessage)
        {
            byte[] msgBuf = Encoding.UTF8.GetBytes(msg); 
            System.Buffer.BlockCopy(msgBuf, 0, Buffer, 4, msgBuf.Length);
        }

        /// <summary>
        /// Возвращает TextMessage основанный на MessageBase
        /// </summary>
        /// <param name="messageBase">Базовое сообщение</param>
        /// <returns></returns>
        public static TextMessage FromMessageBase(MessageBase messageBase)
        {
            return new TextMessage(messageBase.Buffer);
        }
    }
}
