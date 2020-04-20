using Magistr.Math;

namespace Magistr.Physics
{
    public interface IPhysics
    {
        void InitPhysics(float toleranceLength, float toleranceSpeed);
        IGeometry CreateTriangleMeshGeometry(IModelData modelData);
        IGeometry CreateConvexMeshGeometry(IModelData modelData);
        IGeometry CreateSphereGeometry(float radius);
        IGeometry CreateBoxGeometry(Vector3 size);
    }
}