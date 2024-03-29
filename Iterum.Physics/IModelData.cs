﻿using UnityEngine;

namespace Iterum.Physics;

public interface IModelData
{
    int[] Triangles { get; }
    Vector3[] Points { get; }
}