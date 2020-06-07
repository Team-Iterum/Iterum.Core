using System.Net;

namespace Iterum.Network
{
    public struct NetworkMessage
    {
        public byte[] data;
        public uint conn;
    }

    public struct ConnectionData
    {
        public uint conn;
        public IPEndPoint address;
    }
}