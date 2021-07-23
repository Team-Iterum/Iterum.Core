using System;
using Iterum.Math;
using Iterum.Things;

// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global

namespace Iterum.Physics
{
    public interface IPhysicsWorld
    {
        public enum WorldState
        {
            None,
            Created,
            Destroyed,
        }
        
        void Create();
        void Destroy();

        public WorldState State { get; }
        
        int Timestamp { get; }

        event EventHandler<ContactReport> ContactReport;

        void Step(float dt, float subSteps = 1);
        
        int Raycast(Buffer refBuffer, Vector3 position, Vector3 direction, float maxDist);
        int SphereCast1(Buffer buffer, Vector3 position, IGeometry geometry);
        int SphereCast10(Buffer buffer, Vector3 position, IGeometry geometry);
        int SphereCast1000(Buffer buffer, Vector3 position, IGeometry geometry);

        IStaticObject     CreateStatic(IGeometry geometry, Vector3 pos, Quaternion quat, PhysicsObjectFlags flags, IMaterial mat);
        IDynamicObject    CreateDynamic(IGeometry[] geometries, Vector3 pos, Quaternion quat, PhysicsObjectFlags flags, float mass, uint word, IMaterial mat);
        IPhysicsCharacter CreateCapsuleCharacter(Vector3 pos, Vector3 up, float height, float radius, float stepOffset, IMaterial mat);
    }

    public struct ContactReport
    {
        // ReSharper disable InconsistentNaming
        
        public int index;
        public int count;
        
        public IThing obj0;
        public IThing obj1;
        
        public Vector3 normal;
        public Vector3 position;
        public Vector3 impulse;
        
        public float separation;
        
        public bool isTrigger;
        public int faceIndex0;
        public int faceIndex1;

        // ReSharper restore InconsistentNaming
    }
}
