using Magistr.Math;
using Magistr.Things;
using System;

namespace Magistr.Physics
{
    public interface IPhysicsObject
    {
        void Destroy();
        Vector3 Position { get; set; }
        IThing Thing { get; set; }
        Quaternion Rotation { get; set; }
        bool IsDestroyed { get; }
    }

    public interface IPhysicsStaticObject : IPhysicsObject
    {

    }

    public interface IPhysicsDynamicObject : IPhysicsObject
    {
        Vector3 LinearVelocity { get; set; }
        Vector3 AngularVelocity { get; set; }
        float MaxLinearVelocity { get; set; }
        float MaxAngularVelocity { get; set; }
        void SetKinematicTarget(Vector3 position, Quaternion rotation);
    }

    [Flags]
    public enum MoveDirection
    {
        None = 0,
        Forward = 1 << 0,
        Backward = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Up = 1 << 4,
        Down = 1 << 5
    }

    public interface IPhysicsCharacter : IPhysicsObject
    {
        Vector3 FootPosition { get; set; }

        event Action<Vector3, bool> PositionChange;
        float Speed { get; set; }
        Vector3 Direction { get; set; }
        float CharacterRotation { get; set; }
        void Move(MoveDirection directions, bool clearQueue);
        bool AutoMove(Vector3 toPosition);
    }
}
