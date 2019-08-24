using Magistr.Math;
using Magistr.Things;
using PhysX;
using System;
using System.Threading.Tasks;
using Mat4x4 = System.Numerics.Matrix4x4;
using Vec3 = System.Numerics.Vector3;

namespace Magistr.Physics.PhysXImpl
{
    public class PhysXCharacter : IPhysicsCharaceter
    {
        #region IPhysicsCharacter
        public Vector3 Direction { get; set; }
        public float Speed { get; set; } = 0.1f;

        public float CharacterRotation { get; set; }
        private Vector3 cacheFootPosition;
        public Vector3 FootPosition { get => (Vector3)getFootPosition(); set => px.FootPosition = (Vec3)value; }

        public void Destroy()
        {
            ((PhysXWorld)world).Characters.Remove(this);
            var actor = Px().Result;
            actor.Dispose();
            destroyed = true;
        }

        #endregion

        #region IPhysicsObject

        private CapsuleController _px;
        private CapsuleController px => Px().Result;
        private Vector3 cachePosition;
        public Vector3 Position { get => (Vector3)getPosition(); set => px.Position = (Vec3)value; }

        private Vector3 prevPosition;

        public event Action<Vector3> PositionChange;
        public Quaternion Rotation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private bool destroyed = false;
        public bool IsDestroyed => destroyed;
        private IPhysicsWorld world;
        public IPhysicsWorld OwnerWorld { get => world; }
        public IThing Thing { get; set; }

        private async Task<CapsuleController> Px()
        {
            await world.WaitEndOfFrame();
            return _px;
        }

        private Vector3 getPosition()
        {
            Task.Run(async () =>
            {
                await world.WaitEndOfFrame();
                cachePosition = (Vector3)_px.Position;
            }).ConfigureAwait(false);
            return cachePosition;
        }

        private Vector3 getFootPosition()
        {
            Task.Run(async () =>
            {
                await world.WaitEndOfFrame();
                cacheFootPosition = (Vector3)_px.FootPosition;
            }).ConfigureAwait(false);
            return cacheFootPosition;
        }
        #endregion

        internal PhysXCharacter(CapsuleController px, IPhysicsWorld w)
        {
            this._px = px;
            this.world = w;
            _px.ClimbingMode = CapsuleClimbingMode.Easy;
            _px.NonWalkableMode = ControllerNonWalkableMode.PreventClimbingAndForceSliding;
        }

        internal void Update()
        {
            _px.Move((Vec3)Direction * Speed + (Vec3)OwnerWorld.Gravity, TimeSpan.FromMilliseconds(world.DeltaTime));
            var pos = Position;
            if (pos != prevPosition)
                PositionChange?.Invoke(pos);
            prevPosition = pos;
        }


        public bool AutoMove(Vector3 toPosition)
        {
            bool hasPath = false;
            return hasPath;
        }

        public void Move(MoveDirection directions, bool clearQueue = false)
        {

            var rotation = Quaternion.Euler(0, -CharacterRotation, 0);

            Vec3 forward = (Vec3)(rotation * Vector3.forward);
            Vec3 up = Vec3.UnitY;
            Vec3 right = (Vec3)(rotation * Vector3.right);

            Vec3 moveDelta = Vec3.Zero;

            if (directions.HasFlag(MoveDirection.Forward))
                moveDelta += -forward;
            if (directions.HasFlag(MoveDirection.Backward))
                moveDelta += forward;
            if (directions.HasFlag(MoveDirection.Left))
                moveDelta += -right;
            if (directions.HasFlag(MoveDirection.Right))
                moveDelta += right;
            if (directions.HasFlag(MoveDirection.Up))
                moveDelta += up;
            if (directions.HasFlag(MoveDirection.Down))
                moveDelta += -up;


            Direction = (Vector3)(moveDelta.LengthSquared() == 0 ?
                 Vec3.Zero :
                 Vec3.Normalize(moveDelta));
        }



    }
}
