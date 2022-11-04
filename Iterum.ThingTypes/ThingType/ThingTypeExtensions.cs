using System;
using System.Globalization;
using System.Linq;
using Iterum.DataBlocks;

namespace Iterum.ThingTypes
{
    public static class TTExtensions
    {
        public static bool HasFlag(this IThingType tt, string flag)
        {
            return tt.Flags != null && tt.Flags.Contains(flag);
        }

        public static string GetAttr(this IThingType tt, string attr, string def = null)
        {
            if (tt.Attrs == null) return def;
            if (!tt.Attrs.ContainsKey(attr)) return def;
            return tt.Attrs[attr];
        }

        public static string Str(this IThingType tt, string attr, string def = null)
        {
            if (tt.Attrs == null) return def;
            if (!tt.Attrs.ContainsKey(attr)) return def;
            return tt.Attrs[attr];
        }

        #region Float Attr Accessors
        
        public static float Float(this IThingType tt, string attr, float def = 0)
        {
            if(tt.GetAttr(attr) == null) return def;

            return float.TryParse(tt.GetAttr(attr), 
                NumberStyles.Any, CultureInfo.InvariantCulture, out float result)
                ? result : def;
        }

        public static float[] Float2(this IThingType tt, string attr, float[] def = null)
        {
            if(tt.GetAttr(attr) == null) return def;

            var str = tt.GetAttr(attr).Split(' ');
            return new[]
            {
                float.Parse(str[0], CultureInfo.InvariantCulture), 
                float.Parse(str[1], CultureInfo.InvariantCulture)
            };
        }

        public static float[] Float3(this IThingType tt, string attr, float[] def = null)
        {
            if(tt.GetAttr(attr) == null) return def;

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
        

        public static int Int(this IThingType tt, string attr, int def = 0)
        {
            if(tt.GetAttr(attr) == null) return def;

            return int.TryParse(tt.GetAttr(attr), 
                NumberStyles.Any, CultureInfo.InvariantCulture, out int result)
                ? result : def;
        }

        public static int[] Int2(this IThingType tt, string attr, int[] def = null)
        {
            if(tt.GetAttr(attr) == null) return def;

            var str = tt.GetAttr(attr).Split(' ');
            return new[]
            {
                int.Parse(str[0], CultureInfo.InvariantCulture),
                int.Parse(str[1], CultureInfo.InvariantCulture)
            };
        }

        
        public static int[] Int3(this IThingType tt, string attr, int[] def = null)
        {
            if(tt.GetAttr(attr) == null) return def;

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