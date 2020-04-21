﻿using System;
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
        
        private readonly Dictionary<long, IPhysicsObject> refs = new Dictionary<long, IPhysicsObject>();
        
        public void Create(ContactReportCallbackFunc contactReport, TriggerReportCallbackFunc trigger)
        {
            Ref = API.createScene(Gravity, contactReport, trigger);
            
            Debug.LogV(LogGroup, $"Create. Gravity: {Gravity}");
        }

        public IPhysicsObject GetObject(in long nRef)
        {
            if (!refs.ContainsKey(nRef)) return null;
            
            Debug.LogV(LogGroup, $"GetObject with Ref: {nRef}");
            
            return refs[nRef];
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
            
            Debug.LogV(LogGroup, $"StaticObject Ref: ({e.Ref}) destroyed", ConsoleColor.Red);
        }
        public void Destroy(DynamicObject e)
        {
            refs.Remove(e.Ref);
            API.destroyRigidDynamic(e.Ref);
            
            Debug.LogV(LogGroup, $"DynamicObject Ref: ({e.Ref}) destroyed", ConsoleColor.Red);
        }
        public void Destroy(PhysicsCharacter e)
        {
            refs.Remove(e.Ref);
            
            API.destroyController(e.Ref);
            
            Debug.LogV(LogGroup, $"PhysicsCharacter Ref: ({e.Ref}) destroyed", ConsoleColor.Red);

        } 

        #endregion

        #region Create objects
        
        public IStaticObject CreateStatic(IGeometry geometry, Transform transform, PhysicsObjectFlags flags)
        {
            var obj = new StaticObject(geometry, flags, transform, this);
            refs.Add(obj.Ref, obj);
            
            Debug.LogV(LogGroup, $"StaticObject Ref: ({obj.Ref}) created", ConsoleColor.DarkGreen);
            return obj;
        }

        public IDynamicObject CreateDynamic(IGeometry geometry, Transform transform, PhysicsObjectFlags flags, float mass)
        {
            var obj = new DynamicObject(geometry, flags, mass, transform, this);
            refs.Add(obj.Ref, obj);
            
            Debug.LogV(LogGroup, $"DynamicObject Ref: ({obj.Ref}) created", ConsoleColor.DarkGreen);
            
            return obj;
        }

        public IPhysicsCharacter CreateCapsuleCharacter(Vector3 position, Vector3 up, float height, float radius)
        {
            var obj = new PhysicsCharacter(position, up, height, radius, this);
            refs.Add(obj.Ref, obj);

            Debug.LogV(LogGroup, $"CapsuleCharacter Ref: ({obj.Ref}) created", ConsoleColor.DarkGreen);
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

            Debug.LogV(LogGroup, $"Overlap count: {count} hits: {hits.Count}");
            
            return hits;
        }
    }
}