using Magistr.Framework.Physics;
using Magistr.Log;
using Magistr.Math;
using Magistr.Things;
using System;
using System.Threading.Tasks;
using Mat4x4 = System.Numerics.Matrix4x4;
using Vec3 = System.Numerics.Vector3;

namespace Magistr.Physics.PhysXImplCore
{
    public class PhysXCharacter : IPhysicsCharaceter
    {

        public int Index => _px;
        #region IPhysicsCharacter

        private Vector3 cacheDirection;
        public Vector3 Direction { get => cacheDirection; set { cacheDirection = value; } }
        public float Speed { get; set; } = 0.1f;

        public float CharacterRotation { get; set; }
        private Vector3 cacheFootPosition;
        public Vector3 FootPosition
        {
            get => (Vector3)getFootPosition();
            set
            {
                if (!IsDestroyed)
                {
                    Task.Run(async () =>
                    {
                        await OwnerWorld.WaitEndOfFrame();
                        API.setControllerFootPosition(_px, Scene.Index, new DVector3(value).ToApi());
                    }).ConfigureAwait(false);
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
                    Scene.Destroy(this);
                }).ConfigureAwait(false);
            }
            IsDestroyed = true;

        }

        #endregion

        #region IPhysicsObject

        private int _px;
        private Scene Scene;

        private Vector3 cachePosition;
        public Vector3 Position
        {
            get => (Vector3)getPosition();
            set
            {
                if (!IsDestroyed)
                {
                    Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    API.setControllerPosition(_px, Scene.Index, new DVector3(value).ToApi());
                }).ConfigureAwait(false);
                }
                
            }
        }

        private Vector3 prevPosition;

        public event Action<Vector3, bool> PositionChange;
        public Quaternion Rotation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsDestroyed { get; private set; } = false;

        private IPhysicsAPI API;
        public int UserDataReference;

        public IPhysicsWorld OwnerWorld { get; }
        public IThing Thing { get; set; }


        private Vector3 getPosition()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
                {
                    await OwnerWorld.WaitEndOfFrame();
                    cachePosition = (Vector3)API.getControllerPosition(_px, Scene.Index).ToVector3();
                }).ConfigureAwait(false);
            }
            return cachePosition;
        }

        private Vector3 getFootPosition()
        {
            if (!IsDestroyed)
            {
                Task.Run(async () =>
            {
                await OwnerWorld.WaitEndOfFrame();
                cacheFootPosition = (Vector3)API.getControllerFootPosition(_px, Scene.Index).ToVector3();
            }).ConfigureAwait(false);
            }
            return cacheFootPosition;
        }
        #endregion

        internal PhysXCharacter(int UserDataIndex, Vector3 pos, Vector3 up, float height, float radius, Scene scene, IPhysicsWorld world, IPhysicsAPI aPI)
        {
            UserDataReference = UserDataIndex;
            Scene = scene;
            this.OwnerWorld = world;
            API = aPI;
            _px = API.createCapsuleCharacter(Scene.Index, UserDataIndex, pos.ToApi(), up.ToApi(), height, radius);
            cachePosition = (Vector3)API.getControllerPosition(_px, Scene.Index).ToVector3();
            API.setControllerDirection(_px, Scene.Index, (Vector3.zero + world.Gravity).ToApi());
        }

        private float timeUpdate = 0;
        const float forceUpdateSeconds = 2f;
        internal void Update(float dt)
        {
            
            bool force = false;
            timeUpdate += dt;
            if(timeUpdate >= forceUpdateSeconds)
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
            bool hasPath = false;
            return hasPath;
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


            Direction = (Vector3)(moveDelta.LengthSqr() == 0 ?
                 Vector3.zero :
                 Vector3.Normalize(moveDelta)) * Speed + OwnerWorld.Gravity;
            API.setControllerDirection(_px, Scene.Index, Direction.ToApi());

        }



    }
}
