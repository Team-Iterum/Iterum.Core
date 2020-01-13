using Magistr.Framework.Physics;
using Magistr.Math;
using static Magistr.Physics.PhysXImplCore.PhysicsAlias;

namespace Magistr.Physics.PhysXImplCore
{
    internal class BoxGeometry : IGeometry
    {
        private long nRef;

        public BoxGeometry(Vector3 half)
        {
            nRef = API.createBoxGeometry(half);
        }

        public void Destroy()
        {
            API.cleanupGeometry(nRef);
        }
        public object GetInternalGeometry()
        {
            return nRef;
        }

        public GeoType GeoType { get; } = GeoType.SimpleGeometry;
    }
}