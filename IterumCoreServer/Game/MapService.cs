using Magistr.Log;
using Magistr.Physics;
using Magistr.WorldMap;
using System;
using System.Collections.Generic;
using System.IO;

namespace Magistr.Services
{
    public static class MapService
    {
        static List<IPhysicsWorld> Worlds;
        static List<Map> Maps;
        private static PhysicsWorldWatcher watcher;

        public static void Start()
        {
            Maps = new List<Map>();
            Worlds = new List<IPhysicsWorld>();
            watcher = new PhysicsWorldWatcher(Worlds);
            Debug.Log("[MapService] created", ConsoleColor.Green);
            
        }

        internal static Map GetMap(int mapId)
        {
            return Maps[mapId];
        }

        public static Map CreateMap()
        {
            Map map = null;
            IPhysicsWorld world = default;
            try {
                const int TPS = (int)(1000 / 60f);
                world = PhysicsWorldFactory.CreateWorld(TPS);
                world.Create();
            }
            catch(Exception e)
            {
                Debug.LogError("[PhysicsWorld] " + e.ToString());
                throw;
            }
            finally
            {
                Debug.Log("[PhysicsWorld] created", ConsoleColor.Green);
                map = new Map(world);
                Maps.Add(map);
                Worlds.Add(world);
            }
            return map;
        }
    }
}
