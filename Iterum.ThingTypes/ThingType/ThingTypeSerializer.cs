using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Iterum.DataBlocks;
using YamlDotNet.Serialization;

namespace Iterum.ThingTypes
{

    public static class ThingTypeSerializer
    {
        public static ThingType Find(IEnumerable<ThingType> thingTypes, string ttName)
        {
            var tt = thingTypes.FirstOrDefault(e => e.Name == ttName);
            return tt;
        }
        
        public static ThingType Find(string path, string ttName)
        {
            var thingTypes = DeserializeAll(path).Values;
            var tt = thingTypes.FirstOrDefault(e => e.Name == ttName);
            return tt;
        }
        
        public static ThingType Find(string path, int id)
        {
            var thingTypes = DeserializeAll(path).Values;
            var tt = thingTypes.FirstOrDefault(e => e.ID == id);
            return tt;
        }

        public static ThingType Find(Dictionary<int, ThingType> all, int id)
        {
            var tt = all.Values.FirstOrDefault(e => e.ID == id);
            return tt;
        }

        private static IEnumerable<(string FlagKeyword, Type type)> GetDataBlocksTypes() 
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) 
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if(!type.GetInterfaces().Contains(typeof(IDataBlock))) continue;
                    
                    var attr = type.GetCustomAttribute(typeof(AutoRegisterDataBlock), true);
                    if (attr == null || !type.GetInterfaces().Contains(typeof(IDataBlock))) continue;
                    
                    if (attr is AutoRegisterDataBlock regAttr)
                    {
                        yield return (regAttr.FlagKeyword, type);
                    }
                }
            }
        }
        public static Dictionary<int, ThingType> DeserializeAll(string directory)
        {
            var things = new Dictionary<int, ThingType>();

            var builder = new DeserializerBuilder();

            foreach (var dataBlock in GetDataBlocksTypes()) 
                builder.WithTagMapping($"!{dataBlock.FlagKeyword}", dataBlock.type);
            
            var deserializer =  builder.Build();
            
            var files = Directory.EnumerateFiles(directory, "*.yml", SearchOption.AllDirectories);
            foreach (string fileName in files)
            {
                var tt = Deserialize(deserializer, fileName);
                if(tt.Name == null) continue;
                
                things.Add(tt.ID, tt);
            }

            return things;
        }

        public static ThingType Deserialize(IDeserializer deserializer, string fileName)
        {
            if (!File.Exists(fileName)) return default;
            return deserializer.Deserialize<ThingType>(File.ReadAllText(fileName));
        }
        
        public static void Serialize(string fileName, ThingType tt, bool overwrite = true, bool outputFlags = true)
        {
            var builder = new SerializerBuilder();
            builder.DisableAliases();
            builder.WithEventEmitter(e => new FlowFloatSequences(e));
            builder.WithEventEmitter(e => new FlowIntSequences(e));

            foreach (var dataBlock in GetDataBlocksTypes()) 
                builder.WithTagMapping($"!{dataBlock.FlagKeyword}", dataBlock.type);

            var serializer = builder.Build();
            
            if (File.Exists(fileName))
            {
                if(overwrite) File.Delete(fileName);
            }

            using StreamWriter w = File.AppendText(fileName);
            
            w.Write($"---\n" +
                    $"# ThingType #{tt.ID} {tt.Category}/{tt.Name}\n" +
                    $"# Created: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\n" +
                    $"\n");
                
            serializer.Serialize(w, tt);
        }
    }
}