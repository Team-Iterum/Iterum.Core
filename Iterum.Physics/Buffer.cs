using Iterum.Things;

namespace Iterum.Physics
{
    public struct Buffer
    {
        public long Ref;
        public IThing[] Things;

        public Buffer(long nRef, int count)
        {
            Ref = nRef;
            Things = new IThing[count];
        }
    }
}