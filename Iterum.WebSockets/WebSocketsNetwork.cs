using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Iterum.Logs;
using vtortola.WebSockets;
using vtortola.WebSockets.Rfc6455;

namespace Iterum.Network;

public sealed class WebSocketsNetwork : INetworkServer
{
    private static long _bytesSent;
    private static long _bytesReceived;
    public static long BytesSent => Interlocked.Read(ref _bytesSent);
    public static long BytesReceived => Interlocked.Read(ref _bytesReceived);

    private WebSocketListener server;

    public WebSocketListener Server => server;
    public CancellationTokenSource CancellationTokenSource { get; private set; }

    class WrapperWebSocket : IDisposable
    {
        public WebSocket socket;
        public int con;
        public CancellationTokenSource token;

        public WrapperWebSocket(WebSocket socket, int con)
        {
            this.socket = socket;
            this.con = con;
            this.token = new CancellationTokenSource();
        }

        public void Close()
        {
            token?.Cancel();
        }

        private SemaphoreSlim sendSlim = new(1, 1);
        private SemaphoreSlim readSlim = new(1, 1);

        public async Task WriteBytesAsync(byte[] bytes)
        {
            bool acquired = false;
            try
            {
                await sendSlim.WaitAsync();
                acquired = true;

                await using var writer = socket.CreateMessageWriter(WebSocketMessageType.Binary);
                await writer.WriteAndCloseAsync(bytes, 0, bytes.Length, CancellationToken.None).ConfigureAwait(false);
            }
            finally
            {
                if (acquired)
                    sendSlim.Release();
            }
        }

        public async Task<WebSocketMessageReadStream> ReadMessageAsync(CancellationToken socketCancellation)
        {
            bool acquired = false;
            try
            {
                await readSlim.WaitAsync(socketCancellation);
                acquired = true;

                return await socket.ReadMessageAsync(socketCancellation).ConfigureAwait(false);
            }
            finally
            {
                if (acquired)
                    readSlim.Release();
            }
        }

        public void Dispose()
        {
            token?.Dispose();
            sendSlim?.Dispose();
            readSlim?.Dispose();
        }
    }

    private Dictionary<int, WrapperWebSocket> sockets = new();

    private int connCounter;

    private const string LogGroup = "WebSocketsNetwork";

    public void Stop()
    {
        server?.StopAsync().Wait();
        CancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Start web socket server
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port">IGNORED</param>
    /// <param name="cert">certificate</param>
    public void StartServer(string host, int port, X509Certificate2 cert)
    {
        try
        {
            var options = new WebSocketListenerOptions
            {
                PingTimeout = TimeSpan.FromSeconds(15),
                NegotiationTimeout = TimeSpan.FromSeconds(15),
                PingMode = PingMode.LatencyControl
            };

            options.Transports.ConfigureTcp(tcp =>
            {
                tcp.NoDelay = true;
            });
            options.Standards.RegisterRfc6455();
            if (cert != null)
                options.ConnectionExtensions.RegisterSecureConnection(cert);

            server = new WebSocketListener(new IPEndPoint(IPAddress.Parse(host), port), options);

            server.StartAsync().Wait();

            CancellationTokenSource = new CancellationTokenSource();

            Task.Run(AcceptWebSocketsAsync);

            Log.Success(LogGroup, $"Started at {host}:{port.ToString()}");
        }
        catch (Exception ex)
        {
            Log.Error(LogGroup, "Server start error");
            Log.Exception(LogGroup, ex);
        }
    }

    /// <summary>
    /// Start web socket server
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port">IGNORED</param>
    public void StartServer(string host, int port)
    {
        StartServer(host, port, null);
    }


    private async Task AcceptWebSocketsAsync()
    {
        await Task.Yield();

        var globalCancellation = CancellationTokenSource.Token;

        while (!globalCancellation.IsCancellationRequested)
        {
            try
            {
                var webSocket = await server.AcceptWebSocketAsync(globalCancellation).ConfigureAwait(false);
                if (webSocket == null)
                {
                    if (globalCancellation.IsCancellationRequested || !server.IsStarted)
                        break; // stopped

                    continue; // retry immediately — AcceptWebSocketAsync is already blocking
                }

                Log.Debug(LogGroup, "WebSocket accepted");

#pragma warning disable 4014
                Task.Run(() => ClientIncomingMessagesAsync(webSocket), globalCancellation);
#pragma warning restore 4014
            }
            catch (OperationCanceledException)
            {
                /* server is stopped */
                break;
            }
            catch (WebSocketException acceptError)
            {
                // A single failed handshake must not kill the accept loop.
                Log.Debug(LogGroup, $"Accept rejected ({acceptError.GetType().Name}): {acceptError.Message}");
                await Task.Delay(100, globalCancellation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(LogGroup, "Unexpected error while accepting client.");
                Log.Exception(LogGroup, ex);
                await Task.Delay(1000, globalCancellation).ConfigureAwait(false);
            }
        }

        Log.Warn(LogGroup, "Server has stopped accepting new clients.");
    }

    private async Task ClientIncomingMessagesAsync(WebSocket webSocket)
    {
        var conn = Interlocked.Increment(ref connCounter);

        var endpoint = webSocket.RemoteEndpoint as IPEndPoint;
        // todo vtortola bug - TryGetValue returns the inverted result for `return header.IsEmpty`
        if (!webSocket.HttpRequest.Headers.TryGetValue("X-Real-IP", out var realAddress))
        {
            if (IPAddress.TryParse(realAddress, out var address))
            {
                endpoint = new IPEndPoint(address, 0);
            }
        }

        var conData = new ConnectionData
        {
            conn = conn,
            address = endpoint
        };

        var socket = new WrapperWebSocket(webSocket, conData.conn);
        var socketCancellation = socket.token.Token;
        sockets.TryAdd(conData.conn, socket);

        Connected?.Invoke(conData);

        Log.Info(LogGroup,
            $"Client connected - ID: {conData.conn.ToString()} IP: {endpoint} X-Real-IP: {realAddress}",
            ConsoleColor.Magenta);

        var exceptionKind = DisconnectExceptionKind.None;

        try
        {
            while (webSocket.IsConnected && !socketCancellation.IsCancellationRequested)
            {
                try
                {
                    await using var message = await socket.ReadMessageAsync(socketCancellation).ConfigureAwait(false);
                    if (message == null)
                        break; // webSocket is disconnected

                    await using var stream = new MemoryStream();
                    await message.CopyToAsync(stream, socketCancellation);
                    Interlocked.Add(ref _bytesReceived, stream.Length);

                    Received?.Invoke(new NetworkMessage {conn = conData.conn, dataSegment = stream.ToArray()});
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (WebSocketException ex) when (ex.InnerException is SocketException socketException)
                {
                    Log.Debug(LogGroup, $"Client {conData.conn} dropped (SocketErrorCode: {socketException.SocketErrorCode})");
                    exceptionKind = DisconnectExceptionKind.SocketDrop;
                    break;
                }
                catch (WebSocketException ex) when (ex.InnerException is IOException ioException)
                {
                    Log.Debug(LogGroup, $"Client {conData.conn} I/O drop ({ioException.GetType().Name}: {ioException.Message})");
                    exceptionKind = DisconnectExceptionKind.IoError;
                    break;
                }
                catch (Exception readWriteError)
                {
                    Log.Error(LogGroup, "An error occurred while reading/writing echo message.");
                    Log.Exception(readWriteError);
                    exceptionKind = DisconnectExceptionKind.Other;
                }
            }

            // close socket before dispose
            await webSocket.CloseAsync(WebSocketCloseReason.NormalClose);
        }
        finally
        {
            // Null close reason = no close frame received, treat as 1006 abnormal.
            int closeCode = webSocket.CloseReason is { } reason ? (int)reason : 1006;
            var info = new DisconnectInfo(closeCode, exceptionKind);

            webSocket.Dispose();
            socket.Dispose();

            DisconnectedDetailed?.Invoke(conData.conn, info);
            Disconnected?.Invoke(conData);
            sockets.Remove(conData.conn);

            Log.Info(LogGroup, $"Client disconnected - ID: {conData.conn.ToString()} CloseCode: {closeCode}", ConsoleColor.Magenta);
        }
    }

    public void Disconnect(int conn)
    {
        try
        {
            if (sockets.TryGetValue(conn, out var socket))
                socket.Close();
        }
        catch (Exception ex)
        {
            Log.Error(LogGroup, "Disconnect error");
            Log.Exception(LogGroup, ex);
        }
    }

    public void Send<T>(int conn, T packet) where T : struct, ISerializablePacketSegment
    {
        Send(conn, packet.Serialize());
    }

    public void Send(int conn, byte[] packet)
    {
        try
        {
            Interlocked.Add(ref _bytesSent, packet.Length);
            if (sockets.TryGetValue(conn, out var socket) && socket.socket.IsConnected)
                socket.WriteBytesAsync(packet).ContinueWith(t =>
                    Log.Error(LogGroup, $"Send error (async) ID: {conn} - {t.Exception?.GetBaseException().Message}"),
                    TaskContinuationOptions.OnlyOnFaulted);
        }
        catch (Exception ex)
        {
            Log.Error(LogGroup, "Send error");
            Log.Exception(LogGroup, ex);
        }
    }

    public void Send(int conn, ArraySegment<byte> packet)
    {
        try
        {
            Interlocked.Add(ref _bytesSent, packet.Count);
            if (sockets.TryGetValue(conn, out var socket) && socket.socket.IsConnected)
                socket.WriteBytesAsync(packet.ToArray()).ContinueWith(t =>
                    Log.Error(LogGroup, $"Send error (async) ID: {conn} - {t.Exception?.GetBaseException().Message}"),
                    TaskContinuationOptions.OnlyOnFaulted);
        }
        catch (Exception ex)
        {
            Log.Error(LogGroup, "Send error");
            Log.Exception(LogGroup, ex);
        }
    }

    public event Action<NetworkMessage> Received;
    public event Func<ConnectionData, bool> Connecting;
    public event Action<ConnectionData> Connected;
    public event Action<ConnectionData> Disconnected;
    public event Action<int, DisconnectInfo> DisconnectedDetailed;
}
