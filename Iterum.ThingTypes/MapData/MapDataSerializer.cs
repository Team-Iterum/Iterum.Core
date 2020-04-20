using System;
using System.IO;
using YamlDotNet.Serialization;

namespace Iterum.ThingTypes
{
    public static class MapDataSerializer
    {
        public static MapData Deserialize(string mapName)
        {
            var builder = new DeserializerBuilder();

            var serializer =  builder.Build();
            
            var mapData = serializer.Deserialize<MapData>(File.ReadAllText(mapName));

            return mapData;
        }
        
        public static void Serialize(string fileName, MapData mapData, bool overwrite = true)
        {
            var builder = new SerializerBuilder();
            builder.DisableAliases();
            builder.WithEventEmitter(e => new FlowStyleFloatSequences(e));

            var serializer = builder.Build();
            
            if (File.Exists(fileName))
            {
                if(overwrite) File.Delete(fileName);
            }

            using (StreamWriter w = File.AppendText(fileName))
            {
                w.Write($"---\n" +
                        $"# MapData {mapData.Name}\n" +
                        $"# Created: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\n" +
                        $"\n");
                
                serializer.Serialize(w, mapData);
            }
        }
    }
}