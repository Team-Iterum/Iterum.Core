using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using AdvancedDLSupport;
using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Things;

[assembly: InternalsVisibleTo("AdvancedDLSupport")]

namespace Magistr.Physics.PhysXImplCore
{
    public class PhysXWorld : IPhysicsWorld
    {
        private static bool PhysXCreated;
        private static IPhysicsAPI API;

        

        public bool IsDestroyed { get; private set; }
        public bool IsCreated { get; private set; }
        public bool IsRunning { get; private set; }
        
        public float SceneFrame { get; private set; }
        public int Timestamp { get; set; }
        public float DeltaTime { get; private set; }
        public int TPS { get; set; } = 1000 / 60;
        

        public Vector3 Gravity { get; set; }


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
                if(API == null) throw new NullReferenceException("API not yet created or destroyed");
                overlapSphereRadius = value;
                overlapSphere = new SphereGeometry(OverlapSphereRadius, API);
            }
        }

        private void InitPhysics()
        {
            if (!PhysXCreated)
            {
                var builder = new NativeLibraryBuilder(ImplementationOptions.EnableOptimizations | ImplementationOptions.UseIndirectCalls);
                API = builder.ActivateInterface<IPhysicsAPI>("PhysXSharpNative");

                API.initLog((s) => Log.Debug.Log("PhysX", s, ConsoleColor.Yellow), (s) => Log.Debug.LogError("PhysX", s));
                
                API.initPhysics(true, Environment.ProcessorCount, (s) => Log.Debug.LogError("PhysX Critical", s));

                PhysXCreated = true;
            }

        }

        private void InitScene()
        {
            scene = new Scene(API, this);
            overlapSphere = new SphereGeometry(OverlapSphereRadius, API);

        }
        

        private void StepPhysics()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            last = stopwatch.ElapsedTicks;
            
            while (IsRunning)
            {
                int sleep = TPS;

                var dtSpan = TimeSpan.FromTicks(stopwatch.ElapsedTicks - last);
                var dt = (float)dtSpan.TotalMilliseconds;
                last = stopwatch.ElapsedTicks;

                DeltaTime = dt;
                Timestamp = (int)scene.Timestamp;

                API.stepPhysics(scene.Ref, (float)dtSpan.TotalSeconds);
                var stop = stopwatch.ElapsedTicks;

                SceneFrame = (float)TimeSpan.FromTicks(stop - last).TotalMilliseconds;
                
                sleep -= Mathf.CeilToInt(SceneFrame);
                
                // very bad - OVERLOAD
                if (sleep < 0) sleep = 1;
                
                Thread.Sleep(sleep);
            }
        }


        public AddRemoveThings Overlap(Vector3 position, List<IThing> except, bool isStaticOnly)
        {
            
            var hits = scene.Overlap(position, overlapSphere);

            var remove = except.Where(e => !hits.Contains(e));
            var add = hits.Where(e => !except.Contains(e));

            return new AddRemoveThings() { Add = add.ToList(), Remove = remove.ToList() };
        }

        public void Create()
        {
            InitPhysics();
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

        private void StepScene()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            beforeScene = stopwatch.ElapsedTicks;
            
            while (IsRunning)
            {
                int sleep = TPS;    

                var dtSpan = TimeSpan.FromTicks(stopwatch.ElapsedTicks - beforeScene);

                beforeScene = stopwatch.ElapsedTicks;
                {
                    SceneUpdate?.Invoke((float) dtSpan.TotalSeconds);
                    scene.Update((float) dtSpan.TotalSeconds);
                }
                var afterScene = stopwatch.ElapsedTicks;

                var sceneFrame = (float)TimeSpan.FromTicks(afterScene - beforeScene).TotalMilliseconds;
                
                sleep -= (int)sceneFrame;
                if (sleep < 0)
                {
                    sleep = 1;
                    Thread.Sleep(sleep);
                }
                else
                {
                    Thread.Sleep(sleep);
                }
            }
        }

        public void Stop()
        {
            sceneThread = null;
            workerThread = null;
            IsRunning = false;
        }

        public IGeometry CreateTriangleMeshGeometry(IModelData modelData)
        {
            return new ModelGeometry(GeoType.TriangleMeshGeometry, modelData, API);
        }

        public IGeometry CreateConvexMeshGeometry(IModelData modelData)
        {
            return new ModelGeometry(GeoType.ConvexMeshGeometry, modelData, API);
        }

        public IGeometry CreateSphereGeometry(float radius)
        {
            return new SphereGeometry(radius, API);
        }

        public IGeometry CreateBoxGeometry(Vector3 size)
        {
            return new BoxGeometry(size, API);
        }

        public IPhysicsStaticObject CreateStatic(IGeometry geometry, Vector3 pos, Quaternion rot)
        {
            var rigidActor = scene.CreateRigidStatic(geometry);
            rigidActor.Position = pos;
            rigidActor.Rotation = rot;
            return rigidActor;
        }

        public IPhysicsDynamicObject CreateDynamic(IGeometry geometry, bool kinematic, Vector3 pos, Quaternion rot)
        {
            var rigidActor = scene.CreateRigidDynamic(geometry, kinematic);
            rigidActor.Position = pos;
            rigidActor.Rotation = rot;

            return rigidActor;
        }

        public IPhysicsCharacter CreateCapsuleCharacter(Vector3 pos, Vector3 up, float height = 4, float radius = 1)
        {
            var controller = scene.CreateCapsuleController(pos, up, height, radius);
            return controller;
        }

    }


}
