using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Jobs
{
    public class RaceHandler : Script
    {
        Dictionary<Client, Race> activeRaces = new Dictionary<Client, Race>();

        public RaceHandler() { }

        // If the player is in an instance of the racing class.
        public bool isPlayerRacing(Client player)
        {
            foreach (Race instance in activeRaces.Values)
            {
                if (instance.isPlayerRacing(player))
                {
                    return true;
                }
            }
            return false;
        }

        [Command("startrace")]
        public void cmdStartRace(Client player, double wager)
        {
            // Check if player already has a race going.
            if (activeRaces.ContainsKey(player))
            {
                API.sendChatMessageToPlayer(player, "~r~Error: You already have an active race.");
                return;
            }

            // Check if the player is already racing.
            if (isPlayerRacing(player))
            {
                API.sendChatMessageToPlayer(player, "~r~Error: You are already in an active race.");
                return;
            }

            // Check if the wager is less than zero..
            if (wager < 0)
            {
                API.sendChatMessageToPlayer(player, "~r~Error: The wager must be more than -1.");
                return;
            }

            // Create a new instance of the Race Script, construct with the player position and assign it to the player.
            Race race = new Race(player.position, player, wager);
            activeRaces.Add(player, race);
        }

        [Command("joinrace")]
        public void cmdJoinRace(Client player)
        {
            if (isPlayerRacing(player))
            {
                API.sendChatMessageToPlayer(player, "~r~Error: You are already in an active race.");
                return;
            }


        }

        [Command("cancelrace")]
        public void cmdCancelRace(Client player)
        {
            if (activeRaces.ContainsKey(player))
            {
                activeRaces[player].Dispose();
                activeRaces.Remove(player);
            }
        }

        [Command("finishposition")]
        public void cmdFinishPosition(Client player)
        {
            // Check if player already has a race going.
            if (!activeRaces.ContainsKey(player))
            {
                API.sendChatMessageToPlayer(player, "~r~Error: You do not have an active race going. /startrace");
                return;
            }

            activeRaces[player].setEndPosition(player.position);
            activeRaces[player].setEndMarker();
            API.sendNotificationToPlayer(player, "~g~End Position Set");
        }
    }
}
