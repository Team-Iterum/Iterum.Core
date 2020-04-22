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

        public TelepathyNetwork()
        {
            server = new Server();

            Logger.Log = (s) => Debug.Log(nameof(TelepathyNetwork), s);
            Logger.LogWarning = (s) => Debug.Log(nameof(TelepathyNetwork), $"(Warning) {s}", ConsoleColor.Yellow);
            Logger.LogWarning = (s) => Debug.LogError(nameof(TelepathyNetwork), $"(Error) {s}");
        }
        public void Stop()
        {
            server.Stop();
        }

        public void StartServer(string host, int port)
        {
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

                            ConnectionData connection = new ConnectionData()
                            {
                                connection = (uint) msg.connectionId,
                            };
                            Disconnected?.Invoke(connection);

                            Debug.Log(nameof(TelepathyNetwork),
                                $"Client disconnected - ID: {msg.connectionId}", ConsoleColor.Magenta);

                            break;
                        }
                    }
                }
                
                Thread.Sleep(1);
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
