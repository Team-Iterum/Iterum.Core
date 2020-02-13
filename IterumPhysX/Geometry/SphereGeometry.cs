using static Magistr.Physics.PhysXImpl.PhysicsAlias;

namespace Magistr.Physics.PhysXImpl
{
    internal class SphereGeometry : IGeometry
    {
        private long nRef;
        public SphereGeometry(float radius)
        {
            nRef = API.createSphereGeometry(radius);
        }

        public void Destroy ()
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