using Magistr.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Binder = Magistr.Utils.Binder;

namespace Magistr.Things
{

    public static class ThingTypeManager
    {
        private static Dictionary<int, ThingType> ThingTypes;
        public static Dictionary<int, ThingType>.ValueCollection All => ThingTypes.Values;

        public static ThingType Find(string title)
        {
            return All.FirstOrDefault(e => e.Title == title);
        }

        static ThingTypeManager()
        {
            ThingTypes = new Dictionary<int, ThingType>();
        }

        private static bool HasThingType(int thingTypeId)
        {
            return ThingTypes.ContainsKey(thingTypeId);
        }

        public static ThingType Get(int thingTypeId)
        {
            return HasThingType(thingTypeId) ? ThingTypes[thingTypeId] : default;
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
                Debug.LogSuccess(nameof(ThingTypeManager),$"'{thingsArchive.Name}' saved");
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
                    int thingTypeId = thingsArchive.ThingTypes[i].ThingTypeId;
                    ThingTypes.Add(thingTypeId, thingsArchive.ThingTypes[i]);
                    
                }
            }
            catch (SerializationException e)
            {
                Debug.LogError(nameof(ThingTypeManager), " Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                Debug.LogSuccess(nameof(ThingTypeManager),$"'{thingsArchive.Name}' loaded");
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
