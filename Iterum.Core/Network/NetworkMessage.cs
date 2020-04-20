using System;
using System.Net;

namespace Iterum.Network
{
    public struct NetworkMessage
    {
        public byte[] data;
        public int length;
        public UInt32 connection;
        public long userData;
        public Int64 timeReceived;
        public long messageNumber;
        public int channel;
    }

    public struct ConnectionData
    {
        public uint connection;
        public IPEndPoint address;
    }
}