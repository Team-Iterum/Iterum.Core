using System;
using System.Collections.Concurrent;
using Iterum.Log;
using Iterum.Math;
using Iterum.Things;
using System.Collections.Generic;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl
{
    public class Scene
    {
        public long Ref { get; private set; }
        public int Timestamp => (int) API.getSceneTimestamp(Ref);
        public Vector3 Gravity;

        private string LogGroup => $"Scene ({Ref})";
        
        private Dictionary<long, IPhysicsObject> refs = new Dictionary<long, IPhysicsObject>();
        
        public void Create(ContactReportCallbackFunc contactReport, TriggerReportCallbackFunc trigger)
        {
            Ref = API.createScene(Gravity, contactReport, trigger);
            
            Debug.LogV(LogGroup, $"Create. Gravity: {Gravity}");
        }

        public IPhysicsObject GetObject(in long nRef)
        {
            refs.TryGetValue(nRef, out var obj);
            return obj;
        }

        public void StepPhysics(in float dt)
        {
            API.stepPhysics(Ref, dt);
            API.charactersUpdate(dt, 0.01f);
        }

        internal void Cleanup()
        {
            API.cleanupScene(Ref);
            
            Debug.LogV(LogGroup, $"Cleanup");
        }

        #region Destroy objects

        public void Destroy(StaticObject e)
        {
            refs.Remove(e.Ref);
            
            API.destroyRigidStatic(e.Ref);
            
            if(ExtendedVerbose) Debug.LogV(LogGroup, $"StaticObject Ref: ({e.Ref}) destroyed", ConsoleColor.Red);
        }
        public void Destroy(DynamicObject e)
        {
            refs.Remove(e.Ref);
            API.destroyRigidDynamic(e.Ref);
            
            if(ExtendedVerbose) Debug.LogV(LogGroup, $"DynamicObject Ref: ({e.Ref}) destroyed", ConsoleColor.Red);
        }
        public void Destroy(PhysicsCharacter e)
        {
            refs.Remove(e.Ref);
            
            API.destroyController(e.Ref);
            
            if(ExtendedVerbose) Debug.LogV(LogGroup, $"PhysicsCharacter Ref: ({e.Ref}) destroyed", ConsoleColor.Red);

        } 

        #endregion

        #region Create objects
        
        public IStaticObject CreateStatic(IGeometry geometry, Transform transform, PhysicsObjectFlags flags)
        {
            var obj = new StaticObject(geometry, flags, transform, this);
            refs.Add(obj.Ref, obj);
            
            if(ExtendedVerbose) Debug.LogV(LogGroup, $"StaticObject Ref: ({obj.Ref}) created", ConsoleColor.DarkGreen);
            return obj;
        }

        public IDynamicObject CreateDynamic(IGeometry[] geometries, Transform transform, PhysicsObjectFlags flags, float mass, uint word)
        {
            var obj = new DynamicObject(geometries, flags, mass, word, transform,  this);
            refs.Add(obj.Ref, obj);
            
            if(ExtendedVerbose) Debug.LogV(LogGroup, $"DynamicObject Ref: ({obj.Ref}) created", ConsoleColor.DarkGreen);
            
            return obj;
        }

        public IPhysicsCharacter CreateCapsuleCharacter(Vector3 position, Vector3 up, float height, float radius)
        {
            var obj = new PhysicsCharacter(position, up, height, radius, this);
            refs.Add(obj.Ref, obj);

            if(ExtendedVerbose) Debug.LogV(LogGroup, $"CapsuleCharacter Ref: ({obj.Ref}) created", ConsoleColor.DarkGreen);
            return obj;
        }
        
        #endregion

        public List<IThing> Overlap(IGeometry geometry, Vector3 position)
        {
            var hits = new List<IThing>();  

            int count = API.sceneOverlap(Ref, (long)geometry.GetInternalGeometry(), position, (nRef) =>
            {
                if (refs.ContainsKey(nRef))
                {
                    if (refs[nRef] == null)
                    {
                        Debug.LogError(LogGroup, $"Overlap. Ref: {nRef} == null");
                        return;
                    }
                    
                    hits.Add(refs[nRef].Thing);
                }
                else
                {
                    Debug.LogError(LogGroup, $"Overlap. No reference: {nRef}");
                }
            });

            if(ExtendedVerbose) Debug.LogV(LogGroup, $"Overlap count: {count} hits: {hits.Count}");
            
            return hits;
        }
    }
}