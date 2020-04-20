using Iterum.Math;

namespace Iterum.Things
{
    public class Transform
    {
        // Position & Rotation
        public virtual Vector3 Position { get; set; } = Vector3.zero;
        public virtual Quaternion Rotation { get; set; } = Quaternion.identity;

        // Calculated
        public Vector3 Forward => Position + LocalForward;
        public Vector3 Up => Position + LocalUp;
        public Vector3 Right => Position + LocalRight;
        
        public Vector3 LocalForward => Rotation * Vector3.forward;
        public Vector3 LocalUp => Rotation * Vector3.up;
        public Vector3 LocalRight => Rotation * Vector3.up;
    }
}