using Iterum.Math;
using Iterum.Physics;

namespace Iterum.Things
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