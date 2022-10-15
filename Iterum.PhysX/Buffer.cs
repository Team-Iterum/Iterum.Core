﻿using Iterum.Things;
using UnityEngine;

namespace Iterum.Physics.PhysXImpl;

public struct Buffer<T> where T : IThing
{
    public long Ref;
    public T[] Things;
    public long[] Refs;
    
    // supportData
    public float[] Dists;
    public Vector3[] Positions;
    public Vector3[] Normals;
    
    public int Count { get; private set; }

    public Buffer(long nRef, int max)
    {
        Ref = nRef;
        Things = new T[max];
        Refs = new long[max];
        Dists = new float[max];
        Positions = new Vector3[max];
        Normals = new Vector3[max];
        Count = 0;
    }

    public void SetResultsCount(int count) => Count = count;
}