using UnityEngine;
using Iterum.Things;

// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global

namespace Iterum.Physics;

public interface IPhysicsWorld
{
    public enum WorldState
    {
        None,
        Created,
        Destroyed,
    }

}

public struct ContactReport
{
    // ReSharper disable InconsistentNaming
        
    public int index;
    public int count;
        
    public IThing obj0;
    public IThing obj1;
        
    public Vector3 normal;
    public Vector3 position;
    public Vector3 impulse;
        
    public float separation;
        
    public bool isTrigger;
    public int faceIndex0;
    public int faceIndex1;

    // ReSharper restore InconsistentNaming
}