﻿using System;
using Iterum.Things;
using UnityEngine;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl;

internal class TerrainObject : IStaticObject
{
    public long Ref { get; }

    private readonly Scene scene;

    #region IPhysicsObject

    public Vector3 Position
    {
        get => API.getRigidStaticPosition(Ref);
        set => API.setRigidStaticPosition(Ref, value);
    }

    public Quaternion Rotation
    {
        get => API.getRigidStaticRotation(Ref);
        set => API.setRigidStaticRotation(Ref, value);
    }
    public bool IsDestroyed { get; private set; }

    public IThing Thing { get; set; }

    public void Destroy()
    {
        if (IsDestroyed) return;

        scene.Destroy(this);
        IsDestroyed = true;

    }
    #endregion

    internal TerrainObject(Memory<float> samples, float hfScale, float hfSize, IMaterial mat, Vector3 pos, Scene scene)
    {
        this.scene = scene;

        Ref = API.createTerrain(samples.ToArray(), hfScale, hfSize, scene.Ref, (long)mat.GetInternal(), pos);
    }

    public void ModifyTerrain(Memory<float> samples, int startCol, int startRow, int countCol, int countRow, float hfScale, bool shrinkBounds)
    {
        API.modifyTerrain(Ref, samples.ToArray(), startCol, startRow, countCol, countRow, hfScale, shrinkBounds);
    }

}