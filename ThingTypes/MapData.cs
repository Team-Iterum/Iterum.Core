using System.Collections.Generic;

namespace Magistr.New.ThingTypes
{
    public class MapData
    {
        public string Name;
        
        public Dictionary<string, string> Attrs;
        
        public MapRef[] Refs;
        
    }

    public class MapRef
    {
        public int ID;

        public float[] position;
        public float[] rotation;
    }
    
}