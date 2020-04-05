using Magistr.Math;
using Magistr.Things;
using System;

namespace Magistr.Physics
{
    public interface IPhysicsObject
    {
        void Destroy();
        Vector3 Position { get; set; }
        IThing Thing { get; set; }
        Quaternion Rotation { get; set; }
        bool IsDestroyed { get; }
    }

    public interface IStaticObject : IPhysicsObject
    {

    }

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
    public enum ForceMode
    {
        eFORCE,				//!< parameter has unit of mass * distance/ time^2, i.e. a force
        eIMPULSE,			//!< parameter has unit of mass * distance /time
        eVELOCITY_CHANGE,	//!< parameter has unit of distance / time, i.e. the effect is mass independent: a velocity change.
        eACCELERATION		//!< parameter has unit of distance/ time^2, i.e. an acceleration. It gets treated just like a force except the mass is not divided out before integration.
    };

    [Flags]
    public enum MoveDirection
    {
        None = 0,
        Forward = 1 << 0,
        Backward = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Up = 1 << 4,
        Down = 1 << 5
    }

    public interface IPhysicsCharacter : IPhysicsObject
    {
        Vector3 FootPosition { get; set; }
        
        float Speed { get; set; }
        Vector3 Direction { get; }
        float CharacterRotation { get; set; }
        void Move(MoveDirection directions);
        public void Move(Vector3 direction);
    }
}
