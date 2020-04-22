using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Iterum.Log;
using Telepathy;

namespace Iterum.Network
{
    public sealed class TelepathyNetwork : INetworkServer
    {
        public int ThreadSleepTime = 1;
        
        private Server server;
        private Thread workerThread;

        private Dictionary<uint, ConnectionData> connections = new Dictionary<uint, ConnectionData>();

        public TelepathyNetwork()
        {
            server = new Server();

            Logger.Log = (s) => Debug.Log(nameof(TelepathyNetwork), s);
            Logger.LogWarning = (s) => Debug.Log(nameof(TelepathyNetwork), $"(Warning) {s}", ConsoleColor.Yellow);
            Logger.LogWarning = (s) => Debug.LogError(nameof(TelepathyNetwork), $"(Error) {s}");
        }
        
        public void Stop()
        {
            if (!server.Active) return;
            server.Stop();
        }

        public void StartServer(string host, int port)
        {
            if (server.Active) return;
            
            connections.Clear();
            
            server.Start(port);
            
            workerThread = new Thread(Update)
            {
                Name = $"{nameof(TelepathyNetwork)} Thread"
            };
            workerThread.Start();
            
            Debug.LogSuccess(nameof(TelepathyNetwork), $"Started at {host}:{port}");
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

                            ConnectionData conData = new ConnectionData()
                            {
                                connection = (uint) msg.connectionId,
                                address = new IPEndPoint(IPAddress.Parse(address), 0)
                            };
                            connections.Add((uint) msg.connectionId, conData);
                            
                            Connecting?.Invoke(conData);
                            Connected?.Invoke(conData);

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

                            ConnectionData conData = new ConnectionData()
                            {
                                connection = (uint) msg.connectionId,
                            };
                            Disconnected?.Invoke(conData);
                            connections.Remove((uint) msg.connectionId);

                            Debug.Log(nameof(TelepathyNetwork),
                                $"Client disconnected - ID: {msg.connectionId}", ConsoleColor.Magenta);

                            break;
                        }
                    }
                }
                
                Thread.Sleep(ThreadSleepTime);
            }
        }

        

        public void Disconnect(uint con)
        {
            server.Disconnect((int) con);
            connections.Remove(con);
        }

        public void Send(uint con, ISerializablePacket packet)
        {
            if (!connections.ContainsKey( con)) return;
            
            server.Send((int) con, packet.Serialize());
        }

        public event Action<NetworkMessage> Received;
        
        public event Func<ConnectionData, bool> Connecting;
        public event Action<ConnectionData> Connected;
        
        public event Action<ConnectionData> Disconnected;
    }
}
