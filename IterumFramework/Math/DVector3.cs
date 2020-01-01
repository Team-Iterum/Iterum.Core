using System;
using System.ComponentModel;

namespace Magistr.Math
{
    /// <summary>
    ///   <para>Representation of 3D vectors and points.</para>
    /// </summary>
    [Serializable]
    public struct DVector3
    {
        public const double kEpsilon = 1E-05f;

        /// <summary>
        ///   <para>X component of the vector.</para>
        /// </summary>
        public double x;

        /// <summary>
        ///   <para>Y component of the vector.</para>
        /// </summary>
        public double y;

        /// <summary>
        ///   <para>Z component of the vector.</para>
        /// </summary>
        public double z;

        public double this[int index]
        {
            get
            {
                double result;
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
                        throw new IndexOutOfRangeException("Invalid DVector3 index!");
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
                        throw new IndexOutOfRangeException("Invalid DVector3 index!");
                }
            }
        }

        /// <summary>
        ///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
        /// </summary>
        public DVector3 normalized => DVector3.Normalize(this);

        /// <summary>
        ///   <para>Returns the length of this vector (Read Only).</para>
        /// </summary>
        public double magnitude => System.Math.Sqrt(x * x + y * y + z * z);

        /// <summary>
        ///   <para>Returns the squared length of this vector (Read Only).</para>
        /// </summary>
        public double sqrMagnitude => x * x + y * y + z * z;

        /// <summary>
        ///   <para>Shorthand for writing DVector3(0, 0, 0).</para>
        /// </summary>
        public static DVector3 zero => new DVector3(0f, 0f, 0f);

        /// <summary>
        ///   <para>Shorthand for writing DVector3(1, 1, 1).</para>
        /// </summary>
        public static DVector3 one => new DVector3(1f, 1f, 1f);

        /// <summary>
        ///   <para>Shorthand for writing DVector3(0, 0, 1).</para>
        /// </summary>
        public static DVector3 forward => new DVector3(0f, 0f, 1f);

        /// <summary>
        ///   <para>Shorthand for writing DVector3(0, 0, -1).</para>
        /// </summary>
        public static DVector3 back => new DVector3(0f, 0f, -1f);

        /// <summary>
        ///   <para>Shorthand for writing DVector3(0, 1, 0).</para>
        /// </summary>
        public static DVector3 up => new DVector3(0f, 1f, 0f);

        /// <summary>
        ///   <para>Shorthand for writing DVector3(0, -1, 0).</para>
        /// </summary>
        public static DVector3 down => new DVector3(0f, -1f, 0f);

        /// <summary>
        ///   <para>Shorthand for writing DVector3(-1, 0, 0).</para>
        /// </summary>
        public static DVector3 left => new DVector3(-1f, 0f, 0f);

        /// <summary>
        ///   <para>Shorthand for writing DVector3(1, 0, 0).</para>
        /// </summary>
        public static DVector3 right => new DVector3(1f, 0f, 0f);

        [Obsolete("Use DVector3.forward instead.")]
        public static DVector3 fwd => new DVector3(0f, 0f, 1f);

        /// <summary>
        ///   <para>Creates a new vector with given x, y, z components.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public DVector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public DVector3(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        /// <summary>
        ///   <para>Creates a new vector with given x, y components and sets z to zero.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public DVector3(double x, double y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
        }

        public static DVector3 Exclude(DVector3 excludeThis, DVector3 fromThat)
        {
            return fromThat - DVector3.Project(fromThat, excludeThis);
        }

        /// <summary>
        ///   <para>Linearly interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static DVector3 Lerp(DVector3 a, DVector3 b, double t)
        {
            t = Mathf.Clamp01(t);
            return new DVector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        /// <summary>
        ///   <para>Linearly interpolates between two vectors.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static DVector3 LerpUnclamped(DVector3 a, DVector3 b, double t)
        {
            return new DVector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        /// <summary>
        ///   <para>Moves a point current in a straight line towards a target point.</para>
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDistanceDelta"></param>
        public static DVector3 MoveTowards(DVector3 current, DVector3 target, double maxDistanceDelta)
        {
            DVector3 a = target - current;
            double magnitude = a.magnitude;
            DVector3 result;
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

        public static DVector3 SmoothDamp(double deltaTime, DVector3 current, DVector3 target, ref DVector3 currentVelocity, double smoothTime, double maxSpeed)
        {
            return DVector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static DVector3 SmoothDamp(double deltaTime, DVector3 current, DVector3 target, ref DVector3 currentVelocity, double smoothTime)
        {
            double maxSpeed = double.PositiveInfinity;
            return DVector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static DVector3 SmoothDamp(DVector3 current, DVector3 target, ref DVector3 currentVelocity, double smoothTime, [DefaultValue("Mathf.Infinity")] double maxSpeed, [DefaultValue("Time.deltaTime")] double deltaTime)
        {
            smoothTime = System.Math.Max(0.0001f, smoothTime);
            double num = 2f / smoothTime;
            double num2 = num * deltaTime;
            double d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            DVector3 vector = current - target;
            DVector3 vector2 = target;
            double maxLength = maxSpeed * smoothTime;
            vector = DVector3.ClampMagnitude(vector, maxLength);
            target = current - vector;
            DVector3 vector3 = (currentVelocity + num * vector) * deltaTime;
            currentVelocity = (currentVelocity - num * vector3) * d;
            DVector3 vector4 = target + (vector + vector3) * d;
            if (DVector3.Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = (vector4 - vector2) / deltaTime;
            }
            return vector4;
        }

        /// <summary>
        ///   <para>Set x, y and z components of an existing DVector3.</para>
        /// </summary>
        /// <param name="new_x"></param>
        /// <param name="new_y"></param>
        /// <param name="new_z"></param>
        public void Set(double new_x, double new_y, double new_z)
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
        public static DVector3 Scale(DVector3 a, DVector3 b)
        {
            return new DVector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary>
        ///   <para>Multiplies every component of this vector by the same component of scale.</para>
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(DVector3 scale)
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
        public static DVector3 Cross(DVector3 lhs, DVector3 rhs)
        {
            return new DVector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
        }

        public override bool Equals(object other)
        {
            bool result;
            if (!(other is DVector3))
            {
                result = false;
            }
            else
            {
                DVector3 vector = (DVector3)other;
                result = (x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z));
            }
            return result;
        }

        /// <summary>
        ///   <para>Reflects a vector off the plane defined by a normal.</para>
        /// </summary>
        /// <param name="inDirection"></param>
        /// <param name="inNormal"></param>
        public static DVector3 Reflect(DVector3 inDirection, DVector3 inNormal)
        {
            return -2f * DVector3.Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        /// <summary>
        ///   <para></para>
        /// </summary>
        /// <param name="value"></param>
        public static DVector3 Normalize(DVector3 value)
        {
            double num = DVector3.Magnitude(value);
            DVector3 result;
            if (num > 1E-05f)
            {
                result = value / num;
            }
            else
            {
                result = DVector3.zero;
            }
            return result;
        }

        /// <summary>
        ///   <para>Makes this vector have a magnitude of 1.</para>
        /// </summary>
        public void Normalize()
        {
            double num = DVector3.Magnitude(this);
            if (num > 1E-05f)
            {
                this /= num;
            }
            else
            {
                this = DVector3.zero;
            }
        }

        /// <summary>
        ///   <para>Dot Product of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static double Dot(DVector3 lhs, DVector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        /// <summary>
        ///   <para>Projects a vector onto another vector.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="onNormal"></param>
        public static DVector3 Project(DVector3 vector, DVector3 onNormal)
        {
            double num = DVector3.Dot(onNormal, onNormal);
            DVector3 result;
            if (num < Mathf.Epsilon)
            {
                result = DVector3.zero;
            }
            else
            {
                result = onNormal * DVector3.Dot(vector, onNormal) / num;
            }
            return result;
        }

        /// <summary>
        ///   <para>Projects a vector onto a plane defined by a normal orthogonal to the plane.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="planeNormal"></param>
        public static DVector3 ProjectOnPlane(DVector3 vector, DVector3 planeNormal)
        {
            return vector - DVector3.Project(vector, planeNormal);
        }

        /// <summary>
        ///   <para>Returns the angle in degrees between from and to.</para>
        /// </summary>
        /// <param name="from">The angle extends round from this vector.</param>
        /// <param name="to">The angle extends round to this vector.</param>
        public static double Angle(DVector3 from, DVector3 to)
        {
            return System.Math.Acos(Mathf.Clamp(DVector3.Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
        }

        /// <summary>
        ///   <para>Returns the distance between a and b.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static double Distance(DVector3 a, DVector3 b)
        {
            DVector3 vector = new DVector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return System.Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        /// <summary>
        ///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="maxLength"></param>
        public static DVector3 ClampMagnitude(DVector3 vector, double maxLength)
        {
            DVector3 result;
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

        public static double Magnitude(DVector3 a)
        {
            return System.Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
        }

        public static double SqrMagnitude(DVector3 a)
        {
            return a.x * a.x + a.y * a.y + a.z * a.z;
        }

        /// <summary>
        ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static DVector3 Min(DVector3 lhs, DVector3 rhs)
        {
            return new DVector3(System.Math.Min(lhs.x, rhs.x), System.Math.Min(lhs.y, rhs.y), System.Math.Min(lhs.z, rhs.z));
        }

        /// <summary>
        ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static DVector3 Max(DVector3 lhs, DVector3 rhs)
        {
            return new DVector3(System.Math.Max(lhs.x, rhs.x), System.Math.Max(lhs.y, rhs.y), System.Math.Max(lhs.z, rhs.z));
        }

        public static DVector3 operator +(DVector3 a, DVector3 b)
        {
            return new DVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static DVector3 operator -(DVector3 a, DVector3 b)
        {
            return new DVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static DVector3 operator -(DVector3 a)
        {
            return new DVector3(-a.x, -a.y, -a.z);
        }

        public static DVector3 operator *(DVector3 a, double d)
        {
            return new DVector3(a.x * d, a.y * d, a.z * d);
        }

        public static DVector3 operator *(double d, DVector3 a)
        {
            return new DVector3(a.x * d, a.y * d, a.z * d);
        }

        public static DVector3 operator /(DVector3 a, double d)
        {
            return new DVector3(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(DVector3 lhs, DVector3 rhs)
        {
            return DVector3.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
        }

        public static bool operator !=(DVector3 lhs, DVector3 rhs)
        {
            return DVector3.SqrMagnitude(lhs - rhs) >= 9.99999944E-11f;
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

        [Obsolete("Use DVector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
        public static double AngleBetween(DVector3 from, DVector3 to)
        {
            return System.Math.Acos(Mathf.Clamp(DVector3.Dot(from.normalized, to.normalized), -1f, 1f));
        }

        public static implicit operator DVector3(Vector3 p)
        {
            return new DVector3() {x = p.x, y = p.y, z = p.z};
        }
        public static explicit operator Vector3(DVector3 p)
        {
            return new Vector3((float) p.x, (float) p.y, (float) p.z);
        }

    }
}
