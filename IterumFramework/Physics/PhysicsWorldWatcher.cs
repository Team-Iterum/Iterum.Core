using Magistr.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Magistr.Physics
{
    public class PhysicsWorldWatcher
    {
        public const int Period = 3000;
        private Timer timer;
        private static StringBuilder builder = new StringBuilder();
        private List<IPhysicsWorld> worlds;

        public PhysicsWorldWatcher(List<IPhysicsWorld> worlds)
        {
            this.worlds = worlds;
        }

        public void Start()
        {
            timer = new Timer(new TimerCallback(Watch), worlds, 0, Period);
        }
        public static void Watch(object obj)
        {
            List<IPhysicsWorld> worlds = ((List<IPhysicsWorld>)obj);
            if (worlds.Count == 0) return;
            Debug.Back(System.ConsoleColor.DarkBlue);
            var header = "=========== Physics Watcher ===========";
            Debug.Log(header, System.ConsoleColor.White);
            Debug.ResetBack();
            
            foreach (var w in worlds)
            {
                System.ConsoleColor color = System.ConsoleColor.Black;
                System.ConsoleColor backColor = System.ConsoleColor.Green;

                if(!w.IsCreated)
                {
                    Debug.Back(System.ConsoleColor.Gray);
                    var s = $"#{worlds.IndexOf(w)} not yet created";
                    s += Space(header.Length - s.Length);

                    Debug.Log(s, System.ConsoleColor.White);
                    Debug.ResetBack();
                    continue;
                }
                if (w.IsDestoyed)
                {
                    Debug.Back(System.ConsoleColor.Black);
                    var s = $"#{worlds.IndexOf(w)} is destroyed";
                    s += Space(header.Length - s.Length);

                    Debug.Log(s, System.ConsoleColor.White);
                    Debug.ResetBack();
                    continue;
                }
                if (!w.IsRunning)
                {
                    Debug.Back(System.ConsoleColor.Gray);
                    var s = $"#{worlds.IndexOf(w)} stopped";
                    s += Space(header.Length - s.Length);

                    Debug.Log(s, color);
                    Debug.ResetBack();
                    continue;
                }

                string infoText = "(Normal) ";

                if(w.SceneFrame > w.TPS/2)
                {
                    backColor = System.ConsoleColor.DarkYellow;
                    infoText = "(Warning) ";
                }
                if (w.SceneFrame >= w.TPS * 0.75f)
                {
                    backColor = System.ConsoleColor.Red;
                    color = System.ConsoleColor.White;
                    infoText = "(Overload) ";
                }
                if (w.SceneFrame >= w.TPS)
                {
                    backColor = System.ConsoleColor.DarkRed;
                    color = System.ConsoleColor.White;
                    infoText = "(Fatal Overload) ";
                }
                if (!w.IsRunning)
                {
                    backColor = System.ConsoleColor.Gray;
                    infoText = "(Stopped) ";
                }

                Debug.Back(backColor);
                var str = $"{infoText}#{worlds.IndexOf(w)} dt: {w.DeltaTime}ms frame: {w.SceneFrame}ms timestamp: {w.Timestamp}";
                str += Space(header.Length - str.Length);
                
                Debug.Log(str, color);
                Debug.ResetBack();
            }
            Console.ResetColor();
        }
        

        private static string Space(int count)
        {
            builder.Clear();
            for (int i = 0; i < count; i++)
            {
                builder.Append(" ");
            }
            return builder.ToString();

        }
    }
}
