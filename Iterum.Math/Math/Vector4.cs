using System;
using ProtoBuf;

namespace UnityEngine;

[ProtoContract]
[Serializable]
public struct Vector4
{
    public const float kEpsilon = 1E-05f;

    [ProtoMember(1)]
    public float x;

    [ProtoMember(2)]
    public float y;

    [ProtoMember(3)]
    public float z;

    [ProtoMember(4)]
    public float w;

    public float this[int index]
    {
        get
        {
            float result;
            switch (index)
            {
                case 0:
                    result = x;
                    break;
                case 1:
                    result = y;
                    break;
                case 2:
                    result = z;
                    break;
                case 3:
                    result = w;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector4 index!");
            }
            return result;
        }
        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                case 3:
                    w = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector4 index!");
            }
        }
    }

    public Vector4 normalized => Vector4.Normalize(this);

    public float magnitude => Mathf.Sqrt(Vector4.Dot(this, this));

    public float sqrMagnitude => Vector4.Dot(this, this);

    public static Vector4 zero => new Vector4(0f, 0f, 0f, 0f);

    public static Vector4 one => new Vector4(1f, 1f, 1f, 1f);

    public Vector4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Vector4(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        w = 0f;
    }

    public Vector4(float x, float y)
    {
        this.x = x;
        this.y = y;
        z = 0f;
        w = 0f;
    }

    public void Set(float new_x, float new_y, float new_z, float new_w)
    {
        x = new_x;
        y = new_y;
        z = new_z;
        w = new_w;
    }

    public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
    {
        t = Mathf.Clamp01(t);
        return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
    }

    public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t)
    {
        return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
    }

    public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta)
    {
        Vector4 a = target - current;
        float magnitude = a.magnitude;
        Vector4 result;
        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            result = target;
        }
        else
        {
            result = current + a / magnitude * maxDistanceDelta;
        }
        return result;
    }

    public static Vector4 Scale(Vector4 a, Vector4 b)
    {
        return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
    }

    public void Scale(Vector4 scale)
    {
        x *= scale.x;
        y *= scale.y;
        z *= scale.z;
        w *= scale.w;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;
    }

    public override bool Equals(object other)
    {
        bool result;
        if (!(other is Vector4))
        {
            result = false;
        }
        else
        {
            Vector4 vector = (Vector4)other;
            result = (x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z) && w.Equals(vector.w));
        }
        return result;
    }

    public static Vector4 Normalize(Vector4 a)
    {
        float num = Vector4.Magnitude(a);
        Vector4 result;
        if (num > 1E-05f)
        {
            result = a / num;
        }
        else
        {
            result = Vector4.zero;
        }
        return result;
    }

    public void Normalize()
    {
        float num = Vector4.Magnitude(this);
        if (num > 1E-05f)
        {
            this /= num;
        }
        else
        {
            this = Vector4.zero;
        }
    }

    public static float Dot(Vector4 a, Vector4 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    }

    public static Vector4 Project(Vector4 a, Vector4 b)
    {
        return b * Vector4.Dot(a, b) / Vector4.Dot(b, b);
    }

    public static float Distance(Vector4 a, Vector4 b)
    {
        return Vector4.Magnitude(a - b);
    }

    public static float Magnitude(Vector4 a)
    {
        return Mathf.Sqrt(Vector4.Dot(a, a));
    }

    public static Vector4 Min(Vector4 lhs, Vector4 rhs)
    {
        return new Vector4(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z), Mathf.Min(lhs.w, rhs.w));
    }

    public static Vector4 Max(Vector4 lhs, Vector4 rhs)
    {
        return new Vector4(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z), Mathf.Max(lhs.w, rhs.w));
    }

    public static Vector4 operator +(Vector4 a, Vector4 b)
    {
        return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    public static Vector4 operator -(Vector4 a, Vector4 b)
    {
        return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    }

    public static Vector4 operator -(Vector4 a)
    {
        return new Vector4(-a.x, -a.y, -a.z, -a.w);
    }

    public static Vector4 operator *(Vector4 a, float d)
    {
        return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
    }

    public static Vector4 operator *(float d, Vector4 a)
    {
        return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
    }

    public static Vector4 operator /(Vector4 a, float d)
    {
        return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
    }

    public static bool operator ==(Vector4 lhs, Vector4 rhs)
    {
        return Vector4.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
    }

    public static bool operator !=(Vector4 lhs, Vector4 rhs)
    {
        return Vector4.SqrMagnitude(lhs - rhs) >= 9.99999944E-11f;
    }

    public static implicit operator Vector4(Vector3 v)
    {
        return new Vector4(v.x, v.y, v.z, 0f);
    }

    public static implicit operator Vector3(Vector4 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static implicit operator Vector4(Vector2 v)
    {
        return new Vector4(v.x, v.y, 0f, 0f);
    }

    public static implicit operator Vector2(Vector4 v)
    {
        return new Vector2(v.x, v.y);
    }

    public override string ToString()
    {
        return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", new object[]
        {
            x,
            y,
            z,
            w
        });
    }

    public string ToString(string format)
    {
        return string.Format("({0}, {1}, {2}, {3})", new object[]
        {
            x.ToString(format),
            y.ToString(format),
            z.ToString(format),
            w.ToString(format)
        });
    }

    public static float SqrMagnitude(Vector4 a)
    {
        return Vector4.Dot(a, a);
    }

    public float SqrMagnitude()
    {
        return Vector4.Dot(this, this);
    }
}