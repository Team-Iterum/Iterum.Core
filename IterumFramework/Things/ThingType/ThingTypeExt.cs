using System.Linq;
using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Physics;

namespace Magistr.Things
{
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