namespace Iterum.Physics
{
    public enum GeoType
    {
        SimpleGeometry = 1,
        ConvexMeshGeometry = 2,
        TriangleMeshGeometry = 3,
    }
    public interface IGeometry
    {
        object GetInternalGeometry();

        GeoType GeoType { get; }
    }
}
