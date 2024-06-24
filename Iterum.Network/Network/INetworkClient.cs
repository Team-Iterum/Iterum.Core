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
        
    void Send<T>(T packet) where T : struct, ISerializablePacketSegment;
    void Send(byte[] packet);
    void Send(ArraySegment<byte> packet);
}