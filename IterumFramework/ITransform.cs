using Magistr.Math;

namespace Magistr.Game
{
    public interface ITransform
    {
        // Position
        Vector3 Position { get; set; }
        
        // Rotation
        Quaternion Rotation { get; set; }

        // Local directions vectors
        Vector3 Forward { get; }
        Vector3 Up { get; }
        Vector3 Right { get; }

    }
}