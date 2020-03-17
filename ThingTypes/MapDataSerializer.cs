using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace Magistr.New.ThingTypes
{
    public static class MapDataSerializer
    {
        private class FlowStyleFloatSequences : ChainedEventEmitter
        {
            public FlowStyleFloatSequences(IEventEmitter nextEmitter)
                : base(nextEmitter) {}

            public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
            {
                if (typeof(IEnumerable<float>).IsAssignableFrom(eventInfo.Source.Type))
                {
                    eventInfo = new SequenceStartEventInfo(eventInfo.Source)
                    {
                        Style = SequenceStyle.Flow
                    };
                }

                nextEmitter.Emit(eventInfo, emitter);
            }
        }
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