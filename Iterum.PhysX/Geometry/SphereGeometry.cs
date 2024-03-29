﻿using System;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl;

internal class SphereGeometry : IGeometry, IDisposable
{
    private long nRef;
    public SphereGeometry(float radius)
    {
        nRef = API.createSphereGeometry(radius);
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