using NetStack.Compression;
using Magistr.Network;
using Magistr.Buffers;
using Magistr.Math;

namespace Packets
{
    public struct Login : ISerializablePacket
    {
        private const byte channelId = 2;
        public byte ChannelId => channelId;

        // fields
        public string login;
        public string password;


        public void Deserialize(byte[] packet)
        {
            var data = StaticBuffers.PacketToBitBuffer(packet);
			data.ReadByte();
            // deser
            login = data.ReadString();
            password = data.ReadString();


            StaticBuffers.Release(data);
        }

        public byte[] Serialize()
        {
            var data = StaticBuffers.BitBuffers.Acquire();
			data.AddByte((byte)ChannelId);

            // ser
            data.AddString(login);
            data.AddString(password);


            return StaticBuffers.BitBufferToPacket(data);
        }



    }
}
