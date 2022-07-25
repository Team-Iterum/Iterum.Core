using System;
using System.ComponentModel;
using ProtoBuf;

namespace UnityEngine;

/// <summary>
///   <para>Representation of 3D vectors and points.</para>
/// </summary>
[Serializable]
[ProtoContract]
public struct Vector3
{
    public const float kEpsilon = 1E-05f;
    public const float kEpsilonNormalSqrt = 1e-15F;

    /// <summary>
    ///   <para>X component of the vector.</para>
    /// </summary>
    [ProtoMember(1)]
    public float x;

    /// <summary>
    ///   <para>Y component of the vector.</para>
    /// </summary>
    [ProtoMember(2)]
    public float y;

    /// <summary>
    ///   <para>Z component of the vector.</para>
    /// </summary>
    [ProtoMember(3)]
    public float z;

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
                default:
                    throw new IndexOutOfRangeException("Invalid Vector3 index!");
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
                default:
                    throw new IndexOutOfRangeException("Invalid Vector3 index!");
            }
        }
    }

    /// <summary>
    ///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
    /// </summary>
    public Vector3 normalized => Vector3.Normalize(this);

    /// <summary>
    ///   <para>Returns the length of this vector (Read Only).</para>
    /// </summary>
    public float magnitude => Mathf.Sqrt(x * x + y * y + z * z);

    /// <summary>
    ///   <para>Returns the squared length of this vector (Read Only).</para>
    /// </summary>
    public float sqrMagnitude => x * x + y * y + z * z;

    /// <summary>
    ///   <para>Shorthand for writing Vector3(0, 0, 0).</para>
    /// </summary>
    public static Vector3 zero => new Vector3(0f, 0f, 0f);

    /// <summary>
    ///   <para>Shorthand for writing Vector3(1, 1, 1).</para>
    /// </summary>
    public static Vector3 one => new Vector3(1f, 1f, 1f);

    /// <summary>
    ///   <para>Shorthand for writing Vector3(0, 0, 1).</para>
    /// </summary>
    public static Vector3 forward => new Vector3(0f, 0f, 1f);

    /// <summary>
    ///   <para>Shorthand for writing Vector3(0, 0, -1).</para>
    /// </summary>
    public static Vector3 back => new Vector3(0f, 0f, -1f);

    /// <summary>
    ///   <para>Shorthand for writing Vector3(0, 1, 0).</para>
    /// </summary>
    public static Vector3 up => new Vector3(0f, 1f, 0f);

    /// <summary>
    ///   <para>Shorthand for writing Vector3(0, -1, 0).</para>
    /// </summary>
    public static Vector3 down => new Vector3(0f, -1f, 0f);

    /// <summary>
    ///   <para>Shorthand for writing Vector3(-1, 0, 0).</para>
    /// </summary>
    public static Vector3 left => new Vector3(-1f, 0f, 0f);

    /// <summary>
    ///   <para>Shorthand for writing Vector3(1, 0, 0).</para>
    /// </summary>
    public static Vector3 right => new Vector3(1f, 0f, 0f);

    [Obsolete("Use Vector3.forward instead.")]
    public static Vector3 fwd => new Vector3(0f, 0f, 1f);

    /// <summary>
    ///   <para>Creates a new vector with given x, y, z components.</para>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    ///   <para>Creates a new vector with given x, y components and sets z to zero.</para>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Vector3(float x, float y)
    {
        this.x = x;
        this.y = y;
        z = 0f;
    }

    public static Vector3 Exclude(Vector3 excludeThis, Vector3 fromThat)
    {
        return fromThat - Vector3.Project(fromThat, excludeThis);
    }

    /// <summary>
    ///   <para>Linearly interpolates between two vectors.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        t = Mathf.Clamp01(t);
        return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
    }

    /// <summary>
    ///   <para>Linearly interpolates between two vectors.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
    }

    /// <summary>
    ///   <para>Moves a point current in a straight line towards a target point.</para>
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="maxDistanceDelta"></param>
    public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
    {
        Vector3 a = target - current;
        float magnitude = a.magnitude;
        Vector3 result;
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

    public static Vector3 SmoothDamp(float deltaTime, Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
    {
        return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    }

    public static Vector3 SmoothDamp(float deltaTime, Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
    {
        float maxSpeed = float.PositiveInfinity;
        return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    }

    public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
    {
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float num = 2f / smoothTime;
        float num2 = num * deltaTime;
        float d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
        Vector3 vector = current - target;
        Vector3 vector2 = target;
        float maxLength = maxSpeed * smoothTime;
        vector = Vector3.ClampMagnitude(vector, maxLength);
        target = current - vector;
        Vector3 vector3 = (currentVelocity + num * vector) * deltaTime;
        currentVelocity = (currentVelocity - num * vector3) * d;
        Vector3 vector4 = target + (vector + vector3) * d;
        if (Vector3.Dot(vector2 - current, vector4 - vector2) > 0f)
        {
            vector4 = vector2;
            currentVelocity = (vector4 - vector2) / deltaTime;
        }
        return vector4;
    }

    /// <summary>
    ///   <para>Set x, y and z components of an existing Vector3.</para>
    /// </summary>
    /// <param name="new_x"></param>
    /// <param name="new_y"></param>
    /// <param name="new_z"></param>
    public void Set(float new_x, float new_y, float new_z)
    {
        x = new_x;
        y = new_y;
        z = new_z;
    }

    /// <summary>
    ///   <para>Multiplies two vectors component-wise.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static Vector3 Scale(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    /// <summary>
    ///   <para>Multiplies every component of this vector by the same component of scale.</para>
    /// </summary>
    /// <param name="scale"></param>
    public void Scale(Vector3 scale)
    {
        x *= scale.x;
        y *= scale.y;
        z *= scale.z;
    }

    /// <summary>
    ///   <para>Cross Product of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
    }

    public override bool Equals(object other)
    {
        bool result;
        if (!(other is Vector3))
        {
            result = false;
        }
        else
        {
            Vector3 vector = (Vector3)other;
            result = (x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z));
        }
        return result;
    }

    /// <summary>
    ///   <para>Reflects a vector off the plane defined by a normal.</para>
    /// </summary>
    /// <param name="inDirection"></param>
    /// <param name="inNormal"></param>
    public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
    {
        return -2f * Vector3.Dot(inNormal, inDirection) * inNormal + inDirection;
    }

    /// <summary>
    ///   <para></para>
    /// </summary>
    /// <param name="value"></param>
    public static Vector3 Normalize(Vector3 value)
    {
        float num = Vector3.Magnitude(value);
        Vector3 result;
        if (num > 1E-05f)
        {
            result = value / num;
        }
        else
        {
            result = Vector3.zero;
        }
        return result;
    }

    /// <summary>
    ///   <para>Makes this vector have a magnitude of 1.</para>
    /// </summary>
    public void Normalize()
    {
        float num = Vector3.Magnitude(this);
        if (num > 1E-05f)
        {
            this /= num;
        }
        else
        {
            this = Vector3.zero;
        }
    }

    /// <summary>
    ///   <para>Dot Product of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static float Dot(Vector3 lhs, Vector3 rhs)
    {
        return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
    }

    /// <summary>
    ///   <para>Projects a vector onto another vector.</para>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="onNormal"></param>
    public static Vector3 Project(Vector3 vector, Vector3 onNormal)
    {
        float num = Vector3.Dot(onNormal, onNormal);
        Vector3 result;
        if (num < Mathf.Epsilon)
        {
            result = Vector3.zero;
        }
        else
        {
            result = onNormal * Vector3.Dot(vector, onNormal) / num;
        }
        return result;
    }

    /// <summary>
    ///   <para>Projects a vector onto a plane defined by a normal orthogonal to the plane.</para>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="planeNormal"></param>
    public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
    {
        return vector - Vector3.Project(vector, planeNormal);
    }

    /// <summary>
    ///   <para>Returns the angle in degrees between from and to.</para>
    /// </summary>
    /// <param name="from">The angle extends round from this vector.</param>
    /// <param name="to">The angle extends round to this vector.</param>
    public static float Angle(Vector3 from, Vector3 to)
    {
        // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
        float denominator = (float)System.Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
        if (denominator < kEpsilonNormalSqrt)
            return 0F;

        float dot = Mathf.Clamp(Dot(from, to) / denominator, -1F, 1F);
        return ((float)System.Math.Acos(dot)) * Mathf.Rad2Deg;
    }
        
    // The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
    // If you imagine the from and to vectors as lines on a piece of paper, both originating from the same point, then the /axis/ vector would point up out of the paper.
    // The measured angle between the two vectors would be positive in a clockwise direction and negative in an anti-clockwise direction.
    public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
    {
        float unsignedAngle = Angle(from, to);

        float cross_x = from.y * to.z - from.z * to.y;
        float cross_y = from.z * to.x - from.x * to.z;
        float cross_z = from.x * to.y - from.y * to.x;
        float sign = Mathf.Sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
        return unsignedAngle * sign;
    }

    /// <summary>
    ///   <para>Returns the distance between a and b.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static float Distance(Vector3 a, Vector3 b)
    {
        Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }

    //Projects a vector onto a plane. The output is not normalized.
    public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
    {
        return vector - (Vector3.Dot(vector, planeNormal)*planeNormal);
    }

    public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
    {
        Vector3 perpVector;
        float angle;

        //Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
        perpVector = Vector3.Cross(normal, referenceVector);

        //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
        angle = Vector3.Angle(referenceVector, otherVector);
        angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));

        return angle;
    }
        
    /// <summary>
    ///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="maxLength"></param>
    public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
    {
        Vector3 result;
        if (vector.sqrMagnitude > maxLength * maxLength)
        {
            result = vector.normalized * maxLength;
        }
        else
        {
            result = vector;
        }
        return result;
    }

    public static float Magnitude(Vector3 a)
    {
        return Mathf.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
    }

    public static float SqrMagnitude(Vector3 a)
    {
        return a.x * a.x + a.y * a.y + a.z * a.z;
    }

    /// <summary>
    ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static Vector3 Min(Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
    }

    /// <summary>
    ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static Vector3 Max(Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
    }

    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vector3 operator -(Vector3 a)
    {
        return new Vector3(-a.x, -a.y, -a.z);
    }

    public static Vector3 operator *(Vector3 a, float d)
    {
        return new Vector3(a.x * d, a.y * d, a.z * d);
    }

    public static Vector3 operator *(float d, Vector3 a)
    {
        return new Vector3(a.x * d, a.y * d, a.z * d);
    }

    public static Vector3 operator /(Vector3 a, float d)
    {
        return new Vector3(a.x / d, a.y / d, a.z / d);
    }

    public static bool operator ==(Vector3 lhs, Vector3 rhs)
    {
        return Vector3.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
    }

    public static bool operator !=(Vector3 lhs, Vector3 rhs)
    {
        return Vector3.SqrMagnitude(lhs - rhs) >= 9.99999944E-11f;
    }

    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public override string ToString()
    {
        return string.Format("({0:F1}, {1:F1}, {2:F1})", new object[]
        {
            x,
            y,
            z
        });
    }

    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public string ToString(string format)
    {
        return string.Format("({0}, {1}, {2})", new object[]
        {
            x.ToString(format),
            y.ToString(format),
            z.ToString(format)
        });
    }

    [Obsolete("Use Vector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
    public static float AngleBetween(Vector3 from, Vector3 to)
    {
        return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f));
    }

    public static implicit  operator Vector3(System.Numerics.Vector3 p) => new Vector3(p.X, p.Y, p.Z);
    public static implicit  operator System.Numerics.Vector3(Vector3 p) => new System.Numerics.Vector3(p.x, p.y, p.z);

    public static implicit  operator float[](Vector3 p) => new []{p.x, p.y, p.z};
    public static implicit operator Vector3(float[] p) => new Vector3(p[0], p[1], p[2]);
        
}