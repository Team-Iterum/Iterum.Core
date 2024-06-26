﻿using Mono.DllMap.Extensions;
using UnityEngine;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl;

public class PhysicsCharacter : IPhysicsCharacter
{
    public long Ref { get; }

    private readonly Scene scene;
    public float JumpHeight { get; set; } = 11f;
    public float Speed { get; set; } = 0.1f;
    
    #region IPhysicsCharacter
    
    public bool IsControllerCollisionUp => API.isControllerCollisionUp(Ref);
    public bool IsControllerCollisionDown => API.isControllerCollisionDown(Ref);
    public bool IsControllerCollisionSides => API.isControllerCollisionSides(Ref);
    public bool IsControllerMovingUp => API.isControllerMovingUp(Ref);
    
    public Vector3 Direction { get; private set; }
        
    public float CharacterRotation { get; set; }

    public Vector3 FootPosition
    {
        get => (Vector3)(DVector3)API.getControllerFootPosition(Ref);
        set => API.setControllerFootPosition(Ref, value);
    }

    public void Destroy()
    {
        if (IsDestroyed) return;
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"PhysicsCharacter ({Ref})", $"Destroy invoked...");
#endif
        scene.Destroy(this);
        IsDestroyed = true;
    }

    #endregion

    #region IPhysicsObject
        
    public Vector3 Position
    {
        get => (Vector3)(DVector3)API.getControllerPosition(Ref);
        set => API.setControllerPosition(Ref, value);
    }
        
    public Quaternion Rotation { get; set; }
    public bool IsDestroyed { get; private set; }
        
    public ulong ThingId { get; set; }

    #endregion

    internal PhysicsCharacter(IMaterial mat, Vector3 pos, Vector3 up, float height, float radius, float stepOffset, Scene scene)
    {
        this.scene = scene;

        Ref = API.createCapsuleCharacter(this.scene.Ref, (long) mat.GetInternal(), pos, up.normalized, height, radius, stepOffset);

        Move(Vector3.zero + scene.Gravity);
    }

    /// <summary>
    /// Dont use with CharactersUpdate in Scene class
    /// </summary>
    /// <param name="elapsed"></param>
    /// <param name="minDist"></param>
    public void UpdateSingle(float elapsed, float minDist)
    {
        API.characterUpdate(Ref, elapsed, minDist);
    }
        
    public void Move(Vector3 direction)
    {
        Direction = direction;
            
        API.setControllerDirection(Ref, Direction);
    }


    public void SetHeight(float height)
    {
        API.setControllerHeight(Ref, height);
    }

    public void Move(MoveDirection dirs)
    {

        var rotation = Quaternion.Euler(0, CharacterRotation, 0);

        var forward = (rotation * Vector3.forward);
        var up = Vector3.up;
        var right = (rotation * Vector3.right);

        var moveDelta = Vector3.zero;

        if (dirs.HasFlagFast(MoveDirection.Forward)) moveDelta += forward;
        else if (dirs.HasFlagFast(MoveDirection.Backward)) moveDelta += -forward;
            
        if (dirs.HasFlagFast(MoveDirection.Left)) moveDelta += -right;
        else if (dirs.HasFlagFast(MoveDirection.Right)) moveDelta += right;
            
        if (dirs.HasFlagFast(MoveDirection.Up)) moveDelta += up * scene.Gravity.magnitude * JumpHeight;
        else if (dirs.HasFlagFast(MoveDirection.Down)) moveDelta += -up;


        Direction = moveDelta * Speed + scene.Gravity;

        API.setControllerDirection(Ref, Direction);

    }

}