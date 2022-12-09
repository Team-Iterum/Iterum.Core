using System;
using System.Globalization;

namespace Iterum.ThingTypes
{
    public static class MapDataExtensions
    {
        public static string GetAttr(this MapData tt, string attr, string def = null)
        {
            if (tt.Attrs == null) return def;
            if (!tt.Attrs.ContainsKey(attr)) return def;
            return tt.Attrs[attr];
        }

        public static string Str(this MapData tt, string attr, string def = null)
        {
            if (tt.Attrs == null) return def;
            if (!tt.Attrs.ContainsKey(attr)) return def;
            return tt.Attrs[attr];
        }

        #region Float Attr Accessors
        
        public static float Float(this MapData tt, string attr, float def = 0)
        {
            if(tt.GetAttr(attr) == null) return def;

            return float.TryParse(tt.GetAttr(attr), 
                NumberStyles.Any, CultureInfo.InvariantCulture, out float result)
                ? result : def;
        }

        public static float[] Float2(this MapData tt, string attr, float[] def = null)
        {
            if(tt.GetAttr(attr) == null) return def;

            var str = tt.GetAttr(attr).Split(' ');
            return new[]
            {
                float.Parse(str[0], CultureInfo.InvariantCulture), 
                float.Parse(str[1], CultureInfo.InvariantCulture)
            };
        }

        public static float[] Float3(this MapData tt, string attr, float[] def = null)
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
        

        public static int Int(this MapData tt, string attr, int def = 0)
        {
            if(tt.GetAttr(attr) == null) return def;

            return int.TryParse(tt.GetAttr(attr), 
                NumberStyles.Any, CultureInfo.InvariantCulture, out int result)
                ? result : def;
        }

        public static int[] Int2(this MapData tt, string attr, int[] def = null)
        {
            if(tt.GetAttr(attr) == null) return def;

            var str = tt.GetAttr(attr).Split(' ');
            return new[]
            {
                int.Parse(str[0], CultureInfo.InvariantCulture),
                int.Parse(str[1], CultureInfo.InvariantCulture)
            };
        }

        
        public static int[] Int3(this MapData tt, string attr, int[] def = null)
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

        public static T GetEnum<T>(this MapData tt, string attr) where T : struct, Enum
        {
            return Enum.Parse<T>(tt.GetAttr(attr), true);
        }
        
        public static T GetEnum<T>(this MapData tt) where T : struct, Enum
        {
            return Enum.Parse<T>(tt.GetAttr(typeof(T).Name), true);
        }

        #endregion
        
       
    }
}