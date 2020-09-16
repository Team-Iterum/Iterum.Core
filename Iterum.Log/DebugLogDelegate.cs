using System;

namespace Iterum.Log
{
    public delegate void DebugLogDelegate(DateTime time, string group, string msg, string fullMessage, ConsoleColor color);
}