using System;
using System.Runtime.CompilerServices;

namespace Iterum.Logs
{
    public static partial class Log
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Send(Level level, string e, ConsoleColor color, bool timestamp = true) => Send(level, null, e, color, ConsoleColor.Gray, timestamp);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string group, string e, ConsoleColor color = ConsoleColor.DarkGray)   => Send(Level.Debug, group, e, color);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string e, ConsoleColor color = ConsoleColor.DarkGray)   => Send(Level.Debug, null, e, color);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(string group, string e, ConsoleColor color = ConsoleColor.White)    => Send(Level.Info, group, e, color);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Success(string group, string e, ConsoleColor color = ConsoleColor.DarkGreen) => Send(Level.Success, group, e, color);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string group, string e, ConsoleColor color = ConsoleColor.DarkYellow)    => Send(Level.Warn, group, e, color);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fatal(string group, string e, ConsoleColor color = ConsoleColor.White)   => Send(Level.Fatal, group, e, color);
        
        
        #region Overloads

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, object e, ConsoleColor color = ConsoleColor.White) => Debug(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, double e, ConsoleColor color = ConsoleColor.White) => Debug(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, float e, ConsoleColor color = ConsoleColor.White)  => Debug(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, long e, ConsoleColor color = ConsoleColor.White)   => Debug(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, int e, ConsoleColor color = ConsoleColor.White)    => Debug(group, e.ToString(), color);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(object e, ConsoleColor color = ConsoleColor.White) => Debug(e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(double e, ConsoleColor color = ConsoleColor.White) => Debug(e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(float e, ConsoleColor color = ConsoleColor.White)  => Debug(e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(long e, ConsoleColor color = ConsoleColor.White)   => Debug(e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(int e, ConsoleColor color = ConsoleColor.White)    => Debug(e.ToString(), color);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, object e, ConsoleColor color = ConsoleColor.White) => Info(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, double e, ConsoleColor color = ConsoleColor.White) => Info(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, float e, ConsoleColor color = ConsoleColor.White)  => Info(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, long e, ConsoleColor color = ConsoleColor.White)   => Info(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, int e, ConsoleColor color = ConsoleColor.White)    => Info(group, e.ToString(), color);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, object e, ConsoleColor color = ConsoleColor.DarkGreen) => Success(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, double e, ConsoleColor color = ConsoleColor.DarkGreen) => Success(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, float e, ConsoleColor color = ConsoleColor.DarkGreen)  => Success(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, long e, ConsoleColor color = ConsoleColor.DarkGreen)   => Success(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, int e, ConsoleColor color = ConsoleColor.DarkGreen)    => Success(group, e.ToString(), color);
        
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, object e, ConsoleColor color = ConsoleColor.DarkYellow) => Warn(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, double e, ConsoleColor color = ConsoleColor.DarkYellow) => Warn(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, float e, ConsoleColor color = ConsoleColor.DarkYellow)  => Warn(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, long e, ConsoleColor color = ConsoleColor.DarkYellow)   => Warn(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, int e, ConsoleColor color = ConsoleColor.DarkYellow)    => Warn(group, e.ToString(), color);
        
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, object e, ConsoleColor color = ConsoleColor.White) => Fatal(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, double e, ConsoleColor color = ConsoleColor.White) => Fatal(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, float e, ConsoleColor color = ConsoleColor.White)  => Fatal(group, e.ToString("F2"), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, long e, ConsoleColor color = ConsoleColor.White)   => Fatal(group, e.ToString(), color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, int e, ConsoleColor color = ConsoleColor.White)    => Fatal(group, e.ToString(), color);

        #endregion

        
    }
}