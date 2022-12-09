using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Iterum.ThingTypes
{
    [Serializable]
    [DataContract]
    public class MapData
    {
        [DataMember]
        public string Name;

        [DataMember]
        public Dictionary<string, string> Attrs;
        
        [DataMember]
        public MapRef[] Refs;
        
        public List<MapRef> GetRefsWithTag(string tag)
        {
            var returnRefs = new List<MapRef>();
            for (int i = 0; i < Refs.Length; i++)
            {
                if (Refs[i].tag == tag)
                {
                    returnRefs.Add(Refs[i]);
                }
            }

            return returnRefs;
        }
    }

    [Serializable]
    [DataContract]
    public class MapRef
    {
        [DataMember]
        public int ID;
        [DataMember]
        public int record;
        [DataMember]
        public string name;
        [DataMember]
        public string tag;
        [DataMember]
        public float[] position;
        [DataMember]
        public float[] rotation;
    }
    
}