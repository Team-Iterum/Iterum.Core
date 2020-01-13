using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Magistr.Utils
{
    public static class TimeConversion
    {
        public static double TicksToSeconds(double ticks) => ticks / Stopwatch.Frequency;
        public static double TicksToMs(double ticks) => (ticks / Stopwatch.Frequency) * 1000;
        public static float SecondsToMs(float seconds) => seconds * 1000;
    }

}
