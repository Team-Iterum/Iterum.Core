using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Iterum.ThingTypes
{
    public interface IThingTypeSerializer
    {
        string FileExtension { get; }

        public void SerializeAll(string fileName, ThingTypeStore store)
        {
            
        }
        
        public ThingTypeStore DeserializeAll(string directory)
        {
            return null;
        }
        
        ThingType Deserialize(string fileName);
        void Serialize(string fileName, ThingType tt);
    }
}