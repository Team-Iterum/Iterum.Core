using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Things;


namespace Magistr.Physics.PhysXImplCore
{
    public class PhysXDynamicObject : IPhysicsDynamicObject
    {
        public long Ref { get; set; }

        private IPhysicsAPI api;
        private Scene scene;

        #region IPhysicsObject

        public Vector3 Position
        {
            get => api.getRigidDynamicPosition(Ref).ToVector3();
            set => api.setRigidDynamicPosition(Ref, value.ToApi());
        }

        public Quaternion Rotation
        {
            get => api.getRigidDynamicRotation(Ref).ToQuat();
            set => api.setRigidDynamicRotation(Ref, value.ToQuat());
        }


        public bool IsDestroyed { get; private set; }

        private float maxLinearVelocity;
        public float MaxLinearVelocity
        {
            get => maxLinearVelocity;
            set
            {
                maxLinearVelocity = value;
                api.setRigidDynamicMaxLinearVelocity(Ref, value);
            }
        }


        private Vector3 linearVelocity;
        public Vector3 LinearVelocity
        {
            get => linearVelocity;
            set
            {
                linearVelocity = value;
                api.setRigidDynamicLinearVelocity(Ref, value.ToApi());
            }
        }

        private float maxAngularVelocity;
        public float MaxAngularVelocity
        {
            get => maxAngularVelocity;
            set
            {
                maxAngularVelocity = value;
                api.setRigidDynamicMaxAngularVelocity(Ref, value);
            }
        }
        public void SetKinematicTarget(Vector3 position, Quaternion rotation)
        {
            api.setRigidDynamicKinematicTarget(Ref, position.ToApi(), rotation.ToQuat());
        }

        public IThing Thing { get; set; }

        public void Destroy()
        { 
            scene.Destroy(this);
    
            IsDestroyed = true;

        }
        #endregion

        internal PhysXDynamicObject(IGeometry geometry, bool kinematic, float mass, Scene scene, IPhysicsAPI api)
        {
            this.api = api;
            this.scene = scene;

            Ref = api.createRigidDynamic((int) geometry.GeoType,(long)geometry.GetInternalGeometry(), scene.Ref, kinematic, mass, Vector3.zero.ToApi(), Quaternion.identity.ToQuat());
        }


    }
}