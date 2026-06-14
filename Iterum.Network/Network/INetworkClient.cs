using System;

namespace Iterum.Network;

public interface INetworkClient
{
    event Action<NetworkMessage> Received;
    event Action<ConnectionData> Connected;
    event Action<ConnectionData> Disconnected;
        
    void Stop();
    void Connect(string host, int port);
    void Disconnect();
        
    // Returns false when the transport rejected the message (frame too large, send queue full,
    // not connected) so callers can avoid treating a dropped send as delivered.
    bool Send<T>(T packet) where T : struct, ISerializablePacketSegment;
    bool Send(byte[] packet);
    bool Send(ArraySegment<byte> packet);
}