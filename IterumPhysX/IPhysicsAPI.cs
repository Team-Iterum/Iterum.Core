using Magistr.Math;

// ReSharper disable InconsistentNaming

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
    public delegate void ErrorCallbackFunc(string message);
    public interface IPhysicsAPI
    {
        void charactersUpdate(float elapsed, float minDist);
        void setControllerDirection(long nRef, APIVec3 dir);
        int sceneOverlap(long nRefScene, long nRefGeo, APIVec3 pos, OverlapCallback callback);
        
        
        
        long createTriangleMesh(APIVec3[] vertices, int pointsCount, uint[] indices, int triCount);
        long createConvexMesh(APIVec3[] vertices, int pointsCount);
        void cleanupConvexMesh(long nRef);
        void cleanupTriangleMesh(long nRef);


        long createSphereGeometry(float radius);
        long createBoxGeometry(APIVec3 half);
        void cleanupGeometry(long nRef);

        long createRigidStatic(int geoType, long nRefGeo, long nRefScene, APIVec3 pos, APIQuat quat);
        void destroyRigidStatic(long nRef);

        APIVec3 getRigidDynamicPosition(long nRef);
        APIQuat getRigidDynamicRotation(long nRef);
        void setRigidDynamicPosition(long nRef, APIVec3 p);
        void setRigidDynamicRotation(long nRef, APIQuat q);

        
        APIVec3 getRigidStaticPosition(long nRef);
        APIQuat getRigidStaticRotation(long nRef);
        void setRigidStaticPosition(long nRef, APIVec3 p);
        void setRigidStaticRotation(long nRef, APIQuat q);

        
        long createRigidDynamic(int geoType, long nRefGeo, long nRefScene, bool kinematic, float mass, APIVec3 pos, APIQuat quat);
        void destroyRigidDynamic(long nRef);

        void setRigidDynamicKinematicTarget(long nRef, APIVec3 p, APIQuat q);
        
        void setRigidDynamicLinearVelocity(long nRef, APIVec3 v);
        void setRigidDynamicMaxLinearVelocity(long nRef, float v);

        
        
        
        
        long createCapsuleCharacter(long nRefScene, APIVec3 pos, APIVec3 up, float height, float radius, float stepOffset);
        void destroyController(long nRef);
        
        APIDoubleVec3 getControllerPosition(long nRef);
        APIDoubleVec3 getControllerFootPosition(long nRef);

        void setControllerPosition(long nRef, APIDoubleVec3 p);
        void setControllerFootPosition(long nRef, APIDoubleVec3 p);
        
        
        
        long createScene(APIVec3 gravity);
        void cleanupScene(long nRef);
        long getSceneTimestamp(long nRef);
        
        void initPhysics(bool isCreatePvd, int numThreads, ErrorCallbackFunc func);
        void stepPhysics(long nRef, float dt);
        void cleanupPhysics();
    }

    internal static class APIExt
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
