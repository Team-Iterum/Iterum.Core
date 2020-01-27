using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Magistr.Log;
using MapData;

namespace Magistr.MapData
{
    public static class MapDataManager
    {
        private static Dictionary<string, MapArchive> MapArchives = new Dictionary<string, MapArchive>();

        public static MapArchive GetMapData(string name)
        {
            return MapArchives[name];
        }
        private static MapArchive CreateArchive(string mapName)
        {
            var mapArchive = new MapArchive
            {
                Name = mapName,
                Created = DateTime.UtcNow
            };

            return mapArchive;
        }

        public static void Save(FileStream fs, string mapName)
        {
            var mapArchive = CreateArchive(mapName);

            try
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, mapArchive);
            }
            catch (SerializationException e)
            {
                Debug.LogError(nameof(MapDataManager), "Failed to serialize: " + e.Message);
                throw;
            }
            finally
            {
                Debug.LogSuccess(nameof(MapDataManager),$"'{mapArchive.Name}' loaded");
                fs.Close();
            }
        }

        public static void Load(FileStream fs)
        {
            MapArchive mapArchive = default;
            try
            {
                var formatter = new BinaryFormatter
                {
                    Binder = new Utils.Binder()
                };
                mapArchive = (MapArchive) formatter.Deserialize(fs);
                MapArchives.Add(mapArchive.Name, mapArchive);
            }
            catch (SerializationException e)
            {
                Debug.LogError(nameof(MapDataManager), "Failed to deserialize: " + e.Message);
                throw;
            }
            finally
            {
                Debug.LogSuccess(nameof(MapDataManager),$"'{mapArchive.Name}' loaded");
                fs.Close();
            }

        }

        private sealed class BinaryTypeBinderLocal : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (a.GetType(typeName) != null)
                    {
                        typeToDeserialize = a.GetType(typeName);
                    }
                }

                return typeToDeserialize;
            }
        }

        
    }
}