using System;

namespace Iterum.Network
{
    public interface INetworkServer
    {
        event Action<NetworkMessage> Received;
        event Func<ConnectionData, bool> Connecting;
        event Action<ConnectionData> Connected;
        event Action<ConnectionData> Disconnected;
        
        void Stop();
        void StartServer(string host, int port);
        void Disconnect(int connection);
        
        void Send<T>(int con, T packet) where T : struct, ISerializablePacket;
        void Send(int con, byte[] packet);
    }
}
