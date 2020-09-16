using System;

namespace Iterum.Log
{
    public static partial class Debug
    {
        
        public static void LogError(string group, string e)
        {
            Log(group, e, ConsoleColor.Red, ConsoleColor.Gray);
        }
        public static void LogError(string e)
        {
            Log(e, ConsoleColor.Red);
        }
        
        public static void LogError(string group, Exception e)
        {
            Log(group, e.ToString(), ConsoleColor.Red, ConsoleColor.Gray);
        }
        public static void LogError(Exception e)
        {
            Log(e, ConsoleColor.Red);
        }

        #region Overloads
        
        public static void LogError(object e)
        {
            LogError(e.ToString());
        }

        public static void LogError(double e)
        {
            LogError(e.ToString("F2"));
        }

        public static void LogError(float e)
        {
            LogError(e.ToString("F2"));
        }

        public static void LogError(long e)
        {
            LogError(e.ToString());
        }

        public static void LogError(int e)
        {
            LogError(e.ToString());
        }
        
        #endregion
    }
}