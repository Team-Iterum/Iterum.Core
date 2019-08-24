using Magistr.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magistr.Protocol
{
    public class ProtocolCircularManager
    {
        public readonly List<ProtocolCircular> Circulars = new List<ProtocolCircular>();

        public event Action<NetworkMessage> Dispatch;
        public ProtocolCircularManager(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddCirclular();
            }
        }

        public void Start()
        {
            foreach (var c in Circulars)
            {
                c.Dispatch = Dispatch;
                c.Start();
            }
        }

        public void Stop()
        {
            foreach (var c in Circulars)
            {
                c.Stop();
            }
        }

        public void Push(NetworkMessage msg)
        {
            var circular = Circulars.Select(e => e).Min();
            circular.Push(msg);
        }

        private void AddCirclular()
        {
            ProtocolCircular protocolCircular = new ProtocolCircular(1000, 15);
            protocolCircular.Dispatch = Dispatch;
            Circulars.Add(protocolCircular);
        }


    }
}
