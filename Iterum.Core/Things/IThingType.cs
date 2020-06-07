using System.Collections.Generic;

namespace Iterum.ThingTypes
{
    public interface IThingType
    {
        int ID { get; set; }
        
        string Name { get; set; }
        string Category { get; set; }
        string Description { get; set; }

        string[] Flags { get; set; }
        Dictionary<string, string> Attrs { get; set; }

    }
}