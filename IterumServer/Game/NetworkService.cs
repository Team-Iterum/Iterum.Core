using Magistr.Log;
using Magistr.Network;
using Magistr.Protocol;
using Packets;
using System;

namespace Magistr.Services
{
    public static class NetworkAlias
    {
        public static INetworkServer Net =>  NetworkService.Net;
    }
    public static class NetworkService
    {
        private static ProtocolCircularManager protocol;
        public static INetworkServer Net;

        private static string clientKey = "myKey";
        private static int protocolVersion = 100;

        public static void Start(int port, bool websocketSupport = false)
        {
            protocol.Start();
            Net.StartServer(null, port);
        }
        public static void Create()
        {
            StartDispatcher();
            CreateNetwork();
        }
        private static void StartDispatcher()
        {
            try
            {
                protocol = new ProtocolCircularManager(4);
                protocol.Dispatch += (e) =>
                {
                    ChannelPackets.Dispatch(e.connection, e.data);
                };
            }
            catch(Exception e)
            {
                Debug.LogError("[ProtocolDispatcher] " + e.ToString());
                throw;
            }
            finally
            {
                Debug.Log("[ProtocolDispatcher] started", ConsoleColor.Green);
            }
            
        }

        private static void CreateNetwork()
        {
            try
            {
                Net = new ValveSocketsNetwork();
                Net.Connecting += (e) =>
                {
                    return true;
                };
                Net.Recieved += (e) =>
                {
                    protocol.Push(e);
                };

                ChannelPackets.ParseFirstPacket += (c, e) =>
                {
                    if (e.protocolVersion != protocolVersion)
                    {
                        Net.Disconnect(c);
                        return;
                    }
                    Net.Send(c, new FirstPacket() { key = clientKey, protocolVersion = protocolVersion });
                };

            }catch(Exception e)
            {
                Debug.LogError("[NetworkServer] " + e.ToString());
                throw;
            }
            finally
            {
                Debug.Log("[NetworkServer] created", ConsoleColor.Green);
            }
        }



    }
}
