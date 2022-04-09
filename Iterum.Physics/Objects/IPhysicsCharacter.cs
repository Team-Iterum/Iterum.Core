using System;
using System.Numerics;

namespace Iterum.Physics;

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
        
    Vector3 Direction { get; }
        
    float Speed { get; set; }
    float CharacterRotation { get; set; }
    float JumpHeight { get; }
        
    void Move(MoveDirection directions);
    void Move(Vector3 direction);
}