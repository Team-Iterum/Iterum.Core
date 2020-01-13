using System;
using System.Linq;
using Magistr.Framework.Physics;
using static Magistr.Physics.PhysXImplCore.PhysicsAlias;

namespace Magistr.Physics.PhysXImplCore
{
    internal class ModelGeometry : IGeometry
    {
        private long nRef;

        public ModelGeometry(GeoType geoType, IModelData model)
        {
            GeoType = geoType;
            var vertices = model.Points.Select(e => (APIVec3) e).ToArray();
            var indices = model.Triangles.Select(e => (uint) e).ToArray();
            switch (geoType)
            {

                case GeoType.TriangleMeshGeometry:
                    nRef = API.createTriangleMesh(vertices, vertices.Length, indices, indices.Length);
                    break;
                case GeoType.ConvexMeshGeometry:
                    nRef = API.createConvexMesh(vertices, vertices.Length);
                    break;
                default:
                    throw new Exception("Model geometry can't be GeoType.SimpleGeometry");
            }

        }

        public GeoType GeoType { get; }

        public void Destroy()
        {
            switch (GeoType)
            {
                case GeoType.ConvexMeshGeometry:
                    API.cleanupConvexMesh(nRef);
                    break;
                case GeoType.TriangleMeshGeometry:
                    API.cleanupTriangleMesh(nRef);
                    break;
            }
        }
        public object GetInternalGeometry()
        {
            return nRef;
        }
    }
}