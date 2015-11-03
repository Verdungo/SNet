using System;
using System.Text;

namespace SNet.Messages
{
    public class MessageText : MessageBase
    {
        public MessageText(string msg) 
            : base((ushort)(4 + msg.Length), MessageType.MessageText)
        {
            
            byte[] msgBuf = new byte[msg.Length];
            //msgBuf = Encoding.UTF8.GetBytes(msg);
            msgBuf = Encoding.ASCII.GetBytes(msg);
            Buffer.BlockCopy(msgBuf, 0, _buffer, 4, msgBuf.Length);
        }
    }
}
