using Iterum.ThingTypes;

namespace Iterum.Things
{
    public interface IThing
    {
        IThingType ThingType { get; }
    }
}
