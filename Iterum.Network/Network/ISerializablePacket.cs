using System;

namespace Iterum.Network
{
    public interface ISerializablePacketSegment
    {
        ArraySegment<byte> Serialize();
        void Deserialize(ArraySegment<byte> packet);

        ulong GetLogFilter()
        {
            return 0;
        }

        string GetLogFormat()
        {
            return null;
        }
    }
}
