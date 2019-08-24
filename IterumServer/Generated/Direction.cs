using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct Direction : ISerializablePacket
    {
        private const byte channelId = 11;
        public byte ChannelId => channelId;

        // fields
        public uint directionFlags;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            directionFlags = data.Read(5);


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.Add(5, directionFlags);


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
