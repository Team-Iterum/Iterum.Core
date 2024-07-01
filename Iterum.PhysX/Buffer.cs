using System;
using UnityEngine;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl;

public struct BufferId : IDisposable
{
    public long Ref;
    public ulong[] ThingIds;
    public long[] Refs;
    
    // supportData
    public float[] Distances;
    public Vector3[] Positions;
    public Vector3[] Normals;
    
    public int Count { get; private set; }

    public BufferType BufferType { get; }
    
    public BufferId(long nRef, int max, BufferType bufferType)
    {
        Ref = nRef;
        ThingIds = new ulong[max];
        Refs = new long[max];
        Distances = new float[max];
        Positions = new Vector3[max];
        Normals = new Vector3[max];
        Count = 0;

        BufferType = bufferType;
    }

    public void SetResultsCount(int count) => Count = count;

    public void Dispose()
    {
        switch (BufferType)
        {
            case BufferType.Raycast:
                API.cleanupRaycastBuffer(Ref);
                break;
            case BufferType.Sphere:
                API.cleanupOverlapBuffer(Ref);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum BufferType
{
    Raycast,
    Sphere
}