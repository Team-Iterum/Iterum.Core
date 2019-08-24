using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Physics;
using System;

namespace Magistr.Things
{
    [Serializable]
    public struct ThingType
    {
        public ThingCategory Category;
        public ThingAttr Attributes;

        public LightData Light;
        public MarketData Market;

        public byte Color;
        public string Description;

        public ModelData Model;

        public Vector3 Size;
        public float Radius;

        [NonSerialized]
        public int ThingTypeId;

        public bool HasAttr(ThingAttr attr)
        {
            return Attributes.HasFlag(attr);
        }

        public IGeometry CreateGeometry(IPhysicsWorld world)
        {
            if(HasAttr(ThingAttr.Model))
            {
                return world.CreateStaticModelGeometry(Model);
            }
            if (HasAttr(ThingAttr.BoxGeometry))
            {
                return world.CreateBoxGeometry(Size);
            }
            if (HasAttr(ThingAttr.SphereGeometry))
            {
                return world.CreateSphereGeometry(Radius);
            }
            return default;
        }
    }

    [Serializable]
    public struct LightData
    {
        public byte Intensity;
        public byte Color;
        public byte Radius;
    }

    [Serializable]
    public struct ModelData : IModelData
    {
        public int[] Triangles { get; set; }
        public System.Numerics.Vector3[] Points { get; set; }
    }

    [Serializable]
    public struct MarketData
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
        Walkable = 1 << 5,
        Moveable = 1 << 6,
        NotBlockProjectile = 1 << 7,
        Pickupable = 1 << 8,
        Rotateable = 1 << 9,
        Usable = 1 << 10,
        Teleport = 1 << 11,
        Model = 1 << 12,
        BoxGeometry = 1 << 13,
        SphereGeometry = 1 << 14,
        Static = 1 << 15,
        Dynamic = 1 << 16,
    }

}
