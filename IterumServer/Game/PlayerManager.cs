using Magistr.Log;
using Magistr.Network;
using Magistr.Things;
using Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using static Magistr.Services.NetworkAlias;

namespace Magistr.Game
{
    public static class PlayerManager
    {
        public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();
        public static void Create()
        {
            ChannelPackets.ParseLogin += ChannelPackets_ParseLogin;
            ChannelPackets.ParseDirection += ChannelPackets_ParseDirection;
            ChannelPackets.ParseLookFull += ChannelPackets_ParseLookFull; ;

            Net.Disconnected += Net_Disconnected;

            Debug.Log($"[PlayerManager] created", ConsoleColor.Green);
        }

        private static void Net_Disconnected(ConnectionData obj)
        {
            if (Players.ContainsKey(obj.connection)) {
                Players[obj.connection].DestroyObserver();
                Players[obj.connection].Despawn();
            }
        }

        private static void ChannelPackets_ParseLookFull(uint con, LookFull data)
        {
            var player = Players[con];
            player.PlayerThing.CharacterRotation = data.byteRotation;
        }
        private static void ChannelPackets_ParseDirection(uint con, Direction data)
        {
            var player = Players[con];
            player.PlayerThing.Move((Physics.MoveDirection)data.directionFlags);
        }

        private static void ChannelPackets_ParseLogin(uint con, Login data)
        {

            // TODO validate login
            Net.Send(con, new LoginResult() { code = LoginResult.LoginResultCode.OK, value = 0 });

            var player = new Player() { ConnectionId = con, ThingTypeId = ThingTypeManager.All.Where(e => e.Category == ThingCategory.Creature).First().ThingTypeId };

            player.Spawn();
            player.CreateObserver();

            Players.Add(con, player);


        }
    }
}
