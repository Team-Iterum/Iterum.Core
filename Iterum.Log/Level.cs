using System;

namespace Iterum.Logs
{
    [Flags]
    public enum Level
    {
        None = 0,
        Debug = 1 << 8,
        Info = 2 << 8,
        Success = 3 << 8,
        Warn = 4 << 8,
        Error = 5 << 8,
        Exception = 6 << 8,
        Fatal = 7 << 8,
    }

    public static class LevelExtensions
    {
        public static bool HasFlagFast(this Level value, Level flag)
        {
            return (value & flag) != 0;
        }

        public static string ToLevelString(this Level level)
        {
            var color = level switch
            {
                Level.Debug     => "Debug",
                Level.Info      => "Info",
                Level.Success   => "Success",
                Level.Warn      => "Warn",
                Level.Error     => "Error",
                Level.Exception => "Exception",
                Level.Fatal     => "Fatal",
                _ => string.Empty
            };

            return color;
        }
    }
}