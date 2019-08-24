using Magistr.Log;
using Magistr.Protocol.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace IterumPacketCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("config.txt"))
            {
                try
                {
                    var keyMap = ParseConfig("config.txt");

                    if (keyMap.ContainsKey("OutputClient"))
                        PacketCreator.PathClient = keyMap["OutputClient"];
                    if (keyMap.ContainsKey("OutputServer"))
                        PacketCreator.PathServer = keyMap["OutputServer"];
                    if (keyMap.ContainsKey("PacketsSource"))
                        PacketCreator.PacketsSource = keyMap["PacketsSource"];

                    PacketCreator.Save();
                    Debug.Log("Generated", ConsoleColor.Green);
                    Thread.Sleep(1000);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.ToString());
                    Console.ReadKey();
                }
                return;

            }
            Debug.Log("No config.txt");
            Console.ReadKey();
            Environment.Exit(-10);
        }

        private static Dictionary<string, string> ParseConfig(string file)
        {
            var configLines = File.ReadAllLines(file);
            Dictionary<string, string> keyMap = new Dictionary<string, string>();
            foreach (var item in configLines)
            {
                var keyVal = item.Split("=");
                if (keyVal.Length == 2)
                {
                    if (!keyMap.ContainsKey(keyVal[0]))
                        keyMap.Add(keyVal[0], keyVal[1]);
                }
            }
            return keyMap;
        }
    }
}
