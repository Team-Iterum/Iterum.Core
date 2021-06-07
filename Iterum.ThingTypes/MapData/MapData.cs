using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Iterum.ThingTypes
{
    public class MapData
    {
        public string Name;
        
        public Dictionary<string, string> Attrs;
        
        public MapRef[] Refs;

        public float[] GetFloat3(string key, char separator = ' ')
        {
            var split = Attrs[key].Split(separator);
            return new[]
            {
                float.Parse(split[0], CultureInfo.InvariantCulture),
                float.Parse(split[1], CultureInfo.InvariantCulture), 
                float.Parse(split[2], CultureInfo.InvariantCulture)
            };
        }

        public IEnumerable<MapRef> GetRefsWithTag(string tag)
        {
            return Refs.Where(e => e.tag == tag);
        }
        
        
        public string GetString(string key)
        {
            return Attrs[key];
        }
        
        public uint GetUInt(string key)
        {
            return uint.Parse(Attrs[key], CultureInfo.InvariantCulture);
        }
        
        public byte GetByte(string key)
        {
            return byte.Parse(Attrs[key], CultureInfo.InvariantCulture);
        }

    }

    public class MapRef
    {
        public int ID;
        public int record;
        public string name;
        public string tag;
        public float[] position;
        public float[] rotation;
    }
    
}