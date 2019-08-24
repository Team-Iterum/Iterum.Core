//using Magistr.Log;
//using Magistr.Math;
//using System;
//using System.Threading.Tasks;
//using PhysX

//namespace Magistr.Physics.PhysXImpl
//{
//    public class PhysXDynamicObject : IPhysicsObject
//    {
//        private RigidDynamic _px;
//        private PxRigidDynamicPtr px => Px().Result;
//        private Vector3 cachePosition;
//        public Vector3 Position { get => (Vector3)getPosition(); set => px.setGlobalPose(new PxTransform((PxVec3)value, px.getGlobalPose().q)); }
//        private Quaternion cacheRotation;
//        public Quaternion Rotation { get => (Quaternion)getRotation(); set => px.setGlobalPose(new PxTransform(px.getGlobalPose().p, (PxQuat)value)); }
//        public Vector3 LinearVelocity { get => (Vector3)(px.getLinearVelocity()); set => px.setLinearVelocity((PxVec3)value); }
//        public Vector3 AngularVelocity { get => (Vector3)px.getAngularVelocity(); set => px.setAngularVelocity((PxVec3)value); }
//        public float AngularDamping { get => px.getAngularDamping(); set => px.setAngularDamping(value); }
//        public float Mass { get => px.getMass(); set => px.setMass(value); }

//        public bool IsDynamic => true;
//        private bool destroyed = false;
//        public bool IsDestroyed => destroyed;

//        private IPhysicsWorld world;
//        public IPhysicsWorld OwnerWorld { get => world; }

//        private async Task<PxRigidDynamicPtr> Px()
//        {
//            await world.WaitEndOfFrame();
//            return _px;
//        }

//        private Vector3 getPosition()
//        {
//            Task.Run(async () =>
//            {
//                await world.WaitEndOfFrame();
//                cachePosition = (Vector3)_px.getGlobalPose().p;
//            }).ConfigureAwait(false);
//            return cachePosition;
//        }
//        private Quaternion getRotation()
//        {
//            Task.Run(async () =>
//            {
//                await world.WaitEndOfFrame();
//                cacheRotation = (Quaternion)_px.getGlobalPose().q;
//            }).ConfigureAwait(false);
//            return cacheRotation;
//        }

//        internal PhysXDynamicObject(PxRigidDynamicPtr px, IPhysicsWorld w)
//        {
//            this._px = px;
//            this.world = w;
//        }

//        public void Destroy()
//        {
//            var actor = Px().Result;
//            actor.getScene().removeActor(actor);
//            destroyed = true;


//        }

//    }
//}
