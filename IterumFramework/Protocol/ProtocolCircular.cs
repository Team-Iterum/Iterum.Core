using CircularBuffer;
using Magistr.Network;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Magistr.Protocol
{
    public class ProtocolCircular : IComparable<ProtocolCircular>
    {
        private CircularBuffer<NetworkMessage> Messages;
        private Thread workerThread;
        private bool IsRunning = false;
        private int ThreadSleep = 15;

        private bool IsStopping = false;
        private int MaxCapacity;
        public Action<NetworkMessage> Dispatch;

        public float Fill => Messages.Size / (float)Messages.Capacity;
        public ProtocolCircular(int capacity, int sleepTime)
        {
            MaxCapacity = (int)(capacity * 0.9f);
            Messages = new CircularBuffer<NetworkMessage>(capacity);
            ThreadSleep = sleepTime;

            
        }

        public bool CanPush()
        {
            if (IsStopping) return false;
            if (Messages.Size > MaxCapacity) return false;
            return true;
        }

        public bool Push(NetworkMessage msg)
        {
            if (!CanPush()) return false;

            Messages.PushFront(msg);
            return true;
        }

        public void Start()
        {
            workerThread = new Thread(DispatchLoop);
            IsRunning = true;
            workerThread.Start();
        }

        public void Stop()
        {
            IsStopping = true;
        }

        private void DispatchLoop()
        {
            while(IsRunning)
            {
                if (!Messages.IsEmpty)
                {
                    var msg = Messages.Back();
                    Dispatch.Invoke(msg);
                    msg.data = null;
                    msg = default;
                    
                    //Buffers.StaticBuffers.Buffers.Return(msg.data);
                    Messages.PopBack();
                    // stop after full remove buffer
                    if (IsStopping && Messages.Size <= 0)
                    {
                        IsRunning = false;
                    }
                }

                Thread.Sleep(ThreadSleep);
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
