using Iterum.Math;
using Iterum.Things;

// ReSharper disable UnusedMember.Global

namespace Iterum.Physics
{
    public interface IPhysicsObject
    {
        void Destroy();
        bool IsDestroyed { get; }
        
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        
        IThing Thing { get; set; }
    }
}
