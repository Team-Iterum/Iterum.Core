using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct FirstPacket : ISerializablePacket
    {
        private const byte channelId = 1;
        public byte ChannelId => channelId;

        // fields
        public int protocolVersion;
        public string key;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            protocolVersion = data.ReadInt();
            key = data.ReadString();


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.AddInt(protocolVersion);
            data.AddString(key);


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
