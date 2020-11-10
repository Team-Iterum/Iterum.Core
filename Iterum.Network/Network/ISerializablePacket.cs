namespace Iterum.Network
{
    public interface ISerializablePacket
    {
        byte[] Serialize();
        void Deserialize(byte[] packet);
    }
}
