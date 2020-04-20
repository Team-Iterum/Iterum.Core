namespace Magistr.Network
{
    public interface ISerializablePacket
    {
        byte ChannelId { get; }
        byte[] Serialize();
        void Deserialize(byte[] packet);
    }
}
