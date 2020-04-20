using System;
using System.Collections.Generic;
using System.Threading;
using Iterum.Log;

namespace Iterum.Physics
{
    public class PhysicsWorldWatcher
    {
        private Timer WatchTimer { get; set; }
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
            var worlds = ((List<IPhysicsWorld>)obj);

            if (worlds.Count == 0) return;

            const string header = "\n";

            foreach (var w in worlds)
            {
                var color = ConsoleColor.Black;
                var backColor = ConsoleColor.DarkGreen;

                if(w.State == IPhysicsWorld.WorldState.None)
                {
                    Debug.Back(ConsoleColor.Gray);
                    var s = $"#{worlds.IndexOf(w)} not yet created";
                    s += PhysicsWorldWatcher.s(header.Length - s.Length);

                    Debug.Log(s);
                    Debug.ResetBack();
                    continue;
                }
                
                if(w.State == IPhysicsWorld.WorldState.Destroyed)
                {
                    Debug.Back(ConsoleColor.Black);
                    string s = $"#{worlds.IndexOf(w)} is destroyed";
                    s += PhysicsWorldWatcher.s(header.Length - s.Length);

                    Debug.Log(s);
                    Debug.ResetBack();
                    continue;
                }
                
                if(w.State != IPhysicsWorld.WorldState.Running)
                {
                    Debug.Back(ConsoleColor.Gray);
                    
                    string s = $"#{worlds.IndexOf(w)} stopped";
                    s += PhysicsWorldWatcher.s(header.Length - s.Length);

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

                if (!w.State.HasFlag(IPhysicsWorld.WorldState.Running))
                {
                    backColor = ConsoleColor.Gray;
                    infoText = "(Stopped) ";
                    draw = false;
                }
                
                if (draw)
                {
                    string str =
                        $"{infoText}#{worlds.IndexOf(w)} dt: {w.DeltaTime}ms frame: {w.SceneFrame}ms timestamp: {w.Timestamp}";
                    str += s(header.Length - str.Length);

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

        // ReSharper disable once InconsistentNaming
        private static string s(int count)
        {
            StringSpaces.Create(count);
            return StringSpaces.s(count);
            
        }
    }
}
