using System;
using System.Runtime.CompilerServices;

namespace Iterum.Logs
{
    [Flags]
    public enum Level
    {
        None       = 0,
        Debug      = 1 << 8,
        Info       = 2 << 8,
        Success    = 3 << 8,
        Warn       = 4 << 8,
        Error      = 5 << 8,
        Exception  = 6 << 8,
        Fatal      = 7 << 8,
    }

    
    public static partial class Log
    {
        public static event LogDelegate LogCallback;

        public static Level Enabled = Level.Debug | Level.Info | Level.Success | Level.Warn | Level.Error |
                                            Level.Exception | Level.Fatal;
        
        #region Back Color
        
        private static ConsoleColor backColor;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Back(ConsoleColor consoleColor)
        {
            backColor = Console.BackgroundColor;
            Console.BackgroundColor = consoleColor;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetBack()
        {
            Console.BackgroundColor = backColor;
        }
        #endregion
        
        private static void Send(Level level, string group, string s, 
            ConsoleColor color = ConsoleColor.White, ConsoleColor groupColor = ConsoleColor.Gray, 
            bool timestamp = true)
        {

            if (!Enabled.HasFlag(level)) return;
   
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
            
            // Level
            {
                var foreground = Console.ForegroundColor;
                if (timestamp)
                {
                    var text = $"[{level}] ";
                    finalText += text;
                    Console.ForegroundColor = GetColorLevel(level);
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
                    Console.Write(text);
                    Console.ForegroundColor = foreground;
                }
            }

            // Text
            {
                finalText += s;
                var foreground = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.Write(s);
                Console.ForegroundColor = foreground;
                Console.Write("\n");
            }

            OnLogCallback(dateTime, level, group, s, finalText, color);
        }

        private static ConsoleColor GetColorLevel(Level level)
        {
            var color = level switch
            {
                Level.Debug     => ConsoleColor.Cyan,
                Level.Info      => ConsoleColor.White,
                Level.Success   => ConsoleColor.Green,
                Level.Warn      => ConsoleColor.Yellow,
                Level.Error     => ConsoleColor.Red,
                Level.Exception => ConsoleColor.DarkRed,
                Level.Fatal     => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };

            return color;
        }


        private static void OnLogCallback(DateTime time, Level level, string group, string msg, string fullMessage, ConsoleColor color)
        {
            LogCallback?.Invoke(time, level, group, msg, fullMessage, color);
        }
    }
}
