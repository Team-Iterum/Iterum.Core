// ReSharper disable InconsistentNaming

using AdvancedDLSupport;

namespace Iterum.Physics.PhysXImpl
{

    public delegate void OverlapCallback(int index, long nRef);
    public delegate void RaycastCallback(int index, long nRef);

    public delegate void ErrorCallbackFunc(string message);

    public delegate void DebugLogFunc(string message);

    public delegate void DebugLogErrorFunc(string message);

    public delegate void ContactReportCallbackFunc(long ref0, long ref1, APIVec3 normal, APIVec3 position,
        APIVec3 impulse, float separation);

    public delegate void TriggerReportCallbackFunc(long ref0, long ref1);

    public interface IPhysicsAPI
    {
        void characterUpdate(long nRef, float elapsed, float minDist);
        void charactersUpdate(long refScene, float elapsed, float minDist);
        void setControllerDirection(long nRef, APIVec3 dir);
        long createRaycastBuffer10();
        long createOverlapBuffer1();
        long createOverlapBuffer10();
        long createOverlapBuffer1000();
        int sceneOverlap1(long refScene, long refOverlapBuffer, long refGeo, APIVec3 pos, OverlapCallback callback);
        int sceneOverlap10(long refScene, long refOverlapBuffer, long refGeo, APIVec3 pos, OverlapCallback callback);
        int sceneOverlap1000(long refScene, long refOverlapBuffer, long refGeo, APIVec3 pos, OverlapCallback callback);
        int sceneRaycast(long refScene, long refRaycastBuffer, APIVec3 origin, APIVec3 unitDir, float distance,
            RaycastCallback callback);

        long createConvexMesh(APIVec3[] vertices, int pointsCount);
        void cleanupConvexMesh(long nRef);
        void cleanupTriangleMesh(long nRef);


        long createSphereGeometry(float radius);
        long createBoxGeometry(APIVec3 half);
        void cleanupGeometry(long nRef);

        long createRigidStatic(int geoType, long nRefGeo, long nRefScene, APIVec3 pos, APIQuat quat, bool isTrigger);
        void destroyRigidStatic(long nRef);

        APITrans getRigidDynamicTransform(long nRef);
        void setRigidDynamicTransform(long nRef, APITrans t);


        APIVec3 getRigidStaticPosition(long nRef);
        APIQuat getRigidStaticRotation(long nRef);
        void setRigidStaticPosition(long nRef, APIVec3 p);
        void setRigidStaticRotation(long nRef, APIQuat q);
        void setRigidDynamicDisable(long nRef, bool disabled);
        void setRigidDynamicWord(long nRef, uint word);


        long createRigidDynamic(int geoType, int refGeoCount, long[] refGeo, long nRefScene, bool kinematic, bool ccd, bool retain,
            bool disableGravity, bool isTrigger, float mass, uint word, APIVec3 pos, APIQuat quat);

        void destroyRigidDynamic(long nRef);

        void setRigidDynamicKinematicTarget(long nRef, APITrans t);

        void setRigidDynamicLinearVelocity(long nRef, APIVec3 v);
        void setRigidDynamicAngularVelocity(long nRef, APIVec3 v);


        void setRigidDynamicLinearDamping(long nRef, float v);
        void setRigidDynamicAngularDamping(long nRef, float v);

        void addRigidDynamicForce(long nRef, APIVec3 v, ForceMode mode);
        void addRigidDynamicTorque(long nRef, APIVec3 v, ForceMode mode);

        void setRigidDynamicMaxLinearVelocity(long nRef, float v);
        void setRigidDynamicMaxAngularVelocity(long nRef, float v);

        APIVec3 getRigidDynamicAngularVelocity(long nRef);
        APIVec3 getRigidDynamicLinearVelocity(long nRef);
        float getRigidDynamicMaxAngularVelocity(long nRef);
        float getRigidDynamicMaxLinearVelocity(long nRef);




        long createCapsuleCharacter(long nRefScene, APIVec3 pos, APIVec3 up, float height, float radius,
            float stepOffset);

        void destroyController(long nRef);

        APIDoubleVec3 getControllerPosition(long nRef);
        APIDoubleVec3 getControllerFootPosition(long nRef);

        void setControllerPosition(long nRef, APIDoubleVec3 p);
        void setControllerFootPosition(long nRef, APIDoubleVec3 p);


        long loadTriangleMesh(string name);

        long createScene(APIVec3 gravity, [DelegateLifetime(DelegateLifetime.Persistent)]
            ContactReportCallbackFunc func,
            [DelegateLifetime(DelegateLifetime.Persistent)]
            TriggerReportCallbackFunc triggerFunc);

        void cleanupScene(long nRef);
        long getSceneTimestamp(long nRef);

        void initLog([DelegateLifetime(DelegateLifetime.Persistent)]
            DebugLogFunc func, [DelegateLifetime(DelegateLifetime.Persistent)]
            DebugLogErrorFunc func2);

        void initPhysics(bool isCreatePvd, int numThreads, float toleranceLength, float toleranceSpeed,
            [DelegateLifetime(DelegateLifetime.Persistent)]
            ErrorCallbackFunc func);

        void initGlobalMaterial(float staticFriction, float dynamicFriction, float restitution);
        void stepPhysics(long nRef, float dt);
        void cleanupPhysics();

    }
}
