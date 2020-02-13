using Magistr.Math;

namespace Magistr.Things
{
    public class Transform
    {
        // Position & Rotation
        public Vector3 Position { get; set; } = Vector3.zero;
        public Quaternion Rotation { get; set; } = Quaternion.identity;

        // Calculated
        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Up => Rotation * Vector3.up;
        public Vector3 Right => Rotation * Vector3.up;
    }
}