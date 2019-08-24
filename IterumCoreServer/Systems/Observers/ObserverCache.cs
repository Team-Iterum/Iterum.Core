using Magistr.Math;
using Magistr.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magistr.Observers
{
    public static class ObserverCache
    {
        public class CacheView
        {
            public int Frame;
            public List<IThing> ToAdd;
            public List<IThing> ToRemove;
            public List<IThing> All;
        }

        const int FrameLimit = 50;
        static int LastFrame = 0;

        private static Dictionary<int, Dictionary<Vector3, CacheView>> _cache = new Dictionary<int, Dictionary<Vector3, CacheView>>();

        public static CacheView Get(int Frame, Vector3 pos, float tolerance)
        {
            var _frame = NearestAvaibleFrame(Frame);
            if (_frame == null) return null;

            var cacheView = _frame.Where(e => (e.Key - pos).magnitude < tolerance);
            var count = cacheView.Count();
            if (count > 1)
            {
                return cacheView.OrderBy(e => (e.Key - pos).magnitude).First().Value;
            }
            else if (count == 1)
            {
                return cacheView.First().Value;
            }
            return null;
        }

        public static void Cache(int Frame, Vector3 pos, List<IThing> viewList, List<IThing> add, List<IThing> remove)
        {
            ClearCache(Frame);

            if (!_cache.ContainsKey(Frame))
                _cache.Add(Frame, new Dictionary<Vector3, CacheView>());
            _cache[Frame].Add(pos, new CacheView() { Frame = Frame, All = viewList, ToAdd = add, ToRemove = remove });
        }

        private static void ClearCache(int Frame)
        {
            for (int i = Frame - FrameLimit; i >= LastFrame; i--)
            {
                if (_cache.ContainsKey(i))
                {
                    _cache.Remove(i);
                }
            }
            LastFrame = Frame;

        }

        private static Dictionary<Vector3, CacheView> NearestAvaibleFrame(int Frame)
        {
            for (int i = Frame - FrameLimit; i >= LastFrame; i--)
            {
                if (_cache.ContainsKey(i))
                {
                    return _cache[i];
                }
            }
            return null;

        }
    }
}
