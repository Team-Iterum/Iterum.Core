﻿using System;
using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Things;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        AddRemoveThings Overlap(Vector3 position, List<IThing> except, bool staticOnly);

        IPhysicsStaticObject CreateStatic(IGeometry geometry, Vector3 position, Quaternion rotation);
        IPhysicsDynamicObject CreateDynamic(IGeometry geometry, bool kinematic, float mass, Vector3 position, Quaternion rotation);
        IPhysicsCharacter CreateCapsuleCharacter(Vector3 position, Vector3 up, float height, float radius);
        

    }

    public struct ContactReport
    {
        public IThing obj0;
        public IThing obj1;
        public Vector3 normal;
        public Vector3 position;
        public Vector3 impulse;
        public float separation;
    }

    public interface IPhysics
    {
        void InitPhysics();
        IGeometry CreateTriangleMeshGeometry(IModelData modelData);
        IGeometry CreateConvexMeshGeometry(IModelData modelData);
        IGeometry CreateSphereGeometry(float radius);
        IGeometry CreateBoxGeometry(Vector3 size);
    }

    public struct AddRemoveThings
    {
        public List<IThing> Add;
        public List<IThing> Remove;
    } 
}
