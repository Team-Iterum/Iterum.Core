using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Iterum.Log;
using vtortola.WebSockets;
using vtortola.WebSockets.Deflate;
using vtortola.WebSockets.Rfc6455;

namespace Iterum.Network
{
    public sealed class WebSocketNetwork : INetworkServer, IDisposable
    {
        private class WSConnection
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
        private long connectionCounter;
        private Dictionary<long, WSConnection> connections = new Dictionary<long, WSConnection>();

        public event Action<NetworkMessage> Received;
        public event Func<ConnectionData, bool> Connecting;
        public event Action<ConnectionData> Connected;
        public event Action<ConnectionData> Disconnected;


        public void Stop()
        {
            foreach (var conn in connections)
            {
                conn.Value.RequestDisconnect();
            }

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
                    ip = new IPEndPoint(IPAddress.Parse(host), port);

                var options = new WebSocketListenerOptions {PingMode = PingMode.BandwidthSaving};
                options.Standards.RegisterRfc6455(f => f.MessageExtensions.Add(new WebSocketDeflateExtension()));

                sockets = new WebSocketListener(ip, options);
                sockets.StartAsync();

                Task.Run(() => AcceptWebSocketClientsAsync(sockets, stopToken.Token));

            }
            catch (Exception e)
            {
                Debug.LogError(nameof(WebSocketNetwork), e);
                throw;
            }
            finally
            {
                Debug.LogSuccess(nameof(WebSocketNetwork), $"Started at {port}");
            }

        }

        private async Task AcceptWebSocketClientsAsync(WebSocketListener server, CancellationToken stopAcceptToken)
        {
            while (!stopAcceptToken.IsCancellationRequested)
            {
                try
                {
                    WebSocket ws = await server.AcceptWebSocketAsync(stopAcceptToken).ConfigureAwait(false);

                    uint connection = (uint) (connectionCounter++);

                    if (Connecting != null && Connecting.Invoke(new ConnectionData
                        {address = (IPEndPoint) ws.RemoteEndpoint, connection = connection}))
                    {
                        var disconnectTokenSource = new CancellationTokenSource();

                        var wsc = new WSConnection
                        {
                            Ws = ws,
                            Token = disconnectTokenSource,
                            Connection = connection
                        };

                        connections.Add(connection, wsc);

                        _ = Task.Run(() => HandleConnectionAsync(ws, wsc, disconnectTokenSource.Token),
                            disconnectTokenSource.Token);
                    }
                    else
                    {
                        await ws.CloseAsync().ConfigureAwait(false);
                        ws.Dispose();
                    }

                }
                catch (Exception e)
                {
                    Debug.LogError(nameof(WebSocketNetwork),
                        "Error Accepting clients: " + e.GetBaseException().Message);
                    throw;
                }
            }

            foreach (var wsc in connections)
            {
                wsc.Value.RequestDisconnect();
            }

            connections.Clear();
            await sockets.StopAsync().ConfigureAwait(false);

            Debug.Log(nameof(WebSocketNetwork), "Stop accepting clients");
        }

        private async Task HandleConnectionAsync(WebSocket ws, WSConnection wsc, CancellationToken disconnectToken)
        {
            uint conn = wsc.Connection;
            try
            {
                Debug.Log(nameof(WebSocketNetwork),
                    $"Client connected - ID: {conn} IP: {((IPEndPoint) ws.RemoteEndpoint).Address}",
                    ConsoleColor.Magenta);

                Connected?.Invoke(
                    new ConnectionData {address = (IPEndPoint) ws.RemoteEndpoint, connection = conn});

                while (ws.IsConnected && (!disconnectToken.IsCancellationRequested))
                {

                    var message = await ws.ReadMessageAsync(disconnectToken).ConfigureAwait(false);

                    if (message == null || message.MessageType != WebSocketMessageType.Binary) continue;

                    await using var stream = new MemoryStream();

                    await message.CopyToAsync(stream, disconnectToken).ConfigureAwait(false);

                    if (stream.Length <= 0) continue;
                    var buffer = stream.GetBuffer();

                    var msg = new NetworkMessage
                    {
                        data = buffer,
                        channel = buffer[0],
                        connection = conn,
                        length = buffer.Length,
                        messageNumber = 0,
                        timeReceived = 0,
                        userData = 0
                    };

                    Received?.Invoke(msg);
                }
            }
            catch (WebSocketException e)
            {
                Debug.Log(nameof(WebSocketNetwork), "Error Handling connection: " + e.GetBaseException().Message);
            }
            finally
            {
                connections.Remove(wsc.Connection);

                await ws.CloseAsync().ConfigureAwait(false);
                ws.Dispose();

                Disconnected?.Invoke(new ConnectionData
                    {address = (IPEndPoint) ws.RemoteEndpoint, connection = conn});

                Debug.Log(nameof(WebSocketNetwork),
                    $"Client disconnected - ID: {conn} IP: {((IPEndPoint) ws.RemoteEndpoint).Address}", ConsoleColor.Magenta);
            }
        }


        public void Disconnect(uint connection)
        {
            if (connections.ContainsKey(connection))
                connections[(int) connection].RequestDisconnect();
        }

        public void Send(uint connection, ISerializablePacket packet)
        {
            if (!connections.ContainsKey(connection)) return;

            var ws = connections[connection];

            var buffer = packet.Serialize();

            try
            {
                ws.Ws.WriteBytesAsync(buffer, 0, buffer.Length, ws.Token.Token);
            }
            catch (Exception e)
            {
                Debug.LogError(nameof(WebSocketNetwork), e);
            }
        }


        public void Dispose()
        {
            stopToken?.Dispose();
            sockets?.Dispose();

            GC.SuppressFinalize(this);
        }

        ~WebSocketNetwork()
        {
            Dispose();
        }
    }
}
