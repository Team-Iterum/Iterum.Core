using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct CreateObject : ISerializablePacket
    {
        private const byte channelId = 5;
        public byte ChannelId => channelId;

        // fields
        public DynamicLevel dynamicLevel;
        public uint thingTypeId;
        public uint viewId;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            dynamicLevel = (DynamicLevel)data.ReadByte();
            thingTypeId = data.ReadUInt();
            viewId = data.Read(10);


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.AddByte((byte)dynamicLevel);
            data.AddUInt(thingTypeId);
            data.Add(10, viewId);


            return StaticBuffers.BitBufferToPacket(data);
        }

        public enum DynamicLevel : byte
        {
        	Empty,
        	Static,
        	Kinematic,
        	Dynamic,

        }


    }
}
