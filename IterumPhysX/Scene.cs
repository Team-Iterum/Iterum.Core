using Magistr.Framework.Physics;
using Magistr.Log;
using Magistr.Math;
using Magistr.Things;
using System;
using System.Collections.Generic;

namespace Magistr.Physics.PhysXImplCore
{
    internal class Scene
    {
        private IPhysicsAPI api;
        private IPhysicsWorld world;

        public long Ref { get; }

        private Dictionary<long, PhysXCharacter> characters = new Dictionary<long, PhysXCharacter>();
        private Dictionary<long, IPhysicsObject> references = new Dictionary<long, IPhysicsObject>();

        public long Timestamp => api.getSceneTimestamp(Ref);

        public Scene(IPhysicsAPI api, IPhysicsWorld world)
        {
            this.api = api;
            this.world = world;
            Ref = this.api.createScene(world.Gravity.ToApi());
            
        }

        public void Simulate(float step)
        {
            api.stepPhysics(Ref, step);
        }

        internal void Cleanup()
        {
            api.cleanupScene(Ref);
        }

        internal void Destroy(PhysXStaticObject obj)
        {
            references.Remove(obj.Ref);
            api.destroyRigidStatic(obj.Ref);

        }

        internal void Destroy(PhysXDynamicObject obj)
        {
            references.Remove(obj.Ref);
            api.destroyRigidDynamic(obj.Ref);

        }

        internal void Destroy(PhysXCharacter obj)
        {
            characters.Remove(obj.Ref);
            references.Remove(obj.Ref);
            api.destroyController(obj.Ref);

        }

        internal PhysXStaticObject CreateRigidStatic(IGeometry geometry)
        {

            var obj = new PhysXStaticObject(geometry, this, world, api);

            references.Add(obj.Ref, obj);

            return obj;
        }

        internal PhysXCharacter CreateCapsuleController(Vector3 pos, Vector3 up, float height, float radius)
        {
            var obj = new PhysXCharacter(pos, up, height, radius, this, world, api);

            characters.Add(obj.Ref, obj);
            references.Add(obj.Ref, obj);

            return obj;
        }


        public IPhysicsDynamicObject CreateRigidDynamic(IGeometry geometry, bool kinematic)
        {
            var obj = new PhysXDynamicObject(geometry, kinematic, 1.0f, this, api);

            references.Add(obj.Ref, obj);

            return obj;
        }

        internal List<IThing> Overlap(Vector3 pos, IGeometry overlapSphere)
        {
            var hits = new List<IThing>();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var count = api.sceneOverlap(Ref, (long)overlapSphere.GetInternalGeometry(), pos.ToApi(), (nRef) =>
            {
                
                if (references.ContainsKey(nRef))
                    hits.Add(references[nRef].Thing);
                else
                {
                    Debug.Log(nRef.ToString());
                }
            });

            Debug.Log($"[API.sceneOverlap ({count})]={watch.ElapsedMilliseconds}ms", ConsoleColor.Yellow);

            watch.Stop();
            return hits;
        }

        internal void Update(float dt)
        {
            foreach (var item in characters)
            {
                item.Value.Update(dt);
            }
        }

    }
}