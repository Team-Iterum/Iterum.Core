using Magistr.Log;
using Magistr.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Magistr.Things
{
    public static class ThingTypeManager
    {
        public static int Count => ThingTypes.Count;
        private static Dictionary<int, ThingType> ThingTypes;
        public static Dictionary<int, ThingType>.ValueCollection All => ThingTypes.Values;

        public static bool HasThingType(int thingTypeId)
        {
            return ThingTypes.ContainsKey(thingTypeId);
        }

        public static ThingType GetThingType(int thingTypeId)
        {
            if (HasThingType(thingTypeId))
            {
                return ThingTypes[thingTypeId];
            }
            return default;
        }

        private static ThingTypeArchive CreateArchive(string gameName, int version)
        {
            var thingsArchive = new ThingTypeArchive
            {
                Version = version,
                Name = gameName,
                Created = DateTime.UtcNow
            };
            return thingsArchive;

        }
        public static void Save(FileStream fs, string gameName, int version)
        {
            var thingsArchive = CreateArchive(gameName, version);

            try
            {
                var formatter = new BinaryFormatter();
                thingsArchive.ThingTypes = ThingTypes.Values.ToArray();
                formatter.Serialize(fs, thingsArchive);
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to serializer ThingTypeArchive. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
        public static void Load(FileStream fs)
        {
            try
            {
                ThingTypes = new Dictionary<int, ThingType>();
                var formatter = new BinaryFormatter
                {
                    Binder = new BinaryTypeBinder()
                };

                var thingsArchive = (ThingTypeArchive)formatter.Deserialize(fs);
                for (int i = 0; i < thingsArchive.ThingTypes.Length; i++)
                {
                    thingsArchive.ThingTypes[i].ThingTypeId = i;
                    ThingTypes.Add(i, thingsArchive.ThingTypes[i]);
                }
            }
            catch (SerializationException e)
            {
                Debug.LogError("[ThingTypeManager] Failed to deserialize ThingTypeArchive. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
                Debug.Log($"[ThingTypeManager] {Path.GetFileName(fs.Name)} loaded", ConsoleColor.Green);
            }

        }

        [Serializable]
        private struct ThingTypeArchive
        {
            public DateTime Created;
            public int Version;
            public string Name;
            public ThingType[] ThingTypes;
        }
    }

}
