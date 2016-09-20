using System;
using System.IO;
using System.Collections.Generic;
using Game;

namespace Game.Msg
{
    /// <summary>
    /// 接收消息包（不包含消息头部）
    /// </summary>
    public class RecvMsgPacket : DynamicPacket
    {
        #region Singleton
        private static readonly RecvMsgPacket m_Instance = new RecvMsgPacket();
        public static RecvMsgPacket Instance { get { return m_Instance; } }
        static RecvMsgPacket()
        {
        }
        private RecvMsgPacket()
            : base()
        {
        }
        #endregion

        public void Init(MsgDynamic msg)
        {
            memStream.SetLength(0);
            if (msg.Buffer != null)
            {
                memStream.Write(msg.Buffer, 0, msg.GetDataLength());
            }
            memStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
