using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace Iterum.ThingTypes
{
    public class FlowStyleFloatSequences : ChainedEventEmitter
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
}