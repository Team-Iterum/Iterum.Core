using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Iterum.DataBlocks;

namespace Iterum.ThingTypes
{
    [Serializable]
    [DataContract]
    public struct ThingType : IThingType
    {
        [DataMember] public int ID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string Category { get; set; }
        [DataMember] public string Description { get; set; }

        [DataMember] public string[] Flags { get; set; }
        [DataMember] public Dictionary<string, string> Attrs { get; set; }

        [DataMember] public IDataBlock[] DataBlocks;
        
        public override string ToString()
        {
            return $"ThingType {Category}/{Name}";
        }

    }
    
}