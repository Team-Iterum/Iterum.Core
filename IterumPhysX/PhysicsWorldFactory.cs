using Magistr.Physics.PhysXImplCore;

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
            PhysicsAlias.GlobalPhysics = new PhysXImplCore.Physics();
            PhysicsAlias.GlobalPhysics.InitPhysics();
        }
        public static IPhysicsWorld CreateWorld(int tps)
        {
            // PhysX
            var world = new PhysXWorld {TPS = tps};
            return world;
        }
    }
}
