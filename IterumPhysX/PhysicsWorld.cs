using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Magistr.Math;
using Magistr.Things;
using Magistr.Utils;

[assembly: InternalsVisibleTo("AdvancedDLSupport")]
namespace Magistr.Physics.PhysXImpl
{
    public sealed class PhysicsWorld : IPhysicsWorld
    {

        public bool IsDestroyed { get; private set; }
        public bool IsCreated { get; private set; }
        public bool IsRunning { get; private set; }
        
        public float SceneFrame { get; private set; }
        public int Timestamp { get; private set; }
        public float DeltaTime { get; private set; }
        public int TPS { get; set; }

        
        public Vector3 Gravity { get; set; }
        
        public float OverlapSphereRadius { get; set; }
        
        
        public event EventHandler<ContactReport> ContactReport;

        
        private Thread workerThread;
        private long last;
        
        
        private Scene scene;
        
        private void InitScene()
        {
            scene = new Scene(this, OnContactReport, OnTriggerReport)
            {
                OverlapSphereRadius = OverlapSphereRadius
            };
        }
        

        private void StepPhysics()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            last = stopwatch.ElapsedTicks;
            
            while (IsRunning)
            {
                int sleep = TPS;

                var dt = (float) TimeConversion.TicksToSeconds(stopwatch.ElapsedTicks - last);
                last = stopwatch.ElapsedTicks;

                // Information
                DeltaTime = TimeConversion.SecondsToMs(dt);
                Timestamp = scene.Timestamp;

                // Simulate
                scene.StepPhysics(dt);

                // Calculate remaining time
                SceneFrame = (float)TimeConversion.TicksToMs(stopwatch.ElapsedTicks - last);
                
                sleep -= Mathf.CeilToInt(SceneFrame);
                if (sleep < 0) sleep = 1;
                
                Thread.Sleep(sleep);
            }
        }
        
        public void Create()
        {
            InitScene();

            IsCreated = true;
        }

        public void Destroy()
        {
            IsDestroyed = true;

            Stop();
            scene.Cleanup();
        }

        public void Start()
        {
            workerThread = new Thread(StepPhysics)
            {
                IsBackground = false,
                Priority = ThreadPriority.Highest
            };
            workerThread.Name = $"PhysicsWorldThread";
            
            IsRunning = true;

            workerThread.Start();

        }

        public void Stop()
        {
            workerThread = null;
            IsRunning = false;
        }
        
        public AddRemoveThings Overlap(Vector3 position, List<IThing> except)
        {
            var hits = scene.Overlap(position);

            var remove = except.Where(e => !hits.Contains(e));
            var add = hits.Where(e => !except.Contains(e));

            return new AddRemoveThings() { Add = add, Remove = remove };
        }

        public IStaticObject CreateStatic(IGeometry geometry, Transform transform, bool isTrigger)
        {
            return scene.CreateStatic(geometry, transform, isTrigger);
        }
        public IDynamicObject CreateDynamic(IGeometry geometry, bool kinematic,  bool isTrigger, bool disableGravity, float mass, Transform transform)
        {
            return scene.CreateDynamic(geometry, kinematic, isTrigger, disableGravity, mass, transform);
        }
        public IPhysicsCharacter CreateCapsuleCharacter(Vector3 position, Vector3 up, float height, float radius)
        {
            return scene.CreateCapsuleCharacter(position, up, height, radius);
        }

        private void OnTriggerReport(long ref0, long ref1)
        {
            var obj0 = scene.GetObject(ref0);
            var obj1 = scene.GetObject(ref1);

            ContactReport?.Invoke(this, new ContactReport()
            {
                obj0 = obj0.Thing,
                obj1 = obj1.Thing,
                
                IsTrigger = true,
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
    }
}
