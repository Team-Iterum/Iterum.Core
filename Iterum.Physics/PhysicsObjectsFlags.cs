using System;

namespace Iterum.Physics
{
    [Flags]
    public enum PhysicsObjectFlags
    {
        None = 0,
        Trigger = 1 << 0,
        DisableGravity = 1 << 1,
        Kinematic = 1 << 2,
        CCD = 1 << 3,
        Retain = 1 << 4,
    }
}