using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Things;
using System.Threading.Tasks;
namespace Magistr.Physics.PhysXImplCore
{
    public class PhysXStaticObject : IPhysicsStaticObject
    {
        public long Ref { get; set; }

        private IPhysicsAPI api;
        private Scene scene;

        #region IPhysicsObject
        private Vector3 cachePosition;
        public Vector3 Position
        {
            get => GetPosition();
            set
            {
                if (!IsDestroyed)
                {
                    //Task.Run(async () =>
                    //{
                    //    await OwnerWorld.WaitEndOfFrame();
                        api.setRigidStaticPosition(Ref, value.ToApi());
                    //    cachePosition = value;
                    //}).ConfigureAwait(false);
                }
            }
        }

        private Quaternion cacheRotation;
        public Quaternion Rotation
        {
            get => GetRotation();
            set
            {
                if (!IsDestroyed)
                {
                    //Task.Run(async () =>
                    //{
                    //    await OwnerWorld.WaitEndOfFrame();
                        api.setRigidStaticRotation(Ref, value.ToQuat());
                    //    cacheRotation = value;
                    //}).ConfigureAwait(false);
                }
                
            }
        }
        public bool IsDestroyed { get; private set; }
        public IPhysicsWorld OwnerWorld { get; }
        public IThing Thing { get; set; }

        public void Destroy()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    scene.Destroy(this);
                }).ConfigureAwait(false);
            }

            IsDestroyed = true;

        }

        private Vector3 GetPosition()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    cachePosition = api.getRigidStaticPosition(Ref).ToVector3();
                }).ConfigureAwait(false);
            }
            return cachePosition;
        }
        private Quaternion GetRotation()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    cacheRotation = api.getRigidStaticRotation(Ref).ToQuat();
                }).ConfigureAwait(false);
            }
            return cacheRotation;
        }
        #endregion

        internal PhysXStaticObject(IGeometry geometry, Scene scene, IPhysicsWorld world, IPhysicsAPI api)
        {
            this.api = api;
            OwnerWorld = world;
            this.scene = scene;

            Ref = api.createRigidStatic((long)geometry.GetInternalGeometry(), scene.Ref, Vector3.zero.ToApi(), Quaternion.identity.ToQuat());
           
            
        }

    }
}