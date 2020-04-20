using Iterum.Math;
using Iterum.Things;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl
{
    public class PhysicsCharacter : IPhysicsCharacter
    {

        public readonly long Ref;

        private readonly Scene scene;
        public float JumpHeight { get; } = 11f;

        #region IPhysicsCharacter

        public Vector3 Direction { get; private set; }
        public float Speed { get; set; } = 0.1f;
        public float CharacterRotation { get; set; }

        public Vector3 FootPosition
        {
            get => (Vector3)(DVector3)API.getControllerFootPosition(Ref);
            set => API.setControllerFootPosition(Ref, value);
        }

        public void Destroy()
        {
            scene.Destroy(this);
            IsDestroyed = true;
        }

        #endregion

        #region IPhysicsObject
        
        public Vector3 Position
        {
            get => (Vector3)(DVector3)API.getControllerPosition(Ref);
            set => API.setControllerPosition(Ref, value);
        }
        
        public Quaternion Rotation { get; set; }
        public bool IsDestroyed { get; private set; }
        
        public IThing Thing { get; set; }

        #endregion

        internal PhysicsCharacter(Vector3 pos, Vector3 up, float height, float radius, Scene scene)
        {
            this.scene = scene;

            Ref = API.createCapsuleCharacter(this.scene.Ref, pos, up.normalized, height, radius, 0.05f);

            Move(Vector3.zero + scene.Gravity);
        }

        public void Move(Vector3 direction)
        {
            Direction = direction;
            
            API.setControllerDirection(Ref, Direction);
        }

        public void Move(MoveDirection dirs)
        {

            var rotation = Quaternion.Euler(0, CharacterRotation, 0);

            var forward = (rotation * Vector3.forward);
            var up = Vector3.up;
            var right = (rotation * Vector3.right);

            var moveDelta = Vector3.zero;

            if (dirs.HasFlag(MoveDirection.Forward)) moveDelta += forward;
            else if (dirs.HasFlag(MoveDirection.Backward)) moveDelta += -forward;
            
            if (dirs.HasFlag(MoveDirection.Left)) moveDelta += -right;
            else if (dirs.HasFlag(MoveDirection.Right)) moveDelta += right;
            
            if (dirs.HasFlag(MoveDirection.Up)) moveDelta += up * scene.Gravity.magnitude * JumpHeight;
            else if (dirs.HasFlag(MoveDirection.Down)) moveDelta += -up;


            Direction = moveDelta * Speed + scene.Gravity;

            API.setControllerDirection(Ref, Direction);

        }

    }
}
