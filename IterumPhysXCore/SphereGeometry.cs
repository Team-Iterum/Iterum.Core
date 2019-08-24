using Magistr.Framework.Physics;

namespace Magistr.Physics.PhysXImplCore
{
    internal class SphereGeometry : IGeometry
    {
        private int geoIndex = -1;
        IPhysicsAPI API;
        public SphereGeometry(float radius, IPhysicsAPI api)
        {
            API = api;
            geoIndex = api.createSphereGeometry(radius);
        }

        public void Destroy ()
        {
            API.cleanupGeometry(geoIndex);
        }
        public object GetInternalGeometry()
        {
            return geoIndex;
        }
    }
}