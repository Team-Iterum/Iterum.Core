using Iterum.ThingTypes;

namespace Iterum.Things
{
    public interface IThing
    {
        Transform Transform { get; }
        IThingType ThingType { get; }
    }
}
