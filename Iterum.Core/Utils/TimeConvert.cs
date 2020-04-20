using System.Diagnostics;

namespace Iterum.Utils
{
    public static class TimeConvert
    {
        public static double TicksToSeconds(double ticks) => ticks / Stopwatch.Frequency;
        public static double TicksToMs(double ticks) => (ticks / Stopwatch.Frequency) * 1000;
        public static float SecondsToMs(float seconds) => seconds * 1000;
    }

}
