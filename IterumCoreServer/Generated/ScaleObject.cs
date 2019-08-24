using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct ScaleObject : ISerializablePacket
    {
        private const byte channelId = 8;
        public byte ChannelId => channelId;

        // fields
        public uint viewId;
        public Vector3 scale;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            viewId = data.Read(10);
            scale = new Vector3(HalfPrecision.Decompress(data.ReadUShort()), HalfPrecision.Decompress(data.ReadUShort()), HalfPrecision.Decompress(data.ReadUShort()));


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.Add(10, viewId);
            var m_scale_2 = scale;
            data.AddUShort(HalfPrecision.Compress(m_scale_2.x));
            data.AddUShort(HalfPrecision.Compress(m_scale_2.y));
            data.AddUShort(HalfPrecision.Compress(m_scale_2.z));


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
