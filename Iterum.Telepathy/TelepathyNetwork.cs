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
        public int ServerFrequency { get; set; } = 60;
        public bool UseYield { get; set; } = false;
        public bool IsReport { get; set; } = true;
        public int MaxMessageSize { get; set; } = 64 * 1024;
        
        
        private Server server;
        private Thread workerThread;
        
        private Stopwatch stopwatch;
        private long messagesReceived = 0;
        private long dataReceived = 0;

        private const string LogGroup = "TelepathyNetwork";

        public TelepathyNetwork()
        {
            Telepathy.Log.Info = s => Log.Info(LogGroup, s);
            Telepathy.Log.Warning = s => Log.Warn(LogGroup, s);
            Telepathy.Log.Error = s => Log.Error(LogGroup, s);
        }
        
        
        public void Stop()
        {
            if (!server.Active) return;
            server.Stop();
        }

        public void StartServer(string host, int port)
        {
            if (server != null && server.Active) return;

            server = new Server(MaxMessageSize);
            server.OnConnected = Server_Connected;
            server.OnData = Server_Data;
            server.OnDisconnected = Server_Disconnected;
            server.Start(port);
            
            stopwatch = Stopwatch.StartNew();
            
            workerThread = new Thread(Update);
            workerThread.Start();
            
            Log.Success(LogGroup, $"Started at {host}:{port.ToString()}");
        }

        private void Server_Disconnected(int connectionId)
        {
            var conData = new ConnectionData
            {
                conn = connectionId
            };
            Disconnected?.Invoke(conData);

            Log.Info(LogGroup, $"Client disconnected - ID: {connectionId.ToString()}", ConsoleColor.Magenta);

        }

        private void Server_Data(int connectionId, ArraySegment<byte> message)
        {
            Received?.Invoke(new NetworkMessage
            {
                conn = connectionId,
                dataSegment = message
            });
        }

        private void Server_Connected(int connectionId)
        {
            string address = string.Empty;
            try
            {
                address = server.GetClientAddress(connectionId);
            }
            catch (Exception ex)
            {
                Log.Debug(LogGroup, ex.ToString());
            }

            if (string.IsNullOrEmpty(address))
            {
                Log.Warn(LogGroup, $"Client empty address - ID: {connectionId.ToString()}");
                return;
            }

            var conData = new ConnectionData { conn = connectionId, address = new IPEndPoint(IPAddress.Parse(address), 0) };

            Connected?.Invoke(conData);

            Log.Info(LogGroup, $"Client connected - ID: {connectionId.ToString()} IP: {address}", ConsoleColor.Magenta);
        }

        private void Update()
        {
            while (server.Active)
            {
                server.Tick(100000);
            
                if (UseYield)
                {
                    if(!Thread.Yield())
                        Thread.Sleep(0);
                }
                else
                {
                    // sleep
                    Thread.Sleep(1000 / ServerFrequency);
                }

                if (IsReport)
                {
                    // report every 10 seconds
                    if (stopwatch.ElapsedMilliseconds > 1000 * 2)
                    {
                        Log.Info(LogGroup, string.Format(
                            "Thread[{3}]: Server in={0} ({1} KB/s)  out={0} ({1} KB/s) ReceiveQueue={2}", messagesReceived.ToString(),
                            (dataReceived * 1000 / (stopwatch.ElapsedMilliseconds * 1024)).ToString(),
                            server.ReceivePipeTotalCount.ToString(), Thread.CurrentThread.ManagedThreadId.ToString()));
                    
                        stopwatch.Stop();
                        stopwatch = Stopwatch.StartNew();
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
        
        public void Send<T>(int conn, T packet) where T : struct, ISerializablePacketSegment
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
