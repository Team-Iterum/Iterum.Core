using System;
using System.Collections.Generic;
using Iterum.Things;
using UnityEngine;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl;

internal class Scene
{
    public long Ref { get; private set; }
    public int Timestamp => (int) API.getSceneTimestamp(Ref);
    public Vector3 Gravity;

    private string LogGroup => $"[Scene ({Ref})]";
        
    private Dictionary<long, IPhysicsObject> refs = new Dictionary<long, IPhysicsObject>();
    private IMaterial legacySceneGlobalMaterial;
        
    public bool ccd;
    public bool determenism;

    public void Create(ContactReportCallbackFunc contactReport, TriggerReportCallbackFunc trigger)
    {
        Ref = API.createScene(Gravity, contactReport, trigger, ccd, determenism);
        legacySceneGlobalMaterial = new Material();

#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} Create. Gravity: {Gravity}");
#endif
    }

    public IPhysicsObject GetObject(in long nRef)
    {
        return refs.TryGetValue(nRef, out var obj) ? obj : null;
    }

    public void StepPhysics(in float dt)
    {
        API.stepPhysics(Ref, dt);
    }

    internal void Cleanup()
    {
        API.cleanupScene(Ref);
            
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} Cleanup");
#endif
    }

    #region Destroy objects

    public void Destroy(StaticObject e)
    {
        refs.Remove(e.Ref);
            
        API.destroyRigidStatic(e.Ref);
#if PHYSICS_DEBUG_LEVEL            
            Console.WriteLine($"{LogGroup} StaticObject Ref: ({e.Ref}) destroyed");
#endif
    }
    public void Destroy(TerrainObject e)
    {
        refs.Remove(e.Ref);

        API.destroyTerrain(e.Ref);
#if PHYSICS_DEBUG_LEVEL            
            Console.WriteLine($"{LogGroup} StaticObject Ref: ({e.Ref}) destroyed");
#endif
    }
    public void Destroy(DynamicObject e)
    {
        refs.Remove(e.Ref);
        API.destroyRigidDynamic(Ref, e.Ref);
#if PHYSICS_DEBUG_LEVEL            
            Console.WriteLine($"{LogGroup} DynamicObject Ref: ({e.Ref}) destroyed");
#endif
    }
    public void Destroy(PhysicsCharacter e)
    {
        refs.Remove(e.Ref);
            
        API.destroyController(e.Ref);
#if PHYSICS_DEBUG_LEVEL            
            Console.WriteLine($"{LogGroup} PhysicsCharacter Ref: ({e.Ref}) destroyed");
#endif

    } 

    #endregion

    #region Create objects

    public TerrainObject CreateTerrain(Memory<short> heightmap, long hfSize, Vector3 scale, Vector3 pos,
        float thickness = -1, float convexEdgeThreshold = 0, bool noBoundaries = false, IMaterial mat = null)
    {
        mat ??= legacySceneGlobalMaterial;
        var obj = new TerrainObject(heightmap, hfSize, thickness, convexEdgeThreshold, noBoundaries, scale,
            mat, pos, this);
        refs.Add(obj.Ref, obj);
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} StaticObject Ref: ({obj.Ref}) created");
#endif
        return obj;
    }

    public IStaticObject CreateStatic(IGeometry geometry, Vector3 pos, Quaternion quat, PhysicsObjectFlags flags, IMaterial mat = null)
    {
        mat ??= legacySceneGlobalMaterial;
        var obj = new StaticObject(geometry, mat, flags, pos, quat, this);
        refs.Add(obj.Ref, obj);
#if PHYSICS_DEBUG_LEVEL            
            Console.WriteLine($"{LogGroup} StaticObject Ref: ({obj.Ref}) created");
#endif
        return obj;
    }

    public IDynamicObject CreateDynamic(IGeometry[] geometries, Vector3 pos, Quaternion quat, PhysicsObjectFlags flags, float mass, uint word, IMaterial mat = null)
    {
        mat ??= legacySceneGlobalMaterial;
        var obj = new DynamicObject(geometries, mat, flags, mass, word, pos, quat,  this);
        refs.Add(obj.Ref, obj);
#if PHYSICS_DEBUG_LEVEL            
            Console.WriteLine($"{LogGroup} DynamicObject Ref: ({obj.Ref}) created");
#endif
        return obj;
    }

    public IPhysicsCharacter CreateCapsuleCharacter(Vector3 pos, Vector3 up, float height, float radius, float stepOffset = 0.05f, IMaterial mat = null)
    {
        mat ??= legacySceneGlobalMaterial;
        var obj = new PhysicsCharacter(mat, pos, up, height, radius, stepOffset, this);
        refs.Add(obj.Ref, obj);
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} CapsuleCharacter Ref: ({obj.Ref}) created");
#endif
        return obj;
    }
        
    #endregion
        
    public int Raycast<T>(Buffer<T> buffer, Vector3 position,  Vector3 direction, float maxDist) where T : class, IThing
    {
        int count = API.sceneRaycast(Ref, buffer.Ref,
            position, direction, maxDist, (i, nRef, dist, pos, normal) =>
            {
                var physicsObject = GetObject(nRef);
                if (physicsObject != null)
                {
                    buffer.Things[i] = physicsObject.Thing as T;
                    buffer.Dists[i] = dist;
                    buffer.Positions[i] = pos;
                    buffer.Normals[i] = normal;
                }
            });
        buffer.SetResultsCount(count);

        return count;
    }
        
    public int SphereCast<T>(Buffer<T> buffer, IGeometry geometry, Vector3 position) where T : class, IThing
    {
        int count = API.sceneOverlap(Ref, buffer.Ref, buffer.Refs, (long)geometry.GetInternal(), position);
        
        for (int i = 0; i < count; i++)
        {
            var physicsObject = GetObject(buffer.Refs[i]);
            if (physicsObject != null)
            {
                buffer.Things[i] = physicsObject.Thing as T;
            }
        }
            
        buffer.SetResultsCount(refs.Count);
        
        return buffer.Count;
    }

    public Vector4 ComputePenetration(IGeometry geo1, IGeometry geo2, APITrans t1, APITrans t2)
    {
        return API.computePenetration((long)geo1.GetInternal(), (int)geo1.GeoType, (long)geo2.GetInternal(),
            (int)geo2.GeoType, t1, t2);
    }

    public void CharactersUpdate(float elapsed, float minDist)
    {
        API.charactersUpdate(Ref, elapsed, minDist);
    }
}