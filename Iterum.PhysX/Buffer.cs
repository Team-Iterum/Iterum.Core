using Iterum.Things;

namespace Iterum.Physics.PhysXImpl;

public struct Buffer<T> where T : IThing
{
    public long Ref;
    public T[] Things;
    public int Count { get; private set; }

    public Buffer(long nRef, int max)
    {
        Ref = nRef;
        Things = new T[max];
        Count = 0;
    }

    public void SetResultsCount(int count) => Count = count;
}