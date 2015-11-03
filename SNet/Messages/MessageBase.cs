using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNet.Messages
{
    public class MessageBase
    {
        protected byte[] _buffer;

        /// <summary>
        /// Содержимое сообщения
        /// </summary>
        public byte[] Data
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Text => Encoding.ASCII.GetString(_buffer, 4, _buffer.Length - 4);

        /// <summary>
        /// Длина сообщения
        /// </summary>
        public int Length => BitConverter.ToInt16(_buffer, 0);

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public MessageType Type => (MessageType)BitConverter.ToInt16(_buffer, 2);

        /// <summary>
        /// Сообщение
        /// </summary>
        /// <param name="lenght">Длина сообщения</param>
        /// <param name="type">Тип сообщения</param>
        public MessageBase(ushort lenght, MessageType type)
        {
            _buffer = new byte[lenght];
            byte[] lenghtBuf = new byte[2];
            lenghtBuf = BitConverter.GetBytes(lenght);
            byte[] typeBuf = new byte[2];
            typeBuf = BitConverter.GetBytes((ushort)type);
            Buffer.BlockCopy(lenghtBuf, 0, _buffer, 0, 2);
            Buffer.BlockCopy(typeBuf, 0, _buffer, 2, 2);
        }

        public MessageBase(byte[] buffer)
        {
            Data = buffer;
        }
    }
}
