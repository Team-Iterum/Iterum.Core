using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Iterum.ThingTypes
{
    public interface IMapDataSerializer 
    {
        string FileExtension { get; }
        
        MapData Deserialize(string fileName);
        
        void Serialize(string fileName, MapData mapData);   
    }
}