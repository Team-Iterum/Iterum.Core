using System;
using System.Net;
using System.Threading;
using Iterum.Log;
using Telepathy;

namespace Iterum.Network
{
    public sealed class TelepathyNetwork : INetworkServer
    {
        private Server server;
        private Thread workerThread;

        public void Stop()
        {
            server.Stop();
        }

        public void StartServer(string host, int port)
        {
            server = new Server();
            server.Start(port);
            
            workerThread = new Thread(Update)
            {
                Name = $"{nameof(TelepathyNetwork)} thread"
            };
            workerThread.Start();
            
            Debug.LogSuccess(nameof(TelepathyNetwork), $"Started at {port}");
        }

        private void Update()
        {
            while (server.Active)
            {
                while (server.GetNextMessage(out Message msg))
                {
                    switch (msg.eventType)
                    {
                        case EventType.Connected:
                        {
                            string address = server.GetClientAddress(msg.connectionId);

                            ConnectionData connection = new ConnectionData()
                            {
                                connection = (uint) msg.connectionId,
                                address = new IPEndPoint(IPAddress.Parse(address), 0)
                            };
                            Connecting?.Invoke(connection);
                            Connected?.Invoke(connection);

                            Debug.Log(nameof(TelepathyNetwork),
                                $"Client connected - ID: {msg.connectionId} IP: {address}", ConsoleColor.Magenta);
                            break;
                        }
                        case EventType.Data:
                        {
                            Received?.Invoke(new NetworkMessage
                            {
                                connection = (uint) msg.connectionId,
                                data = msg.data
                            });

                            break;
                        }
                        case EventType.Disconnected:
                        {
                            string address = server.GetClientAddress(msg.connectionId);

                            ConnectionData connection = new ConnectionData()
                            {
                                connection = (uint) msg.connectionId,
                                address = new IPEndPoint(IPAddress.Parse(address), 0)
                            };
                            Disconnected?.Invoke(connection);

                            Debug.Log(nameof(TelepathyNetwork),
                                $"Client disconnected - ID: {msg.connectionId} IP: {address}", ConsoleColor.Magenta);

                            break;
                        }
                    }
                }
            }
        }

        public void Disconnect(uint con)
        {
            server.Disconnect((int) con);
        }

        public void Send(uint con, ISerializablePacket packet)
        {
            server.Send((int) con, packet.Serialize());
        }

        public event Action<NetworkMessage> Received;
        
        public event Func<ConnectionData, bool> Connecting;
        public event Action<ConnectionData> Connected;
        
        public event Action<ConnectionData> Disconnected;
    }
}
