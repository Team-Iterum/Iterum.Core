using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Iterum.Physics;

public interface IPhysicsObject
{
    void Destroy();
    bool IsDestroyed { get; }
        
    Vector3 Position { get; set; }
    Quaternion Rotation { get; set; }
        
    ulong Thing { get; set; }
}