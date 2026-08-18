using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Iterum.Network
{
    public static class SpamBlockIpList
    {
        public static List<string> Addresses = new List<string>();
        
        public static bool IsLogDisconnects { get; set; } = false;
        public static bool IsLogDisconnectsErrors { get; set; } = true;

        public static async void CheckIncoming(string address)
        {
            if (IsLoopback(address)) return;

            var result = await OnCheck(address);
            if(result)
            {
                Addresses.Add(address);
            }
        }
        public static async void Check(string address)
        {
            if (IsLoopback(address)) return;

            var result = await OnAdd(address);
            if(result)
            {
                Addresses.Add(address);
            }
        }

        // addresses arrive as "::ffff:127.0.0.1" on dual-stack listeners
        public static bool IsLoopback(string address)
        {
            if (!IPAddress.TryParse(address, out var ip)) return false;

            if (ip.IsIPv4MappedToIPv6) ip = ip.MapToIPv4();
            return IPAddress.IsLoopback(ip);
        }

        public static Func<string, Task<bool>> OnAdd = e => Task.FromResult(true);
        public static Func<string, Task<bool>> OnCheck = e => Task.FromResult(true);

        public static bool Exist(string address)
        {
            for (int i = 0; i < Addresses.Count; i++)
                if (string.CompareOrdinal(Addresses[i], address) == 0)
                    return true;
            
            return false;
        }
    }
}