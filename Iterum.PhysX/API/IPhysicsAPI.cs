// ReSharper disable InconsistentNaming

using AdvancedDLSupport;

namespace Iterum.Physics.PhysXImpl;

public delegate void OverlapCallback(int index, long nRef);
public delegate void RaycastCallback(int index, long nRef);

public delegate void ErrorCallbackFunc(string message);

public delegate void DebugLogFunc(string message);

public delegate void DebugLogErrorFunc(string message);

public delegate void ContactReportCallbackFunc(int index, int count, long ref0, long ref1, APIVec3 normal, APIVec3 position, APIVec3 impulse, float separation, int faceIndex0, int faceIndex1);

public delegate void TriggerReportCallbackFunc(long ref0, long ref1);

public interface IPhysicsAPI
{
    void characterUpdate(long nRef, float elapsed, float minDist);
    void charactersUpdate(long refScene, float elapsed, float minDist);
    void setControllerDirection(long nRef, APIVec3 dir);
    long createRaycastBuffer(int max);
    long createOverlapBuffer(int max);
    int sceneOverlap(long refScene, long refOverlapBuffer, long refGeo, APIVec3 pos, OverlapCallback callback);
    int sceneRaycast(long refScene, long refRaycastBuffer, APIVec3 origin, APIVec3 unitDir, float distance,
        RaycastCallback callback);

    long createConvexMesh(APIVec3[] vertices, int pointsCount);
    void cleanupConvexMesh(long nRef);
    void cleanupTriangleMesh(long nRef);


    long createSphereGeometry(float radius);
    long createCapsuleGeometry(float radius, float halfHeight);
    long createBoxGeometry(APIVec3 half);
    void cleanupGeometry(long nRef);

    long createRigidStatic(int geoType, long nRefGeo, long nRefScene, long nRefMat, APIVec3 pos, APIQuat quat, bool isTrigger);
    void destroyRigidStatic(long nRef);
    
    long createTerrain(float[] heightmap, float hfScale, long hfSize,
        float thickness, float convexEdgeThreshold, bool noBoundaries,
        float heightScale, float rowScale, float columnScale,
        long refScene, long refMat, APIVec3 pos);
    void destroyTerrain(long nRef);

    APITrans getRigidDynamicTransform(long nRef);
    void setRigidDynamicTransform(long nRef, APITrans t);


    APIVec3 getRigidStaticPosition(long nRef);
    APIQuat getRigidStaticRotation(long nRef);
    void setRigidStaticPosition(long nRef, APIVec3 p);
    void setRigidStaticRotation(long nRef, APIQuat q);
    void setRigidDynamicDisable(long nRef, bool disabled);
    void setRigidDynamicWord(long nRef, uint word);
    
    APIVec3 getTerrainPosition(long nRef);
    void setTerrainPosition(long nRef, APIVec3 t);


    long createRigidDynamic(int geoType, int refGeoCount, long[] refGeo, long nRefScene, long nRefMat, bool kinematic, bool ccd, bool retain,
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




    long createCapsuleCharacter(long nRefScene, long nRefMat, APIVec3 pos, APIVec3 up, float height, float radius,
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
        TriggerReportCallbackFunc triggerFunc, bool enableCCD, bool enableEnhancedDetermenism);

    void cleanupScene(long nRef);
    long getSceneTimestamp(long nRef);

    void initLog([DelegateLifetime(DelegateLifetime.Persistent)]
        DebugLogFunc func, [DelegateLifetime(DelegateLifetime.Persistent)]
        DebugLogErrorFunc func2);

    void initPhysics(bool isCreatePvd, int numThreads, float toleranceLength, float toleranceSpeed,
        [DelegateLifetime(DelegateLifetime.Persistent)]
        ErrorCallbackFunc func);

    long createMaterial(float staticFriction, float dynamicFriction, float restitution);
    void cleanupMaterial(long nRef);

    void modifyTerrain(long nRef, float[] heightmap, long startCol, long startRow, long nbCols, long nbRows, float hfScale, bool shrinkBounds);

    APIVec4 computePenetration(long refGeo1, int geoType1, long refGeo2, int geoType2, APITrans t1, APITrans t2);


    void stepPhysics(long nRef, float dt);
        
    void cleanupPhysics();

}