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

            var serializerDictionary = _serializers.ToDictionary(e => $".{e.FileExtension}", e => e);
            var store = new ThingTypeStore();

            var files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).ToList();
            foreach (string fileName in files)
            {
                var info = new FileInfo(fileName);

                if(!serializerDictionary.ContainsKey(info.Extension)) continue;
                
                var serializer = serializerDictionary[info.Extension];
                
                var newStore = serializer.DeserializeAll(fileName);

                store.Add(newStore);
            }

            return store;
        }
    }
}