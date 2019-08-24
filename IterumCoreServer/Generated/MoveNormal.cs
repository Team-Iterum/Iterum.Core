using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct MoveNormal : ISerializablePacket
    {
        private const byte channelId = 11;
        public byte ChannelId => channelId;

        // fields
        public uint viewId;
        public Vector3 Position;
        public uint Rot;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            viewId = data.Read(10);
            Position = new Vector3(HalfPrecision.Decompress(data.ReadUShort()), HalfPrecision.Decompress(data.ReadUShort()), HalfPrecision.Decompress(data.ReadUShort()));
            Rot = data.Read(9);


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.Add(10, viewId);
            var m_position_2 = Position;
            data.AddUShort(HalfPrecision.Compress(m_position_2.x));
            data.AddUShort(HalfPrecision.Compress(m_position_2.y));
            data.AddUShort(HalfPrecision.Compress(m_position_2.z));
            data.Add(9, Rot);


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
