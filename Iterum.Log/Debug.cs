using System;
using System.Collections.Generic;

namespace Iterum.Log
{
    public static partial class Debug
    {
        public static event DebugLogDelegate LogCallback;
        public static bool Verbose { get; set; } = false;
        
        /// <summary>
        /// Filter log by group
        /// </summary>
        public static HashSet<string> IgnoreGroups { get; } = new HashSet<string>();
        

        #region Back Color
        
        private static ConsoleColor backColor;
        public static void Back(ConsoleColor consoleColor)
        {
            backColor = Console.BackgroundColor;
            Console.BackgroundColor = consoleColor;
        }
        
        public static void ResetBack()
        {
            Console.BackgroundColor = backColor;
        }
        #endregion


        public static void EmptyLine() => Log(string.Empty, ConsoleColor.Black, false);
        
        public static void LogSuccess(string group, string e) => Log(@group, e, ConsoleColor.Green, ConsoleColor.Gray);

        private static void Log(string group, string e, 
            ConsoleColor color = ConsoleColor.White, 
            ConsoleColor groupColor = ConsoleColor.Gray, 
            bool timestamp = true,
            bool verboseGroup = false)
        {
            if (verboseGroup && !Verbose) return;
            if (IgnoreGroups.Contains(group)) return;
            
            var dateTime = DateTime.Now;
            var finalText = string.Empty;
            
            // Timestamp
            {
                var foreground = Console.ForegroundColor;
                if (timestamp)
                {
                    var text = $"{dateTime.ToLongTimeString()} ";
                    finalText += text;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(text);
                    Console.ForegroundColor = foreground;
                }
            }

            // Group
            {
                var foreground = Console.ForegroundColor;
                if (group != null)
                {
                    var text = $"[{group}] ";
                    finalText += text;
                    Console.ForegroundColor = groupColor;
                    Console.Write($"[{group}] ");
                    Console.ForegroundColor = foreground;
                }
            }

            // Text
            {
                finalText += e;
                var foreground = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.Write(e);
                Console.ForegroundColor = foreground;
                Console.Write("\n");
            }

            OnLogCallback(dateTime, group, e, finalText, color);
        }
        
        
        private static void OnLogCallback(DateTime time, string group, string msg, string fullMessage, ConsoleColor color)
        {
            LogCallback?.Invoke(time, group, msg, fullMessage, color);
        }
    }
}
