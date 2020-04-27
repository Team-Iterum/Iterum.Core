using System;
using Iterum.Log;
using Iterum.Math;
using Iterum.Things;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl
{
    public class DynamicObject : IDynamicObject
    {
        public long Ref { get; }
        
        private readonly Scene scene;

        #region IPhysicsObject

        public Vector3 Position
        {
            get => API.getRigidDynamicPosition(Ref);
            set => API.setRigidDynamicTransform(Ref, value, Rotation);
        }

        public Quaternion Rotation
        {
            get => API.getRigidDynamicRotation(Ref);
            set => API.setRigidDynamicTransform(Ref, Position, value);
        }


        public bool IsDestroyed { get; private set; }

        public float MaxLinearVelocity
        {
            get => API.getRigidDynamicMaxLinearVelocity(Ref);
            set => API.setRigidDynamicMaxLinearVelocity(Ref, value);
        }


        public Vector3 LinearVelocity
        {
            get => API.getRigidDynamicLinearVelocity(Ref);
            set => API.setRigidDynamicLinearVelocity(Ref, value);
        }

        public float MaxAngularVelocity
        {
            get => API.getRigidDynamicMaxAngularVelocity(Ref);
            set => API.setRigidDynamicMaxAngularVelocity(Ref, value);
        }


        public Vector3 AngularVelocity
        {
            get => API.getRigidDynamicAngularVelocity(Ref);
            set => API.setRigidDynamicAngularVelocity(Ref, value);
        }

        public float LinearDamping
        {
            set => API.setRigidDynamicLinearDamping(Ref, value);
        }

        public float AngularDamping
        {
            set => API.setRigidDynamicAngularDamping(Ref, value);
        }

        public void SetKinematicTarget(Vector3 position, Quaternion rotation)
        {
            API.setRigidDynamicKinematicTarget(Ref, position, rotation);
        }
        
        public void SetKinematicTarget(Transform transform)
        {
            API.setRigidDynamicKinematicTarget(Ref, transform.Position, transform.Rotation);
        }

        public void AddForce(Vector3 force, ForceMode mode)
        {
            API.addRigidDynamicForce(Ref, force, mode);
        }

        public void AddTorque(Vector3 torque, ForceMode mode)
        {
            API.addRigidDynamicTorque(Ref, torque, mode);
        }

        public IThing Thing { get; set; }

        public void Destroy()
        {
            Debug.LogV($"DynamicObject ({Ref})", $"Destroy invoked...", ConsoleColor.DarkRed);
            
            scene.Destroy(this);
            IsDestroyed = true;

        }
        #endregion

        internal DynamicObject(IGeometry geometry, PhysicsObjectFlags flags, float mass, Transform transform, Scene scene)
        {
            this.scene = scene;

            Ref = API.createRigidDynamic((int) geometry.GeoType, (long)geometry.GetInternalGeometry(), scene.Ref, 
                flags.HasFlag(PhysicsObjectFlags.Kinematic), 
                flags.HasFlag(PhysicsObjectFlags.CCD), 
                flags.HasFlag(PhysicsObjectFlags.Retain), 
                flags.HasFlag(PhysicsObjectFlags.DisableGravity), 
                flags.HasFlag(PhysicsObjectFlags.Trigger), 
                mass, 
                transform.Position, transform.Rotation);
        }


    }
}