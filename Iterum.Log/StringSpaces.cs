using System.Collections.Generic;
using System.Text;

namespace Iterum.Logs
{
    public static class StringSpaces
    {
        private static char space = ' ';
        private static Dictionary<int, string> spaces = new Dictionary<int, string>();

        static StringSpaces()
        {
            for (int i = 1; i < 40; i++)
            {
                Create(i);
            }
        }

        public static void Create(int count)
        {
            if (count <= 0) return;
            if(spaces.ContainsKey(count)) return;
            
            StringBuilder str = new StringBuilder(count);
            for (int i = 0; i < count; i++)
            {
                str.Append(space);
            }
            spaces.Add(count, str.ToString());
        }
        
        // ReSharper disable once InconsistentNaming
        public static string s(int count)
        {
            return count <= 0 ? string.Empty : spaces[count];
        }
    }
}