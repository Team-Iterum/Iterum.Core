using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Telepathy;
using Log = Iterum.Logs.Log;

namespace Iterum.Network
{
    public sealed class TelepathyNetwork : INetworkServer
    {
        public int ServerFrequency = 60;
        public bool IsReport = true;
        
        private Server server;
        private Thread workerThread;

        private const string LogGroup = "TelepathyNetwork";
        

        public TelepathyNetwork()
        {
            server = new Server();

            Logger.Log = s => Log.Info(LogGroup, s);
            Logger.LogWarning = s => Log.Warn(LogGroup, s);
            Logger.LogWarning = s => Log.Error(LogGroup, s);
        }
        
        public void Stop()
        {
            if (!server.Active) return;
            server.Stop();
        }

        public void StartServer(string host, int port)
        {
            if (server.Active) return;

            server.Start(port);
            
            workerThread = new Thread(Update);
            #if DEBUG
            workerThread.Name = $"{LogGroup} Thread";
            #endif
            workerThread.Start();
            
            Log.Success(LogGroup, $"Started at {host}:{port}");
        }

        private void Update()
        {
            long messagesReceived = 0;
            long dataReceived = 0;
            var sw = Stopwatch.StartNew();
            
            while (server.Active)
            {
                while (server.GetNextMessage(out var msg))
                {
                    switch (msg.eventType)
                    {
                        case EventType.Connected:
                        {
                            string address = server.GetClientAddress(msg.connectionId);

                            if (string.IsNullOrEmpty(address))
                            {
                                Log.Warn(LogGroup, $"Client empty address - ID: {msg.connectionId}");
                                break;
                            };
                            
                            var conData = new ConnectionData
                            {
                                conn = msg.connectionId,
                                address = new IPEndPoint(IPAddress.Parse(address), 0)
                            };
                            
                            Connected?.Invoke(conData);

                            Log.Info(LogGroup, $"Client connected - ID: {msg.connectionId} IP: {address}", ConsoleColor.Magenta);
                            break;
                        }
                        case EventType.Data:
                        {
                            Received?.Invoke(new NetworkMessage
                            {
                                conn = msg.connectionId,
                                data = msg.data
                            });

                            break;
                        }
                        case EventType.Disconnected:
                        {
                            var conData = new ConnectionData()
                            {
                                conn = msg.connectionId,
                            };
                            Disconnected?.Invoke(conData);

                            Log.Info(LogGroup,
                                $"Client disconnected - ID: {msg.connectionId}", ConsoleColor.Magenta);

                            break;
                        }
                    }
                }

                
                // sleep
                Thread.Sleep(1000 / ServerFrequency);

                if (IsReport)
                {
                    // report every 10 seconds
                    if (sw.ElapsedMilliseconds > 1000 * 2)
                    {
                        Log.Debug($"Thread {Thread.CurrentThread.ManagedThreadId}", string.Format("In={0} ({1} KB/s)  Out={0} ({1} KB/s) ReceiveQueue={2}", 
                            messagesReceived, dataReceived * 1000 / (sw.ElapsedMilliseconds * 1024), server.ReceiveQueueCount.ToString()), ConsoleColor.DarkGray);
                    
                        sw.Stop();
                        sw = Stopwatch.StartNew();
                        messagesReceived = 0;
                        dataReceived = 0;
                    }
                }
                
            }
        }
        

        public void Disconnect(int conn)
        {
            server.Disconnect(conn);
        }
        
        public void Send<T>(int conn, T packet) where T : struct, ISerializablePacket
        {
            server.Send(conn, packet.Serialize());
        }
        
        public void Send(int conn, byte[] packet)
        {
            server.Send(conn, packet);
        }

        public event Action<NetworkMessage> Received;
        public event Func<ConnectionData, bool> Connecting;
        public event Action<ConnectionData> Connected;
        public event Action<ConnectionData> Disconnected;
    }
}
