﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public void SetKinematicTarget(Transform transform) =>
            API.setRigidDynamicKinematicTarget(Ref, new APITrans(transform.Position, transform.Rotation));

        public void AddForce(Vector3 force, ForceMode mode) => API.addRigidDynamicForce(Ref, force, mode);
        public void AddTorque(Vector3 torque, ForceMode mode) => API.addRigidDynamicTorque(Ref, torque, mode);
        
        public void SetEnabled(bool enabled) => API.setRigidDynamicDisable(Ref, !enabled);
        public void SetWord(uint word) => API.setRigidDynamicWord(Ref, word);
        
        public void Destroy()
        {
            if (IsDestroyed) return;
            
            if(ExtendedVerbose) Debug.LogV($"DynamicObject ({Ref})", $"Destroy invoked...", ConsoleColor.DarkRed);
            
            scene.Destroy(this);
            IsDestroyed = true;

        }
        #endregion

        internal DynamicObject(IReadOnlyList<IGeometry> geometries, PhysicsObjectFlags flags, float mass, uint word, Transform transform, Scene scene)
        {
            this.scene = scene;
            
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
                transform.Position, transform.Rotation);
            
            var trans = Transform;
            Transform = trans;
        }


    }
}