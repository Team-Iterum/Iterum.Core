﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Iterum.Math;
using Iterum.Things;
using Iterum.Utils;
using Debug = Iterum.Log.Debug;


[assembly: InternalsVisibleTo("AdvancedDLSupport")]
namespace Iterum.Physics.PhysXImpl
{
    public sealed class PhysicsWorld : IPhysicsWorld
    {
        public IPhysicsWorld.WorldState State { get; set; } = IPhysicsWorld.WorldState.None;
            
        public int Timestamp { get; private set; }

        public event EventHandler<ContactReport> ContactReport;
        
        private Scene scene;
        
        public PhysicsWorld(Vector3 gravity)
        {
            scene = new Scene { Gravity = gravity };

            Debug.LogV(LogGroup, $"Constructor. Gravity: {scene.Gravity}");
        }

        public void Step(float dt, float subSteps = 1)
        {
            scene.StepPhysics(dt);
        }
        
        public void Create()
        {
            if (State != IPhysicsWorld.WorldState.None) return;
            
            scene.Create(OnContactReport, OnTriggerReport);
            
            State = IPhysicsWorld.WorldState.Created;
            
            Debug.LogV(LogGroup, $"Created");
        }

        public void Destroy()
        {
            if (State != IPhysicsWorld.WorldState.Created) return;
            
            scene.Cleanup();
            
            State = IPhysicsWorld.WorldState.Destroyed;
            
            Debug.LogV(LogGroup, $"Destroyed");
        }
        

        #region Overlaps / Raycasts

        public AddRemoveThings Overlap(Vector3 position, IGeometry geometry, List<IThing> except)
        {
            var hits = scene.Overlap(geometry, position);

            var remove = except.Where(e => !hits.Contains(e));
            var add = hits.Where(e => !except.Contains(e));

            if(PhysicsAlias.ExtendedVerbose) Debug.LogV(LogGroup, $"Overlap. Position: {position} Geo: {geometry.GeoType}");
            
            return new AddRemoveThings { Add = add, Remove = remove };
        }

        public IEnumerable<IThing> Raycast(Vector3 position, Vector3 direction)
        {
            if(PhysicsAlias.ExtendedVerbose) Debug.LogV(LogGroup, $"Raycast. Position: {position} Direction: {direction}");
            throw new NotImplementedException();
        }

        public IEnumerable<IThing> SphereCast(Vector3 position, IGeometry geometry)
        {
            if(PhysicsAlias.ExtendedVerbose) Debug.LogV(LogGroup, $"SphereCast. Position: {position} Geometry: {geometry.GetInternalGeometry()}");
            throw new NotImplementedException();
        }

        public string LogGroup => $"PhysicsWorld ({scene.Ref})";

        #endregion

        #region Create objects

        public IStaticObject CreateStatic(IGeometry geometry, Transform transform, PhysicsObjectFlags flags)
        {
            return scene.CreateStatic(geometry, transform, flags);
        }
        public IDynamicObject CreateDynamic(IGeometry[] geometries, Transform transform, PhysicsObjectFlags flags, float mass, uint word)
        {
            return scene.CreateDynamic(geometries, transform, flags, mass, word);
        }
        public IPhysicsCharacter CreateCapsuleCharacter(Transform transform, Vector3 up, float height, float radius)
        {
            return scene.CreateCapsuleCharacter(transform.Position, up, height, radius);
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
    }
}
