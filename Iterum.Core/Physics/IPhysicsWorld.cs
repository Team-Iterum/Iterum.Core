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
        int SphereCast(Buffer buffer, Vector3 position, IGeometry geometry);

        IStaticObject     CreateStatic(IGeometry geometry, Transform transform, PhysicsObjectFlags flags);
        IDynamicObject    CreateDynamic(IGeometry[] geometries, Transform transform, PhysicsObjectFlags flags, float mass, uint word);
        IPhysicsCharacter CreateCapsuleCharacter(Transform transform, Vector3 up, float height, float radius);
    }

    public struct ContactReport
    {
        // ReSharper disable InconsistentNaming
        
        public IThing obj0;
        public IThing obj1;
        
        public Vector3 normal;
        public Vector3 position;
        public Vector3 impulse;
        
        public float separation;
        
        public bool isTrigger;
        
        // ReSharper restore InconsistentNaming
    }
}
