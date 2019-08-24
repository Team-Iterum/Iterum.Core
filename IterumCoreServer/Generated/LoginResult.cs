using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct LoginResult : ISerializablePacket
    {
        private const byte channelId = 3;
        public byte ChannelId => channelId;

        // fields
        public LoginResultCode code;
        public int value;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            code = (LoginResultCode)data.ReadByte();
            value = data.ReadInt();


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.AddByte((byte)code);
            data.AddInt(value);


            return StaticBuffers.BitBufferToPacket(data);
        }

        public enum LoginResultCode : byte
        {
        	Empty,
        	OK,
        	ErrorLoginPassword,
        	ErrorBlock,
        	ErrorLockdown,

        }


    }
}
