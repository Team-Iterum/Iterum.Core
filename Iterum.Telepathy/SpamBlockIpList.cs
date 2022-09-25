using System.Collections.Generic;
using Telepathy;

namespace Iterum.Network
{
    public static class SpamBlockIpList
    {
        public static List<string> Addresses = new List<string>();
        public static bool IsLogDisconnects { get; set; } = true;

        public static void Add(string address)
        {
            Log.Info($"Add spam ip: {address}");
            Addresses.Add(address);
        }

        public static bool Exist(string address)
        {
            for (int i = 0; i < Addresses.Count; i++)
                if (string.CompareOrdinal(Addresses[i], address) == 0)
                    return true;
            
            return false;
        }
    }
}