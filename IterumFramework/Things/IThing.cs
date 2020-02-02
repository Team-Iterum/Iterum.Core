using Magistr.Math;
using Magistr.Physics;
using Magistr.Game;

namespace Magistr.Things
{
    public delegate void ThingTransformChange(IThing thing, ITransform transform, bool force);
    public interface IThing
    {
        event ThingTransformChange TransformChanged;
        ITransform Transform { get; set; }
        int ThingTypeId { get; set; }
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
