using Magistr.Framework.Physics;
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
            set => api.setRigidDynamicPosition(Ref, value);
        }

        public Quaternion Rotation
        {
            get
            {  
                // workaround threading bug on disconnect
                if(IsDestroyed) return Quaternion.identity;
                return api.getRigidDynamicRotation(Ref);
            }
            set => api.setRigidDynamicRotation(Ref, value);
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

        public void SetKinematicTarget(Vector3 position, Quaternion rotation)
        {
            api.setRigidDynamicKinematicTarget(Ref, position, rotation);
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