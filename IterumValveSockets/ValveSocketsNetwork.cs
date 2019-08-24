using Magistr.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Valve.Sockets;

namespace Magistr.Network
{
    public class ValveSocketsNetwork : INetworkServer
    {
        private NetworkingSockets sockets;
        private Thread workerThread;
        private uint listenSocket;
        private bool IsRecieveRunning = false;
        private const int RecieveThreadSleep = 15;

        public event Action<NetworkMessage> Recieved;
        public event Func<ConnectionData, bool> Connecting;
        public event Action<ConnectionData> Connected;
        public event Action<ConnectionData> Disconnected;
        
        public ValveSocketsNetwork()
        {
            Library.Initialize();
        }

        public void Stop()
        {
            IsRecieveRunning = false;
        }

        public void StartServer(string host, int port)
        {
            try
            {
                sockets = new NetworkingSockets();
                Address address = new Address();
                address.SetLocalHost((ushort)port);
                listenSocket = sockets.CreateListenSocket(ref address);
                IsRecieveRunning = true;

                workerThread = new Thread(RecieveLoop);
                workerThread.Start();

            }
            catch (Exception e)
            {
                Debug.LogError("[NetworkServer] " + e.ToString());
                throw;
            }
            finally
            {
                Debug.Log($"[NetworkServer] started at port {port}", ConsoleColor.Green);
            }

        }

        private void Status(StatusInfo info, IntPtr context)
        {
            switch (info.connectionInfo.state)
            {
                case ConnectionState.None:
                    break;

                case ConnectionState.Connecting:
                    if (Connecting.Invoke(new ConnectionData() {
                        address = new IPEndPoint(IPAddress.Parse(info.connectionInfo.address.GetIP()), info.connectionInfo.address.port),
                        connection = info.connection
                    }))
                    {
                        sockets.AcceptConnection(info.connection);
                    }

                    break;

                case ConnectionState.Connected:
                    Debug.Log($"Client connected - ID: {info.connection}, IP: {info.connectionInfo.address.GetIP()}");
                    Connected?.Invoke(new ConnectionData() {
                        address = new IPEndPoint(IPAddress.Parse(info.connectionInfo.address.GetIP()), info.connectionInfo.address.port),
                        connection = info.connection
                    });
                    break;

                case ConnectionState.ClosedByPeer:
                    Disconnected.Invoke(new ConnectionData() {
                        address = new IPEndPoint(IPAddress.Parse(info.connectionInfo.address.GetIP()), info.connectionInfo.address.port),
                        connection = info.connection
                    });
                    sockets.CloseConnection(info.connection);
                    Debug.Log($"Client disconnected - ID: {info.connection}, IP: {info.connectionInfo.address.GetIP()}");
                    break;
            }
        }

        public void Disconnect(uint connection)
        {
            sockets?.CloseConnection(connection);
        }

        public void Send(uint connection, ISerializablePacket packet)
        {
              var packetBuffer = packet.Serialize();
              var buffer = Compress(packetBuffer);
              sockets.SendMessageToConnection(connection, buffer, SendType.Reliable);
        }

        private void MessageCallback(in NetworkingMessage net)
        {
            // copy data to own structure
            NetworkMessage msg = new NetworkMessage
            {
                data = new byte[net.length],
                channel = net.channel,
                connection = net.connection,
                length = net.length,
                messageNumber = net.messageNumber,
                timeReceived = net.timeReceived,
                userData = net.userData
            };
            net.CopyTo(msg.data);
            var buffer = Decompress(msg.data);
            msg.data = buffer;
            Recieved.Invoke(msg);
        }

        private void RecieveLoop()
        {
            const int maxMessages = 20;

            NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

            while (IsRecieveRunning)
            {
                sockets.DispatchCallback(Status);

                sockets.ReceiveMessagesOnListenSocket(listenSocket, MessageCallback, maxMessages);

                Thread.Sleep(RecieveThreadSleep);
            }
        }
        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (var dstream = new GZipStream(output, System.IO.Compression.CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (var dstream = new GZipStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}
