using NetStack.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magistr.Network
{
    public interface ISerializablePacket
    {
        byte ChannelId { get; }
        byte[] Serialize();
        void Deserialize(byte[] packet);
    }
}
