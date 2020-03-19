using System.Collections.Generic;

namespace Magistr.New.ThingTypes
{
    public class MapData
    {
        public string Name;
        
        public Dictionary<string, string> Attrs;
        
        public MapRef[] Refs;

        public float[] GetFloat3(string key, char separator = ' ')
        {
            var split = Attrs[key].Split(separator);
            return new[] {float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2])};
        }

        public string GetString(string key)
        {
            return Attrs[key];
        }
        
        public uint GetUInt(string key)
        {
            return uint.Parse(Attrs[key]);
        }
        
        public byte GetByte(string key)
        {
            return byte.Parse(Attrs[key]);
        }

    }

    public class MapRef
    {
        public int ID;

        public float[] position;
        public float[] rotation;
    }
    
}