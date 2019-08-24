using Magistr.Framework.Physics;
using Magistr.Log;
using PhysX;

namespace Magistr.Physics.PhysXImpl
{

    public class ControllerHitReport : UserControllerHitReport
    {
        public override void OnControllerHit(ControllersHit hit)
        {
            //Debug.Log("Hit controller at " + hit.WorldPosition);
        }

        public override void OnObstacleHit(ControllerObstacleHit hit)
        {

        }

        public override void OnShapeHit(ControllerShapeHit hit)
        {

        }
    }

    public class SampleFilterShader : SimulationFilterShader
    {
        public override FilterResult Filter(int attributes0, FilterData filterData0, int attributes1, FilterData filterData1)
        {
            return new FilterResult
            {
                FilterFlag = FilterFlag.Default,
                // Cause PhysX to report any contact of two shapes as a touch and call SimulationEventCallback.OnContact
                PairFlags = PairFlag.ContactDefault | PairFlag.NotifyTouchFound | PairFlag.NotifyTouchLost
            };
        }
    }

    public class PhysXGeometry : IGeometry
    {
        internal Geometry geometry;

        public object GetInternalGeometry()
        {
            return geometry;
        }
    }
}