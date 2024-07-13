using System.Globalization;

namespace UnityEngine;

public static class QuaternionEx
{
    public static Quaternion DeltaTo(this Quaternion quat, Quaternion target)
    {
        return target * Quaternion.Inverse(quat);
    }

    public static string ToStringEx(this Quaternion quat)
    {
        return string.Format("[{0}, {1}, {2}, {3}]",
            quat.x.ToString(CultureInfo.InvariantCulture),
            quat.y.ToString(CultureInfo.InvariantCulture),
            quat.z.ToString(CultureInfo.InvariantCulture),
            quat.w.ToString(CultureInfo.InvariantCulture));
    }
}