using Magistr.Framework.Physics;
using Magistr.Log;
using Magistr.Math;
using Magistr.Things;
using System;
using System.Collections.Generic;

namespace Magistr.Physics.PhysXImplCore
{
    public class Scene
    {
        private IPhysicsAPI api;
        private IPhysicsWorld world;

        public long Ref { get; }

        private Dictionary<long, PhysicsCharacter> characters = new Dictionary<long, PhysicsCharacter>();
        private Dictionary<long, IPhysicsObject> references = new Dictionary<long, IPhysicsObject>();

        public long Timestamp => api.getSceneTimestamp(Ref);

        public Scene(IPhysicsAPI api, IPhysicsWorld world)
        {
            this.api = api;
            this.world = world;
            Ref = this.api.createScene(world.Gravity);
            
        }

        public void Simulate(float step)
        {
            //api.stepPhysics(Ref, step);
        }

        internal void Cleanup()
        {
            api.cleanupScene(Ref);
        }

        public void Destroy(StaticObject obj)
        {
            references.Remove(obj.Ref);
            api.destroyRigidStatic(obj.Ref);

        }
        public void Destroy(DynamicObject obj)
        {
            references.Remove(obj.Ref);
            api.destroyRigidDynamic(obj.Ref);

        }
        public void Destroy(PhysicsCharacter obj)
        {
            characters.Remove(obj.Ref);
            references.Remove(obj.Ref);
            api.destroyController(obj.Ref);

        }

        public StaticObject CreateRigidStatic(IGeometry geometry)
        {
            var obj = new StaticObject(geometry, this, api);
            references.Add(obj.Ref, obj);
            return obj;
        }

        public PhysicsCharacter CreateCapsuleController(Vector3 pos, Vector3 up, float height, float radius)
        {
            var obj = new PhysicsCharacter(pos, up, height, radius, this, world, api);

            characters.Add(obj.Ref, obj);
            references.Add(obj.Ref, obj);

            return obj;
        }


        public IPhysicsDynamicObject CreateRigidDynamic(IGeometry geometry, bool kinematic)
        {
            var obj = new DynamicObject(geometry, kinematic, 1.0f, this, api);

            references.Add(obj.Ref, obj);

            return obj;
        }

        public List<IThing> Overlap(Vector3 pos, IGeometry overlapSphere)
        {
            var hits = new List<IThing>();  

            var count = api.sceneOverlap(Ref, (long)overlapSphere.GetInternalGeometry(), pos, (nRef) =>
            {
                if (references.ContainsKey(nRef))
                    hits.Add(references[nRef].Thing);
                else
                {
                    Debug.Log(nRef.ToString());
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

    }
}