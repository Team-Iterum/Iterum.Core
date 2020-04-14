using System.Collections.Generic;
using System.Linq;
using Magistr.DataBlocks;

namespace Magistr.New.ThingTypes
{
    public struct ThingType
    {
        public int ID;
        
        public string Name;
        public string Category;
        
        public string Description;

        public string[] Flags;
        
        public Dictionary<string, string> Attrs;

        public IDataBlock[] DataBlocks;

        public string GetAttr(string attr)
        {
            if (Attrs == null) return null;
            if (!Attrs.ContainsKey(attr)) return null;
            return Attrs[attr];
        }

        public float GetFloat(string attr)
        {
            return float.TryParse(GetAttr(attr), out float result) ? result : 0;
        }

        public float[] GetFloat2(string attr)
        {
            var str = GetAttr(attr).Split(' ');
            return new[] {float.Parse(str[0]), float.Parse(str[1])};
        }

        public int GetInt(string attr)
        {
            return int.TryParse(GetAttr(attr), out int result) ? result : 0;
        }
        
        
        public bool HasFlag(string flag)
        {
            return Flags != null && Flags.Contains(flag);
        }

        public override string ToString()
        {
            return $"ThingType {Category}/{Name}";
        }

    }
    
}