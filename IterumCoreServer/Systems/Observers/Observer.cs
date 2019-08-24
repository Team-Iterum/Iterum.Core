using Magistr.Log;
using Magistr.Math;
using Magistr.Physics;
using Magistr.Things;
using Packets;
using System;
using System.Collections.Generic;
using Perf = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using static Magistr.Services.NetworkAlias;

namespace Magistr.Observers
{
    public partial class Observer
    {
        public static float MaxViewDistance = 150;

        private ViewID ViewIDPool = new ViewID(1023);
        private Vector3 Position;
        private Vector3 LastOverlapPosition;

        private IPhysicsWorld World;

        public Dictionary<int, ThingViewState> ViewThing = new Dictionary<int, ThingViewState>();
        private Dictionary<IThing, int> ThingView = new Dictionary<IThing, int>();

        public uint ConnectionId;
        public event Action OverlapSuccess;

        public Observer(IPhysicsWorld world, uint conn)
        {
            World = world;
            ConnectionId = conn;
        }

        public void ChangePosition(IThing self, Vector3 pos, bool force = false)
        {
            var length = (pos - LastOverlapPosition).magnitude;
            Position = pos;
            if (length > MaxViewDistance/2 || force)
            {
                //if (!force)
                //    Debug.Log($"[Observer {ConnectionId}] RequestOverlap Pos: {pos}|{length} LastPos: {LastOverlapPosition}", ConsoleColor.Magenta);
                //else
                //    Debug.Log($"[Observer {ConnectionId}] FORCE RequestOverlap Pos: {pos}", ConsoleColor.Red);
                {
                    var watch = Perf.StartNew();
                    Task.Run(() => MakeOverlap()).ConfigureAwait(false);
                    watch.Stop();
                }
                
                LastOverlapPosition = Position;
            }
            
        }

        public void ChangeThingState(int viewId)
        {
            ViewThing[viewId].OnChange(ConnectionId);   
        }

        private async void MakeOverlap()
        {
            var watch1 = Perf.StartNew();
            var viewList = ViewThing.Values.Select(e=>e.thing).ToList();

            // check cache
            //var cacheView = ObserverCache.Get(World.Timestamp, Position, MaxViewDistance/2 - 1);

            List<IThing> toAdd;
            List<IThing> toRemove;
            //if (cacheView != null)
            //{
            //    toAdd = cacheView.ToAdd;
            //    toRemove = cacheView.ToRemove;
            //Debug.Log($"[Observer {ConnectionId}] Overlap Time:{World.Timestamp} Pos:{Position}", ConsoleColor.DarkGreen);
            //}
            //else
            //{
            var watch = Perf.StartNew();
            // make real overlap
            var list = await World.Overlap(Position, viewList, false);
            watch.Stop();
            toAdd = list.Item1;
            toRemove = list.Item2;
            //}

            
            // add list
            foreach (var e in toAdd)
            {
                Add(e);
            }

            // remove list
            foreach (var e in toRemove)
            {
                Remove(e);

            }

            //if (cacheView == null)
            //    ObserverCache.Cache(World.Timestamp, Position, viewList, toAdd, toRemove);

            OverlapSuccess?.Invoke();
            watch1.Stop();
            //Debug.Log($"[MakeOverlap]={watch1.ElapsedMilliseconds}ms", ConsoleColor.Gray);
        }

        private void Thing_PositionChange(IThing self, Vector3 pos, bool arg2)
        {
            ChangeThingState(ThingView[self]);
           
        }

        public void Add(IThing e)
        {
            if (ThingView.ContainsKey(e)) return;
            if (e.IsDestroyed) return;

            var id = ViewIDPool.GetId();

            ViewThing.Add(id, new ThingViewState() { thing = e, viewId = (uint)id, Owner = this });
            ThingView.Add(e, id);

            Net.Send(ConnectionId, new CreateObject() { dynamicLevel = CreateObject.DynamicLevel.Static, thingTypeId = (uint)e.ThingTypeId, viewId = (uint)id });
            Net.Send(ConnectionId, new MoveFull() { Position = e.Position, Rotation = e.Rotation, viewId = (uint)id });

            if (!string.IsNullOrEmpty(e.ThingName))
                Net.Send(ConnectionId, new SetObjectName() { name = e.ThingName, viewId = (uint)id });

            e.PositionChange += Thing_PositionChange;
        }

        public void RemoveAll()
        {
            Task.Run(async () =>
            {
                await World.WaitEndOfFrame();
                foreach (var e in ThingView.Keys)
                {
                    Remove(e);
                }
                ThingView.Clear();
                ViewThing.Clear();
            });
        } 
        public void Remove(IThing e)
        {
            if (!ThingView.ContainsKey(e)) return;
            
            Net.Send(ConnectionId, new RemoveObject() { viewId = (uint)ThingView[e] });

            e.PositionChange -= Thing_PositionChange;

            ViewIDPool.ReturnId(ThingView[e]);
            ViewThing.Remove(ThingView[e]);
            ThingView.Remove(e);
           
        }
    }
}
