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
            Debug.Log("/// ---------------------------------", ConsoleColor.Blue, false);
            Debug.Log("/// Magistr.Polygon Server (c) 2019", ConsoleColor.Blue, false);
            Debug.Log("/// ---------------------------------", ConsoleColor.Blue, false);
            Debug.Log($"/// Game: {Assembly.GetExecutingAssembly().FullName.Split(',')[0]}", ConsoleColor.DarkYellow, false);

            ThingTypeManager.Load(File.OpenRead("Things\\Things.dat"));
            MapService.Start();
            var map = MapService.CreateMap();//.Load(File.OpenRead("Things\\Default.map"));
            NetworkService.Create();
            PlayerManager.Create();
            map.Start();
            Thread.Sleep(1000);
            NetworkService.Start(9955);

            Debug.Log("/// Ready", ConsoleColor.Blue, false);

            Console.ReadLine();
        }
    }
}
