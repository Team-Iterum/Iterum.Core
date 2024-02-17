using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Iterum.Logs
{
    
    public static partial class Log
    {
        public static event LogDelegate LogCallback;
        public static event Func<string, bool> BeforeLog;

        public static HashSet<string> HideGroups = new();
        
        public static string TimeStampFormat = "t"; // short

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
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static void Send(Level level, string group, string s, 
            ConsoleColor color = ConsoleColor.White, ConsoleColor groupColor = ConsoleColor.Gray, 
            bool timestamp = true)
        {

            if (!Enabled.HasFlagFast(level)) return;
            if (HideGroups.Contains(group)) return;
            if (BeforeLog != null && !BeforeLog.Invoke(s)) return;
            
            var dateTime = DateTime.Now;
            var finalText = new StringBuilder();
            
            // Timestamp
            {
                var foreground = Console.ForegroundColor;
                if (timestamp)
                {
                    var text = $"{dateTime.ToString(TimeStampFormat)} ";
                    
#if UNITY_2018_3_OR_NEWER
                    text = Tagged(text, ConsoleColor.DarkGray);
                    finalText.Append(text);
#else
                    finalText.Append(text);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(text);
                    Console.ForegroundColor = foreground;
#endif
                }
            }
            
            // Level
            {
                var foreground = Console.ForegroundColor;
                if (timestamp)
                {
                    var text = $"[{level.ToLevelString()}] ";
#if UNITY_2018_3_OR_NEWER
                    text = Tagged(text, GetColorLevel(level));
                    finalText.Append(text);
#else
                    finalText.Append(text);
                    Console.ForegroundColor = GetColorLevel(level);
                    Console.Write(text);
                    Console.ForegroundColor = foreground;
#endif
                }
            }

            // Group
            {
                var foreground = Console.ForegroundColor;
                if (group != null)
                {
                    var text = $"[{group}] ";
#if UNITY_2018_3_OR_NEWER
                    text = Tagged(text, groupColor);
                    finalText.Append(text);
#else
                    finalText.Append(text);
                    Console.ForegroundColor = groupColor;
                    Console.Write(text);
                    Console.ForegroundColor = foreground;
#endif
                }
            }

            // Text
            {
                
#if UNITY_2018_3_OR_NEWER

               s = Tagged(s, color);
               finalText.Append(s);
#else
                finalText.Append(s);
                var foreground = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.Write(s);
                Console.ForegroundColor = foreground;
                Console.Write("\n");
#endif
            }

#if UNITY_2018_3_OR_NEWER   
            UnityEngine.Debug.Log(finalText);
#endif
            OnLogCallback(dateTime, level, group, s, finalText.ToString(), color);
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
        
        
        private static string Tagged(string text, ConsoleColor color)
        {
            string textColor = color switch
            {
                ConsoleColor.Black       => "#000",
                ConsoleColor.Blue        => "#00f",
                ConsoleColor.Cyan        => "#0ff",
                ConsoleColor.DarkBlue    => "#009",
                ConsoleColor.DarkCyan    => "#099",
                ConsoleColor.DarkGray    => "#aaa",
                ConsoleColor.DarkGreen   => "#090",
                ConsoleColor.DarkMagenta => "#909",
                ConsoleColor.DarkRed     => "#900",
                ConsoleColor.DarkYellow  => "#aa0",
                ConsoleColor.Gray        => "#ccc",
                ConsoleColor.Green       => "#0f0",
                ConsoleColor.Magenta     => "#f0f",
                ConsoleColor.Red         => "#f00",
                ConsoleColor.White       => "#fff",
                ConsoleColor.Yellow      => "#ff0",
                _ => "#fff"
            };


            return $"<color={textColor}>{text}</color>";
        }
        

        private static void OnLogCallback(DateTime time, Level level, string group, string msg, string fullMessage, ConsoleColor color)
        {
            LogCallback?.Invoke(time, level, group, msg, fullMessage, color);
        }
    }
}
