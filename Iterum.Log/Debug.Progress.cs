using System;
using ShellProgressBar;

namespace Iterum.Log
{
    public static partial class Debug
    {
        
        public static ProgressBarOptions ProgressBarOptions = new ProgressBarOptions
        {
            ProgressBarOnBottom = true,
            
            ForegroundColor = ConsoleColor.Yellow,
            ForegroundColorDone = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.DarkGray,
        };
    }
}