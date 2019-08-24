using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct LookFull : ISerializablePacket
    {
        private const byte channelId = 12;
        public byte ChannelId => channelId;

        // fields
        public uint byteRotation;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            byteRotation = data.Read(9);


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.Add(9, byteRotation);


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
