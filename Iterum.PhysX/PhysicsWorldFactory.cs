using Iterum.Math;
using Iterum.Physics.PhysXImpl;

namespace Iterum.Physics
{
    /// <summary>
    /// PhysX Factory
    /// </summary>
    public static class PhysicsWorldFactory
    {
        public static void CreatePhysics(bool isCreatePvd = true, float toleranceLength = 1, float toleranceSpeed = 5,
            float staticFriction = 0.5f, float dynamicFriction = 0.5f, float restitution = 0.5f)
        {
            // PhysX
            PhysicsAlias.GlobalPhysics = new PhysXImpl.Physics();
            PhysicsAlias.GlobalPhysics.Init(isCreatePvd, toleranceLength, toleranceSpeed, 
                staticFriction, dynamicFriction, restitution);
        }
        public static IPhysicsWorld CreateWorld(Vector3 gravity, int tps)
        {
            // PhysX
            return new PhysicsWorld(gravity, tps);
        }
    }
}
