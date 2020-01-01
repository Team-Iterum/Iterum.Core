using System;
using Magistr.Math;

namespace MapData
{
    [Serializable]
    public struct MapArchive
    {
        public DateTime Created;
        public string Name;

        public MapObject[] Objects;
    }

    [Serializable]
    public struct MapObject
    {
        public int ThingTypeId;
        
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }
}