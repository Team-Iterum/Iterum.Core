using System;
using AdvancedDLSupport;
using Magistr.Log;
using Magistr.Math;

namespace Magistr.Physics.PhysXImpl
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
        
        public void InitPhysics(float toleranceLength = 1, float toleranceSpeed = 5)
        {
            if (isCreated) return;
            
            var builder = new NativeLibraryBuilder(ImplementationOptions.UseIndirectCalls | ImplementationOptions.EnableOptimizations);
            API = builder.ActivateInterface<IPhysicsAPI>("PhysXSharpNative");

            API.initLog(LogDebug, LogError);

            bool isCreatePvd = true;

            #if !DEBUG
            isCreatePvd = false;
            #endif

            API.initPhysics(isCreatePvd, Environment.ProcessorCount, toleranceLength, toleranceSpeed, LogCritical);
                
            API.initGlobalMaterial(0.5f, 0.5f, 0.5f);

            isCreated = true;

        }

        private static void LogDebug(string message)
        {
            Debug.Log("PhysX Debug", message, ConsoleColor.Yellow);
        }

        private static void LogCritical(string message)
        {
            Debug.LogError("PhysX Critical", message);
        }

        private static void LogError(string message)
        {
            Debug.LogError("PhysX Error", message);
        }

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

        public static IGeometry LoadTriangleMeshGeometry(string name)
        {
            return new ModelGeometry(GeoType.TriangleMeshGeometry, name);
        }
    }
}