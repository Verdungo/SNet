using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNet.Messages
{
    /// <summary>
    /// Базовый класс сообщений
    /// </summary>
    public class MessageBase
    {
        /// <summary>
        /// Содержимое сообщения
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Тело сообщения
        /// </summary>
        public byte[] Body => Buffer.Skip(4).ToArray();

        /// <summary>
        /// Длина сообщения
        /// </summary>
        public int Length => BitConverter.ToInt16(Buffer, 0);

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public MessageType Type => (MessageType)BitConverter.ToInt16(Buffer, 2);

        /// <summary>
        /// Сообщение
        /// </summary>
        /// <param name="lenght">Длина сообщения</param>
        /// <param name="type">Тип сообщения</param>
        public MessageBase(ushort lenght, MessageType type)
        {
            Buffer = new byte[lenght];
            byte[] lenghtBuf = new byte[2];
            lenghtBuf = BitConverter.GetBytes(lenght);
            byte[] typeBuf = new byte[2];
            typeBuf = BitConverter.GetBytes((ushort)type);
            System.Buffer.BlockCopy(lenghtBuf, 0, Buffer, 0, 2);
            System.Buffer.BlockCopy(typeBuf, 0, Buffer, 2, 2);
        }

        /// <summary>
        /// Базовый класс сообщения
        /// </summary>
        /// <param name="buffer">Буфер сообщения</param>
        public MessageBase(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}
