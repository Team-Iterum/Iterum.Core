using System;
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
        public IPhysicsWorld.WorldState State { get; private set; }
        
        public float SceneFrame { get; private set; }
        public int Timestamp { get; private set; }
        public float DeltaTime { get; private set; }
        public int TPS { get; }
        
        
        public event EventHandler<ContactReport> ContactReport;
        
        private Thread workerThread;
        private long last;
        
        private Scene scene;
        
        public PhysicsWorld(Vector3 gravity, int tps)
        {
            scene = new Scene { Gravity = gravity };
            TPS = tps;
            
            Debug.LogV(LogGroup, $"Constructor. Gravity: {scene.Gravity} TPS: {TPS}");
        }
        private void StepPhysics()
        {
            Debug.LogV(LogGroup, $"Enter StepPhysics");
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            last = stopwatch.ElapsedTicks;
            
            while (State == IPhysicsWorld.WorldState.Running)
            {
                int sleep = TPS;

                float dt = (float) TimeConvert.TicksToSeconds(stopwatch.ElapsedTicks - last);
                last = stopwatch.ElapsedTicks;

                // Information
                DeltaTime = TimeConvert.SecondsToMs(dt);
                Timestamp = scene.Timestamp;

                // Simulate
                scene.StepPhysics(dt);

                // Calculate remaining time
                SceneFrame = (float)TimeConvert.TicksToMs(stopwatch.ElapsedTicks - last);
                
                sleep -= Mathf.CeilToInt(SceneFrame);
                if (sleep < 0) sleep = 1;
                
                Thread.Sleep(sleep);
            }
            
            Debug.LogV(LogGroup, $"Exit StepPhysics");
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
            
            State = IPhysicsWorld.WorldState.Destroyed;

            Stop();
            scene.Cleanup();
            
            Debug.LogV(LogGroup, $"Destroyed");
        }

        public void Start()
        {
            if (State != IPhysicsWorld.WorldState.Created) return;
            
            workerThread = new Thread(StepPhysics)
            {
                Name = $"PhysicsWorld Thread #{scene.Ref}",
                Priority = ThreadPriority.Highest,
                IsBackground = false,
            };

            workerThread.Start();

            State = IPhysicsWorld.WorldState.Running;

            Debug.LogV(LogGroup, $"Started");
        }

        public void Stop()
        {
            workerThread = null;
            State = IPhysicsWorld.WorldState.None;
            
            Debug.LogV(LogGroup, $"Stop");
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
        public IDynamicObject CreateDynamic(IGeometry geometry, Transform transform, PhysicsObjectFlags flags, float mass, uint word)
        {
            return scene.CreateDynamic(geometry, transform, flags, mass, word);
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
