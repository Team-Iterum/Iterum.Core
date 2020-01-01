using System;
using System.Linq;
using Magistr.Framework.Physics;
using Magistr.Math;

namespace Magistr.Physics.PhysXImplCore
{
    internal class ModelGeometry : IGeometry
    {
        private long nRef;
        private IPhysicsAPI api;

        public ModelGeometry(GeoType geoType, IModelData model, IPhysicsAPI api)
        {
            this.api = api;
            GeoType = geoType;

            switch (geoType)
            {
                case GeoType.TriangleMeshGeometry:
                    nRef = api.createTriangleMesh(model.Points.Select(e => (APIVec3)e).ToArray(), model.Points.Length,
                        model.Triangles.Select(e => (uint) e).ToArray(), model.Triangles.Length);
                    break;
                case GeoType.ConvexMeshGeometry:
                    nRef = api.createConvexMesh(model.Points.Select(e => (APIVec3)e).ToArray(), model.Points.Length);
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
                    api.cleanupConvexMesh(nRef);
                    break;
                case GeoType.TriangleMeshGeometry:
                    api.cleanupTriangleMesh(nRef);
                    break;
            }
        }
        public object GetInternalGeometry()
        {
            return nRef;
        }
    }
}