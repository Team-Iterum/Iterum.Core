using System.Collections.Generic;
namespace Magistr.Observers
{
    public partial class Observer
    {
        class ViewID
        {
            int Capacity;
            Stack<int> IdPool;

            public ViewID(int capacity)
            {
                Capacity = capacity;
                IdPool = new Stack<int>(capacity);
                for (int i = capacity; i >= 0; i--)
                {
                    IdPool.Push(i);
                }
            }

            public int GetId()
            {
                if (IdPool.Count == 0) return -1;
                return IdPool.Pop();
            }

            public void ReturnId(int id)
            {
                if (id < 0 || id >= Capacity) return;
                IdPool.Push(id);
            }

        }
    }
}
