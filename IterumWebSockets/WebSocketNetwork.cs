using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Magistr.Log;
using vtortola.WebSockets;

namespace Magistr.Network
{
    public class WebSocketNetwork : INetworkServer
    {
        private class WebSocketConnection
        {
            public uint Connection;
            public WebSocket Ws;
            public CancellationTokenSource Token;

            public void RequestDisconnect()
            {
                Token.Cancel();
            }
        }

        private CancellationTokenSource stopToken;
        private WebSocketListener sockets;
        private List<WebSocketConnection> connections = new List<WebSocketConnection>();

        public event Action<NetworkMessage> Received;
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

                Task.Run(() => AcceptWebSocketClientsAsync(sockets, stopToken.Token));

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

        private async Task AcceptWebSocketClientsAsync(WebSocketListener server, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var ws = await server.AcceptWebSocketAsync(token).ConfigureAwait(false);
                    if (ws != null)
                    {                        
                        if (Connecting.Invoke(new ConnectionData { address = ws.RemoteEndpoint, connection = 0 }))
                        {
                            var cancel = new CancellationTokenSource();

                            var connection = (uint)connections.Count;
                            var wsc = new WebSocketConnection { Ws = ws, Token = cancel, Connection = connection };
                            connections.Add(wsc);

                            // ReSharper disable once MethodSupportsCancellation
                            // ReSharper disable once AssignmentIsFullyDiscarded
                            _ = Task.Run(() => HandleConnectionAsync(ws, wsc, token, cancel.Token));
                        }
                        else
                        {
                            try { ws.Close(); }
                            catch
                            {
                                // ignored
                            }

                            ws.Dispose();
                        }
                        
                    }
                }
                catch (Exception aex)
                {
                    Debug.LogError("Error Accepting clients: " + aex.GetBaseException().Message);
                }
            }
            foreach (var wsc in connections)
            {
                wsc.RequestDisconnect();
            }

            connections.Clear();
            sockets.Stop();

            Debug.Log("NetworkServer Stop accepting clients");
        }

        private async Task HandleConnectionAsync(WebSocket ws, WebSocketConnection wsc, CancellationToken cancellation, CancellationToken disconnect)
        {
            var connection = wsc.Connection;
            try
            {
                Debug.Log($"Client connected - ID: {connection}, IP: {ws.RemoteEndpoint.Address}");
                Connected?.Invoke(new ConnectionData { address = ws.RemoteEndpoint, connection = connection });

                while (ws.IsConnected && (!cancellation.IsCancellationRequested && !disconnect.IsCancellationRequested))
                {
                    var message = await ws.ReadMessageAsync(cancellation).ConfigureAwait(false);
                    if (message != null && message.MessageType == WebSocketMessageType.Binary)
                    {
                        Debug.Log("Received message");
                     
                        var ms = new MemoryStream();
                        message.CopyTo(ms);
                        
                        ms.Position = 0;
                        var net = ms.GetBuffer();
                        
                        Debug.Log($"readLength {net.Length} {net[0]}");
                        
                        var msg = new NetworkMessage
                        {
                            data = net,
                            channel = net[0],
                            connection = connection,
                            length = net.Length,
                            messageNumber = 0,
                            timeReceived = 0,
                            userData = 0
                        };

                        Debug.Log("Received message invoke... " + net.Length);
                        Received.Invoke(msg);
                    }
                }
            }
            catch (Exception aex)
            {
                Debug.LogError("Error Handling connection: " + aex.GetBaseException().Message);

                try { ws.Close(); }
                catch
                {
                    // ignored
                }
            }
            finally
            {
                connections.Remove(wsc);

                try { ws.Close(); }
                catch
                {
                    // ignored
                }

                ws.Dispose();
                
                Disconnected.Invoke(new ConnectionData { address = ws.RemoteEndpoint, connection = connection });

                Debug.Log($"Client disconnected - ID: {connection}, IP: {ws.RemoteEndpoint.Address}");
            }
        }


        public void Disconnect(uint connection)
        {
            if (connection < connections.Count)
                connections[(int)connection].RequestDisconnect();
        }

        public void Send(uint connection, ISerializablePacket packet)
        {
            if (connection < connections.Count)
            {
                var ws = connections[(int)connection];
                using var messageWriter = ws.Ws.CreateMessageWriter(WebSocketMessageType.Binary);

                var buffer = packet.Serialize();
                messageWriter.Write(buffer, 0, buffer.Length);
            }
        }

    }
}
