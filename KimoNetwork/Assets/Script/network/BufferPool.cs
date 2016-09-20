using System.Threading;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// Lock-free ring buffer
    /// 支持多线程的缓存池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class BufferPool
    {
        //private static readonly ILog m_Log = LogManager.GetLogger(typeof(BufferPool));
        internal const int BufferLength = 256;
        private const int MaxNum = 100;
        private volatile static int m_Consumer;
        private volatile static int m_Producer;
        private static byte[][] m_BufferArray = new byte[MaxNum][];

        private BufferPool()
        {
        }
        internal static int Count()
        {
            return (m_Producer + MaxNum - m_Consumer) % MaxNum;
        }
        internal static bool Empty()
        {
            return (m_Consumer == m_Producer);
        }
        internal static byte[] GetBuffer()
        {
            //m_Log.InfoFormat("BufferPool::GetBuffer: Producer={0}/Consumer={1}, Frees={2}, PoolSize={3}", m_Producer, m_Consumer, Count(), MaxNum);

            if (!Empty())
            {
                byte[] buffer = m_BufferArray[m_Consumer];
                m_Consumer = (m_Consumer + 1) % MaxNum;
                if (buffer != null)
                {
                    return buffer;
                }
            }
            else
            {
                //m_Log.WarnFormat("BufferPool::IsEmpty: Producer={0}/Consumer={1}, Frees={2}, PoolSize={3}", m_Producer, m_Consumer, Count(), MaxNum);
            }
            return new byte[BufferLength];
        }
        internal static bool ReleaseBufferToPool(ref byte[] buffer)
        {
            if (buffer == null)
                return false;

            if (buffer.Length == BufferLength)
            {
                //m_Log.InfoFormat("BufferPool::ReleaseBuffer: Producer={0}/Consumer={1}, Frees={2}, PoolSize={3}", m_Producer, m_Consumer, Count(), MaxNum);

                int nextIndex = (m_Producer + 1) % MaxNum;
                if (nextIndex != m_Consumer)
                {
                    m_BufferArray[m_Producer] = buffer;
                    m_Producer = nextIndex;
                    buffer = null;
                    return true;
                }
            }
            // if no space, just drop it on the floor
            buffer = null;
            return false;
        }
    }
}