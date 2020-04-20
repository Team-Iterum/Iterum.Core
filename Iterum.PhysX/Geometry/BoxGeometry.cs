using Iterum.Math;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl
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