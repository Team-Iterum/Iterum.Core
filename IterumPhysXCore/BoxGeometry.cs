using Magistr.Framework.Physics;
using Magistr.Math;

namespace Magistr.Physics.PhysXImplCore
{
    internal class BoxGeometry : IGeometry
    {
        private int geoIndex = -1;
        IPhysicsAPI API;
        public BoxGeometry(Vector3 half, IPhysicsAPI api)
        {
            API = api;
            geoIndex = api.createBoxGeometry(half.ToApi());
        }

        public void Destroy()
        {
            API.cleanupGeometry(geoIndex);
        }
        public object GetInternalGeometry()
        {
            return geoIndex;
        }
    }
}