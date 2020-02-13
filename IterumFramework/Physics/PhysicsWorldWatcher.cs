using Magistr.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Magistr.Physics
{
    public class PhysicsWorldWatcher
    {
        private Timer WatchTimer { get; set; }
        private static readonly StringBuilder Builder = new StringBuilder();
        private readonly List<IPhysicsWorld> worlds;

        public PhysicsWorldWatcher(List<IPhysicsWorld> worlds)
        {
            this.worlds = worlds;
        }

        public void Start()
        {
            const int period = 3000;
            WatchTimer = new Timer(Watch, worlds, 0, period);
        }

        private static void Watch(object obj)
        {
            List<IPhysicsWorld> worlds = ((List<IPhysicsWorld>)obj);

            if (worlds.Count == 0) return;

            var header = "\n";

            foreach (var w in worlds)
            {
                var color = ConsoleColor.Black;
                var backColor = ConsoleColor.DarkGreen;

                if(!w.IsCreated)
                {
                    Debug.Back(ConsoleColor.Gray);
                    var s = $"#{worlds.IndexOf(w)} not yet created";
                    s += Space(header.Length - s.Length);

                    Debug.Log(s);
                    Debug.ResetBack();
                    continue;
                }
                if (w.IsDestroyed)
                {
                    Debug.Back(ConsoleColor.Black);
                    var s = $"#{worlds.IndexOf(w)} is destroyed";
                    s += Space(header.Length - s.Length);

                    Debug.Log(s);
                    Debug.ResetBack();
                    continue;
                }
                if (!w.IsRunning)
                {
                    Debug.Back(ConsoleColor.Gray);
                    var s = $"#{worlds.IndexOf(w)} stopped";
                    s += Space(header.Length - s.Length);

                    Debug.Log(s, color);
                    Debug.ResetBack();
                    continue;
                }

                bool draw = false;
                string infoText = "(Normal) ";

                if(w.SceneFrame > w.TPS * 0.5f)
                {
                    backColor = ConsoleColor.DarkYellow;
                    infoText = "(Warning) ";
                    draw = true;
                }
                if (w.SceneFrame >= w.TPS * 0.75f)
                {
                    backColor = ConsoleColor.Red;
                    color = ConsoleColor.White;
                    infoText = "(Overload) ";
                    draw = true;
                }
                if (w.SceneFrame >= w.TPS)
                {
                    backColor = ConsoleColor.DarkRed;
                    color = ConsoleColor.White;
                    infoText = "(Fatal Overload) ";
                    draw = true;
                }

                if (!w.IsRunning)
                {
                    backColor = ConsoleColor.Gray;
                    infoText = "(Stopped) ";
                    draw = false;
                }


                if (draw)
                {
                    var str =
                        $"{infoText}#{worlds.IndexOf(w)} dt: {w.DeltaTime}ms frame: {w.SceneFrame}ms timestamp: {w.Timestamp}";
                    str += Space(header.Length - str.Length);

                    var foreground = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"[Watcher] ");
                    Console.ForegroundColor = foreground;

                    Debug.Back(backColor);

                    Console.ForegroundColor = color;
                    Console.Write(str);
                    Console.ForegroundColor = foreground;
                    Console.Write("\n");

                    Debug.ResetBack();
                }
            }
            Console.ResetColor();
        }
        

        private static string Space(int count)
        {
            Builder.Clear();
            for (int i = 0; i < count; i++)
            {
                Builder.Append(" ");
            }
            return Builder.ToString();

        }
    }
}
