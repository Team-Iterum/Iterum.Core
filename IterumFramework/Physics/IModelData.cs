
using System;
using Vec3 = System.Numerics.Vector3;
namespace Magistr.Framework.Physics
{
    public interface IModelData
    {
        int[] Triangles { get; }
        Vec3[] Points { get; }
    }
}
