using Magistr.Game;
using Magistr.Log;
using Magistr.Services;
using Magistr.Things;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Magistr
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ResetColor();
            Console.Clear();
            Debug.Log("/// ---------------------------------", ConsoleColor.Blue, false);
            Debug.Log("/// Magistr.Polygon Core Server (c) 2019", ConsoleColor.Blue, false);
            Debug.Log("/// ---------------------------------", ConsoleColor.Blue, false);
            Debug.Log("/// Neverland Project", ConsoleColor.Cyan, false);

            ThingTypeManager.Load(File.OpenRead("Neverland.dat"));
            MapService.Start();
            var map = MapService.CreateMap().Load(File.OpenRead("Neverland.map"));
            NetworkService.Create();
            PlayerManager.Create();
            Thread.Sleep(1000);
            map.Start();
            NetworkService.Start(9955);

            Debug.Log("/// Ready", ConsoleColor.Red, false);

            Console.ReadKey();
        }
    }
}
