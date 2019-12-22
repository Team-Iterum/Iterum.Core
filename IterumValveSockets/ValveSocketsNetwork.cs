using Magistr.Log;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using Valve.Sockets;

namespace Magistr.Network
{

    public class ValveSocketsNetwork : INetworkServer
    {
        private NetworkingSockets sockets;
        private Thread workerThread;
        private uint listenSocket;
        private bool isReceiveRunning;
        private const int ReceiveThreadSleep = 15;

        public event Action<NetworkMessage> Received;
        public event Func<ConnectionData, bool> Connecting;
        public event Action<ConnectionData> Connected;
        public event Action<ConnectionData> Disconnected;
        
        public ValveSocketsNetwork()
        {
            Library.Initialize();
        }

        public void Stop()
        {
            isReceiveRunning = false;
        }

        public void StartServer(string host, int port)
        {
            try
            {
                sockets = new NetworkingSockets();
                var address = new Address();
                address.SetLocalHost((ushort)port);
                listenSocket = sockets.CreateListenSocket(ref address);
                isReceiveRunning = true;

                workerThread = new Thread(ReceiveLoop);
                workerThread.Start();

            }
            catch (Exception e)
            {
                Debug.LogError("[NetworkServer] " + e);
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
            var msg = new NetworkMessage
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
            Received.Invoke(msg);
        }

        private void ReceiveLoop()
        {
            const int maxMessages = 20;

            while (isReceiveRunning)
            {
                sockets.DispatchCallback(Status);

                sockets.ReceiveMessagesOnListenSocket(listenSocket, MessageCallback, maxMessages);

                Thread.Sleep(ReceiveThreadSleep);
            }
        }

        private static byte[] Compress(byte[] data)
        {
            var output = new MemoryStream();
            using (var stream = new GZipStream(output, CompressionLevel.Optimal))
            {
                stream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        private static byte[] Decompress(byte[] data)
        {
            var input = new MemoryStream(data);
            var output = new MemoryStream();
            using (var stream = new GZipStream(input, CompressionMode.Decompress))
            {
                stream.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}
