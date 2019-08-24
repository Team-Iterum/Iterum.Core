using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct SetCamera : ISerializablePacket
    {
        private const byte channelId = 4;
        public byte ChannelId => channelId;

        // fields
        public uint viewId;
        public bool reset;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            viewId = data.Read(10);
            reset = data.ReadBool();


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.Add(10, viewId);
            data.AddBool(reset);


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
