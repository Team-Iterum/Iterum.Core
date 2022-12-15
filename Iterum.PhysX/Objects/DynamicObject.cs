using System.Collections.Generic;
using System.Linq;
using Iterum.Things;
using Mono.DllMap.Extensions;
using UnityEngine;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl;

public class DynamicObject : IDynamicObject
{
    public long Ref { get; }

    private readonly Scene scene;
    private bool disabledSimulation;
    private uint word;

    #region IPhysicsObject

    public IThing Thing
    {
        get => UserData as IThing;
        set => UserData = value;
    }

    public object UserData { get; set; }

    public APITrans Transform
    {
        get => API.getRigidDynamicTransform(scene.Ref, Ref);
        set => API.setRigidDynamicTransform(scene.Ref, Ref, value);
    }

    public Vector3 Position
    {
        get => API.getRigidDynamicTransform(scene.Ref, Ref).p;
        set => API.setRigidDynamicTransform(scene.Ref, Ref, new APITrans(value, Rotation));
    }

    public Quaternion Rotation
    {
        get => API.getRigidDynamicTransform(scene.Ref, Ref).q;
        set => API.setRigidDynamicTransform(scene.Ref, Ref, new APITrans(Position, value));
    }

    public bool IsDestroyed { get; private set; }

    public float MaxLinearVelocity
    {
        get => API.getRigidDynamicMaxLinearVelocity(scene.Ref, Ref);
        set => API.setRigidDynamicMaxLinearVelocity(scene.Ref, Ref, value);
    }

    public Vector3 LinearVelocity
    {
        get => API.getRigidDynamicLinearVelocity(scene.Ref, Ref);
        set => API.setRigidDynamicLinearVelocity(scene.Ref, Ref, value);
    }

    public float MaxAngularVelocity
    {
        get => API.getRigidDynamicMaxAngularVelocity(scene.Ref, Ref);
        set => API.setRigidDynamicMaxAngularVelocity(scene.Ref, Ref, value);
    }

    public Vector3 AngularVelocity
    {
        get => API.getRigidDynamicAngularVelocity(scene.Ref, Ref);
        set => API.setRigidDynamicAngularVelocity(scene.Ref, Ref, value);
    }

    public float LinearDamping
    {
        set => API.setRigidDynamicLinearDamping(scene.Ref, Ref, value);
    }

    public float AngularDamping
    {
        set => API.setRigidDynamicAngularDamping(scene.Ref, Ref, value);
    }

    public void SetKinematicTarget(Vector3 position, Quaternion rotation) =>
        API.setRigidDynamicKinematicTarget(scene.Ref, Ref, new APITrans(position, rotation));

    public void SetKinematicTarget(APITrans transform) => API.setRigidDynamicKinematicTarget(scene.Ref, Ref, transform);
    
    public void SetLockFlags(DynamicLockFlag lockFlag, bool value) => API.setRigidDynamicLockFlag(scene.Ref, Ref, lockFlag, value);


    public void AddForce(Vector3 force, ForceMode mode) => API.addRigidDynamicForce(scene.Ref, Ref, force, mode);
    public void AddTorque(Vector3 torque, ForceMode mode) => API.addRigidDynamicTorque(scene.Ref, Ref, torque, mode);

    public bool DisabledSimulation
    {
        set
        {
            if (disabledSimulation != value)
            {
                API.setRigidDynamicDisable(scene.Ref, Ref, value);
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
                API.setRigidDynamicWord(scene.Ref, Ref, value);
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

    internal DynamicObject(IReadOnlyList<IGeometry> geometries, IMaterial mat, PhysicsObjectFlags flags, float mass,
        uint word, Vector3 pos, Quaternion quat, Scene scene)
    {
        this.scene = scene;
        this.word = word;
        
        var array = new long[geometries.Count];
        for (int i = 0; i < geometries.Count; i++)
        {
            array[i] = geometries[i].GetInternal();
        }

        Ref = API.createRigidDynamic((int)geometries[0].GeoType,
            geometries.Count,
            array,
            scene.Ref,
            mat.GetInternal(),
            flags.HasFlagFast(PhysicsObjectFlags.Kinematic),
            flags.HasFlagFast(PhysicsObjectFlags.CCD),
            flags.HasFlagFast(PhysicsObjectFlags.Retain),
            flags.HasFlagFast(PhysicsObjectFlags.DisableGravity),
            flags.HasFlagFast(PhysicsObjectFlags.Trigger),
            mass,
            word,
            pos, quat);

    }

}