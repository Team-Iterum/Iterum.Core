using Magistr.Math;
using Magistr.Physics;
using System;

namespace Magistr.Things
{
    public delegate void ThingPositionChange(IThing thing, Vector3 pos, bool force);
    public interface IThing
    {
        event ThingPositionChange PositionChange;

        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }

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
