using Magistr.Math;
using System;

namespace Magistr.Physics.PhysXImplCore
{
    public struct APIVec3
    {
        public float x;
        public float y;
        public float z;
    }
    public struct APIDoubleVec3
    {
        public double x;
        public double y;
        public double z;
    }
    public struct APIQuat
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
    public delegate void OverlapCallback(int t1);
    public interface IPhysicsAPI
    {
        void setControllerDirection(int controllerIndex, int sceneIndex, APIVec3 dir);
        int sceneOverlap(int sceneIndex, int geoIndex, APIVec3 pos, OverlapCallback callback);
        int createSphereGeometry(float radius);
        int createBoxGeometry(APIVec3 half);
        void cleanupGeometry(int index);
        int createRigidStatic(int geoIndex, int userDataReference, int sceneIndex, APIVec3 pos, APIQuat quat);
        void destroyRigidStatic(int rigidStaticIndex, int sceneIndex);
        APIVec3 getRigidStaticPosition(int rigidStaticIndex, int sceneIndex);
        APIQuat getRigidStaticRotation(int rigidStaticIndex, int sceneIndex);
        void setRigidStaticPosition(int rigidStaticIndex, int sceneIndex, APIVec3 p);
        void setRigidStaticRotation(int rigidStaticIndex, int sceneIndex, APIQuat q);
        int createRigidDynamic(int geoIndex, int sceneIndex, APIVec3 pos, APIQuat quat);
        void destroyRigidDynamic(int rigidDynamicIndex, int sceneIndex);
        int createCapsuleCharacter(int sceneIndex, int userDataIndex, APIVec3 pos, APIVec3 up, float height, float radius);
        void destroyController(int controllerIndex, int sceneIndex);
        APIVec3 getControllerPosition(int controllerIndex, int sceneIndex);
        APIVec3 getControllerFootPosition(int controllerIndex, int sceneIndex);
        void setControllerPosition(int controllerIndex, int sceneIndex, APIDoubleVec3 p);
        void setControllerFootPosition(int controllerIndex, int sceneIndex, APIDoubleVec3 p);
        int createScene(APIVec3 gravity);
        void stepPhysics(int sceneIndex, float dt);
        void cleanupScene(int sceneIndex);
        int getSceneTimestamp(int sceneIndex);
        void initPhysics();
        void cleanupPhysics();
    }

    static class APIExt
    {
        public static APIVec3 ToApi(this Vector3 v)
        {
            return new APIVec3() { x = v.x, y = v.y, z = v.z };
        }
        public static APIDoubleVec3 ToApi(this DVector3 v)
        {
            return new APIDoubleVec3() { x = v.x, y = v.y, z = v.z };
        }

        public static APIQuat ToQuat(this Quaternion q)
        {
            return new APIQuat() { x = q.x, y = q.y, z = q.z, w = q.w };
        }

        public static Vector3 ToVector3(this APIVec3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static DVector3 ToVector3(this APIDoubleVec3 v)
        {
            return new DVector3(v.x, v.y, v.z);
        }


        public static Quaternion ToQuat(this APIQuat q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }
    }
}
