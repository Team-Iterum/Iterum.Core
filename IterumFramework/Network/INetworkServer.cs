using System;
using System.Collections.Generic;
using System.Text;

namespace Magistr.Network
{
    public interface INetworkServer
    {
        event Action<NetworkMessage> Received;
        event Func<ConnectionData, bool> Connecting;
        event Action<ConnectionData> Connected;
        event Action<ConnectionData> Disconnected;
        void Stop();
        void StartServer(string host, int port);
        void Disconnect(uint connection);
        void Send(uint connection, ISerializablePacket packet);
    }
}
