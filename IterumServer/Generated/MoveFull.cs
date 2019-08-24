using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct MoveFull : ISerializablePacket
    {
        private const byte channelId = 8;
        public byte ChannelId => channelId;

        // fields
        public uint viewId;
        public Vector3 Position;
        public Quaternion Rotation;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            viewId = data.Read(10);
            Position = new Vector3(HalfPrecision.Decompress(data.ReadUShort()), HalfPrecision.Decompress(data.ReadUShort()), HalfPrecision.Decompress(data.ReadUShort()));
            Rotation = SmallestThree.Decompress(new CompressedQuaternion(data.ReadByte(), data.ReadShort(), data.ReadShort(), data.ReadShort()));


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
            var m_rotation_3 = SmallestThree.Compress(Rotation);
            data.AddByte(m_rotation_3.m);
            data.AddShort(m_rotation_3.a);
            data.AddShort(m_rotation_3.b);
            data.AddShort(m_rotation_3.c);


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
