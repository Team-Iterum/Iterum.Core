namespace Magistr.Physics
{
    /// <summary>
    /// USE PHYSX
    /// </summary>
    public static class PhysicsWorldFactory
    {
        public static IPhysicsWorld CreateWorld(int tps)
        {
            // PhysX
            var world = new PhysXImplCore.PhysXWorld {TPS = tps};
            return world;
        }
    }
}
