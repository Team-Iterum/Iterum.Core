using System;

namespace Iterum.Log
{
    public static partial class Debug
    {
        
        public static void Log(string group, string e, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            Log(group, e, color, ConsoleColor.Gray, timestamp);
        }

        public static void LogV(string group, string e, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            Log(group, e, color, ConsoleColor.Gray, timestamp, true);
        }

        public static void Log(string e, ConsoleColor color, bool timestamp = true)
        {
            Log(null, e, color, ConsoleColor.Gray, timestamp);
        }

        
        #region Log overloades
        
        public static void Log(object e, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            Log(e.ToString(), color, timestamp);
        }
        public static void Log(double e, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            Log(e.ToString("F2"), color, timestamp);
        }
        public static void Log(float e, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            Log(e.ToString("F2"), color, timestamp);
        }
        public static void Log(long e, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            Log(e.ToString(), color, timestamp);
        }
        public static void Log(int e, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            Log(e.ToString(), color, timestamp);
        }

        #endregion
    }
}