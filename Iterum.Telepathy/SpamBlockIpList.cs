using System;
using System.Collections.Generic;
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
            var result = await OnCheck(address);
            if(result)
            {
                Addresses.Add(address);
            }
        }
        public static async void Check(string address)
        {
            var result = await OnAdd(address);
            if(result)
            {
                Addresses.Add(address);
            }
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