using System;
using System.IO;
using YamlDotNet.Serialization;

namespace Iterum.ThingTypes
{
    public class YamlMapDataSerializer : IMapDataSerializer
    {
        public string FileExtension => "yml";

        public MapData Deserialize(string fileName)
        {
            var builder = new DeserializerBuilder();

            var serializer =  builder.Build();
            
            fileName = Path.ChangeExtension(fileName, FileExtension);
            
            var mapData = serializer.Deserialize<MapData>(File.ReadAllText(fileName));

            return mapData;
        }

        public void Serialize(string fileName, MapData mapData)
        {
            var builder = new SerializerBuilder();
            builder.DisableAliases();
            builder.WithEventEmitter(e => new FlowFloatSequences(e));
            builder.WithEventEmitter(e => new FlowIntSequences(e));

            var serializer = builder.Build();
            
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            fileName = Path.ChangeExtension(fileName, FileExtension);
            
            using StreamWriter w = File.AppendText(fileName);
            
            w.Write($"---\n" +
                    $"# MapData {mapData.Name}\n" +
                    $"# Created: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\n" +
                    $"\n");
                
            serializer.Serialize(w, mapData);
        }
    }
}