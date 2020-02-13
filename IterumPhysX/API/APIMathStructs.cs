﻿using Magistr.Math;
// ReSharper disable MemberCanBePrivate.Global

namespace Magistr.Physics.PhysXImpl
{
    public struct APIVec3
    {
        public float x;
        public float y;
        public float z;

        public static implicit operator APIVec3(Vector3 p)
        {
            return new APIVec3() {x = p.x, y = p.y, z = p.z};
        }
        public static implicit operator Vector3(APIVec3 p)
        {
            return new Vector3(p.x, p.y, p.z);
        }
    }
    public struct APIDoubleVec3
    {
        public double x;
        public double y;
        public double z;

        public static implicit operator APIDoubleVec3(Vector3 p)
        {
            return new APIDoubleVec3() {x = p.x, y = p.y, z = p.z};
        }
        public static implicit operator APIDoubleVec3(DVector3 p)
        {
            return new APIDoubleVec3() {x = p.x, y = p.y, z = p.z};
        }
        public static implicit operator DVector3(APIDoubleVec3 p)
        {
            return new DVector3(p.x, p.y, p.z);
        }
    }
    public struct APIQuat
    {
        public float x;
        public float y;
        public float z;
        public float w;

        
        public static implicit operator APIQuat(Quaternion p)
        {
            return new APIQuat() {x = p.x, y = p.y, z = p.z, w = p.w};
        }
        public static implicit operator Quaternion(APIQuat p)
        {
            return new Quaternion(p.x, p.y, p.z, p.w);
        }
    }
}