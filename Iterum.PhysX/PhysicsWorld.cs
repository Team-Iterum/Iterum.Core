using System;
using System.Runtime.CompilerServices;
using Iterum.Things;
using UnityEngine;

[assembly: InternalsVisibleTo("AdvancedDLSupport")]
namespace Iterum.Physics.PhysXImpl;

public sealed class PhysicsWorld
{
    public IPhysicsWorld.WorldState State { get; private set; } = IPhysicsWorld.WorldState.None;

    public int Timestamp { get; private set; }

    public event EventHandler<ContactReport> ContactReport;
        
    private Scene scene;
        
    public PhysicsWorld(Vector3 gravity, bool ccd, bool determenism)
    {
        scene = new Scene { Gravity = gravity, ccd = ccd, determenism = determenism };


#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"Constructor. Gravity: {scene.Gravity}");
#endif
    }

    public void Step(float dt, float subSteps = 1)
    {
        scene.StepPhysics(dt);
        Timestamp = scene.Timestamp;
    }
        
    public void CharactersUpdate(float elapsed, float minDist)
    {
        scene.CharactersUpdate(elapsed, minDist);
    }
        
    public void Create()
    {
        if (State != IPhysicsWorld.WorldState.None) return;
            
        scene.Create(OnContactReport, OnTriggerReport);
            
        State = IPhysicsWorld.WorldState.Created;
            
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} Created");
#endif
    }

    public void Destroy()
    {
        if (State != IPhysicsWorld.WorldState.Created) return;
            
        scene.Cleanup();
            
        State = IPhysicsWorld.WorldState.Destroyed;
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} Destroyed");
#endif
    }
        

    #region Overlaps / Raycasts
        
    /// <summary>
    /// ComputePenetration
    /// </summary>
    /// <param name="geo1"></param>
    /// <param name="geo2"></param>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns>
    /// x,y,z = direction
    /// w = depth
    /// </returns>
    public Vector4 ComputePenetration(IGeometry geo1, IGeometry geo2, APITrans t1, APITrans t2)
    {
        return scene.ComputePenetration(geo1, geo2, t1, t2);
    }

    public int Raycast<T>(Buffer<T> refBuffer, Vector3 position, Vector3 direction, float maxDist) where T : class, IThing
    {
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} Raycast. Position: {position} Direction: {direction}");
#endif
            
        int count = scene.Raycast<T>(refBuffer, position, direction, maxDist);
        return count;
    }

    public int SphereCast<T>(Buffer<T> buffer, Vector3 position, IGeometry geometry) where T : class, IThing
    {
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} SphereCast. Position: {position} Geometry: {geometry.GetInternalGeometry()}");
#endif
            
        int count = scene.SphereCast<T>(buffer, geometry, position);
        return count;
    }


    private string LogGroup => $"[PhysicsWorld ({scene.Ref})]";

    #endregion

    #region Create objects

    public IStaticObject CreateTerrain(Memory<float> buffer, float scale, float size, Vector3 pos, IMaterial mat = null)
    {
        return scene.CreateTerrain(buffer, scale, size, pos, mat);
    }
    public IStaticObject CreateStatic(IGeometry geometry, Vector3 pos, Quaternion quat, PhysicsObjectFlags flags, IMaterial mat = null)
    {
        return scene.CreateStatic(geometry, pos, quat, flags, mat);
    }
    public IDynamicObject CreateDynamic(IGeometry[] geometries, Vector3 pos, Quaternion quat, PhysicsObjectFlags flags, float mass, uint word, IMaterial mat = null)
    {
        return scene.CreateDynamic(geometries, pos, quat, flags, mass, word, mat);
    }
    public IPhysicsCharacter CreateCapsuleCharacter(Vector3 pos, Vector3 up, float height, float radius, float stepOffset = 0.05f, IMaterial mat = null)
    {
        return scene.CreateCapsuleCharacter(pos,  up, height, radius, stepOffset, mat);
    }
        
        

    #endregion

    #region Contact reports

    private void OnTriggerReport(long ref0, long ref1)
    {
        var obj0 = scene.GetObject(ref0);
        var obj1 = scene.GetObject(ref1);

        if (obj0 == null || obj1 == null) return;
            
        ContactReport?.Invoke(this, new ContactReport
        {
            obj0 = obj0.Thing,
            obj1 = obj1.Thing,
                
            isTrigger = true,
        });
    }
    private void OnContactReport(int index, int count, long ref0, long ref1, APIVec3 normal, APIVec3 position, APIVec3 impulse, float separation, int faceIndex0, int faceIndex1)
    {
        var obj0 = scene.GetObject(ref0);
        var obj1 = scene.GetObject(ref1);
            
        if (obj0 == null || obj1 == null) return;
            
        ContactReport?.Invoke(this, new ContactReport
        {
            index = index,
            count = count,
                
            obj0 = obj0.Thing,
            obj1 = obj1.Thing,
                
            position = position,
                
            normal = normal,
            impulse = impulse,
                
            separation = separation,
                 
            faceIndex0 = faceIndex0,
            faceIndex1 = faceIndex1
        });
    }

    #endregion

        
    public override string ToString() => LogGroup;
}