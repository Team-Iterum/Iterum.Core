using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Telepathy;
using Log = Iterum.Logs.Log;

namespace Iterum.Network
{
    public class TelepathyClient : INetworkClient
    {
        public Client Client;
        private Thread _workerThread;

        public bool UseYield { get; set; } = false;
        public int ServerFrequency { get; set; } = 60;

        // Telepathy's wire limit: a frame larger than this is rejected by Send. Exposed as a const
        // so callers (e.g. delta chunking) can size against the same ceiling without duplicating it.
        public const int DefaultMaxMessageSize = 64 * 1024;

        public int MaxMessageSize { get; set; } = DefaultMaxMessageSize;

        public bool IsActive = false;

        private string hostPort;

        public string LogGroup { get; set; } = "TelepathyClient";

        public TelepathyClient()
        {
            Telepathy.Log.Info = s => Log.Info(LogGroup, s);
            Telepathy.Log.Warning = s => Log.Warn(LogGroup, s);
            Telepathy.Log.Error = s => Log.Error(LogGroup, s);
        }
        
        public void Stop()
        {
            Disconnect();
            IsActive = false;
        }

        public void Connect(string host, int port)
        {
            // re-entrant Connect would spawn a second worker thread ticking the same
            // client — events get dispatched once per worker and old sockets leak
            if (Client != null && (Client.Connecting || Client.Connected))
            {
                Log.Warn(LogGroup, $"Connect ignored — already connecting/connected {hostPort}");
                return;
            }

            IsActive = true;
            hostPort = $"{host}:{port}";

            Log.Debug(LogGroup, $"Client connecting... {hostPort}", ConsoleColor.Magenta);

            Client ??= new Client(MaxMessageSize)
            {
                OnConnected = Client_OnConnected,
                OnData = Client_OnData,
                OnDisconnected = Client_OnDisconnected
            };
            Client.Connect(host, port);

            if (_workerThread == null || !_workerThread.IsAlive)
            {
                _workerThread = new Thread(Update);
                _workerThread.IsBackground = true;
                _workerThread.Start();
            }
        }

        private void Update()
        {
            while (IsActive)
            {
                Client.Tick(100000);
            
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
            }
        
        }
        

        private void Client_OnDisconnected()
        {
            var conData = new ConnectionData();
            Disconnected?.Invoke(conData);

            Log.Info(LogGroup, $"Client disconnected {hostPort}", ConsoleColor.Magenta);
        }

        private void Client_OnData(ArraySegment<byte> data)
        {
            Received?.Invoke(new NetworkMessage
            {
                dataSegment = data
            });
        }

        private void Client_OnConnected()
        {
            var conData = new ConnectionData();

            Connected?.Invoke(conData);

            Log.Info(LogGroup, $"Client connected {hostPort}", ConsoleColor.Magenta);
        }

        public void Disconnect()
        {
            Client?.Disconnect();
        }

        public bool Send<T>(T packet) where T : struct, ISerializablePacketSegment
        {
            return Client?.Send(packet.Serialize()) ?? false;
        }

        public bool Send(byte[] packet)
        {
            return Client?.Send(packet) ?? false;
        }

        public bool Send(ArraySegment<byte> packet)
        {
            return Client?.Send(packet) ?? false;
        }

        public event Action<NetworkMessage> Received;
        public event Action<ConnectionData> Connected;
        public event Action<ConnectionData> Disconnected;
    }
}