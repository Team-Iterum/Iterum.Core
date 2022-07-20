using System;
using Iterum.Things;
using UnityEngine;
using static Iterum.Physics.PhysXImpl.PhysicsAlias;

namespace Iterum.Physics.PhysXImpl;

public class TerrainObject : IStaticObject
{
    public long Ref { get; }

    private readonly Scene scene;

    #region IPhysicsObject

    public Vector3 Position
    {
        get => API.getTerrainPosition(Ref);
        set => API.setTerrainPosition(Ref, value);
    }

    public Quaternion Rotation
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
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

    internal TerrainObject(Memory<short> heightmap, long hfSize, 
        float thickness, float convexEdgeThreshold, bool noBoundaries, 
        Vector3 scale,
        IMaterial mat, Vector3 pos, Scene scene)
    {
        this.scene = scene;

        Ref = API.createTerrain(heightmap.ToArray(), hfSize,
            thickness, convexEdgeThreshold, noBoundaries,
            scale.y, scale.x, scale.z,
            scene.Ref, (long)mat.GetInternal(), pos);
    }

    public void ModifyTerrain(Memory<short> samples, long startCol, long startRow, long countCol, long countRow, float hfScale, bool shrinkBounds)
    {
        API.modifyTerrain(Ref, samples.ToArray(), startCol, startRow, countCol, countRow, hfScale, shrinkBounds);
    }

    /// <summary>
    /// Sample terrain height
    /// </summary>
    /// <param name="pos">x, z used, y ignored</param>
    public float SampleTerrain(Vector3 pos)
    {
        return API.sampleTerrainHeight(Ref, pos);
    }

}