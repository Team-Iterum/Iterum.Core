using Iterum.Math;
using Iterum.Physics.PhysXImpl;

namespace Iterum.Physics
{
    /// <summary>
    /// PhysX Factory
    /// </summary>
    public static class PhysicsWorldFactory
    {
        public static void CreatePhysics(float toleranceLength, float toleranceSpeed)
        {
            // PhysX
            PhysicsAlias.GlobalPhysics = new PhysXImpl.Physics();
            PhysicsAlias.GlobalPhysics.Init(toleranceLength, toleranceSpeed);
        }
        public static IPhysicsWorld CreateWorld(Vector3 gravity, int tps)
        {
            // PhysX
            return new PhysicsWorld(gravity, tps);
        }
    }
}
