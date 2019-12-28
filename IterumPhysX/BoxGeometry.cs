using Magistr.Framework.Physics;
using Magistr.Math;

namespace Magistr.Physics.PhysXImplCore
{
    internal class BoxGeometry : IGeometry
    {
        private long nRef;
        private IPhysicsAPI api;

        public BoxGeometry(Vector3 half, IPhysicsAPI api)
        {
            this.api = api;
            nRef = api.createBoxGeometry(half.ToApi());
        }

        public void Destroy()
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