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
        
    // Returns false when the transport rejected the message (too large / queue full / not connected).
    bool Send<T>(T packet) where T : struct, ISerializablePacketSegment;
    bool Send(byte[] packet);
    bool Send(ArraySegment<byte> packet);
}