using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;

namespace Magistr.New.ThingTypes
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
        private static IEnumerable<Type> GetDataBlocksTypes() 
        {
            // this is making the assumption that all assemblies we need are already loaded.
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) 
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if(!type.GetInterfaces().Contains(typeof(IDataBlock))) continue;
                    
                    yield return type;
                }
            }
        }
        public static Dictionary<int, ThingType> DeserializeAll(string directory)
        {
            var things = new Dictionary<int, ThingType>();
            var builder = new DeserializerBuilder();

            foreach (var dataBlock in GetDataBlocksTypes())
            {
                builder.WithTagMapping($"!{dataBlock.Name}", dataBlock);
            }
            var serializer =  builder.Build();

            var files = Directory.EnumerateFiles(directory, "*.yml", SearchOption.AllDirectories);
            foreach (string fileName in files)
            {
                var tt = serializer.Deserialize<ThingType>(File.ReadAllText(fileName));
                things.Add(tt.ID, tt);
            }

            return things;
        }
        
        public static  void Serialize(string fileName, ThingType tt, bool overwrite = true, bool outputFlags = true)
        {
            var builder = new SerializerBuilder();
            builder.DisableAliases();
            builder.WithEventEmitter(e => new FlowStyleFloatSequences(e));
            
            foreach (var dataBlock in GetDataBlocksTypes())
            {
                builder.WithTagMapping($"!{dataBlock.Name}", dataBlock);
            }
            

            var serializer = builder.Build();
            
            if (File.Exists(fileName))
            {
                if(overwrite) File.Delete(fileName);
            }

            using (StreamWriter w = File.AppendText(fileName))
            {
                w.Write($"---\n" +
                        $"# ThingType #{tt.ID} {tt.Category}/{tt.Name}\n" +
                        $"# Created: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\n" +
                        $"\n");
                
                serializer.Serialize(w, tt);
            }
        }
    }
}