using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdvancedDLSupport;
using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Things;
using Nito.AsyncEx;

[assembly: InternalsVisibleTo("AdvancedDLSupport")]

namespace Magistr.Physics.PhysXImplCore
{
    public class PhysXWorld : IPhysicsWorld
    {
        private static bool PhysXCreated;

        private Scene scene;
        private Thread workerThread;

        private long last;

        public bool IsDestroyed { get; private set; }
        public bool IsCreated { get; private set; }
        public bool IsRunning { get; private set; }
        public float SceneFrame { get; private set; }
        public int Timestamp { get; set; }


        public float DeltaTime { get; private set; }
        public int TPS { get; set; } = 1000 / 60;
        public Guid WorldUid { get; } = Guid.NewGuid();

        public Vector3 Gravity { get; set; }

        private SphereGeometry overlapSphere;
        private float overlapSphereRadius = 150;


        private static IPhysicsAPI API;
        public float OverlapSphereRadius
        {
            get => overlapSphereRadius;
            set
            {
                overlapSphereRadius = value;
                Task.Run(async () =>
                {
                    await WaitEndOfFrame();
                    overlapSphere?.Destroy();
                    overlapSphere = new SphereGeometry(OverlapSphereRadius, API);
                }).ConfigureAwait(false);
            }
        }

        private void InitPhysics()
        {
            if (!PhysXCreated)
            {
                var builder = new NativeLibraryBuilder(ImplementationOptions.EnableOptimizations | ImplementationOptions.UseIndirectCalls);
                API = builder.ActivateInterface<IPhysicsAPI>("PhysXSharpNative");
                API.initPhysics(false, Environment.ProcessorCount, (message) =>
                {
                    Log.Debug.LogError(message);
                });


                PhysXCreated = true;
            }

        }

        private void InitScene()
        {

            scene = new Scene(API, this);
            overlapSphere = new SphereGeometry(OverlapSphereRadius, API);


        }
        private AsyncManualResetEvent mre = new AsyncManualResetEvent(true);
        private Thread sceneThread;
        private long beforeScene;

        public Action<float> SceneUpdate;

        private void StepPhysics()
        {
            mre.Reset();
            
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

                
                if (dt > 1)
                {
                    mre.Reset();
                    API.stepPhysics(scene.Ref, (float)dtSpan.TotalSeconds);
                    API.charactersUpdate((float)dtSpan.TotalSeconds, 0.05f);
                    mre.Set();
                }
               

                var stop = stopwatch.ElapsedTicks;

                SceneFrame = (float)TimeSpan.FromTicks(stop - last).TotalMilliseconds;
                
                sleep -= Mathf.CeilToInt(SceneFrame);
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


        public async Task<(List<IThing>, List<IThing>)> Overlap(Vector3 pos, List<IThing> except, bool isStaticOnly)
        {
            var thingsAdd = new List<IThing>();
            var thingsRemove = new List<IThing>();
            
            await WaitEndOfFrame();
            var hits = scene.Overlap(pos, overlapSphere);

            var remove = except.Where(e => !hits.Contains(e));
            var add = hits.Where(e => !except.Contains(e));

            thingsRemove.AddRange(remove);
            thingsAdd.AddRange(add);

            return (thingsAdd, thingsRemove);
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

        public IGeometry CreateStaticModelGeometry(IModelData modelData)
        {
            
            return new SphereGeometry(1, API);
        }

        public IGeometry CreateSphereGeometry(float radius)
        {
            return new SphereGeometry(radius, API);
        }

        public IGeometry CreateBoxGeometry(Vector3 size)
        {
            return new BoxGeometry(size / 2, API);
        }

        public IPhysicsStaticObject CreateStatic(IGeometry geometry, Vector3 pos, Quaternion rot)
        {
            var rigidActor = scene.CreateRigidStatic(geometry);

            return rigidActor;
        }

        public IPhysicsDynamicObject CreateDynamic(IGeometry geometry, Vector3 pos, Quaternion rot)
        {
            var rigidActor = scene.CreateRigidDynamic(geometry);   
            //var rigidActor = scene.Physics.CreateRigidDynamic();
            //RigidActorExt.CreateExclusiveShape(rigidActor, (Geometry)geometry.GetInternalGeometry(), Material);

            //rigidActor.GlobalPosePosition = (Vec3)pos;
            //rigidActor.GlobalPoseQuat = (Quat)rot;
            ////rigidActor.UserData = controller;
            //scene.AddActor(rigidActor);

            return default;
        }

        public IPhysicsCharaceter CreateCapsuleCharacter(Vector3 pos, Vector3 up, float height = 4, float radius = 1)
        {
            var controller = scene.CreateCapsuleController(pos, up, height, radius);
            return controller;
        }

        public Task WaitEndOfFrame()
        {
            return mre.WaitAsync();
        }
    }



}
