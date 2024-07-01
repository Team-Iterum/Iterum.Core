namespace Iterum.Physics;

public struct Buffer
{
    public long Ref;
    public ulong[] Things;

    public Buffer(long nRef, int count)
    {
        Ref = nRef;
        Things = new ulong[count];
    }
}