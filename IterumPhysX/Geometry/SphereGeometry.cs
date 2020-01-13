using Magistr.Framework.Physics;
using static Magistr.Physics.PhysXImplCore.PhysicsAlias;

namespace Magistr.Physics.PhysXImplCore
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