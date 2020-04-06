using System;
using Magistr.Log;
using Magistr.Math;
using Magistr.Things;
using System.Collections.Generic;
using static Magistr.Physics.PhysXImpl.PhysicsAlias;

namespace Magistr.Physics.PhysXImpl
{
    public class Scene
    {
        private readonly IPhysicsWorld world;
        public long Ref { get; }
        
        private readonly Dictionary<long, IPhysicsObject> refs = new Dictionary<long, IPhysicsObject>();

        public int Timestamp => (int) API.getSceneTimestamp(Ref);

        private SphereGeometry overlapSphere;
        private float overlapSphereRadius = 150;
        public float OverlapSphereRadius
        {
            get => overlapSphereRadius;
            set
            {
                overlapSphereRadius = value;
                overlapSphere = new SphereGeometry(OverlapSphereRadius);
            }
        }

        public Scene(IPhysicsWorld world, ContactReportCallbackFunc contactReportCallback, TriggerReportCallbackFunc triggerCallback)
        {
            this.world = world;
            
            Ref = API.createScene(world.Gravity, contactReportCallback, triggerCallback);

        }
        
        public IPhysicsObject GetObject(in long nRef)
        {
            return refs[nRef];
        }

        public void StepPhysics(in float dt)
        {
            API.stepPhysics(Ref, dt);
            API.charactersUpdate(dt, 0.05f);
        }

        internal void Cleanup()
        {
            API.cleanupScene(Ref);
        }

        #region Destroy objects

        public void Destroy(StaticObject obj)
        {
            refs.Remove(obj.Ref);
            
            API.destroyRigidStatic(obj.Ref);

        }
        public void Destroy(DynamicObject obj)
        {
            refs.Remove(obj.Ref);
            API.destroyRigidDynamic(obj.Ref);
            
            Debug.LogV($"DynamicObject ({obj.Ref})", $"Destroyed", ConsoleColor.Red);

        }
        public void Destroy(PhysicsCharacter obj)
        {
            refs.Remove(obj.Ref);
            
            API.destroyController(obj.Ref);

        } 

        #endregion

        #region Create objects
        
        public IStaticObject CreateStatic(IGeometry geometry, Transform transform, bool isTrigger)
        {
            var obj = new StaticObject(geometry, this, API, isTrigger)
            {
                Position = transform.Position,
                Rotation = transform.Rotation
            };
            refs.Add(obj.Ref, obj);
            
            return obj;
        }

        public IDynamicObject CreateDynamic(IGeometry geometry, bool kinematic, bool isTrigger, float mass, Transform transform)
        {
            var obj = new DynamicObject(geometry, kinematic, isTrigger, mass, this, API)
            {
                Position = transform.Position,
                Rotation = transform.Rotation
            };
            refs.Add(obj.Ref, obj);

            return obj;
        }

        public IPhysicsCharacter CreateCapsuleCharacter(Vector3 position, Vector3 up, float height, float radius)
        {
            var obj = new PhysicsCharacter(position, up, height, radius, this, world, API);
            refs.Add(obj.Ref, obj);

            return obj;
        }
        
        #endregion

        public List<IThing> Overlap(Vector3 position)
        {
            var hits = new List<IThing>();  

            int unused = API.sceneOverlap(Ref, (long)overlapSphere.GetInternalGeometry(), position, (nRef) =>
            {
                if (refs.ContainsKey(nRef))
                {
                    if (refs[nRef] == null)
                    {
                        Debug.LogError("Scene Overlap", $"Ref: {nRef} == null");
                        return;
                    }
                    
                    hits.Add(refs[nRef].Thing);
                }
                else
                {
                    Debug.LogError("Scene Overlap", $"No reference: {nRef}");
                }
            });

            return hits;
        }
    }
}