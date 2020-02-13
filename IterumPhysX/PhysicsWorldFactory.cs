using Magistr.Physics.PhysXImpl;

namespace Magistr.Physics
{
    /// <summary>
    /// USE PHYSX
    /// </summary>
    public static class PhysicsWorldFactory
    {
        public static void CreatePhysics()
        {
            // PhysX
            PhysicsAlias.GlobalPhysics = new PhysXImpl.Physics();
            PhysicsAlias.GlobalPhysics.InitPhysics();
        }
        public static IPhysicsWorld CreateWorld(int tps)
        {
            // PhysX
            var world = new PhysicsWorld {TPS = tps};
            return world;
        }
    }
}
