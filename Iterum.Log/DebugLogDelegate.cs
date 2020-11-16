using System;

namespace Iterum.Logs
{
    public delegate void LogDelegate(DateTime time, Level level, string group, string msg, string fullMessage, ConsoleColor color);
}