using Magistr.Framework.Physics;
using Magistr.Log;
using Magistr.Math;
using Magistr.Things;


namespace Magistr.Physics.PhysXImplCore
{
    public class DynamicObject : IPhysicsDynamicObject
    {
        public long Ref { get; set; }

        private IPhysicsAPI api;
        private Scene scene;

        #region IPhysicsObject

        public Vector3 Position
        {
            get => api.getRigidDynamicPosition(Ref);
            set => api.setRigidDynamicTransform(Ref, value, Rotation);
        }

        public Quaternion Rotation
        {
            get => api.getRigidDynamicRotation(Ref);
            set => api.setRigidDynamicTransform(Ref, Position, value);
        }


        public bool IsDestroyed { get; private set; }

        public float MaxLinearVelocity
        {
            get => api.getRigidDynamicMaxLinearVelocity(Ref);
            set => api.setRigidDynamicMaxLinearVelocity(Ref, value);
        }


        public Vector3 LinearVelocity
        {
            get => api.getRigidDynamicLinearVelocity(Ref);
            set => api.setRigidDynamicLinearVelocity(Ref, value);
        }

        public float MaxAngularVelocity
        {
            get => api.getRigidDynamicMaxAngularVelocity(Ref);
            set => api.setRigidDynamicMaxAngularVelocity(Ref, value);
        }


        public Vector3 AngularVelocity
        {
            get => api.getRigidDynamicAngularVelocity(Ref);
            set => api.setRigidDynamicAngularVelocity(Ref, value);
        }

        public float LinearDamping
        {
            set => api.setRigidDynamicLinearDamping(Ref, value);
        }

        public float AngularDamping
        {
            set => api.setRigidDynamicAngularDamping(Ref, value);
        }

        public void SetKinematicTarget(Vector3 position, Quaternion rotation)
        {
            api.setRigidDynamicKinematicTarget(Ref, position, rotation);
        }

        public void AddForce(Vector3 force)
        {
            api.addRigidDynamicForce(Ref, force);
        }

        public void AddTorque(Vector3 torque)
        {
            api.addRigidDynamicTorque(Ref, torque);
        }

        public IThing Thing { get; set; }

        public void Destroy()
        { 
            scene.Destroy(this);
    
            IsDestroyed = true;

        }
        #endregion

        internal DynamicObject(IGeometry geometry, bool kinematic, float mass, Scene scene, IPhysicsAPI api)
        {
            this.api = api;
            this.scene = scene;

            Ref = api.createRigidDynamic((int) geometry.GeoType,(long)geometry.GetInternalGeometry(), scene.Ref, kinematic, mass, Vector3.zero, Quaternion.identity);
        }


    }
}