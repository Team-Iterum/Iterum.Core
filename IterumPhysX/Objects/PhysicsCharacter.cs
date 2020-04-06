using Magistr.Math;
using Magistr.Things;
using System;

namespace Magistr.Physics.PhysXImpl
{
    public class PhysicsCharacter : IPhysicsCharacter
    {

        public readonly long Ref;

        private readonly Scene scene;
        private readonly IPhysicsAPI api;


        #region IPhysicsCharacter

        public Vector3 Direction { get; private set; }

        public float Speed { get; set; } = 0.1f;

        public float CharacterRotation { get; set; }

        public Vector3 FootPosition
        {
            get => (Vector3)(DVector3)api.getControllerFootPosition(Ref);
            set => api.setControllerFootPosition(Ref, value);
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
            get => (Vector3)(DVector3)api.getControllerPosition(Ref);
            set => api.setControllerPosition(Ref, value);
        }
        

        public Quaternion Rotation { get; set; }

        public bool IsDestroyed { get; private set; }

        public IPhysicsWorld OwnerWorld { get; }
        public IThing Thing { get; set; }

        #endregion

        internal PhysicsCharacter(Vector3 pos, Vector3 up, float height, float radius, Scene scene, IPhysicsWorld world, IPhysicsAPI api)
        {
            this.scene = scene;
            this.api = api;
            OwnerWorld = world;
            
            Ref = api.createCapsuleCharacter(this.scene.Ref, pos, up.normalized, height, radius, 0.05f);

            Move(Vector3.zero + world.Gravity);
        }

        public void Move(Vector3 direction)
        {
            Direction = direction;
            
            api.setControllerDirection(Ref, Direction);
        }

        public void Move(MoveDirection directions)
        {

            var rotation = Quaternion.Euler(0, CharacterRotation, 0);

            var forward = (rotation * Vector3.forward);
            var up = Vector3.up;
            var right = (rotation * Vector3.right);

            var moveDelta = Vector3.zero;

            if (directions.HasFlag(MoveDirection.Forward))
                moveDelta += forward;
            if (directions.HasFlag(MoveDirection.Backward))
                moveDelta += -forward;
            if (directions.HasFlag(MoveDirection.Left))
                moveDelta += -right;
            if (directions.HasFlag(MoveDirection.Right))
                moveDelta += right;
            if (directions.HasFlag(MoveDirection.Up))
                moveDelta += up;
            if (directions.HasFlag(MoveDirection.Down))
                moveDelta += -up;


            Direction = (System.Math.Abs(moveDelta.LengthSqr()) < 0.01f ?
                            Vector3.zero :
                            Vector3.Normalize(moveDelta)) * Speed + OwnerWorld.Gravity;

            api.setControllerDirection(Ref, Direction);

        }

    }
}
