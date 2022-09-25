using System;
using System.Net;

namespace Iterum.Network
{
    public struct NetworkMessage
    {
        public byte[] data;
        public ArraySegment<byte> dataSegment;
        public int conn;
    }

    public struct ConnectionData
    {
        public int conn;
        public IPEndPoint address;
    }
}