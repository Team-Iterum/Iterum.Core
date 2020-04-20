using Magistr.Math;
using Magistr.Physics;

namespace Magistr.Things
{
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