using System;
using AdvancedDLSupport;
using Magistr.Framework.Physics;
using Magistr.Math;

namespace Magistr.Physics.PhysXImplCore
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
        
        public void InitPhysics()
        {
            if (isCreated) return;
            
            var builder = new NativeLibraryBuilder(ImplementationOptions.UseIndirectCalls | ImplementationOptions.EnableOptimizations);
            API = builder.ActivateInterface<IPhysicsAPI>("PhysXSharpNative");

            API.initLog(LogDebug, LogError);

            const bool isCreatePvd = true;

#if !DEBUG
                isCreatePvd = false;
#endif

            API.initPhysics(isCreatePvd, Environment.ProcessorCount, 1, 5, LogCritical);
                
            API.initGlobalMaterial(0.99f, 0.99f, 0.5f);

            isCreated = true;

        }

        private void LogDebug(string message)
        {
            Log.Debug.Log("PhysX", message, ConsoleColor.Yellow);
        }

        private void LogCritical(string message)
        {
            Log.Debug.LogError("PhysX Critical", message);
        }

        private void LogError(string message)
        {
            Log.Debug.LogError("PhysX", message);
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

        public IGeometry LoadTriangleMeshGeometry(string name)
        {
            return new ModelGeometry(GeoType.TriangleMeshGeometry, name);
        }
    }
}