using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Iterum.ThingTypes
{
    public interface IThingTypeSerializer
    {
        string FileExtension { get; }
        
        public ThingTypeStore DeserializeAll(string directory)
        {
            var things = new List<ThingType>();
            
            var files = Directory.EnumerateFiles(directory, $"*.{FileExtension}", SearchOption.AllDirectories).ToList();
            foreach (string fileName in files)
            {
                var tt = Deserialize(fileName);
                if(tt.Name == null) continue;
                
                things.Add(tt);
            }

            return new ThingTypeStore(things);
        }
        
        ThingType Deserialize(string fileName);
        void Serialize(string fileName, ThingType tt);
    }
}