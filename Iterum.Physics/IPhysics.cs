using Iterum.Math;

namespace Iterum.Physics
{
    public interface IPhysics
    {
        void Init(bool isCreatePvd, float toleranceLength, float toleranceSpeed);

        // Model geometry
        IGeometry CreateTriangleMeshGeometry(IModelData modelData);
        IGeometry CreateConvexMeshGeometry(IModelData modelData);

        // Simple geometry
        IGeometry CreateSphereGeometry(float radius);
        IGeometry CreateBoxGeometry(Vector3 size);
        
        // Materials
        IMaterial CreateMaterial(float staticFriction, float dynamicFriction, float restitution);
    }
}