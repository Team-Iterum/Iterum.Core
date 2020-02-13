using System;
using Magistr.Physics;

namespace Magistr.Things
{
    [Serializable]
    public struct ThingType
    {
        public int ThingTypeId;

        public string Title;
        public string Description;

        public ThingCategory Category;
        public ThingAttr Attributes;

        public DataBlock[] DataBlocks;

        public bool HasAttr(ThingAttr attr)
        {
            return Attributes.HasFlag(attr);
        }

    }
    
    [Serializable]
    public abstract class DataBlock
    {

    }
    
    [Serializable]
    public class Pivot
    {
        public Math.Vector3 LocalPosition;
        public Math.Quaternion LocalRotation;
        public string Name;
    }
    
    [Serializable]
    public class PivotData : DataBlock
    {
        public Pivot[] Pivots;
    }

    [Serializable]
    public class LightData : DataBlock
    {
        public byte Intensity;
        public byte Color;
        public byte Radius;
    }

    [Serializable]
    public class SpawnData : DataBlock
    {
        public Magistr.Math.Vector3 Position;
        public float RadiusX;
        public float RadiusY;
    }

    [Serializable]
    public class ShapeBoxData : DataBlock
    {
        public Magistr.Math.Vector3 HalfSize;
    }

    [Serializable]
    public class ShapeCapsuleData : DataBlock
    {
        public float Radius;
        public float Height;
    }
    [Serializable]
    public class ShapeSphereData : DataBlock
    {
        public float Radius;
    }

    [Serializable]
    public class ShapeModelData  : DataBlock, IModelData
    {
        public int[] Triangles { get; set; }
        public Magistr.Math.Vector3[] Points { get; set; }
    }

    [Serializable]
    public class MarketData : DataBlock
    {
        public string Description;
        public string Name;
        public long Price;
    }

    [Serializable]
    public enum ThingCategory
    {
        Item,
        Creature,
        Effect,
        Missile,
        Spawn,
        Ship,
        Station,
        Furniture,
        Asteroid,
        Door,
        Wreck,
        Special,
        Monster,
        Npc,
        Waypoint,
        Field,
        InvalidCategory,
    }

    [Flags]
    [Serializable]
    public enum ThingAttr
    {
        None = 0,
        Ground = 1 << 1,
        Container = 1 << 2,
        Writable = 1 << 3,
        WritableOnce = 1 << 4,
        Moveable = 1 << 5,
        NoCollisionProjectile = 1 << 6,
        Pickupable = 1 << 7,
        Rotatable = 1 << 8,
        Usable = 1 << 9,
        Teleport = 1 << 10,
        ShapeModel = 1 << 11,
        ShapeBox = 1 << 12,
        ShapeSphere = 1 << 13,
        ShapeCapsule = 1 << 14,
        ShapeMultiple = 1 << 15,
        Static = 1 << 16,
        Dynamic = 1 << 17,
        Kinematic = 1 << 18,
        NoCollision = 1 << 19,
        Invisible = 1 << 20,
    }

}
