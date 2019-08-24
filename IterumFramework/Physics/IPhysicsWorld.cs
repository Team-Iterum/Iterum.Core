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

        bool IsDestoyed { get; }
        bool IsCreated { get; }
        bool IsRunning { get; }
        int Timestamp { get; set; }
        float SceneFrame { get; }
        float DeltaTime { get; }

        int TPS { get; set; }

        float OverlapSphereRadius { get; set; }

        System.Guid WorldUid { get; }
        Vector3 Gravity { get; }

        Task WaitEndOfFrame();

        Task<(List<IThing>, List<IThing>)> Overlap(Vector3 pos, List<IThing> excpect, bool staticOnly);

        IPhysicsStaticObject CreateStatic(IGeometry geometry, Vector3 pos, Quaternion rot);
        IPhysicsDynamicObject CreateDynamic(IGeometry geometry, Vector3 position, Quaternion rotation);
        IPhysicsCharaceter CreateCapsuleCharacter(Vector3 pos, Vector3 up, float height, float radius);
        IGeometry CreateStaticModelGeometry(IModelData modelData);
        IGeometry CreateSphereGeometry(float radius);
        IGeometry CreateBoxGeometry(Vector3 size);

    }
}
