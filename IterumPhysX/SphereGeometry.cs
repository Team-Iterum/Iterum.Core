using Magistr.Framework.Physics;

namespace Magistr.Physics.PhysXImplCore
{
    internal class SphereGeometry : IGeometry
    {
        private long nRef;
        private IPhysicsAPI api;
        public SphereGeometry(float radius, IPhysicsAPI api)
        {
            this.api = api;
            nRef = api.createSphereGeometry(radius);
        }

        public void Destroy ()
        {
            api.cleanupGeometry(nRef);
        }
        public object GetInternalGeometry()
        {
            return nRef;
        }

        public GeoType GeoType { get; } = GeoType.SimpleGeometry;
    }
}