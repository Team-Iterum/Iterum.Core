using Magistr.Framework.Physics;
using Magistr.Log;
using Magistr.Math;
using Magistr.Things;
using System.Collections.Generic;
using static Magistr.Physics.PhysXImplCore.PhysicsAlias;

namespace Magistr.Physics.PhysXImplCore
{
    public class Scene
    {
        private IPhysicsWorld world;

        public long Ref { get; }

        private Dictionary<long, PhysicsCharacter> characters = new Dictionary<long, PhysicsCharacter>();
        private Dictionary<long, IPhysicsObject> references = new Dictionary<long, IPhysicsObject>();

        public int Timestamp => (int) API.getSceneTimestamp(Ref);

        public Scene(IPhysicsWorld world, ContactReportCallbackFunc func)
        {
            this.world = world;

            Ref = API.createScene(world.Gravity, func);

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
            references.Remove(obj.Ref);
            API.destroyRigidStatic(obj.Ref);

        }
        public void Destroy(DynamicObject obj)
        {
            references.Remove(obj.Ref);
            API.destroyRigidDynamic(obj.Ref);

        }
        public void Destroy(PhysicsCharacter obj)
        {
            characters.Remove(obj.Ref);
            references.Remove(obj.Ref);
            API.destroyController(obj.Ref);

        } 

        #endregion

        #region Create objects

        public StaticObject CreateRigidStatic(IGeometry geometry)
        {
            var obj = new StaticObject(geometry, this, API);
            references.Add(obj.Ref, obj);
            return obj;
        }

        public PhysicsCharacter CreateCapsuleController(Vector3 pos, Vector3 up, float height, float radius)
        {
            var obj = new PhysicsCharacter(pos, up, height, radius, this, world, API);

            characters.Add(obj.Ref, obj);
            references.Add(obj.Ref, obj);

            return obj;
        }

        public IPhysicsDynamicObject CreateRigidDynamic(IGeometry geometry, bool kinematic, float mass)
        {
            var obj = new DynamicObject(geometry, kinematic, mass, this, API);

            references.Add(obj.Ref, obj);

            return obj;
        }

        #endregion

        public List<IThing> Overlap(Vector3 pos, IGeometry overlapSphere)
        {
            var hits = new List<IThing>();  

            int unused = API.sceneOverlap(Ref, (long)overlapSphere.GetInternalGeometry(), pos, (nRef) =>
            {
                if (references.ContainsKey(nRef))
                    hits.Add(references[nRef].Thing);
                else
                {
                    Debug.LogError("Overlap", "No reference: " + nRef);
                }
            });

            return hits;
        }

        public void Update(float dt)
        {
            foreach (var item in characters)
            {
                item.Value.Update(dt);
            }
        }

        public IPhysicsObject GetReference(in long nRef)
        {
            return references[nRef];
        }
    }
}