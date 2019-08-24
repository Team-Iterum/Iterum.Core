using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Physics.PhysXImplCore;
using Magistr.Things;
using System.Threading.Tasks;
using Quat = System.Numerics.Quaternion;
using Vec3 = System.Numerics.Vector3;
namespace Magistr.Physics.PhysXImplCore
{
    public class PhysXStaticObject : IPhysicsStaticObject
    {
        public int Index => _px;

        private IPhysicsAPI API;
        #region IPhysicsObject
        private int _px;
        private Scene Scene;
        internal int UserDataReference;
        private Vector3 cachePosition;
        public Vector3 Position
        {
            get => getPosition();
            set
            {
                if (!IsDestroyed)
                {
                    Task.Run(async () =>
                    {
                        await OwnerWorld.WaitEndOfFrame();
                        API.setRigidStaticPosition(_px, Scene.Index, value.ToApi());
                        cachePosition = value;
                    }).ConfigureAwait(false);
                }
            }
        }
        private Quaternion cacheRotation;
        

        public Quaternion Rotation
        {
            get => getRotation();
            set
            {
                if (!IsDestroyed)
                {
                    Task.Run(async () =>
                    {
                        await OwnerWorld.WaitEndOfFrame();
                        API.setRigidStaticRotation(_px, Scene.Index, value.ToQuat());
                        cacheRotation = value;
                    }).ConfigureAwait(false);
                }
                
            }
        }
        public bool IsDestroyed { get; private set; } = false;
        public IPhysicsWorld OwnerWorld { get; }
        public IThing Thing { get; set; }

        public void Destroy()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    Scene.Destroy(this);
                }).ConfigureAwait(false);
            }

            IsDestroyed = true;

        }

        private Vector3 getPosition()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    cachePosition = (Vector3)API.getRigidStaticPosition(_px, Scene.Index).ToVector3();
                }).ConfigureAwait(false);
            }
            return cachePosition;
        }
        private Quaternion getRotation()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    cacheRotation = (Quaternion)API.getRigidStaticRotation(_px, Scene.Index).ToQuat();
                }).ConfigureAwait(false);
            }
            return cacheRotation;
        }
        #endregion

        internal PhysXStaticObject(IGeometry geometry, int userDataReference, Scene scene, IPhysicsWorld w, IPhysicsAPI api)
        {
            API = api;
            UserDataReference = userDataReference;
            this._px = API.createRigidStatic((int)geometry.GetInternalGeometry(), userDataReference, scene.Index, Vector3.zero.ToApi(), Quaternion.identity.ToQuat());
            this.Scene = scene;
            this.OwnerWorld = w;
        }

    }
}