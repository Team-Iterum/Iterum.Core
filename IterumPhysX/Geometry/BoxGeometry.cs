using Magistr.Math;
using static Magistr.Physics.PhysXImpl.PhysicsAlias;

namespace Magistr.Physics.PhysXImpl
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