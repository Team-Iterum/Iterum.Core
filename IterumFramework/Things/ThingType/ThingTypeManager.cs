#if !(ENABLE_MONO || ENABLE_IL2CPP)
using Magistr.Log;
using Magistr.Utils;
#else
using Debug = UnityEngine.Debug;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Magistr.MapData;
using Binder = Magistr.Utils.Binder;

namespace Magistr.Things
{

    public static class ThingTypeManager
    {
        public static int Count => ThingTypes.Count;
        public static Dictionary<int, ThingType> ThingTypes;
        public static Dictionary<int, ThingType>.ValueCollection All => ThingTypes.Values;

        static ThingTypeManager()
        {
            ThingTypes = new Dictionary<int, ThingType>();
        }

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

        public static void Save(Stream fs, string gameName, int version)
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
                Debug.LogError(nameof(ThingTypeManager), " Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                Debug.LogSuccess(nameof(ThingTypeManager),$"'{thingsArchive.Name}' saved", ConsoleColor.Green);
                fs.Close();
            }
        }

        public static void Load(Stream fs)
        {
            ThingTypeArchive thingsArchive = default;
            try

            {
                ThingTypes = new Dictionary<int, ThingType>();
                var formatter = new BinaryFormatter
                {
                    Binder = new Binder()
                };

                thingsArchive = (ThingTypeArchive) formatter.Deserialize(fs);
                for (int i = 0; i < thingsArchive.ThingTypes.Length; i++)
                {
                    ThingTypes.Add(thingsArchive.ThingTypes[i].ThingTypeId, thingsArchive.ThingTypes[i]);
                }
            }
            catch (SerializationException e)
            {
                Debug.LogError(nameof(ThingTypeManager), " Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                Debug.LogSuccess(nameof(ThingTypeManager),$"'{thingsArchive.Name}' loaded", ConsoleColor.Green);
                fs.Close();
            }
        }

    }
    

    
    [Serializable]
    public struct ThingTypeArchive
    {
        public DateTime Created;
        public int Version;
        public string Name;
        public ThingType[] ThingTypes;
    }

}
