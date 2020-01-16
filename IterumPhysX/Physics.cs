using System;
using AdvancedDLSupport;
using Magistr.Framework.Physics;
using Magistr.Math;

namespace Magistr.Physics.PhysXImplCore
{

    public static class PhysicsAlias
    {
        public static Physics Physics { get; private set; }

        public static IPhysicsAPI API => Physics.API;

        public static void Set(Physics physics)
        {
            Physics = physics;
        }
    }
    public class Physics : IPhysics
    {
        private bool physXCreated;

        public IPhysicsAPI API { get; private set; }
        
        public void InitPhysics()
        {
            if (!physXCreated)
            {
                var builder = new NativeLibraryBuilder();
                API = builder.ActivateInterface<IPhysicsAPI>("PhysXSharpNative");

                API.initLog((s) => Log.Debug.Log("PhysX", s, ConsoleColor.Yellow), (s) => Log.Debug.LogError("PhysX", s));
                
                API.initPhysics(true, Environment.ProcessorCount, 1, 5, (s) => Log.Debug.LogError("PhysX Critical", s));
                API.initGlobalMaterial(0.99f, 0.99f, 0.5f);

                physXCreated = true;
            }

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