using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct MoveShort : ISerializablePacket
    {
        private const byte channelId = 9;
        public byte ChannelId => channelId;

        // fields
        public uint viewId;
        public uint PosX;
        public uint PosY;
        public uint PosZ;
        public uint PosXSign;
        public uint PosYSign;
        public uint PosZSign;
        public uint Rot;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            viewId = data.Read(10);
            PosX = data.Read(10);
            PosY = data.Read(10);
            PosZ = data.Read(10);
            PosXSign = data.Read(1);
            PosYSign = data.Read(1);
            PosZSign = data.Read(1);
            Rot = data.Read(9);


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.Add(10, viewId);
            data.Add(10, PosX);
            data.Add(10, PosY);
            data.Add(10, PosZ);
            data.Add(1, PosXSign);
            data.Add(1, PosYSign);
            data.Add(1, PosZSign);
            data.Add(9, Rot);


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
