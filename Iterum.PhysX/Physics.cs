using System;
using System.Collections.Generic;
using AdvancedDLSupport;
using Iterum.Math;

namespace Iterum.Physics.PhysXImpl
{
    public static class PhysicsAlias
    {
        public static Physics GlobalPhysics;
        public static IPhysicsAPI API => GlobalPhysics.API;
    }

    public class Physics : IPhysics
    {
        private bool isCreated;

        public IPhysicsAPI API { get; private set; }

        public List<IMaterial> Materials { get; }  = new ();

        public void Init(bool isCreatePvd = true, float toleranceLength = 1, float toleranceSpeed = 5)
        {
            if (isCreated) return;

            var builder =
                new NativeLibraryBuilder(ImplementationOptions.UseIndirectCalls |
                                         ImplementationOptions.EnableOptimizations);
            
            API = builder.ActivateInterface<IPhysicsAPI>("PhysXSharpNative");

            API.initLog(LogDebug, LogError);
            
            API.initPhysics(isCreatePvd, Environment.ProcessorCount, toleranceLength, toleranceSpeed, LogCritical);
            
            isCreated = true;
        }

        public IMaterial CreateMaterial(float staticFriction = 0.5f, float dynamicFriction = 0.5f, float restitution = 0.5f)
        {
            var mat = new Material(staticFriction, dynamicFriction, restitution);
            Materials.Add(mat);
            return mat;
        }

        #region Logs
        private static void LogDebug(string message) => Console.WriteLine($"[PhysX Debug] {message}");
        private static void LogCritical(string message) => Console.WriteLine($"[PhysX Critical] {message}");
        private static void LogError(string message) => Console.WriteLine($"[PhysX Error] {message}");

        #endregion

        #region Create geometries

        public IGeometry CreateTriangleMeshGeometry(IModelData modelData)
        {
            return new ModelGeometry(GeoType.TriangleMeshGeometry, modelData);
        }

        public IGeometry CreateConvexMeshGeometry(IModelData modelData)
        {
            return new ModelGeometry(GeoType.ConvexMeshGeometry, modelData);
        }

        public IGeometry CreateSphereGeometry(float radius)
        {
            return new SphereGeometry(radius);
        }

        public IGeometry CreateBoxGeometry(Vector3 size)
        {
            return new BoxGeometry(size);
        }
        
        public Buffer CreateOverlapBuffer1000()
        {
            return new Buffer(API.createOverlapBuffer1000(), 1000);
        }
        public Buffer CreateOverlapBuffer10()
        {
            return new Buffer(API.createOverlapBuffer10(), 10);
        }
        public Buffer CreateOverlapBuffer1()
        {
            return new Buffer(API.createOverlapBuffer1(), 1);
        }
        
        public Buffer CreateRaycastBuffer10()
        {
            return new Buffer(API.createRaycastBuffer10(), 10);
        }
        

        public static IGeometry LoadTriangleMeshGeometry(string name)
        {
            return new ModelGeometry(GeoType.TriangleMeshGeometry, name);
        }

        #endregion

    }
}