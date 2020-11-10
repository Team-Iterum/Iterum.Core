using System.Collections.Generic;
using System.Linq;
using Iterum.Math;
using Iterum.Things;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl
{
    public class DynamicObject : IDynamicObject
    {
        public long Ref { get; }
        
        private readonly Scene scene;
        private bool disabledSimulation;
        private uint word;

        #region IPhysicsObject

        public IThing Thing { get; set; }
        
        public APITrans Transform
        {
            get => API.getRigidDynamicTransform(Ref);
            set => API.setRigidDynamicTransform(Ref, value);
        }
        public Vector3 Position
        {
            get => API.getRigidDynamicTransform(Ref).p;
            set => API.setRigidDynamicTransform(Ref, new APITrans(value, Rotation));
        }

        public Quaternion Rotation
        {
            get => API.getRigidDynamicTransform(Ref).q;
            set => API.setRigidDynamicTransform(Ref, new APITrans(Position, value));
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

        public void SetKinematicTarget(Vector3 position, Quaternion rotation) =>
            API.setRigidDynamicKinematicTarget(Ref, new APITrans(position, rotation));
        
        public void SetKinematicTarget(APITrans transform) => API.setRigidDynamicKinematicTarget(Ref, transform);
        

        public void AddForce(Vector3 force, ForceMode mode) => API.addRigidDynamicForce(Ref, force, mode);
        public void AddTorque(Vector3 torque, ForceMode mode) => API.addRigidDynamicTorque(Ref, torque, mode);

        public bool DisabledSimulation
        {
            set
            {
                if (disabledSimulation != value)
                {
                    API.setRigidDynamicDisable(Ref, value);
                    disabledSimulation = value;
                }
            }
            get => disabledSimulation;
        }

        public uint Word
        {
            set
            {
                if (word != value)
                {
                    API.setRigidDynamicWord(Ref, value);
                    word = value;
                }
            }
            get => word;
        }
        
        public void Destroy()
        {
            if (IsDestroyed) return;
#if PHYSICS_DEBUG_LEVEL
            Console.WriteLine($"DynamicObject ({Ref})", $"Destroy invoked...");
#endif
            scene.Destroy(this);
            IsDestroyed = true;

        }
        #endregion

        internal DynamicObject(IReadOnlyList<IGeometry> geometries, PhysicsObjectFlags flags, float mass, uint word, Vector3 pos, Quaternion quat, Scene scene)
        {
            this.scene = scene;
            this.word = word;
            
            Ref = API.createRigidDynamic((int) geometries[0].GeoType, 
                geometries.Count, 
                geometries.Select(e=> (long)e.GetInternalGeometry()).ToArray(), scene.Ref, 
                flags.HasFlag(PhysicsObjectFlags.Kinematic), 
                flags.HasFlag(PhysicsObjectFlags.CCD), 
                flags.HasFlag(PhysicsObjectFlags.Retain), 
                flags.HasFlag(PhysicsObjectFlags.DisableGravity), 
                flags.HasFlag(PhysicsObjectFlags.Trigger), 
                mass, 
                word,
                pos, quat);
            
            Transform = Transform;
            DisabledSimulation = DisabledSimulation;
            Word = Word;
        }

    }
}