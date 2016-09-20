using System;
using System.IO;
using System.Collections.Generic;
using Game;


namespace Game.Msg
{
    /// <summary>
    /// 发送消息包（包含消息头部）
    /// </summary>
    public class SendMsgPacket : DynamicPacket
    {
        //protected static readonly ILog Log = LogManager.GetLogger(typeof(SendMsgPacket));

        #region Singleton
        private static readonly SendMsgPacket m_Instance = new SendMsgPacket();
        public static SendMsgPacket Instance { get { return m_Instance; } }
        static SendMsgPacket()
        {
        }
        private SendMsgPacket()
            : base()
        {
        }
        #endregion

        public void Init(ushort msgType)
        {
            memStream.SetLength(0);
            memStream.Seek(0, SeekOrigin.Begin);
            uint msgLength = 0;
            writer.Write(msgLength);
            writer.Write(msgType);
        }

        // 写客户端消息时候，最后需要保存长度!!!!!
        private void WriteLength()
        {
            uint msgLength = (uint)memStream.Length;
            memStream.Seek(0, SeekOrigin.Begin);
            writer.Write(msgLength);
            memStream.Seek(msgLength, SeekOrigin.Begin);

            //Log.InfoFormat("SendMsgPacket WriteLength: Len={0}", msgLength);
        }

        public override byte[] GetBuffer()
        {
            WriteLength();
            return memStream.GetBuffer();
        }

        public override byte[] ToArray()
        {
			WriteLength();
            return memStream.ToArray();
        }
    }
}
