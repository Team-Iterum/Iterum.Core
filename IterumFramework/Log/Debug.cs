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

        public static void Log(string str, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            var msg = timestamp ? $"[{DateTime.Now.ToLongTimeString()}] {str}" : str;
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ForegroundColor = foreground;
            Console.Write("\n");
        }

        public static void LogError(string str)
        {
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[{DateTime.Now.ToLongTimeString()}] {str}");
            Console.ForegroundColor = foreground;
            Console.Write("\n");
        }

        public static void LogError(double val)
        {
            LogError(val.ToString("F2"));
        }
        public static void LogError(float val)
        {
            LogError(val.ToString("F2"));
        }
        public static void LogError(long val)
        {
            LogError(val.ToString());
        }

        public static void LogError(int val)
        {
            LogError(val.ToString());
        }

        public static void Log(double val)
        {
            LogError(val.ToString("F2"));
        }
        public static void Log(float val)
        {
            Log(val.ToString("F2"));
        }
        public static void Log(long val)
        {
            LogError(val.ToString());
        }

        public static void Log(int val)
        {
            Log(val.ToString());
        }



    }
}
