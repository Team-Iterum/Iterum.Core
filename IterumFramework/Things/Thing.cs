using Magistr.Math;
using Magistr.Physics;
using System;

namespace Magistr.Things
{
    public class Thing : IThing
    {
        private IPhysicsObject PhysicsObject;
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public int ThingTypeId { get; set; }
        public string ThingName { get; set; } = null;

        public event Action<IThing, Vector3, bool> PositionChange;

        public bool IsDestroyed => PhysicsObject == null ? true : PhysicsObject.IsDestroyed;

        public Thing(int id, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            ThingTypeId = id;

        }

        public void SyncTransform()
        {
            PhysicsObject.Rotation = Rotation;
            PhysicsObject.Position = Position;
            PositionChange?.Invoke(this, Position, false);
        }

        public void Create(IPhysicsWorld world)
        {
            var thingType = ThingTypeManager.GetTningType(ThingTypeId);
            
            var geometry = thingType.CreateGeometry(world);
            if (thingType.HasAttr(ThingAttr.Static))
            {
                PhysicsObject = world.CreateStatic(geometry, Position, Rotation);
            }
            else if (thingType.HasAttr(ThingAttr.Dynamic))
            {
                PhysicsObject = world.CreateDynamic(geometry, Position, Rotation);
            }
            SyncTransform();
            PhysicsObject.Thing = this;
        }

        public void Destroy()
        {
            PhysicsObject.Destroy();
        }
    }
}
