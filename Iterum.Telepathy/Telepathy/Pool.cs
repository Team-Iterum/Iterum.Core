// pool to avoid allocations. originally from libuv2k.
using System;
using System.Collections.Generic;

namespace Telepathy
{
    // shared pool sizing helpers (non-generic so the byte budget lives in one place)
    public static class Pool
    {
        // bound retained pool buffers to ~64MB regardless of MaxMessageSize so a backlog spike can't pin them forever
        public const int PoolMaxBytes = 64 * 1024 * 1024;

        // pool capacity that keeps retained buffers under PoolMaxBytes, never below 16
        public static int CapacityForBudget(int maxMessageSize) => Math.Max(16, PoolMaxBytes / maxMessageSize);
    }

    public class Pool<T>
    {
        // objects
        readonly Stack<T> objects = new Stack<T>();

        // some types might need additional parameters in their constructor, so
        // we use a Func<T> generator
        readonly Func<T> objectGenerator;

        readonly int maxSize;

        // constructor
        public Pool(Func<T> objectGenerator, int maxSize = 0)
        {
            this.objectGenerator = objectGenerator;
            this.maxSize = maxSize;
        }

        // take an element from the pool, or create a new one if empty
        public T Take() => objects.Count > 0 ? objects.Pop() : objectGenerator();

        // return an element to the pool
        public void Return(T item)
        {
            if (maxSize > 0 && objects.Count >= maxSize) return;
            objects.Push(item);
        }

        // clear the pool with the disposer function applied to each object
        public void Clear() => objects.Clear();

        // count to see how many objects are in the pool. useful for tests.
        public int Count() => objects.Count;
    }
}