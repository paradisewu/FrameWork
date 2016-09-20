using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Game.Msg
{
    /// <summary>
    /// 消息结构
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 6)]
    public struct MsgHeader
    {
        [FieldOffset(0)] public int length;
        [FieldOffset(4)] public ushort type;
        [FieldOffset(4)] public byte basetype;
        [FieldOffset(5)] public byte subtype;

        static public int GetHeaderLength()
        {
            return 6;
        }
    }

    public interface MsgCommon : IDynamicData
    {
        uint GetLength();
        void SetLength(uint length);
        int GetHeaderLength();
        int GetDataLength();
        ushort GetMsgType();
        void SetMsgType(ushort type);
    }

    public class MsgBase : IDynamicData
    {
        protected MsgHeader header;

        public virtual void Serialize(DynamicPacket packet)
        {
        }
        public virtual void Deserialize(DynamicPacket packet)
        {
        }
        public int GetLength()
        {
            return header.length;
        }
        public void SetLength(int length)
        {
            header.length = length;
        }
        public int GetHeaderLength()
        {
            return MsgHeader.GetHeaderLength();
        }
        public int GetDataLength()
        {
            return (GetLength() - GetHeaderLength());
        }
        public ushort GetMsgType()
        {
            return header.type;
        }
        public void SetMsgType(ushort type)
        {
            header.type = type;
        }
        public byte GetBaseType()
        {
            return header.basetype;
        }
        protected void SetBaseType(byte value)
        {
            header.basetype = value;
        }
        public byte GetSubType()
        {
            return header.subtype;
        }
        protected void SetSubType(byte value)
        {
            header.subtype = value;
        }
    }

    public class MsgDynamic : MsgBase, IDisposable
    {
        private byte[] m_Buffer;

        public byte[] Buffer
        {
            get { return m_Buffer; }
            set { m_Buffer = value; }
        }
        public void Dispose()
        {
            BufferPool.ReleaseBufferToPool(ref m_Buffer);
        }
    }
}
