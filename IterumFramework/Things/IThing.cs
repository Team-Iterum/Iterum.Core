using Magistr.Math;
using Magistr.New.ThingTypes;
using Magistr.Physics;

namespace Magistr.Things
{
    public delegate void ThingTransformChange(IThing thing, Transform transform, bool force);
    public interface IThing
    {
        event ThingTransformChange TransformChanged;
        Transform Transform { get; }
        ThingType ThingType { get; }
        void Create(IPhysicsWorld world);
        void Destroy();
    }

    public interface ICreature : IThing
    {
        Vector3 FootPosition { get; set; }
        Vector3 Direction { get; set; }
        float Speed { get; set; }
        float CharacterRotation { get; set; }
        void Move(MoveDirection directions);
        bool AutoMove(Vector3 toPosition);
    }
}
