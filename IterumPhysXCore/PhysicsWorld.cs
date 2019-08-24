using AdvancedDLSupport;
using Magistr.Framework.Physics;
using Magistr.Log;
using Magistr.Math;
using Magistr.Physics.PhysXImplCore;
using Magistr.Things;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Quat = System.Numerics.Quaternion;
using Vec3 = System.Numerics.Vector3;

[assembly: InternalsVisibleTo("AdvancedDLSupport")]

namespace Magistr.Physics.PhysXImplCore
{
    public class PhysXWorld : IPhysicsWorld
    {
        private static bool PhysXCreated = false;
        private Scene scene;
        private Thread workerThread;
        private long last;
        private long lastFrame;

        public bool IsDestoyed { get; private set; } = false;
        public bool IsCreated { get; private set; } = false;
        public bool IsRunning { get; private set; } = false;
        public float SceneFrame { get; private set; }
        public int Timestamp { get; set; }

        public float DeltaTime { get; private set; }
        public int TPS { get; set; }
        public System.Guid WorldUid { get; } = System.Guid.NewGuid();

        public Vector3 Gravity => new Vector3(0, -10, 0);

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
                API = builder.ActivateInterface<IPhysicsAPI>("SnippetHelloWorld_64");
                API.initPhysics();
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
        private long lastScene;

        private void StepPhysics()
        {
            mre.Reset();
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            last = stopwatch.ElapsedTicks;
            int firstSkip = 0;
            while (IsRunning)
            {
                int sleep = TPS;

                var elapsed = stopwatch.ElapsedTicks;
                var dtspan = TimeSpan.FromTicks(elapsed - last);
                var dt = (float)dtspan.TotalMilliseconds;
                last = stopwatch.ElapsedTicks;

                this.DeltaTime = dt;
                this.Timestamp = scene.Timestamp;

                if (dt > 1 && firstSkip > 10)
                {
                    API.stepPhysics(scene.Index, (float)dtspan.TotalSeconds);
                }
                var stop = stopwatch.ElapsedTicks;
                mre.Set();
                
                this.SceneFrame = (float)TimeSpan.FromTicks(stop - last).TotalMilliseconds;
                sleep -= Mathf.CeilToInt(this.SceneFrame);
                if (sleep < 0)
                {
                    sleep = 1;
                    Thread.Sleep(sleep);
                    mre.Reset();
                }
                else
                {
                    Thread.Sleep(sleep);
                    mre.Reset();
                }
                firstSkip++;
            }
        }


        public async Task<(List<IThing>, List<IThing>)> Overlap(Vector3 pos, List<IThing> excpect, bool isStaticOnly)
        {
            var watch1 = System.Diagnostics.Stopwatch.StartNew();
            List<IThing> thingsAdd = new List<IThing>();
            List<IThing> thingsRemove = new List<IThing>();
            
            await WaitEndOfFrame();
            var hits = scene.Overlap(pos, overlapSphere);

            var remove = excpect.Where(e => !hits.Contains(e));
            var add = hits.Where(e => !excpect.Contains(e));

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
            IsDestoyed = true;
            Stop();
            scene.Cleanup();
        }

        public void Start()
        {
            workerThread = new Thread(new ThreadStart(StepPhysics));
            workerThread.IsBackground = false;
            workerThread.Priority = ThreadPriority.Highest;

            sceneThread = new Thread(new ThreadStart(StepScene));

            IsRunning = true;
            workerThread.Start();
            sceneThread.Start();


        }

        private void StepScene()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            lastScene = stopwatch.ElapsedTicks;
            while (IsRunning)
            {
                int sleep = 15;
                var elapsed = stopwatch.ElapsedTicks;
                var dtspan = TimeSpan.FromTicks(elapsed - lastScene);
                lastScene = elapsed;

                scene.Update((float)dtspan.TotalSeconds);
                var stop = stopwatch.ElapsedTicks;

                var sceneFrame = (float)TimeSpan.FromTicks(stop - lastScene).TotalMilliseconds;
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
            //var triangleMeshDesc = new TriangleMeshDesc()
            //{
            //    Flags = (MeshFlag)0,
            //    Triangles = modelData.Triangles,
            //    Points = modelData.Points
            //};

            //var cooking = scene.Physics.CreateCooking();

            //var stream = new MemoryStream();
            //_ = cooking.CookTriangleMesh(triangleMeshDesc, stream);

            //stream.Position = 0;

            //var triangleMesh = scene.Physics.CreateTriangleMesh(stream);

            //var geometry = new TriangleMeshGeometry(triangleMesh)
            //{
            //    Scale = new MeshScale((Vec3)new Vector3(1f, 1f, 1f), (Quat)Quaternion.identity)
            //};

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
           
            rigidActor.Position = pos;
            rigidActor.Rotation = rot;

            return rigidActor;
        }

        public IPhysicsDynamicObject CreateDynamic(IGeometry geometry, Vector3 pos, Quaternion rot)
        {
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

        public PhysXWorld()
        {
            TPS = (int)(1000 / 60f);
        }
    }



}
