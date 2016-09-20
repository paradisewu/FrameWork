using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Game.Msg;

namespace Game
{
    sealed class MsgHandler
    {
        //private static readonly ILog m_Log = LogManager.GetLogger(typeof(MsgHandler));

        private const int WEDDINGMAP_ID = 7001;  

        #region Singleton
        private static readonly MsgHandler m_Instance = new MsgHandler();
        public static MsgHandler Instance { get { return m_Instance; } }
        static MsgHandler()
        {
        }
        private MsgHandler()
        {
        }
        #endregion

        public void Init()
        {
            // GuildBag
            MsgManager.Instance.Register(MsgServerResStore.Type, MsgManager.Instance.OnMsgServerResStore);
            MsgManager.Instance.Register(MsgServerResRetrieve.Type, MsgManager.Instance.OnMsgServerResRetrieve);
            MsgManager.Instance.Register(MsgServerResRemove.Type, MsgManager.Instance.OnMsgServerResRemove);
            MsgManager.Instance.Register(MsgServerResInfo.Type, MsgManager.Instance.OnMsgServerResInfo);
            MsgManager.Instance.Register(MsgServerResBonuses.Type, MsgManager.Instance.OnMsgServerResBonuses);
            MsgManager.Instance.Register(MsgServerResRecord.Type, MsgManager.Instance.OnMsgServerResRecord);
		}
        public void Release()
        {
        }
        public void Reset()
        {
        }
        public void Update()
        {
        }
    }
}
