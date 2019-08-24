using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct SetObjectName : ISerializablePacket
    {
        private const byte channelId = 6;
        public byte ChannelId => channelId;

        // fields
        public uint viewId;
        public string name;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            viewId = data.Read(10);
            name = data.ReadString();


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.Add(10, viewId);
            data.AddString(name);


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
