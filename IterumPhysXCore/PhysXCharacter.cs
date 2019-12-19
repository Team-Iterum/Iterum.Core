using Magistr.Math;
using Magistr.Things;
using System;
using System.Threading.Tasks;

namespace Magistr.Physics.PhysXImplCore
{
    public class PhysXCharacter : IPhysicsCharaceter
    {

        public long Ref;

        #region IPhysicsCharacter

        public Vector3 Direction { get; set; }

        public float Speed { get; set; } = 0.1f;

        public float CharacterRotation { get; set; }
        private DVector3 cacheFootPosition;
        public Vector3 FootPosition
        {
            get => GetFootPosition();
            set
            {
                if (!IsDestroyed)
                {
                    //Task.Run(async () =>
                    //{
                    //    await OwnerWorld.WaitEndOfFrame();
                        api.setControllerFootPosition(Ref, new DVector3(value).ToApi());
                    //}).ConfigureAwait(false);
                }
            }
        }

        public void Destroy()
        {

            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    scene.Destroy(this);
                }).ConfigureAwait(false);
            }
            IsDestroyed = true;

        }

        #endregion

        #region IPhysicsObject
        private IPhysicsAPI api;

        private Scene scene;

        private DVector3 cachePosition;
        public Vector3 Position
        {
            get => GetPosition();
            set
            {
                //if (!IsDestroyed)
                //{
                //    Task.Run(async () =>
                //{
                //    await OwnerWorld.WaitEndOfFrame();
                    api.setControllerPosition(Ref, new DVector3(value).ToApi());
                //}).ConfigureAwait(false);
                //}

            }
        }

        private Vector3 prevPosition;

        public event Action<Vector3, bool> PositionChange;

        public Quaternion Rotation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsDestroyed { get; private set; }

        public IPhysicsWorld OwnerWorld { get; }
        public IThing Thing { get; set; }


        private Vector3 GetPosition()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    cachePosition = api.getControllerPosition(Ref).ToVector3();
                }).ConfigureAwait(false);
            }
            return new Vector3((float)cachePosition.x, (float)cachePosition.y, (float)cachePosition.z);
        }

        private Vector3 GetFootPosition()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
            {
                await OwnerWorld.WaitEndOfFrame();
                cacheFootPosition = api.getControllerFootPosition(Ref).ToVector3();
            }).ConfigureAwait(false);
            }
            return new Vector3((float)cacheFootPosition.x, (float)cacheFootPosition.y, (float)cacheFootPosition.z);
        }
        #endregion

        internal PhysXCharacter(Vector3 pos, Vector3 up, float height, float radius, Scene scene, IPhysicsWorld world, IPhysicsAPI api)
        {
            this.scene = scene;
            this.OwnerWorld = world;
            this.api = api;


            Ref = api.createCapsuleCharacter(this.scene.Ref, pos.ToApi(), up.normalized.ToApi(), height, radius, 0.05f);
            cachePosition = new DVector3(pos.x, pos.y, pos.z);
            api.setControllerDirection(Ref, (Vector3.zero + world.Gravity).ToApi());
        }

        private float timeUpdate;
        private const float ForceUpdateSeconds = 2f;
        internal void Update(float dt)
        {

            bool force = false;
            timeUpdate += dt;
            if (timeUpdate >= ForceUpdateSeconds)
            {
                timeUpdate = 0;
                force = true;
            }

            var pos = Position;
            if ((pos - prevPosition).magnitude > 0.05f || force)
            {
                PositionChange?.Invoke(pos, force);
                prevPosition = pos;
            }


        }

        public bool AutoMove(Vector3 toPosition)
        {
            return false;
        }

        public void Move(MoveDirection directions, bool clearQueue = false)
        {

            var rotation = Quaternion.Euler(0, -CharacterRotation, 0);

            var forward = (rotation * Vector3.forward);
            var up = Vector3.up;
            var right = (rotation * Vector3.right);

            var moveDelta = Vector3.zero;

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


            Direction = (moveDelta.LengthSqr() == 0 ?
                            Vector3.zero :
                            Vector3.Normalize(moveDelta)) * Speed + OwnerWorld.Gravity;

            api.setControllerDirection(Ref, Direction.ToApi());

        }



    }
}
