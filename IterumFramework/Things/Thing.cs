using Magistr.Math;
using Magistr.Physics;
using System;
using System.Linq;
using Magistr.Framework.Physics;

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
            physicsObject = thingType.CreatePhysicsObject(world, geometry, Position, Rotation);

            SyncTransform();

            physicsObject.Thing = this;
        }

        public void Destroy()
        {
            physicsObject.Destroy();
        }
    }

    public static class ThingTypeExt
    {
        public static IGeometry CreateGeometry(this ThingType thingType, IPhysicsWorld world)
        {
            // Select geometry
            IGeometry geometry = null;

            if (thingType.HasAttr(ThingAttr.ShapeBox))
            {
                geometry = world.CreateBoxGeometry((thingType.DataBlocks.FirstOrDefault(e=>e is ShapeBoxData) as ShapeBoxData).HalfSize);
            }
            else if (thingType.HasAttr(ThingAttr.ShapeSphere))
            {
                geometry = world.CreateSphereGeometry((thingType.DataBlocks.FirstOrDefault(e=>e is ShapeSphereData) as ShapeSphereData).Radius);
            }
            else if (thingType.HasAttr(ThingAttr.ShapeModel) && thingType.HasAttr(ThingAttr.Kinematic))
            {
                geometry = world.CreateTriangleMeshGeometry(thingType.DataBlocks.FirstOrDefault(e=>e is ShapeModelData) as ShapeModelData);
            }
            else if (thingType.HasAttr(ThingAttr.ShapeModel) && thingType.HasAttr(ThingAttr.Dynamic))
            {
                geometry = world.CreateConvexMeshGeometry(thingType.DataBlocks.FirstOrDefault(e=>e is ShapeModelData) as ShapeModelData);
            }


            return geometry;
        }

        public static IPhysicsObject CreatePhysicsObject(this ThingType thingType, IPhysicsWorld world, IGeometry geometry, Vector3 position, Quaternion rotation)
        {
            IPhysicsObject physicsObject = null;

            // Select physics model
            if (thingType.HasAttr(ThingAttr.Static))
            {
                physicsObject = world.CreateDynamic(geometry, true, position, rotation);
            }
            else if (thingType.HasAttr(ThingAttr.Dynamic))
            {
                physicsObject = world.CreateDynamic(geometry, false, position, rotation);
            }
            else if (thingType.HasAttr(ThingAttr.Kinematic))
            {
                physicsObject = world.CreateDynamic(geometry, true, position, rotation);
            }

            return physicsObject;
        }
    }
}
