using System;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl;

internal class CapsuleGeometry : IGeometry, IDisposable
{
    private long nRef;
    public CapsuleGeometry(float radius, float halfHeight)
    {
        nRef = API.createCapsuleGeometry(radius, halfHeight);
    }

    public void Destroy ()
    {
        API.cleanupGeometry(nRef);
    }
    
    public long GetInternal()
    {
        return nRef;
    }

    public GeoType GeoType { get; } = GeoType.SimpleGeometry;

    public void Dispose()
    {
        Destroy();
    }
}