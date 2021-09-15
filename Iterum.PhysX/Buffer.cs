using Iterum.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iterum.Physics.PhysXImpl
{
    public struct Buffer
    {
        public long Ref;
        public IThing[] Things;
        public int Count { get; private set; }

        public Buffer(long nRef, int max)
        {
            Ref = nRef;
            Things = new IThing[max];
            Count = 0;
        }

        public void SetResultsCount(int count) => Count = count;
    }
}
