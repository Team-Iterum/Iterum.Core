using System;
using System.Drawing;
using Packets.NoPackets;

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

        public static void Log(string group, string e, ConsoleColor color = ConsoleColor.White, bool timestamp = true)
        {
            Log(group, e, color, color, timestamp);
        }

        public static void LogSuccess(string group, string e, ConsoleColor _)
        {
            Log(group, e, ConsoleColor.Green, ConsoleColor.Gray);
        }

        public static void Log(string e, ConsoleColor color, bool timestamp = true)
        {
            Log(null, e, color, ConsoleColor.Gray, timestamp);
        }
        private static void Log(string group, string e, ConsoleColor color = ConsoleColor.White, ConsoleColor groupColor = ConsoleColor.Black,  bool timestamp = true)
        {
            // Timestamp
            {
                var foreground = Console.ForegroundColor;
                if (timestamp)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"{DateTime.Now.ToLongTimeString()} ");
                    Console.ForegroundColor = foreground;
                }
            }

            // Group
            {
                var foreground = Console.ForegroundColor;
                if (group != null)
                {
                    Console.ForegroundColor = groupColor;
                    Console.Write($"[{group}] ");
                    Console.ForegroundColor = foreground;
                }
            }

            // Text
            {
                var foreground = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.Write(e);
                Console.ForegroundColor = foreground;
                Console.Write("\n");
            }
        }

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
