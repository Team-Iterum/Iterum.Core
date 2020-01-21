﻿using Magistr.Framework.Physics;
using Magistr.Math;
using Magistr.Things;

namespace Magistr.Physics.PhysXImplCore
{
    public class StaticObject : IPhysicsStaticObject
    {
        public long Ref { get; set; }

        private IPhysicsAPI api;
        private Scene scene;

        #region IPhysicsObject

        public Vector3 Position
        {
            get => api.getRigidStaticPosition(Ref);
            set => api.setRigidStaticPosition(Ref, value);
        }

        public Quaternion Rotation
        {
            get => api.getRigidStaticRotation(Ref);
            set => api.setRigidStaticRotation(Ref, value);
        }
        public bool IsDestroyed { get; private set; }

        public IThing Thing { get; set; }

        public void Destroy()
        {
            scene.Destroy(this);
            IsDestroyed = true;

        }
        #endregion

        public StaticObject(IGeometry geometry, Scene scene, IPhysicsAPI api)
        {
            this.api = api;
            this.scene = scene;

            Ref = api.createRigidStatic((int) geometry.GeoType, (long)geometry.GetInternalGeometry(), scene.Ref, Vector3.zero, Quaternion.identity);
        }

    }
}