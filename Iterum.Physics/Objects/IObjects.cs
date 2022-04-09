using System.Numerics;

namespace Iterum.Physics;

public enum ForceMode
{
    // ReSharper disable InconsistentNaming
        
    /// <summary>
    /// parameter has unit of mass * distance/ time^2, i.e. a force
    /// </summary>
        
    eFORCE,
    /// <summary>
    /// parameter has unit of mass * distance /time
    /// </summary>
    eIMPULSE,		 
    /// <summary>
    /// parameter has unit of distance / time, i.e. the effect is mass independent: a velocity change.
    /// </summary>
    eVELOCITY_CHANGE, 
    /// <summary>
    /// parameter has unit of distance/ time^2, i.e. an acceleration. It gets treated just like a force except the mass is not divided out before integration.
    /// </summary>
    eACCELERATION
        
    // ReSharper restore InconsistentNaming
};
    
public interface IDynamicObject : IPhysicsObject
{
    Vector3 LinearVelocity { get; set; }
    Vector3 AngularVelocity { get; set; }
    float MaxLinearVelocity { get; set; }
    float MaxAngularVelocity { get; set; }

    float LinearDamping { set; }
    float AngularDamping { set; }

    void SetKinematicTarget(Vector3 position, Quaternion rotation);

    void AddForce(Vector3 force, ForceMode mode);
    void AddTorque(Vector3 torque, ForceMode mode);
}