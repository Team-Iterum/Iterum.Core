using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Things;
using Magistr.Utils;
using static Magistr.Physics.PhysXImplCore.PhysicsAlias;
using Debug = Magistr.Log.Debug;

[assembly: InternalsVisibleTo("AdvancedDLSupport")]

namespace Magistr.Physics.PhysXImplCore
{
    public class PhysXWorld : IPhysicsWorld
    {

        public bool IsDestroyed { get; private set; }
        public bool IsCreated { get; private set; }
        public bool IsRunning { get; private set; }
        
        public float SceneFrame { get; private set; }
        public int Timestamp { get; set; }
        public float DeltaTime { get; private set; }
        public int TPS { get; set; } = 1000 / 60;
        

        public Vector3 Gravity { get; set; }
        public event EventHandler<ContactReport> ContactReport;


        private Thread workerThread;
        private long last;
        

        private Scene scene;
        private Thread sceneThread;

        private long beforeScene;
        public Action<float> SceneUpdate;

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


        private void InitScene()
        {
            scene = new Scene(this, OnContactReport);
            overlapSphere = new SphereGeometry(OverlapSphereRadius);

        }
        

        private void StepPhysics()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            last = stopwatch.ElapsedTicks;
            
            while (IsRunning)
            {
                int sleep = TPS;

                float dt = (float) TimeConversion.TicksToSeconds(stopwatch.ElapsedTicks - last);
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

        private void StepScene()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            beforeScene = stopwatch.ElapsedTicks;

            while (IsRunning)
            {
                int sleep = TPS;    

                float dt = (float) TimeConversion.TicksToSeconds(stopwatch.ElapsedTicks - beforeScene);
                beforeScene = stopwatch.ElapsedTicks;

                // Update scene
                SceneUpdate?.Invoke(dt);
                scene.Update(dt);

                // Calculate remaining time
                float sceneFrame = (float)TimeConversion.TicksToMs(stopwatch.ElapsedTicks - beforeScene);
                
                // Wait
                sleep -= Mathf.CeilToInt(sceneFrame);
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
            sceneThread = new Thread(StepScene);

            IsRunning = true;

            workerThread.Start();
            sceneThread.Start();

        }

        public void Stop()
        {
            sceneThread = null;
            workerThread = null;
            IsRunning = false;
        }

        
        public AddRemoveThings Overlap(Vector3 position, List<IThing> except, bool isStaticOnly)
        {
            var hits = scene.Overlap(position, overlapSphere);

            var remove = except.Where(e => !hits.Contains(e));
            var add = hits.Where(e => !except.Contains(e));

            return new AddRemoveThings() { Add = add.ToList(), Remove = remove.ToList() };
        }


       

        public IPhysicsStaticObject CreateStatic(IGeometry geometry, Vector3 pos, Quaternion rot)
        {
            var rigidActor = scene.CreateRigidStatic(geometry);
            rigidActor.Position = pos;
            rigidActor.Rotation = rot;
            return rigidActor;
        }

        public IPhysicsDynamicObject CreateDynamic(IGeometry geometry, bool kinematic, float mass, Vector3 pos, Quaternion rot)
        {
            var rigidActor = scene.CreateRigidDynamic(geometry, kinematic, mass);
            rigidActor.Position = pos;
            rigidActor.Rotation = rot;

            return rigidActor;
        }

        public IPhysicsCharacter CreateCapsuleCharacter(Vector3 pos, Vector3 up, float height = 4, float radius = 1)
        {
            var controller = scene.CreateCapsuleController(pos, up, height, radius);

            return controller;
        }

        protected virtual void OnContactReport(long ref0, long ref1, APIVec3 normal, APIVec3 position, APIVec3 impulse, float separation)
        {
            var obj0 = scene.GetReference(ref0);
            var obj1 = scene.GetReference(ref1);

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
