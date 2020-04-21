﻿using System;
using ShellProgressBar;

namespace Iterum.Log
{
    public static class Debug
    {
        private static ConsoleColor backColor;
        
        public static ProgressBarOptions ProgressBarOptions = new ProgressBarOptions
        {
            ProgressBarOnBottom = true,
            
            ForegroundColor = ConsoleColor.Yellow,
            ForegroundColorDone = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.DarkGray,
        };

        public delegate void LogCallbackDelegate(DateTime time, string msg, ConsoleColor color);

        public static event  LogCallbackDelegate LogCallback;

        public static bool Verbose { get; set; } = false;

        public static void Back(ConsoleColor consoleColor)
        {
            backColor = Console.BackgroundColor;
            Console.BackgroundColor = consoleColor;
        }
        
        public static void ResetBack()
        {
            Console.BackgroundColor = backColor;
        }

        public static void EmptyLine()
        {
            Log("", ConsoleColor.Black, false);
        }
        public static void Log(string group, string e, ConsoleColor color = ConsoleColor.White, bool timestamp = true) 
            => Log(group, e, color, ConsoleColor.Gray, timestamp);
        
        public static void LogV(string group, string e, ConsoleColor color = ConsoleColor.White, bool timestamp = true) 
            => Log(group, e, color, ConsoleColor.Gray, timestamp, true);

        public static void LogSuccess(string group, string e)
        {
            Log(group, e, ConsoleColor.Green, ConsoleColor.Gray);
        }

        public static void Log(string e, ConsoleColor color, bool timestamp = true)
        {
            Log(null, e, color, ConsoleColor.Gray, timestamp);
        }
        private static void Log(string group, string e, 
            ConsoleColor color = ConsoleColor.White, 
            ConsoleColor groupColor = ConsoleColor.Gray, 
            bool timestamp = true,
            bool verboseGroup = false)
        {
            if (verboseGroup && !Verbose) return;
            
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

            OnLogCallback(dateTime, finalText, color);
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

        private static void OnLogCallback(DateTime time, string msg, ConsoleColor color)
        {
            LogCallback?.Invoke(time, msg, color);
        }
    }
}