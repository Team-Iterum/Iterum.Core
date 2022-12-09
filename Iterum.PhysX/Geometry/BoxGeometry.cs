using System;
using UnityEngine;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl;

internal class BoxGeometry : IGeometry, IDisposable
{
    private long nRef;

    public BoxGeometry(Vector3 half)
    {
        nRef = API.createBoxGeometry(half);
    }

    public void Destroy()
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