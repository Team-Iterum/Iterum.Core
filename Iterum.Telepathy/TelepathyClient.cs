using System;
using System.Diagnostics;
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

        public int MaxMessageSize { get; set; } = 64 * 1024;

        public bool IsActive = false;

        private string hostPort;

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
            IsActive = true;
            hostPort = $"{host}:{port}";
            
            Log.Debug(LogGroup, $"Client connecting... {hostPort}", ConsoleColor.Magenta);
            
            Client = new Client(MaxMessageSize)
            {
                OnConnected = Client_OnConnected,
                OnData = Client_OnData,
                OnDisconnected = Client_OnDisconnected
            };
            Client.Connect(host, port);
            
            _workerThread = new Thread(Update);
            _workerThread.IsBackground = true;
            _workerThread.Start();
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
            Client.Disconnect();
        }

        public void Send<T>(T packet) where T : struct, ISerializablePacketSegment
        {
            Client.Send(packet.Serialize());
        }

        public void Send(byte[] packet)
        {
            Client.Send(packet);
        }

        public event Action<NetworkMessage> Received;
        public event Action<ConnectionData> Connected;
        public event Action<ConnectionData> Disconnected;
        
        private const string LogGroup = "TelepathyClient";
    }
}