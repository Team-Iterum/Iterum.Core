using System.Linq;

namespace Magistr.New.ThingTypes
{
    public struct ThingType
    {
        public int ID;
        
        public string Name;
        public string Category;
        
        public string Description;
        
        public string[] Flags;

        public IDataBlock[] DataBlocks;

        public bool HasFlag(string attr)
        {
            return Flags != null && Flags.Contains(attr);
        }
    }
    
}