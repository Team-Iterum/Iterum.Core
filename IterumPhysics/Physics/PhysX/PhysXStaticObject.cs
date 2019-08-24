using Magistr.Math;
using Magistr.Things;
using PhysX;
using System.Threading.Tasks;
using Quat = System.Numerics.Quaternion;
using Vec3 = System.Numerics.Vector3;
namespace Magistr.Physics.PhysXImpl
{
    public class PhysXStaticObject : IPhysicsStaticObject
    {
        #region IPhysicsObject
        private RigidStatic _px;
        private RigidStatic px => Px().Result;
        private Vector3 cachePosition;
        public Vector3 Position { get => getPosition(); set { px.GlobalPosePosition = (Vec3)value; cachePosition = value; } }
        private Quaternion cacheRotation;
        public Quaternion Rotation { get => getRotation(); set { px.GlobalPoseQuat = (Quat)value; cacheRotation = value; } }
        public bool IsDestroyed { get; private set; } = false;
        public IPhysicsWorld OwnerWorld { get; }
        public IThing Thing { get; set; }

        public void Destroy()
        {
            var actor = Px().Result;
            actor.Scene.RemoveActor(actor);
            IsDestroyed = true;


        }

        private Vector3 getPosition()
        {
            Task.Run(async () =>
            {
                await OwnerWorld.WaitEndOfFrame();
                cachePosition = (Vector3)_px.GlobalPosePosition;
            }).ConfigureAwait(false);
            return cachePosition;
        }
        private Quaternion getRotation()
        {
            Task.Run(async () =>
            {
                await OwnerWorld.WaitEndOfFrame();
                cacheRotation = (Quaternion)_px.GlobalPoseQuat;
            }).ConfigureAwait(false);
            return cacheRotation;
        }
        private async Task<RigidStatic> Px()
        {
            await OwnerWorld.WaitEndOfFrame();
            return _px;
        }
        #endregion

        internal PhysXStaticObject(RigidStatic px, IPhysicsWorld w)
        {
            this._px = px;
            this.OwnerWorld = w;
        }

    }
}