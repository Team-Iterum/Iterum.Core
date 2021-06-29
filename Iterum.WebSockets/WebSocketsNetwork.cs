using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Iterum.Logs;
using vtortola.WebSockets;
using vtortola.WebSockets.Rfc6455;

namespace Iterum.Network
{
    public sealed class WebSocketsNetwork : INetworkServer
    {
        private WebSocketListener server;
        
        public WebSocketListener Server => server;
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        class WrapperWebSocket
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
            
            private SemaphoreSlim sendSlim = new SemaphoreSlim(1, 1);

            public void WriteBytesAsync(byte[] packet)
            {
                sendSlim.Wait();
                
                socket.WriteBytesAsync(packet, 0, packet.Length);
                
                sendSlim.Release();
            }
        }
        private Dictionary<int, WrapperWebSocket> sockets = new Dictionary<int, WrapperWebSocket>();
        
        private int connCounter;

        private const string LogGroup = "WebSocketsNetwork";

        public void Stop()
        {
            server.StopAsync().Wait();
            CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Start web socket server
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port">IGNORED</param>
        public void StartServer(string host, int port, X509Certificate2 cert)
        {
            try
            {
                var options = new WebSocketListenerOptions()
                {
                    NegotiationQueueCapacity = 128,
                    ParallelNegotiations = 16
                };
                options.Standards.RegisterRfc6455();
                options.ConnectionExtensions.RegisterSecureConnection(cert);
                
                server = new WebSocketListener(new IPEndPoint(IPAddress.Parse(host), port), options);

                server.StartAsync().Wait();
                
                CancellationTokenSource = new CancellationTokenSource();

                Task.Run(AcceptWebSocketsAsync);

                Log.Success(LogGroup, $"Started at {host}");
            }
            catch (Exception ex)
            {
                Log.Error(LogGroup, "Client connection error");
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
            try
            {
                var options = new WebSocketListenerOptions();
                options.Standards.RegisterRfc6455();
                server = new WebSocketListener(new IPEndPoint(IPAddress.Parse(host), port), options);
                server.StartAsync().Wait();
                
                CancellationTokenSource = new CancellationTokenSource();

                Task.Run(AcceptWebSocketsAsync);

                Log.Success(LogGroup, $"Started at {host}");
            }
            catch (Exception ex)
            {
                Log.Error(LogGroup, "Client connection error");
                Log.Exception(LogGroup, ex);
            }
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

                        continue; // retry
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
                catch (Exception acceptError)
                {
                    Log.Error("An error occurred while accepting client.", acceptError);
                }
            }

            Log.Warn(LogGroup, "Server has stopped accepting new clients.");
        }

        private async Task ClientIncomingMessagesAsync(WebSocket webSocket)
        {
            var conn = Interlocked.Increment(ref connCounter);

            var conData = new ConnectionData
            {
                conn = conn,
                address = null,
            };

            var socket = new WrapperWebSocket(webSocket, conData.conn);
            var socketCancellation = socket.token.Token;
            sockets.Add(conData.conn, socket);
        
            Connected?.Invoke(conData);
            
            Log.Info(LogGroup, $"Client connected - ID: {conData.conn} IP: {webSocket.RemoteEndpoint}", ConsoleColor.Magenta);
            
            
            try
            {
                while (webSocket.IsConnected && !socketCancellation.IsCancellationRequested)
                {
                    try
                    {
                        
                        WebSocketMessageReadStream message = await webSocket.ReadMessageAsync(socketCancellation).ConfigureAwait(false);
                        if (message == null)
                            break; // webSocket is disconnected

                        await using var stream = new MemoryStream();
                        await message.CopyToAsync(stream, socketCancellation);

                        Received?.Invoke(new NetworkMessage {conn = conData.conn, data = stream.GetBuffer()});
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                    catch (Exception readWriteError)
                    {
                        Log.Error(LogGroup, "An error occurred while reading/writing echo message.");
                        Log.Exception(readWriteError);
                    }
                }

                // close socket before dispose
                await webSocket.CloseAsync(WebSocketCloseReason.NormalClose);
            }
            finally
            {
                // always dispose socket after use
                webSocket.Dispose();
                
                Disconnected?.Invoke(conData);
                sockets.Remove(conData.conn);

                Log.Info(LogGroup, $"Client disconnected - ID: {conData.conn}", ConsoleColor.Magenta);

            }
        }
        
        public void Disconnect(int conn)
        {
            try
            {
                sockets[conn].Close();
            }
            catch (Exception ex)
            {
                Log.Error(LogGroup, "Disconnect error");
                Log.Exception(LogGroup, ex);
            }
        }

        public void Send<T>(int conn, T packet) where T : struct, ISerializablePacket
        {
            Send(conn, packet.Serialize());
        }
        
        public void Send(int conn, byte[] packet)
        {
            try
            {

                if (sockets.ContainsKey(conn) && sockets[conn].socket.IsConnected)
                    sockets[conn].WriteBytesAsync(packet);
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
    }
}
