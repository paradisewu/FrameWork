using System;
using System.Collections.Generic;

namespace Game.Msg
{

    public class MsgGStorage : MsgBase
    {
        protected const byte BaseType = 26;
    }

    public class MsgClientReqStore : MsgGStorage 
    {
        protected const byte SubType = 1;
        public const ushort Type = (SubType << 8) + BaseType;
        public long m_lItemGuid;
        public uint m_uiMoneyPrice;

        public MsgClientReqStore()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_lItemGuid );
            packet.Write( m_uiMoneyPrice );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_lItemGuid );
            packet.Read( out m_uiMoneyPrice );
        }
    }

    public class MsgServerResStore : MsgGStorage 
    {
        protected const byte SubType = 2;
        public const ushort Type = (SubType << 8) + BaseType;
        public bool m_bResult;
        public uint m_uiIndex;

        public MsgServerResStore()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_bResult );
            packet.Write( m_uiIndex );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_bResult );
            packet.Read( out m_uiIndex );
        }
    }

    public class MsgClientReqRetrieve : MsgGStorage 
    {
        protected const byte SubType = 3;
        public const ushort Type = (SubType << 8) + BaseType;
        public long m_lItemGuid;
        public uint m_uiCount;

        public MsgClientReqRetrieve()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_lItemGuid );
            packet.Write( m_uiCount );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_lItemGuid );
            packet.Read( out m_uiCount );
        }
    }

    public class MsgServerResRetrieve : MsgGStorage 
    {
        protected const byte SubType = 4;
        public const ushort Type = (SubType << 8) + BaseType;
        public bool m_bResult;
        public long m_lItemGuid;
        public uint m_uiCount;
        public uint m_uiMoney;

        public MsgServerResRetrieve()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_bResult );
            packet.Write( m_lItemGuid );
            packet.Write( m_uiCount );
            packet.Write( m_uiMoney );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_bResult );
            packet.Read( out m_lItemGuid );
            packet.Read( out m_uiCount );
            packet.Read( out m_uiMoney );
        }
    }

    public class MsgClientReqRemove : MsgGStorage 
    {
        protected const byte SubType = 5;
        public const ushort Type = (SubType << 8) + BaseType;
        public long m_lItemGuid;

        public MsgClientReqRemove()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_lItemGuid );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_lItemGuid );
        }
    }

    public class MsgServerResRemove : MsgGStorage 
    {
        protected const byte SubType = 6;
        public const ushort Type = (SubType << 8) + BaseType;
        public bool m_bResult;
        public long m_lItemGuid;

        public MsgServerResRemove()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_bResult );
            packet.Write( m_lItemGuid );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_bResult );
            packet.Read( out m_lItemGuid );
        }
    }

    public class MsgClientReqInfo : MsgGStorage 
    {
        protected const byte SubType = 7;
        public const ushort Type = (SubType << 8) + BaseType;

        public MsgClientReqInfo()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
        }

        public override void Deserialize(DynamicPacket packet)
    	{
        }
    }

    public class MsgServerResInfo : MsgGStorage 
    {
        protected const byte SubType = 8;
        public const ushort Type = (SubType << 8) + BaseType;
        public uint m_uiMoney;
        public uint m_uiCapacity;
        public List<uint> m_oVecItemIndexVec = new List<uint>();

        public MsgServerResInfo()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_uiMoney );
            packet.Write( m_uiCapacity );
            packet.Write( m_oVecItemIndexVec );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_uiMoney );
            packet.Read( out m_uiCapacity );
            packet.Read( out m_oVecItemIndexVec );
        }
    }

    public class MsgClientReqBonuses : MsgGStorage 
    {
        protected const byte SubType = 9;
        public const ushort Type = (SubType << 8) + BaseType;

        public MsgClientReqBonuses()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
        }

        public override void Deserialize(DynamicPacket packet)
    	{
        }
    }

    public class MsgServerResBonuses : MsgGStorage 
    {
        protected const byte SubType = 10;
        public const ushort Type = (SubType << 8) + BaseType;
        public bool m_bResult;
        public uint m_uiMoney;

        public MsgServerResBonuses()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_bResult );
            packet.Write( m_uiMoney );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_bResult );
            packet.Read( out m_uiMoney );
        }
    }

    public class MsgClientReqRecord : MsgGStorage 
    {
        protected const byte SubType = 11;
        public const ushort Type = (SubType << 8) + BaseType;
        public uint m_uiPage;

        public MsgClientReqRecord()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_uiPage );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_uiPage );
        }
    }

    public class MsgServerResRecord : MsgGStorage 
    {
        protected const byte SubType = 12;
        public const ushort Type = (SubType << 8) + BaseType;
        public uint m_uiPage;
        public List<string> m_oVecRecords = new List<string>();

        public MsgServerResRecord()
        {
            SetMsgType( Type );
        }

        public override void Serialize(DynamicPacket packet)
    	{
            packet.Write( m_uiPage );
            packet.Write( m_oVecRecords );
        }

        public override void Deserialize(DynamicPacket packet)
    	{
            packet.Read( out m_uiPage );
            packet.Read( out m_oVecRecords );
        }
    }

}


