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
        private IPhysicsWorld world;

        #region IPhysicsObject

        public Vector3 Position
        {
            get => api.getRigidStaticPosition(Ref).ToVector3();
            set => api.setRigidStaticPosition(Ref, value.ToApi());
        }

        public Quaternion Rotation
        {
            get => api.getRigidStaticRotation(Ref).ToQuat();
            set => api.setRigidStaticRotation(Ref, value.ToQuat());
        }
        public bool IsDestroyed { get; private set; }

        public IThing Thing { get; set; }

        public void Destroy()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await world.WaitEndOfFrame();
                    scene.Destroy(this);
                }).ConfigureAwait(false);
            }

            IsDestroyed = true;

        }
        #endregion

        internal PhysXStaticObject(IGeometry geometry, Scene scene, IPhysicsWorld world, IPhysicsAPI api)
        {
            this.api = api;
            this.world = world;
            this.scene = scene;

            Ref = api.createRigidStatic((int) geometry.GeoType, (long)geometry.GetInternalGeometry(), scene.Ref, Vector3.zero.ToApi(), Quaternion.identity.ToQuat());
        }

    }
}