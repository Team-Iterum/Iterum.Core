using Iterum.Physics.PhysXImpl;
using UnityEngine;

namespace Iterum.Physics;

/// <summary>
/// PhysX Factory
/// </summary>
public static class PhysicsWorldFactory
{
    public static void CreatePhysics(bool isCreatePvd = true, float toleranceLength = 1, float toleranceSpeed = 5)
    {
        // PhysX
        PhysicsAlias.GlobalPhysics = new PhysXImpl.Physics();
        PhysicsAlias.GlobalPhysics.Init(isCreatePvd, toleranceLength, toleranceSpeed);
    }
    public static PhysicsWorld CreateWorld(Vector3 gravity, bool ccd = false, bool determenism = false)
    {
        // PhysX
        return new PhysicsWorld(gravity, ccd, determenism);
    }
}