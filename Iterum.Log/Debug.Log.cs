using System;
using System.Runtime.CompilerServices;

namespace Iterum.Logs
{
    public static partial class Log
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Send(Level level, string e, ConsoleColor color, bool timestamp = true) => Send(level, null, e, color, ConsoleColor.Gray, timestamp);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string group, string e)   => Send(Level.Debug, group, e);
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string e)   => Send(Level.Debug, null, e);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(string group, string e)    => Send(Level.Info, group, e);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Success(string group, string e) => Send(Level.Success, group, e);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string group, string e)    => Send(Level.Warn, group, e);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fatal(string group, string e)   => Send(Level.Fatal, group, e);
        
        
        #region Overloads

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, object e) => Debug(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, double e) => Debug(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, float e)  => Debug(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, long e)   => Debug(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(string group, int e)    => Debug(group, e.ToString());
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(object e) => Debug(e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(double e) => Debug(e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(float e)  => Debug(e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(long e)   => Debug(e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Debug(int e)    => Debug(e.ToString());
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, object e) => Info(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, double e) => Info(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, float e)  => Info(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, long e)   => Info(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Info(string group, int e)    => Info(group, e.ToString());
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, object e) => Success(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, double e) => Success(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, float e)  => Success(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, long e)   => Success(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Success(string group, int e)    => Success(group, e.ToString());
        
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, object e) => Warn(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, double e) => Warn(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, float e)  => Warn(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, long e)   => Warn(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Warn(string group, int e)    => Warn(group, e.ToString());
        
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, object e) => Fatal(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, double e) => Fatal(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, float e)  => Fatal(group, e.ToString("F2"));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, long e)   => Fatal(group, e.ToString());
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Fatal(string group, int e)    => Fatal(group, e.ToString());

        #endregion

        
    }
}