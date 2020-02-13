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

        public Scene(IPhysicsWorld world, ContactReportCallbackFunc contactReportCallback)
        {
            this.world = world;
            
            Ref = API.createScene(world.Gravity, contactReportCallback);

        }
        
        public IPhysicsObject GetObject(in long nRef)
        {
            return refs[nRef];
        }

        public void StepPhysics(in float dt)
        {
            API.stepPhysics(Ref, dt);
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

        }
        public void Destroy(PhysicsCharacter obj)
        {
            refs.Remove(obj.Ref);
            
            API.destroyController(obj.Ref);

        } 

        #endregion

        #region Create objects
        
        public IStaticObject CreateStatic(IGeometry geometry, Transform transform)
        {
            var obj = new StaticObject(geometry, this, API)
            {
                Position = transform.Position,
                Rotation = transform.Rotation
            };
            refs.Add(obj.Ref, obj);
            
            return obj;
        }

        public IDynamicObject CreateDynamic(IGeometry geometry, bool kinematic, float mass, Transform transform)
        {
            var obj = new DynamicObject(geometry, kinematic, mass, this, API)
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
                    hits.Add(refs[nRef].Thing);
                else
                {
                    Debug.LogError("Overlap", "No reference: " + nRef);
                }
            });

            return hits;
        }
    }
}