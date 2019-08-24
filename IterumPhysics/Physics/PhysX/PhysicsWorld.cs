using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Things;
using Nito.AsyncEx;
using PhysX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quat = System.Numerics.Quaternion;
using Vec3 = System.Numerics.Vector3;

namespace Magistr.Physics.PhysXImpl
{
    public class PhysXWorld : IPhysicsWorld
    {
        private readonly ConsoleErrorCallback ErrorCallback = new ConsoleErrorCallback();
        private static Foundation Foundation;
        private static PhysX.Physics Physics;
        private static bool PhysXCreated = false;

        private async Task<Scene> SceneAsync()
        {
            await WaitEndOfFrame();
            return scene;
        }
        private Scene scene;
        private Scene Scene => SceneAsync().Result;

        private Material Material;

        private bool destroyed = false;
        private bool created = false;
        private bool running = false;
        private Thread workerThread;
        private long last;

        public bool IsDestoyed => destroyed;
        public bool IsCreated => created;
        public bool IsRunning => running;
        private float sceneFrame;
        public float SceneFrame { get => sceneFrame; }
        private float dt;
        public int Timestamp { get; set; }

        public float DeltaTime { get => dt; }
        public int TPS { get; set; }
        private System.Guid guid = System.Guid.NewGuid();
        public System.Guid WorldUid { get => guid; }

        public Vector3 Gravity { get; private set; }

        private SphereGeometry overlapSphere;
        private float overlapSphereRadius = 500;
        public float OverlapSphereRadius
        {
            get => overlapSphereRadius;
            set
            {
                overlapSphereRadius = value;
                Task.Run(async () =>
                {
                    await WaitEndOfFrame();
                    overlapSphere = new SphereGeometry(OverlapSphereRadius);
                }).ConfigureAwait(false);
            }
        }

        private void InitPhysics()
        {
            if (!PhysXCreated)
            {
                
                Foundation = new Foundation(ErrorCallback);
                var pvd = new PhysX.VisualDebugger.Pvd(Foundation);
                Physics = new PhysX.Physics(Foundation, false, pvd);
                PhysXCreated = true;
                pvd.Connect("127.0.0.1");
            }

        }
        private void InitScene()
        {

            SceneDesc CreateSceneDesc(Foundation foundation)
            {
#if GPU
                var cudaContext = new CudaContextManager(foundation);
#endif

                var sceneDesc = new SceneDesc
                {
                    Gravity = (Vec3)new Vector3(0, -9.81f, 0),
#if GPU
				    GpuDispatcher = cudaContext.GpuDispatcher,
#endif
                    FilterShader = new SampleFilterShader()
                };

#if GPU
			    sceneDesc.Flags |= SceneFlag.EnableGpuDynamics;
			    sceneDesc.BroadPhaseType |= BroadPhaseType.Gpu;
#endif

                return sceneDesc;
            }
            
            scene = Physics.CreateScene(CreateSceneDesc(Foundation));
            scene.SetVisualizationParameter(VisualizationParameter.Scale, 1.0f);
            scene.SetVisualizationParameter(VisualizationParameter.CollisionShapes, true);
            scene.SetVisualizationParameter(VisualizationParameter.JointLocalFrames, true);
            scene.SetVisualizationParameter(VisualizationParameter.JointLimits, true);
            scene.SetVisualizationParameter(VisualizationParameter.ActorAxes, true);
            Gravity = (Vector3)scene.Gravity;
            Material = Physics.CreateMaterial(0.1f, 0.5f, 0.5f);

            overlapSphere = new SphereGeometry(OverlapSphereRadius);


        }
        private AsyncManualResetEvent mre = new AsyncManualResetEvent(true);
        private ControllerManager _controllerManager;

        internal List<PhysXCharacter> Characters { get; private set; } = new List<PhysXCharacter>();

        private void StepPhysics()
        {
            mre.Reset();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            last = stopwatch.ElapsedMilliseconds;
            while (running)
            {
                mre.Reset();
                int sleep = TPS;

                var elapsed = stopwatch.ElapsedMilliseconds;
                var dt = elapsed - last;
                last = stopwatch.ElapsedMilliseconds;

                this.dt = dt;
                this.Timestamp = scene.Timestamp;
                if (dt > 0)
                {
                    scene.Simulate(dt / 1000f); // in seconds
                    scene.FetchResults(true);
                }
                mre.Set();
                var stop1 = stopwatch.ElapsedMilliseconds;
                foreach (var item in Characters)
                {
                    item.Update();
                }
                var stop = stopwatch.ElapsedMilliseconds;
                this.sceneFrame = stop - last;
                sleep -= (int)this.sceneFrame;
                if (sleep < 0) sleep = 1;
                Thread.Sleep(sleep);
            }
        }
        

        public async Task<(List<IThing>, List<IThing>)> Overlap(Vector3 pos, List<IThing> excpect, bool isStaticOnly)
        {
            var watch1 = Stopwatch.StartNew();
            List<IThing> thingsAdd = new List<IThing>();
            List<IThing> thingsRemove = new List<IThing>();
            
            OverlapHit[] overlaphits = null;
            await WaitEndOfFrame();
            var watch = Stopwatch.StartNew();
            var result = scene.Overlap(overlapSphere, System.Numerics.Matrix4x4.CreateTranslation((Vec3)pos), 1000, (hit) =>
            {
                overlaphits = hit;
                return true;
            });
            watch.Stop();
            Log.Debug.Log($"[var result = scene.Overlap(]={watch.ElapsedMilliseconds}ms", ConsoleColor.Gray);

            var hits = overlaphits.Select(e => ((IPhysicsObject)e.Actor.UserData).Thing);
            var remove = excpect.Where(e => !hits.Contains(e));
            var add = hits.Where(e => !excpect.Contains(e));

            thingsRemove.AddRange(remove);
            thingsAdd.AddRange(add);
            watch1.Stop();
            Log.Debug.Log($"[Overlap(]={watch.ElapsedMilliseconds}ms", ConsoleColor.Gray);
            return (thingsAdd, thingsRemove);
        }

        private void CleanupPhysics()
        {
            scene.Dispose();
        }

        public void Create()
        {
            InitPhysics();
            InitScene();
            created = true;
        }

        public void Destroy()
        {
            destroyed = true;
            Stop();
            CleanupPhysics();
        }

        public void Start()
        {
            workerThread = new Thread(new ThreadStart(StepPhysics));
            workerThread.IsBackground = false;
            workerThread.Priority = ThreadPriority.Highest;
            running = true;
            workerThread.Start();


        }

        public void Stop()
        {
            workerThread = null;
            running = false;
        }

        public IGeometry CreateStaticModelGeometry(IModelData modelData)
        {
            var triangleMeshDesc = new TriangleMeshDesc()
            {
                Flags = (MeshFlag)0,
                Triangles = modelData.Triangles,
                Points = modelData.Points
            };

            var cooking = scene.Physics.CreateCooking();

            var stream = new MemoryStream();
            _ = cooking.CookTriangleMesh(triangleMeshDesc, stream);

            stream.Position = 0;

            var triangleMesh = scene.Physics.CreateTriangleMesh(stream);

            var geometry = new TriangleMeshGeometry(triangleMesh)
            {
                Scale = new MeshScale((Vec3)new Vector3(1f, 1f, 1f), (Quat)Quaternion.identity)
            };

            return new PhysXGeometry() { geometry = geometry };
        }


        public IGeometry CreateSphereGeometry(float radius)
        {
            return new PhysXGeometry() { geometry = new SphereGeometry(radius) };
        }

        public IGeometry CreateBoxGeometry(Vector3 size)
        {
            return new PhysXGeometry() { geometry = new BoxGeometry((Vec3)size / 2) };
        }

        public IPhysicsStaticObject CreateStatic(IGeometry geometry, Vector3 pos, Quaternion rot)
        {
            var rigidActor = scene.Physics.CreateRigidStatic();
            RigidActorExt.CreateExclusiveShape(rigidActor, (Geometry)geometry.GetInternalGeometry(), Material);

            rigidActor.GlobalPosePosition = (Vec3)pos;
            rigidActor.GlobalPoseQuat = (Quat)rot;

            scene.AddActor(rigidActor);
            var staticObj = new PhysXStaticObject(rigidActor, this);
            rigidActor.UserData = staticObj;
            return staticObj;
        }

        public IPhysicsDynamicObject CreateDynamic(IGeometry geometry, Vector3 pos, Quaternion rot)
        {
            var rigidActor = scene.Physics.CreateRigidDynamic();
            RigidActorExt.CreateExclusiveShape(rigidActor, (Geometry)geometry.GetInternalGeometry(), Material);

            rigidActor.GlobalPosePosition = (Vec3)pos;
            rigidActor.GlobalPoseQuat = (Quat)rot;
            //rigidActor.UserData = controller;
            scene.AddActor(rigidActor);

            return default;
        }

        public IPhysicsCharaceter CreateCapsuleCharacter(Vector3 pos, Vector3 up, float height = 4, float radius = 1)
        {
            if (_controllerManager == null)
                _controllerManager = scene.CreateControllerManager();

            var desc = new CapsuleControllerDesc()
            {
                Height = height,
                Radius = radius,
                Material = Material,
                UpDirection = (Vec3)up,
                Position = (Vec3)pos,
                ReportCallback = new ControllerHitReport()
            };

            var controller = _controllerManager.CreateController<CapsuleController>(desc);
            
            var _char = new PhysXCharacter(controller, this);
            controller.Actor.UserData = _char;
            Characters.Add(_char);
            return _char;
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
