using Iterum.Math;
using Iterum.Things;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl
{
    public class StaticObject : IStaticObject
    {
        public long Ref { get; }
        
        private readonly Scene scene;

        #region IPhysicsObject

        public Vector3 Position
        {
            get => API.getRigidStaticPosition(Ref);
            set => API.setRigidStaticPosition(Ref, value);
        }

        public Quaternion Rotation
        {
            get => API.getRigidStaticRotation(Ref);
            set => API.setRigidStaticRotation(Ref, value);
        }
        public bool IsDestroyed { get; private set; }

        public IThing Thing { get; set; }

        public void Destroy()
        {
            scene.Destroy(this);
            IsDestroyed = true;

        }
        #endregion

        public StaticObject(IGeometry geometry, PhysicsObjectFlags flags, Transform transform, Scene scene)
        {
            this.scene = scene;

            Ref = API.createRigidStatic((int) geometry.GeoType, (long)geometry.GetInternalGeometry(), scene.Ref, 
                transform.Position, transform.Rotation, flags.HasFlag(PhysicsObjectFlags.Trigger));
        }

    }
}