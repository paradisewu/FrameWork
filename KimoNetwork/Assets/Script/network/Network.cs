using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Game.Msg;

namespace Game
{
    public class MsgRecvQueue
    {
        public const int MaxMsgNum = 1000;
        private volatile int m_ReadIndex;       // 由读线程改变
        private volatile int m_WriteIndex;      // 由写线程改变
        private MsgDynamic[] m_MsgNode = new MsgDynamic[MaxMsgNum];

        public MsgRecvQueue()
        {
        }

        public int Count()
        {
            return (m_WriteIndex + MaxMsgNum - m_ReadIndex) % MaxMsgNum;
        }

        public bool Push(MsgDynamic msg)
        {
            int nNextIdx = (m_WriteIndex + 1) % MaxMsgNum;
            if (nNextIdx == m_ReadIndex)
            {
                return false;
            }

            m_MsgNode[m_WriteIndex] = msg;
            m_WriteIndex = nNextIdx;
            return true;
        }

        public MsgDynamic Pop()
        {
            if (m_ReadIndex == m_WriteIndex)
            {
                return null;
            }

            int nIdx = m_ReadIndex;
            m_ReadIndex = (m_ReadIndex + 1) % MaxMsgNum;
            return m_MsgNode[nIdx];
        }
    }

    public class MsgRecvBuffer
    {
        public const int MaxMsgLength = 1024 * 1024;                    // 与服务器约定：最大的消息长度为1M。建议后续项目使用64K就足够 
        public const int RecvBufferLength = MaxMsgLength + 1024;        // Recv Buffer比最大消息长度再大一点
        public const int MaxStartPos = (RecvBufferLength - 1024);       // 起始位置最大值，必须比RecvBufferLength小！
        private byte[] m_Buffer = new byte[RecvBufferLength];

        public MsgRecvBuffer()
        {
        }

        public int RecvBytes { get; set; }
        public int MsgLength { get; set; }
        public int StartPos { get; set; }
        public byte[] Buffer
        {
            get { return m_Buffer; }
        }

        public void Reset()
        {
            RecvBytes = 0;
            MsgLength = 0;
            StartPos = 0;
        }
    }

    enum NetworkFailState
    {
        Invalid,
        Connect,
        Receive,
    }

    public sealed class Session
    {
        private static readonly Session m_Instance = new Session();
        //private static readonly ILog m_Log = LogManager.GetLogger(typeof(Session));
        private MsgRecvQueue m_msgQueue = new MsgRecvQueue();
        private MsgRecvBuffer m_msgBuffer = new MsgRecvBuffer();
        private Socket m_Socket = null;
        private int m_NetworkFailState = 0; // 用该标识的原因是：在回调函数中无法显示错误信息

        const int MaxSendMsgNum = 30;
        const int SendMsgCycle = 3 * 1000;
        const int MaxRPCWaitTime = 60 * 1000;
        private int m_CurSendMsgNum = 0;
        private int m_LastResetTime;

        private Dictionary<ushort, ushort> m_RPCMsgs = new Dictionary<ushort, ushort>();
        private Dictionary<ushort, int> m_WaitForAnswer = new Dictionary<ushort, int>();

        private bool m_Reconnect = true;

        private Session()
        {
            m_LastResetTime = Environment.TickCount;
        }

        public static Session Instance
        {
            get { return m_Instance; }
        }

        public int GetMsgCount()
        {
            return m_msgQueue.Count();
        }

        public MsgDynamic GetMsg()
        {
            return m_msgQueue.Pop();
        }

        public void RegisterRPC(ushort clientMsgType, ushort serverMsgType)
        {
            if (m_RPCMsgs.ContainsKey(clientMsgType))
            {
                //m_Log.DebugFormat("RPCMsg conflict! Client Msg Type: {0}, Server Msg Type: {1}");
            }
            m_RPCMsgs[serverMsgType] = clientMsgType;
            m_WaitForAnswer[clientMsgType] = 0;
        }
        public void ResetRPCWait(ushort clientMsgType)
        {
            if (m_WaitForAnswer.ContainsKey(clientMsgType))
            {
                m_WaitForAnswer[clientMsgType] = 0;
            }
        }

        public bool IsConnected()
        {
            if (null != m_Socket)
            {
                return m_Socket.Connected;
            }
            return false;
        }

        public void Connect(string strHost, int nPort)
        {
            if (null != m_Socket && m_Socket.Connected)
                return;

            if (string.IsNullOrEmpty(strHost))
            {
                //m_Log.ErrorFormat("Network Connect: Host is wrong and port is{0}", nPort);
                return;
            }
            else
            {
                //m_Log.DebugFormat("Network Connect to: {0}:{1}", strHost, nPort);
            }

            m_Host = strHost;
            m_Port = nPort;
            m_Reconnect = true;

            try
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult asyncConnect = m_Socket.BeginConnect(m_Host, m_Port, new AsyncCallback(ConnectCallback), m_Socket);
            }
            catch (System.Exception ex)
            {
                //m_Log.WarnFormat("Network Connect Exception: {0} / {1}", ex.ToString(), ex.Message);
            }
        }

        void ConnectCallback(IAsyncResult asyncConnect)
        {
            try
            {
                m_Socket.EndConnect(asyncConnect);
                if (!m_Socket.Connected)
                {
                    //m_Log.Warn("Network is not connected!!!");
                    StartReconnect();
                    return;
                }
                else
                {
                    //ConnectCallback();
                }

                ReadData();
            }
            catch (Exception e)
            {
                m_NetworkFailState = (int)NetworkFailState.Connect;
                //m_Log.WarnFormat("Network ConnectCallback Exception: {0} / {1}", e.ToString(), e.Message);
            }
        }

        void MoveBuffer()
        {
            if (m_msgBuffer.RecvBytes > 0 && m_msgBuffer.StartPos > 0)
            {
                //m_Log.ErrorFormat("Debug Info: MoveBuffer: We move {0} bytes from pos {1} to buffer_begin.", m_msgBuffer.RecvBytes, m_msgBuffer.StartPos);

                Buffer.BlockCopy(m_msgBuffer.Buffer, m_msgBuffer.StartPos, m_msgBuffer.Buffer, 0, m_msgBuffer.RecvBytes);
                m_msgBuffer.StartPos = 0;
            }
        }

        void ReadData()
        {
            if (m_Socket == null)
            {
                StartReconnect();
                return;
            }
            if (m_Socket.Connected == false)
            {
                StartReconnect();
                return;
            }
            //m_Log.DebugFormat("{0}", "Network is reading data.");

            if (m_msgBuffer.StartPos >= MsgRecvBuffer.MaxStartPos)
            {
                //m_Log.ErrorFormat("Debug Info: ReadData: Buffer StartPos {0} > MaxStartPos {1}", m_msgBuffer.StartPos, MsgRecvBuffer.MaxStartPos);

                MoveBuffer();
            }

            m_Socket.BeginReceive(
                     m_msgBuffer.Buffer,
                     m_msgBuffer.StartPos + m_msgBuffer.RecvBytes,
                     Math.Min((m_msgBuffer.Buffer.Length - m_msgBuffer.RecvBytes - m_msgBuffer.StartPos), MsgRecvBuffer.MaxMsgLength),
                     SocketFlags.None,
                     new AsyncCallback(this.ReceiveCallback),
                     m_msgBuffer);
        }

        //private int m_MsgLargeCount = 0;
        void ReceiveCallback(IAsyncResult asyncReceive)
        {
            if (m_Socket == null)
            {
                //m_Log.InfoFormat("{0}", "Network is reading data. Socket == null");               
                StartReconnect();
                return;
            }
            if (m_Socket.Connected == false)
            {
                //m_Log.InfoFormat("{0}", "Network is reading data. But NOT Connected");             
                StartReconnect();
                return;
            }

            try
            {
                SocketError errorCode;
                int bytesReceived = m_Socket.EndReceive(asyncReceive, out errorCode);
                if (bytesReceived > 0)
                {
                    m_msgBuffer.RecvBytes += bytesReceived;
                    while (m_msgBuffer.RecvBytes >= sizeof(Int32))
                    {
                        m_msgBuffer.MsgLength = BitConverter.ToInt32(m_msgBuffer.Buffer, m_msgBuffer.StartPos);
                        if (m_msgBuffer.MsgLength < MsgHeader.GetHeaderLength() ||
                            m_msgBuffer.MsgLength > MsgRecvBuffer.MaxMsgLength)
                        {
                            //m_Log.ErrorFormat("ReceiveCallback: Invalid message, MsgLength = {0}", m_msgBuffer.MsgLength);
                            CloseSocket();
                            return;
                        }

                        //m_Log.DebugFormat("ReceiveCallback: Loop RecvBytes = {0} StartPos = {1} MsgLength = {2}", m_msgBuffer.RecvBytes, m_msgBuffer.StartPos, m_msgBuffer.MsgLength);

                        if (m_msgBuffer.RecvBytes >= m_msgBuffer.MsgLength)
                        {
                            MsgDynamic msg = new MsgDynamic();
                            msg.SetLength(m_msgBuffer.MsgLength);
                            ushort uType = BitConverter.ToUInt16(m_msgBuffer.Buffer, m_msgBuffer.StartPos + 4);
                            msg.SetMsgType(uType);
                            int bufferLength = (int)(msg.GetLength() - msg.GetHeaderLength());
                            if (bufferLength > 0)
                            {
                                if (bufferLength > BufferPool.BufferLength)
                                {
                                    msg.Buffer = new byte[bufferLength];
                                    //m_Log.DebugFormat("Count {0}: Network received a too long msg type {1}({2}/{3}), length {4}", ++m_MsgLargeCount, uType, msg.GetBaseType(), msg.GetSubType(), msg.GetLength());
                                }
                                else
                                {
                                    msg.Buffer = BufferPool.GetBuffer();
                                }
                                Array.Copy(m_msgBuffer.Buffer, m_msgBuffer.StartPos + msg.GetHeaderLength(), msg.Buffer, 0, bufferLength);
                            }
                            else if (bufferLength < 0)
                            {
                                //m_Log.ErrorFormat("ReceiveCallback: Invalid message, DataLength = {0}", bufferLength);
                                CloseSocket();
                                return;
                            }
                            else
                            {
                                // bufferLength == 0
                            }
                            m_msgQueue.Push(msg);

                            //ushort clientMsgType;
                            //if (m_RPCMsgs.TryGetValue(msg.GetMsgType(), out clientMsgType))
                            //{
                            //    if (m_WaitForAnswer.ContainsKey(clientMsgType))
                            //    {
                            //        m_WaitForAnswer[clientMsgType] = 0;
                            //    }
                            //}

                            m_msgBuffer.RecvBytes -= m_msgBuffer.MsgLength;
                            m_msgBuffer.StartPos += m_msgBuffer.MsgLength;

                            if (m_msgBuffer.RecvBytes >= sizeof(Int32))
                            {
                                // 继续解析MsgLength
                                continue;
                            }
                            else if (m_msgBuffer.RecvBytes > 0)
                            {
                                m_msgBuffer.MsgLength = 0;
                                break;
                            }
                            else if (m_msgBuffer.RecvBytes == 0)
                            {
                                // 数据刚好读完，可以重新设置StartPos
                                m_msgBuffer.StartPos = 0;
                                m_msgBuffer.MsgLength = 0;
                                break;
                            }
                            else
                            {
                                // (m_msgBuffer.RecvBytes < 0) 数据异常
                                //m_Log.ErrorFormat("ReceiveCallback: Buffer recv size error, RecvBytes = {0}, StartPos = {1}",
                                //    m_msgBuffer.RecvBytes, m_msgBuffer.StartPos);

                                CloseSocket();
                                return;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    //m_Log.DebugFormat("ReceiveCallback: Endl RecvBytes = {0} StartPos = {1} MsgLength = {2}", m_msgBuffer.RecvBytes, m_msgBuffer.StartPos, m_msgBuffer.MsgLength);
                    
                    if (m_msgBuffer.MsgLength > MsgRecvBuffer.RecvBufferLength - m_msgBuffer.StartPos)
                    {
                        //m_Log.ErrorFormat("Debug Info: ReceiveCallback: Not enough buffer size: MsgLength {0} > (buffer total size {1} - buffer StartPos {2})",
                        //    m_msgBuffer.MsgLength, MsgRecvBuffer.RecvBufferLength, m_msgBuffer.StartPos);

                        MoveBuffer();
                    }

                    //m_Log.DebugFormat("{0}: {1}", "Network received data. Length is", bytesReceived);
                }
                else
                {
                    //m_Log.InfoFormat("Network recv byte: {0}, errorCode:{1}", bytesReceived, errorCode);
                }

                ReadData();
            }
            catch (Exception ex) // Never called, EXCEPTION WHERE ARE YOU???
            {
                m_NetworkFailState = (int)NetworkFailState.Receive;
                CloseSocket();
                //m_Log.WarnFormat("Network ReceiveCallback Exception: {0} / {1}", ex.ToString(), ex.Message);
            }
        }

        void SendCallBack(IAsyncResult iar)
        {
            if (m_Socket == null)
            {
                StartReconnect();
                return;
            }
            if (m_Socket.Connected == false)
            {
                StartReconnect();
                return;
            }

            int sent = m_Socket.EndSend(iar);

            //m_Log.InfoFormat("Network sended data: Len: {0}", sent);
        }

        private void SendData(byte[] data, int len)
        {
            if (m_Socket == null)
            {
                StartReconnect();
                return;
            }
            if (m_Socket.Connected == false)
            {
                StartReconnect();
                return;
            }

            try
            {
                //ushort uMsgType = 0;
                //byte uBaseType = 0;
                //byte uSubType = 0;
                if (len >= 6)
                {
                    uint uMsgLength = BitConverter.ToUInt32(data, 0);
                    //byte u0 = data[0];
                    //byte u1 = data[1];
                    //byte u2 = data[2];
                    //byte u3 = data[3];
                    //uMsgType = BitConverter.ToUInt16(data, 4);
                    //uBaseType = data[4];
                    //uSubType = data[5];

                    //m_Log.InfoFormat("Network is sending data: Byte0:{0},1:{1},2:{2},3:{3}", u0, u1, u2, u3);
                    if (uMsgLength != len)
                    {
                        //m_Log.ErrorFormat("{0}: msglength: {1} != bufferLen: {2}",
                        //    "Network is sending data. But length is wrong",
                        //    uMsgLength, len);
                    }
                }
                //m_Log.InfoFormat("Network is sending data: Len: {0}, Type: {1}(Base={2}/Sub={3})",
                //    len, uMsgType, uBaseType, uSubType);

                m_Socket.BeginSend(data, 0, len, SocketFlags.None, new AsyncCallback(this.SendCallBack), null);
            }
            catch (System.Exception ex)
            {
                //m_Log.WarnFormat("Network SendData Exception: {0} / {1}", ex.ToString(), ex.Message);
            }
        }

        public void Send(MsgBase msg)
        {
            if (msg == null)
            {
                return;
            }

            //int sendTime;
            //if (m_WaitForAnswer.TryGetValue(msg.GetMsgType(), out sendTime))
            //{
            //    if (sendTime != 0 && Environment.TickCount - sendTime < MaxRPCWaitTime)
            //    {
            //        return;
            //    }
            //}

            if (Environment.TickCount - m_LastResetTime > SendMsgCycle)
            {
                m_CurSendMsgNum = 0;
                m_LastResetTime = Environment.TickCount;
            }
            else
            {
                if (m_CurSendMsgNum >= MaxSendMsgNum)
                {
                    //m_Log.ErrorFormat("Too Much Message! Type: {0}", msg.GetMsgType());
                    return;
                }
            }

            if (m_CurSendMsgNum == 0)
            {
                m_LastResetTime = Environment.TickCount;
            }

            SendMsgPacket.Instance.Init(msg.GetMsgType());
            SendMsgPacket rPacket = SendMsgPacket.Instance;
            msg.Serialize(rPacket);
            SendData(rPacket.ToArray(), rPacket.GetLength());


            //if (m_WaitForAnswer.ContainsKey(msg.GetMsgType()))
            //{
            //    m_WaitForAnswer[msg.GetMsgType()] = Environment.TickCount;
            //}

            ++m_CurSendMsgNum;
        }

        public void CloseSocket()
        {
            m_msgBuffer.Reset();
            if (m_Socket == null)
            {
                return;
            }
            if (m_Socket.Connected == true)
            {
                m_Socket.Shutdown(SocketShutdown.Both);
                m_Socket.Close();
            }
        }

        public void CloseSocket(bool reconnect)
        {
            CloseSocket();
            m_Reconnect = reconnect;
        }

        #region Reconnect Logic
        public enum enumReconnectState
        {
            Default,
            Auto,
            Manual
        }
        private string m_Host = string.Empty;
        private int m_Port = 0;
        private bool m_StartReconnect = false;

        private enumReconnectState m_ReconnectState = enumReconnectState.Default;
        private float m_LastReconnectTime = 0.0f;
        private int m_ReconnectCount = 0; //当前断线重连次数
        private bool m_CanShowMsgBox = true;
        private bool m_IsReconnected = false;
        private bool m_CanReconnect = false; //目前暂定为进入世界场景时启动断线重连逻辑，之后在选人界面断线时加入

        //客户端发心跳包逻辑相关变量
        private float m_LastRecvMsgTime = 0.0f;
        private int m_PingCount = 0;
        private bool m_CanCheckRecvMsgState = false;

        private void StartReconnect()
        {
            if (!m_Reconnect)
            {
                return;
            }
            if (m_ReconnectState == enumReconnectState.Auto ||
                m_ReconnectState == enumReconnectState.Manual)
            {
                return;
            }
            else
            {
                if (m_CanReconnect)
                    m_StartReconnect = true;
            }
        }
        public void CheckIfReconnect()
        {
            if (0 != m_NetworkFailState)
            {
                switch (m_NetworkFailState)
                {
                    case (int)NetworkFailState.Connect:
                        {
                        }
                        break;
                    case (int)NetworkFailState.Receive:
                        {
                        }
                        break;
                }
                m_NetworkFailState = 0;
            }

            if (m_IsReconnected)
            {
                m_IsReconnected = false;
                return;
            }
            TryToPingServer();

            if (m_StartReconnect == true)
            {
                m_StartReconnect = false;

                m_ReconnectState = enumReconnectState.Auto;
                m_ReconnectCount = 0;
                m_CanShowMsgBox = true;
                Reconnect();
                return;
            }

            if (m_ReconnectState != enumReconnectState.Default && TimeToReconnect())
            {
                switch (m_ReconnectState)
                {
                    case enumReconnectState.Manual:
                        if (m_CanShowMsgBox == true)
                        {
                        }
                        break;

                    case enumReconnectState.Auto:
                        Reconnect();
                        break;
                }
            }
        }
        private void Reconnect()
        {
            m_LastReconnectTime = Time.realtimeSinceStartup;
            m_ReconnectCount++;
            if (null != m_Socket && true == m_Socket.Connected)
            {
                //ConnectCallback();
            }
            else
            {
                CloseSocket();
                Connect(m_Host, m_Port);
            }
        }

        public void UpdateRecvMsgTime()
        {
            m_LastRecvMsgTime = Time.realtimeSinceStartup;
        }
        private void TryToPingServer()
        {
            if (!m_CanCheckRecvMsgState)
            {
                return;
            }
            if (m_ReconnectState == enumReconnectState.Auto ||
                m_ReconnectState == enumReconnectState.Manual)
            {
                return;
            }

            float inteval = 10;
            int pingCount = 30;

            if (Time.realtimeSinceStartup - m_LastRecvMsgTime > inteval)
            {
                m_PingCount++;
                if (m_PingCount <= pingCount)
                {
                    m_LastRecvMsgTime = Time.realtimeSinceStartup;
                }
            }

            if (m_PingCount > pingCount)
            {
                m_PingCount = 0;
                CloseSocket();
                StartReconnect();
            }
        }
        public void OnMsgSeverPingMsg(RecvMsgPacket packet)
        {

            m_PingCount = 0;
            m_LastRecvMsgTime = Time.realtimeSinceStartup;
        }

        private bool TimeToReconnect()
        {
            return Time.realtimeSinceStartup - m_LastReconnectTime > 30;// GameConfig.Instance.ReconnectPeriod;
        }

        /// </summary>
        public void ResetReconnectState()
        {
            if (m_CanReconnect == false)
            {
                m_CanReconnect = true;
            }

            if (m_ReconnectState != enumReconnectState.Default)
            {
                m_IsReconnected = true;
            }
            m_ReconnectState = enumReconnectState.Default;
            m_LastRecvMsgTime = Time.realtimeSinceStartup;
            m_CanCheckRecvMsgState = true;
            m_PingCount = 0;
        }
        public void ForbidReconnect()
        {
            m_CanReconnect = false;
        }
        #endregion

    }
}