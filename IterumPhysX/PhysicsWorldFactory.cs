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
            var physics = new PhysXImplCore.Physics();
            physics.InitPhysics();

            PhysicsAlias.Set(physics);
        }
        public static IPhysicsWorld CreateWorld(int tps)
        {
            // PhysX
            var world = new PhysXWorld {TPS = tps};
            return world;
        }
    }
}
