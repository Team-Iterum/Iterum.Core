using System;

namespace Iterum.Network
{
    public interface ISerializablePacket
    {
        byte[] Serialize();
        void Deserialize(byte[] packet);
    }
    
    public interface ISerializablePacketSpan
    {
        Span<byte> Serialize();
        void Deserialize(ReadOnlySpan<byte> packet);    
    }
}
