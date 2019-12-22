using System;

namespace Magistr.Log
{
    public static class Debug
    {
        private static ConsoleColor BackColor;
        public static void Back(ConsoleColor consoleColor)
        {
            BackColor = Console.BackgroundColor;
            Console.BackgroundColor = consoleColor;
        }
        
        public static void ResetBack()
        {
            Console.BackgroundColor = BackColor;
        }

        public static void Log(string e, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            var msg = timestamp ? $"[{DateTime.Now.ToLongTimeString()}] {e}" : e;
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ForegroundColor = foreground;
            Console.Write("\n");
        }

        public static void LogError(string e)
        {
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[{DateTime.Now.ToLongTimeString()}] {e}");
            Console.ForegroundColor = foreground;
            Console.Write("\n");
        }

        #region LogError overloades

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
