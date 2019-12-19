using Magistr.Math;
using Magistr.Physics;
using System;
using System.Threading.Tasks;

namespace Magistr.Things
{
    public class Creature : ICreature
    {
        #region IThing
        private IPhysicsCharaceter PhysicsObject;
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get => 
                Quaternion.AngleAxis(CharacterRotation, Vector3.up);
            set => _ = value; }
        public Vector3 Scale { get; set; }
        public int ThingTypeId { get; set; }
        public string ThingName { get; set; } = null;
        #endregion

        #region ICreature
        public float Height { get; set; }
        public float Radius { get; set; }
        public Vector3 FootPosition { get => PhysicsObject.FootPosition; set => PhysicsObject.FootPosition = value; }
        public float Speed { get; set; }
        public Vector3 Direction { get => PhysicsObject.Direction; set => throw new NotImplementedException(); }

        public bool IsDestroyed => PhysicsObject == null ? true : PhysicsObject.IsDestroyed;
        public float CharacterRotation
        {
            get => PhysicsObject != null ? PhysicsObject.CharacterRotation : 0;
            set
            {
                if (PhysicsObject != null)
                {
                    PhysicsObject.CharacterRotation = value;
                    Move(CurrentMoveDirection);
                }
            }
        }
        #endregion

        public float BoostMultiplier = 10;

        public MoveDirection CurrentMoveDirection;
        private bool _boost;
        public bool Boost
        {
            get
            {
                return _boost;
            }
             set
            {
                _boost = value;
                SyncParams();
            }
        }

        public event Action<IThing, Vector3, bool> PositionChange;
        public event Action<IThing, Vector3, bool> Created;

        public Creature(int id, Vector3 position)
        {
            var thingType = ThingTypeManager.GetThingType(id);
            Position = position;
            Height = thingType.Size.y/2;
            Radius = thingType.Radius;
            ThingTypeId = id;
            PhysicsObject = null;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
            Speed = 1;
           
        }

        public void SyncParams()
        {
            PhysicsObject.Speed = Speed * (Boost ? BoostMultiplier : 1);
            Position = PhysicsObject.Position;
        }


        public void Create(IPhysicsWorld world)
        {
            Task.Run(async () =>
            {
                await world.WaitEndOfFrame();
                var thingType = ThingTypeManager.GetThingType(ThingTypeId);
                PhysicsObject = world.CreateCapsuleCharacter(Position, Vector3.up, Height, Radius);
                PhysicsObject.Thing = this;
                PhysicsObject.PositionChange += PhysicsObject_PositionChange;
                SyncParams();
                PositionChange?.Invoke(this, Position, true);
                Created?.Invoke(this, Position, true);
            }).ConfigureAwait(false);
           
        }

        private void PhysicsObject_PositionChange(Vector3 pos, bool force)
        {
            SyncParams();
            PositionChange?.Invoke(this, pos, force);
        }

        public void Destroy()
        {
            PhysicsObject.Destroy();
        }

        public void Move(MoveDirection directions)
        {
            CurrentMoveDirection = directions;
            PhysicsObject.Move(directions, false);
        }

        public bool AutoMove(Vector3 toPosition)
        {
            return false;
        }
    }
}
