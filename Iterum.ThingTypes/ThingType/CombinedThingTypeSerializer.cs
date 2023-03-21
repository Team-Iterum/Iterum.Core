using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Iterum.ThingTypes
{
    public class CombinedThingTypeSerializer
    {
        private IThingTypeSerializer[] _serializers;

        public CombinedThingTypeSerializer(IThingTypeSerializer[] serializers)
        {
            _serializers = serializers;
        }
        
        public ThingTypeStore DeserializeAll(string directory)
        {

            var dict = _serializers.ToDictionary(e => $".{e.FileExtension}", e => e);
            var things = new List<ThingType>();

            var files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).ToList();
            foreach (string fileName in files)
            {
                var fileInfo = new FileInfo(fileName);

                var serializer = dict[fileInfo.Extension];
                
                var tt = serializer.Deserialize(fileName);
                if(tt.Name == null) continue;
                
                things.Add(tt);
            }

            return new ThingTypeStore(things);
        }
    }
}