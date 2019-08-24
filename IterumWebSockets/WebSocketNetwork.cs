using Magistr.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using vtortola.WebSockets;

namespace Magistr.Network
{
    public class WebSocketNetwork : INetworkServer
    {
        class WebSocketConnection
        {
            public uint connection;
            public WebSocket ws;
            public CancellationTokenSource token;
            public void RequestDisconnect()
            {
                token.Cancel();
            }
        }

        private CancellationTokenSource stopToken;
        private WebSocketListener sockets;
        private List<WebSocketConnection> Connections = new List<WebSocketConnection>();

        public event Action<NetworkMessage> Recieved;
        public event Func<ConnectionData, bool> Connecting;
        public event Action<ConnectionData> Connected;
        public event Action<ConnectionData> Disconnected;

        public void Stop()
        {
            stopToken?.Cancel();
        }

        public void StartServer(string host, int port)
        {
            if (sockets != null) return;
            try
            {
                stopToken = new CancellationTokenSource();
                var ip = new IPEndPoint(IPAddress.Any, port);
                if (host != null)
                {
                    ip = new IPEndPoint(IPAddress.Parse(host), port);
                }
                sockets = new WebSocketListener(ip);
                sockets.Standards.RegisterStandard(new WebSocketFactoryRfc6455());
                sockets.MessageExtensions.RegisterExtension(new WebSocketDeflateExtension());
                sockets.StartAsync();

                _ = Task.Run(() => AcceptWebSocketClientsAsync(sockets, stopToken.Token));

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

        private async Task AcceptWebSocketClientsAsync(WebSocketListener server, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var ws = await server.AcceptWebSocketAsync(token).ConfigureAwait(false);
                    if (ws != null)
                    {                        
                        if (Connecting.Invoke(new ConnectionData() { address = ws.RemoteEndpoint, connection = (uint)0 }))
                        {
                            CancellationTokenSource cancel = new CancellationTokenSource();

                            var connection = (uint)Connections.Count;
                            var wsc = new WebSocketConnection() { ws = ws, token = cancel, connection = connection };
                            Connections.Add(wsc);

                            _ = Task.Run(() => HandleConnectionAsync(ws, wsc, token, cancel.Token));
                        }
                        else
                        {
                            try { ws.Close(); } catch { }
                            ws.Dispose();
                        }
                        
                    }
                }
                catch (Exception aex)
                {
                    Debug.LogError("Error Accepting clients: " + aex.GetBaseException().Message);
                }
            }
            foreach (var wsc in Connections)
            {
                wsc.RequestDisconnect();
            }
            Connections.Clear();
            sockets.Stop();
            Debug.Log("NetworkServer Stop accepting clients");
        }

        private async Task HandleConnectionAsync(WebSocket ws, WebSocketConnection wsc, CancellationToken cancellation, CancellationToken disconnect)
        {
            var connection = wsc.connection;
            try
            {
                Debug.Log($"Client connected - ID: {connection}, IP: {ws.RemoteEndpoint.Address.ToString()}");
                Connected?.Invoke(new ConnectionData() { address = ws.RemoteEndpoint, connection = connection });

                while (ws.IsConnected && (!cancellation.IsCancellationRequested && !disconnect.IsCancellationRequested))
                {
                    var message = await ws.ReadMessageAsync(cancellation).ConfigureAwait(false);
                    if (message != null && message.MessageType == WebSocketMessageType.Binary)
                    {
                        Debug.Log("Recieved message");
                        MemoryStream ms = new MemoryStream();
                        message.CopyTo(ms);
                        //byte[] net = new byte[ms.Length];
                        ms.Position = 0;
                        var net = ms.GetBuffer();
                        Debug.Log($"readLength {net.Length} {net[0]}");
                        NetworkMessage msg = new NetworkMessage
                        {
                            data = net,
                            channel = net[0],
                            connection = connection,
                            length = net.Length,
                            messageNumber = 0,
                            timeReceived = 0,
                            userData = 0,
                        };
                        Debug.Log("Recieved message invoke... " + net.Length);
                        Recieved.Invoke(msg);
                    }
                }
            }
            catch (Exception aex)
            {
                Debug.LogError("Error Handling connection: " + aex.GetBaseException().Message);
                try { ws.Close(); }
                catch { }
            }
            finally
            {
                Connections.Remove(wsc);

                try { ws.Close(); }
                catch { }
                ws.Dispose();
                
                Disconnected.Invoke(new ConnectionData() { address = ws.RemoteEndpoint, connection = connection });
                Debug.Log($"Client disconnected - ID: {connection}, IP: {ws.RemoteEndpoint.Address.ToString()}");
            }
        }


        public void Disconnect(uint connection)
        {
            if (connection < Connections.Count)
                Connections[(int)connection].RequestDisconnect();
        }

        public void Send(uint connection, ISerializablePacket packet)
        {
            if (connection < Connections.Count)
            {
                var ws = Connections[(int)connection];
                using (var messageWriter = ws.ws.CreateMessageWriter(WebSocketMessageType.Binary))
                {
                    var buffer = packet.Serialize();
                    messageWriter.Write(buffer, 0, buffer.Length);
                };
            }
        }

    }
}
