using Magistr.Math;
using Magistr.Physics;
using System;

namespace Magistr.Things
{
    public interface IThing
    {
        event Action<IThing, Vector3, bool> PositionChange;
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Scale { get; set; }
        int ThingTypeId { get; set; }
        string ThingName { get; set; }
        bool IsDestroyed { get; }
        void Create(IPhysicsWorld world);
        void Destroy();
    }

    public interface ICreature : IThing
    {
        Vector3 FootPosition { get; set; }
        float Speed { get; set; }
        Vector3 Direction { get; set; }
        float CharacterRotation { get; set; }
        void Move(MoveDirection directions);
        bool AutoMove(Vector3 toPosition);
    }
}
