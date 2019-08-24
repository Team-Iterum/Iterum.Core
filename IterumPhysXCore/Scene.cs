using Magistr.Framework.Physics;
using Magistr.Log;
using Magistr.Math;
using Magistr.Things;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Magistr.Physics.PhysXImplCore
{
    internal class Scene
    {
        private IPhysicsAPI API;
        private IPhysicsWorld World;

        internal int Index { get; }

        private int UserDataIndex = 0;

        private Dictionary<int, IPhysicsObject> UserDataReferences = new Dictionary<int, IPhysicsObject>();

        private Dictionary<int, IPhysicsCharaceter> Characters = new Dictionary<int, IPhysicsCharaceter>();

        public int Timestamp => API.getSceneTimestamp(Index);

        public Scene(IPhysicsAPI api, IPhysicsWorld world)
        {
            API = api;
            World = world;
            Index = API.createScene(world.Gravity.ToApi());
            
        }

        public void Simulate(float step)
        {
            API.stepPhysics(Index, step);
        }

        internal void Cleanup()
        {
            API.cleanupScene(Index);
        }

        internal void Destroy(PhysXStaticObject obj)
        {
            UserDataReferences.Remove(obj.UserDataReference);
            API.destroyRigidStatic(obj.Index, Index);

        }

        internal void Destroy(PhysXCharacter obj)
        {
            UserDataReferences.Remove(obj.UserDataReference);
            Characters.Remove(obj.UserDataReference);
            API.destroyController(obj.Index, Index);

        }

        internal PhysXStaticObject CreateRigidStatic(IGeometry geometry)
        {

            var staticObj = new PhysXStaticObject(geometry, UserDataIndex, this, World, API);
            UserDataReferences.Add(UserDataIndex, staticObj);
            UserDataIndex++;
            return staticObj;
        }

        internal PhysXCharacter CreateCapsuleController(Vector3 pos, Vector3 up, float height, float radius)
        {
            var obj = new PhysXCharacter(UserDataIndex, pos, up, height, radius, this, World, API);
            UserDataReferences.Add(UserDataIndex, obj);
            Characters.Add(UserDataIndex, obj);
            UserDataIndex++;
            return obj;
        }

        internal List<IThing> Overlap(Vector3 pos, IGeometry overlapSphere)
        {
            List<IThing> hits = new List<IThing>();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var count = API.sceneOverlap(Index, (int)overlapSphere.GetInternalGeometry(), pos.ToApi(), (index) =>
            {
                if (UserDataReferences.ContainsKey(index))
                    hits.Add(UserDataReferences[index].Thing);
            });
            Debug.Log($"[API.sceneOverlap ({count})]={watch.ElapsedMilliseconds}ms", ConsoleColor.Yellow);
            watch.Stop();
            return hits;
        }

        internal void Update(float dt)
        {
            foreach (var item in Characters)
            {
                ((PhysXCharacter)item.Value).Update(dt);
            }
        }
    }
}