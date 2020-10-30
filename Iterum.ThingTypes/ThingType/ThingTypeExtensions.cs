using System;
using System.Globalization;
using System.Linq;
using Iterum.DataBlocks;

namespace Iterum.ThingTypes
{
    public static class TTExtensions
    {
        public static string GetAttr(this IThingType tt, string attr)
        {
            if (tt.Attrs == null) return null;
            if (!tt.Attrs.ContainsKey(attr)) return null;
            return tt.Attrs[attr];
        }
        
        public static bool HasFlag(this IThingType tt, string flag)
        {
            return tt.Flags != null && tt.Flags.Contains(flag);
        }
        

        #region Float Attr Accessors

        public static float GetFloat(this IThingType tt, string attr)
        {
            return float.TryParse(tt.GetAttr(attr), 
                NumberStyles.Any, CultureInfo.InvariantCulture, out float result)
                ? result : 0;
        }

        public static float[] GetFloat2(this IThingType tt, string attr)
        {
            var str = tt.GetAttr(attr).Split(' ');
            return new[]
            {
                float.Parse(str[0], CultureInfo.InvariantCulture), 
                float.Parse(str[1], CultureInfo.InvariantCulture)
            };
        }

        public static float[] GetFloat3(this IThingType tt, string attr)
        {
            var str = tt.GetAttr(attr).Split(' ');
            return new[]
            {
                float.Parse(str[0], CultureInfo.InvariantCulture), 
                float.Parse(str[1], CultureInfo.InvariantCulture),
                float.Parse(str[2], CultureInfo.InvariantCulture)
            };
        }

        #endregion


        #region Int Attrs Accessors

        public static int GetInt(this IThingType tt, string attr)
        {
            return int.TryParse(tt.GetAttr(attr), 
                NumberStyles.Any, CultureInfo.InvariantCulture, out int result)
                ? result : 0;
        }

        public static int[] GetInt2(this IThingType tt, string attr)
        {
            var str = tt.GetAttr(attr).Split(' ');
            return new[]
            {
                int.Parse(str[0], CultureInfo.InvariantCulture),
                int.Parse(str[1], CultureInfo.InvariantCulture)
            };
        }

        public static int[] GetInt3(this IThingType tt, string attr)
        {
            var str = tt.GetAttr(attr).Split(' ');
            return new[]
            {
                int.Parse(str[0], CultureInfo.InvariantCulture),
                int.Parse(str[1], CultureInfo.InvariantCulture),
                int.Parse(str[2], CultureInfo.InvariantCulture)
            };
        }

        #endregion

        #region Enum & DataBlock

        public static T GetEnum<T>(this IThingType tt, string attr) where T : struct, Enum
        {
            return Enum.Parse<T>(tt.GetAttr(attr), true);
        }
        
        public static T GetEnum<T>(this IThingType tt) where T : struct, Enum
        {
            return Enum.Parse<T>(tt.GetAttr(typeof(T).Name), true);
        }

        public static T GetData<T>(this IThingType tt) where T : class, IDataBlock
        {
            return ((ThingType) tt).DataBlocks.FirstOrDefault(e => e is T) as T;
        }

        #endregion
        
       
    }
}