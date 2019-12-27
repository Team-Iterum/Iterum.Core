using Magistr.Math;
using Magistr.Physics;
using System;

namespace Magistr.Things
{
    public class Thing : IThing
    {
        private IPhysicsObject physicsObject;

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public int ThingTypeId { get; set; }
        public string ThingName { get; set; } = null;

        public event ThingPositionChange PositionChange;

        public bool IsDestroyed => physicsObject?.IsDestroyed ?? true;

        public Thing(int id, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            ThingTypeId = id;

        }

        public void SyncTransform()
        {
            physicsObject.Rotation = Rotation;
            physicsObject.Position = Position;
            PositionChange?.Invoke(this, Position, false);
        }

        public void Create(IPhysicsWorld world)
        {
            var thingType = ThingTypeManager.GetThingType(ThingTypeId);
            
            var geometry = thingType.CreateGeometry(world);

            if (thingType.HasAttr(ThingAttr.Static))
            {
                physicsObject = world.CreateStatic(geometry, Position, Rotation);
            }
            else if (thingType.HasAttr(ThingAttr.Dynamic))
            {
                physicsObject = world.CreateDynamic(geometry, Position, Rotation);
            }

            SyncTransform();

            physicsObject.Thing = this;
        }

        public void Destroy()
        {
            physicsObject.Destroy();
        }
    }
}
