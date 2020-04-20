using System.Linq;
using Iterum.DataBlocks;

namespace Iterum.ThingTypes
{
    public static class ThingTypeExtensions
    {
        public static T GetData<T>(this IThingType tt) where T : class, IDataBlock
        {
            return ((ThingType)tt).DataBlocks.FirstOrDefault(e => e is T) as T;
        }
        
        public static string GetAttr(this IThingType tt, string attr)
        {
            if (tt.Attrs == null) return null;
            if (!tt.Attrs.ContainsKey(attr)) return null;
            return tt.Attrs[attr];
        }

        #region Float Attr Accessors
        
        public static float GetFloat(this IThingType tt, string attr)
        {
            return float.TryParse(tt.GetAttr(attr), out float result) ? result : 0;
        }

        public static float[] GetFloat2(this IThingType tt, string attr)
        {
            var str = tt.GetAttr(attr).Split(' ');
            return new[] {float.Parse(str[0]), float.Parse(str[1])};
        }
        public static float[] GetFloat3(this IThingType tt, string attr)
        {
            var str = tt.GetAttr(attr).Split(' ');
            return new[] {float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[3])};
        }
        #endregion
        
        
        #region Int Attrs Accessors
        
        public static int GetInt(this IThingType tt, string attr)
        {
            return int.TryParse(tt.GetAttr(attr), out int result) ? result : 0;
        }
        public static int[] GetInt2(this IThingType tt, string attr)
        {
            var str = tt.GetAttr(attr).Split(' ');
            return new[] {int.Parse(str[0]), int.Parse(str[1])};
        }
        public static int[] GetInt3(this IThingType tt, string attr)
        {
            var str = tt.GetAttr(attr).Split(' ');
            return new[] {int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2])};
        }
        #endregion
        
        
        public static bool HasFlag(this IThingType tt, string flag)
        {
            return tt.Flags != null && tt.Flags.Contains(flag);
        }

        
    }
}