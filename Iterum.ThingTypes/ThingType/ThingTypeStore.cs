using System.Collections.Generic;
using System.Linq;

namespace Iterum.ThingTypes
{
    public class ThingTypeStore
    {
        public Dictionary<int, ThingType> ThingTypes { get; set; } 
        
        public ThingType Find(string ttName)
        {
            var tt = ThingTypes.Values.FirstOrDefault(e => e.Name == ttName);
            return tt;
        }
        
        public ThingType Find(int id)
        {
            return ThingTypes.TryGetValue(id, out var tt) ? tt : default;
        }

        public ThingTypeStore(IEnumerable<ThingType> thingTypes)
        {
            ThingTypes = thingTypes.ToDictionary(e => e.ID, e => e);
        }
        
    }
}