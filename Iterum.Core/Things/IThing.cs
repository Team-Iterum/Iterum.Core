using Magistr.Physics;
using Magistr.ThingsTypes;

namespace Magistr.Things
{
    public delegate void ThingTransformChange(IThing thing, Transform transform, bool force);
    public interface IThing
    {
        event ThingTransformChange TransformChanged;
        Transform Transform { get; }
        IThingType ThingType { get; }
        void Create(IPhysicsWorld world);
        void Destroy();
    }
}
