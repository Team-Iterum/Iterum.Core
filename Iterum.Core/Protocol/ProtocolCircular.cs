using Iterum.CircularBuffer;
using System;
using System.Threading;
using Iterum.Network;

namespace Iterum.Protocol
{
    public class ProtocolCircular : IComparable<ProtocolCircular>
    {
        private CircularBuffer<NetworkMessage> messages;
        private Thread workerThread;
        private bool isRunning;
        private int threadSleep;

        private bool isStopping;
        private int maxCapacity;
        public Action<NetworkMessage> Dispatch;

        public float Fill => messages.Size / (float)messages.Capacity;
        public ProtocolCircular(int capacity, int sleepTime)
        {
            maxCapacity = (int)(capacity * 0.9f);
            messages = new CircularBuffer<NetworkMessage>(capacity);
            threadSleep = sleepTime;

            
        }

        public bool CanPush()
        {
            if (isStopping) return false;
            if (messages.Size > maxCapacity) return false;
            return true;
        }

        public bool Push(NetworkMessage msg)
        {
            if (!CanPush()) return false;

            messages.PushFront(msg);
            return true;
        }

        public void Start()
        {
            workerThread = new Thread(DispatchLoop);
            workerThread.Name = $"ProtocolCircularThread";
            isRunning = true;
            workerThread.Start();
        }

        public void Stop()
        {
            isStopping = true;
        }

        private void DispatchLoop()
        {
            while(isRunning)
            {
                if (!messages.IsEmpty)
                {
                    var msg = messages.Back();
                    Dispatch.Invoke(msg);
                    msg.data = null;

                    //Buffers.StaticBuffers.Buffers.Return(msg.data);
                    messages.PopBack();
                    // stop after full remove buffer
                    if (isStopping && messages.Size <= 0)
                    {
                        isRunning = false;
                    }
                }

                Thread.Sleep(threadSleep);
            }
        }

        public int CompareTo(ProtocolCircular other)
        {
            if (Fill < other.Fill) return -1;
            if (Fill > other.Fill) return 1;
            return 0;
        }
    }
}
