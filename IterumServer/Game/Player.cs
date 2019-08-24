using Magistr.Observers;
using Magistr.Services;
using Magistr.Things;
using Magistr.WorldMap;
using Packets;
using System;
using System.Linq;
using static Magistr.Services.NetworkAlias;
namespace Magistr.Game
{
    public class Player
    {
        private Observer Observer;

        public uint ConnectionId;
        public ICreature PlayerThing;
        public int ThingTypeId;

        // Outfit Paramaters
        public float PlayerSpeed = 0.5f;

        // Current Map Paranmters
        public int MapId = 0;

        private Map Map => MapService.GetMap(MapId);

        private bool IsCameraSet;

        public void Spawn()
        {
            var creature = new Creature(ThingTypeId, new Math.Vector3(30, 20, 0));

            // setup outfit info
            creature.Speed = PlayerSpeed;

            Map.Add(creature);
            PlayerThing = creature;
        }

        public void CreateObserver()
        {   
            Observer = new Observer(Map.PhysicsWorld, ConnectionId);
            Observer.OverlapSuccess += SetCamera;
            PlayerThing.PositionChange += Observer.ChangePosition;
        }

        public void DestroyObserver()
        {
            PlayerThing.PositionChange -= Observer.ChangePosition;
            Observer.OverlapSuccess -= SetCamera;
            Observer.RemoveAll();
            Observer = null;
        }

        public void Despawn()
        {
            MapService.GetMap(MapId).Remove(PlayerThing);
        }

        public void SetCamera()
        {
            if (!IsCameraSet)
            {
                var player = Observer.ViewThing.Values.FirstOrDefault(e => e.thing == PlayerThing);
                if (player != null)
                {
                    Net.Send(ConnectionId, new SetCamera() { viewId = player.viewId });
                    IsCameraSet = true;
                }
            }
        }

        public void ResetCamera()
        {
            if (IsCameraSet)
            {
                Net.Send(ConnectionId, new SetCamera() { reset = true });
                IsCameraSet = false;
            }
        }
    }
}