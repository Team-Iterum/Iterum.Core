using System;
using Magistr.Math;
using Magistr.Things;
using System.Collections.Generic;

namespace Magistr.Physics
{
    public interface IPhysicsWorld
    {
        void Create();
        void Destroy();
        void Start();
        void Stop();

        bool IsDestroyed { get; }
        bool IsCreated { get; }
        bool IsRunning { get; }

        int Timestamp { get; }
        float SceneFrame { get; }
        float DeltaTime { get; }

        int TPS { get; set; }

        float OverlapSphereRadius { get; set; }

        Vector3 Gravity { get; set; }

        event EventHandler<ContactReport> ContactReport;
        
        AddRemoveThings Overlap(Vector3 position, List<IThing> except);

        public IStaticObject CreateStatic(IGeometry geometry, Transform transform, bool isTrigger);

        public IDynamicObject CreateDynamic(IGeometry geometry, bool kinematic, bool isTrigger, bool disableGravity,
            float mass, Transform transform);
        public IPhysicsCharacter CreateCapsuleCharacter(Vector3 position, Vector3 up, float height, float radius);

    }

    public struct ContactReport
    {
        public IThing obj0;
        public IThing obj1;
        public Vector3 normal;
        public Vector3 position;
        public Vector3 impulse;
        public float separation;
        public bool IsTrigger;
    }

    public struct AddRemoveThings
    {
        public IEnumerable<IThing> Add;
        public IEnumerable<IThing> Remove;
    } 
}
