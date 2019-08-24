namespace Magistr.Physics
{
    /// <summary>
    /// USE PHYSX
    /// </summary>
    public static class PhysicsWorldFactory
    {
        public static IPhysicsWorld CreateWorld(int TPS)
        {
            // PhysX
            var world = new PhysXImplCore.PhysXWorld();
            world.TPS = TPS;
            return world;
        }
    }
}
