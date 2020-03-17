namespace Magistr.New.ThingTypes
{
    public class PivotData : IDataBlock
    {
        public struct Pivot
        {
            public string Name;
            public float[] Position;
        }

        public Pivot[] Pivots;

    }

    public class SpawnData : IDataBlock
    {
        public float[] Position;
        public float RadiusX;
        public float RadiusY;
    }
    
    public class ShapeBoxData : IDataBlock
    {
        public float[] HalfSize;
    }

    public class ShapeCapsuleData : IDataBlock
    {
        public float Radius;
        public float Height;
    }

    public class ShapeSphereData : IDataBlock
    {
        public float Radius;
    }

    public class ShapeMeshData : IDataBlock
    {
        public string Name;
    }
}