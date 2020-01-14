using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using Magistr.Framework.Physics;
using static Magistr.Physics.PhysXImplCore.PhysicsAlias;

namespace Magistr.Physics.PhysXImplCore
{
    internal class ModelGeometry : IGeometry
    {
        private long nRef;
        private uint[] indices;
        private APIVec3[] vertices;

        public ModelGeometry(GeoType geoType, IModelData model)
        {
            GeoType = geoType;
            vertices = model.Points.Select(e => (APIVec3) e).ToArray();
            indices = model.Triangles.Select(e => (uint) e).ToArray();

            CreateMeshGeometry();
        }

        public ModelGeometry(GeoType geoType, string name)
        {
            GeoType = geoType;

            nRef = API.loadTriangleMesh(name);
        }

        private void CreateMeshGeometry()
        {
            switch (GeoType)
            {

                case GeoType.TriangleMeshGeometry:
                    //nRef = API.createTriangleMesh(vertices, vertices.Length, indices, indices.Length);
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