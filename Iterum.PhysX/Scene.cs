using System.Collections.Generic;
using Iterum.Math;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl
{
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
            legacySceneGlobalMaterial = new Material(0.5f, 0.5f, 0.5f);

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
        public void Destroy(DynamicObject e)
        {
            refs.Remove(e.Ref);
            API.destroyRigidDynamic(e.Ref);
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
        
        public int Raycast(Buffer buffer, Vector3 position,  Vector3 direction, float maxDist)
        {
            int count = API.sceneRaycast(Ref, buffer.Ref,
                position, direction, maxDist, (i, nRef) =>
                {
                    buffer.Things[i] = GetObject(nRef).Thing;
                });
            return count;
        }

        public Vector4 ComputePenetration(IGeometry geo1, IGeometry geo2, APITrans t1, APITrans t2)
        {
            return API.computePenetration((long)geo1.GetInternal(), (int)geo1.GeoType, (long)geo2.GetInternal(),
                (int)geo2.GeoType, t1, t2);
        }
        
        public int SphereCast1(Buffer buffer, IGeometry geometry, Vector3 position)
        {
            int count = API.sceneOverlap1(Ref, buffer.Ref, 
                (long)geometry.GetInternal(), position, (i, nRef) =>
            {
                var physicsObject = GetObject(nRef);
                if (physicsObject != null)
                {
                    buffer.Things[i] = physicsObject.Thing;
                }
            });
            
            return count;
        }
        public int SphereCast10(Buffer buffer, IGeometry geometry, Vector3 position)
        {
            int count = API.sceneOverlap10(Ref, buffer.Ref, 
                (long)geometry.GetInternal(), position, (i, nRef) =>
                {
                    var physicsObject = GetObject(nRef);
                    if (physicsObject != null)
                    {
                        buffer.Things[i] = physicsObject.Thing;
                    }
                });
            
            return count;
        }
        public int SphereCast1000(Buffer buffer, IGeometry geometry, Vector3 position)
        {
            int count = API.sceneOverlap1000(Ref, buffer.Ref, 
                (long)geometry.GetInternal(), position, (i, nRef) =>
                {
                    var physicsObject = GetObject(nRef);
                    if (physicsObject != null)
                    {
                        buffer.Things[i] = physicsObject.Thing;
                    }
                });
            
            return count;
        }

        public void CharactersUpdate(float elapsed, float minDist)
        {
            API.charactersUpdate(Ref, elapsed, minDist);
        }
    }
}