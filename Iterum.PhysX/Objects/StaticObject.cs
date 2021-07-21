using Iterum.Math;
using Iterum.Things;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl
{
    internal class StaticObject : IStaticObject
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
            if (IsDestroyed) return;
            
            scene.Destroy(this);
            IsDestroyed = true;

        }
        #endregion

        internal StaticObject(IGeometry geometry, IMaterial mat, PhysicsObjectFlags flags,Vector3 pos, Quaternion quat, Scene scene)
        {
            this.scene = scene;

            Ref = API.createRigidStatic((int) geometry.GeoType, (long) geometry.GetInternal(),
                scene.Ref, (long) mat.GetInternal(),
                pos, quat, flags.HasFlag(PhysicsObjectFlags.Trigger));
        }

    }
}