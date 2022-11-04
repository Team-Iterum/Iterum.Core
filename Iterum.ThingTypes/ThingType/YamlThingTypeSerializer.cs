using System;
using System.IO;
using YamlDotNet.Serialization;

namespace Iterum.ThingTypes
{
    public class YamlThingTypeSerializer : IThingTypeSerializer
    {
        public string FileExtension => "yml";
        
        public ThingType Deserialize(string fileName)
        {
            var builder = new DeserializerBuilder();
            
            foreach (var dataBlock in DataBlockUtils.GetDataBlocksTypes()) 
                builder.WithTagMapping($"!{dataBlock.Name}", dataBlock);
            
            var serializer =  builder.Build();

            fileName = Path.ChangeExtension(fileName, FileExtension);
            
            return serializer.Deserialize<ThingType>(File.ReadAllText(fileName));
        }
        
        public void Serialize(string fileName, ThingType tt)
        {
            var builder = new SerializerBuilder();
            builder.DisableAliases();
            builder.WithEventEmitter(e => new FlowFloatSequences(e));
            builder.WithEventEmitter(e => new FlowIntSequences(e));

            foreach (var dataBlock in DataBlockUtils.GetDataBlocksTypes()) 
                builder.WithTagMapping($"!{dataBlock.Name}", dataBlock);

            var serializer = builder.Build();
            
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            fileName = Path.ChangeExtension(fileName, FileExtension);
            
            using StreamWriter w = File.AppendText(fileName);
            
            w.Write($"---\n" +
                    $"# ThingType #{tt.ID} {tt.Category}/{tt.Name}\n" +
                    $"# Created: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}\n" +
                    $"\n");
                
            serializer.Serialize(w, tt);
        }
    }
}