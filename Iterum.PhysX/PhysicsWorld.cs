using System;
using System.Runtime.CompilerServices;
using Iterum.Math;


[assembly: InternalsVisibleTo("AdvancedDLSupport")]
namespace Iterum.Physics.PhysXImpl
{
    public sealed class PhysicsWorld : IPhysicsWorld
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

        public int Raycast(Buffer refBuffer, Vector3 position, Vector3 direction, float maxDist)
        {
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} Raycast. Position: {position} Direction: {direction}");
#endif
            
            int count = scene.Raycast(refBuffer, position, direction, maxDist);
            return count;
        }

        public int SphereCast1000(Buffer buffer, Vector3 position, IGeometry geometry)
        {
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} SphereCast1000. Position: {position} Geometry: {geometry.GetInternalGeometry()}");
#endif
            
            int count = scene.SphereCast1000(buffer, geometry, position);
            return count;
        }
        public int SphereCast10(Buffer buffer, Vector3 position, IGeometry geometry)
        {
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} SphereCast10. Position: {position} Geometry: {geometry.GetInternalGeometry()}");
#endif
            
            int count = scene.SphereCast10(buffer, geometry, position);
            return count;
        }
        public int SphereCast1(Buffer buffer, Vector3 position, IGeometry geometry)
        {
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"{LogGroup} SphereCast1. Position: {position} Geometry: {geometry.GetInternalGeometry()}");
#endif
            
            int count = scene.SphereCast1(buffer, geometry, position);
            return count;
        }


        private string LogGroup => $"[PhysicsWorld ({scene.Ref})]";

        #endregion

        #region Create objects

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
            
            ContactReport?.Invoke(this, new ContactReport()
            {
                obj0 = obj0.Thing,
                obj1 = obj1.Thing,
                
                isTrigger = true,
            });
        }
        private void OnContactReport(long ref0, long ref1, APIVec3 normal, APIVec3 position, APIVec3 impulse, float separation)
        {
            var obj0 = scene.GetObject(ref0);
            var obj1 = scene.GetObject(ref1);
            
            if (obj0 == null || obj1 == null) return;
            
            ContactReport?.Invoke(this, new ContactReport()
            {
                obj0 = obj0.Thing,
                obj1 = obj1.Thing,
                
                position = position,
                
                normal = normal,
                impulse = impulse,
                
                separation = separation
            });
        }

        #endregion

        
        public override string ToString() => LogGroup;
    }
}
