using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Game.Msg;

namespace Game
{
    sealed class MsgManager
    {
        //private static readonly ILog m_Log = LogManager.GetLogger(typeof(MsgManager));
        private Dictionary<ushort, Action<RecvMsgPacket>> m_MsgHandlers = new Dictionary<ushort, Action<RecvMsgPacket>>();

        #region Singleton
        private static readonly MsgManager m_Instance = new MsgManager();
        public static MsgManager Instance { get { return m_Instance; } }
        static MsgManager()
        {
        }
        private MsgManager()
        {
        }
        #endregion

        const int ShowSkillHelpLevel = 21;

        public void Register(ushort msgId, Action<RecvMsgPacket> msgHandler)
        {
            m_MsgHandlers.Add(msgId, msgHandler);
        }
 
        public void ProcessMsg(MsgDynamic msgSrc)
        {
            RecvMsgPacket.Instance.Init(msgSrc);
            RecvMsgPacket packet = RecvMsgPacket.Instance;
            Action<RecvMsgPacket> msgHandler = null;
            if (m_MsgHandlers.TryGetValue(msgSrc.GetMsgType(), out msgHandler))
            {
                msgHandler(packet);
                return;
            }
            else
            {
                //m_Log.InfoFormat("Receive unregister msg, id:{0} ?", msgSrc.GetMsgType()); 
            }

            //m_Log.DebugFormat("End of ProcessMsg Msg: Type: {0}(Base{1}/Sub{2})",
            //  msgSrc.GetMsgType(), msgSrc.GetBaseType(), msgSrc.GetSubType());
        }
        public void ClientReqStore(long itemGuid, uint price)
        {
            MsgClientReqStore msg = new MsgClientReqStore();
            msg.m_lItemGuid = itemGuid;
            msg.m_uiMoneyPrice = price;
            InitServerConfig.Instance.Network.Send(msg);
        }
        public void ClientReqRetrieve(long itemGuid, uint count)
        {
            MsgClientReqRetrieve msg = new MsgClientReqRetrieve();
            msg.m_lItemGuid = itemGuid;
            msg.m_uiCount = count;
            InitServerConfig.Instance.Network.Send(msg);
        }
        public void ClientReqRecord()
        {
            MsgClientReqRecord msg = new MsgClientReqRecord();
            msg.m_uiPage = 1;
            InitServerConfig.Instance.Network.Send(msg);
        }

        public void OnMsgServerResStore(RecvMsgPacket packet)
        {

        }
        public void OnMsgServerResRetrieve(RecvMsgPacket packet)
        {

        }
        public void OnMsgServerResRemove(RecvMsgPacket packet)
        {

        }
        public void OnMsgServerResInfo(RecvMsgPacket packet)
        {

        }
        public void OnMsgServerResBonuses(RecvMsgPacket packet)
        {

        }
        public void OnMsgServerResRecord(RecvMsgPacket packet)
        {

        }
    }
}
