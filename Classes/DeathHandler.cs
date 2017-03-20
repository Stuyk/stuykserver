using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace stuykserver.Util
{
    public class DeathHandler : Script
    {
        DateTime deathScanTime; // Time since last Death Scan.
        Dictionary<Client, DateTime> deadPlayers; // Player and Time of Death.
        List<Client> queueRemove = new List<Client>();

        public DeathHandler()
        {
            API.onPlayerDeath += API_onPlayerDeath;
            API.onPlayerRespawn += API_onPlayerRespawn;
            API.onResourceStart += API_onResourceStart;
            API.onUpdate += API_onUpdate;
        }

        private void API_onUpdate()
        {
            if (DateTime.Now > deathScanTime.AddSeconds(10))
            {
                actionScanDeaths();
            }
        }

        private void API_onResourceStart()
        {
            deathScanTime = DateTime.Now;
            deadPlayers = new Dictionary<Client, DateTime>();
        }

        // When the player dies set their player instance to dead and update the database incase of logout.
        private void API_onPlayerDeath(Client player, NetHandle entityKiller, int weapon)
        {
            API.setEntityData(player, "DeathPosition", player.position);

            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance == null)
            {
                return;
            }

            if (!instance.isDead())
            {
                instance.setDead(true);
            }
            else
            {
                actionSendToHospital(player);
                return;
            }
        }

        // When the player respawns, move them to their last location and force ragdoll.
        // Also setup a date when they died and add it to the dead player list.
        private void API_onPlayerRespawn(Client player)
        {
            if (API.getEntityData(player, "DeathPosition") == null)
            {
                return;
            }

            if (!deadPlayers.ContainsKey(player))
            {
                deadPlayers.Add(player, DateTime.Now);
            }

            Vector3 position = (Vector3)API.getEntityData(player, "DeathPosition");
            API.setEntityPosition(player, position);
            API.sendChatMessageToPlayer(player, "~y~You have died. You may ~r~/bleedout ~y~ at any time.");
            API.sendChatMessageToPlayer(player, "~y~Bleeding out will ~r~remove all ~y~of your weapons.");
            API.sendChatMessageToPlayer(player, "~y~You may also wait for an ~b~EMT ~y~ to rescue you.");
            API.sendNativeToPlayer(player, (ulong)Hash.SET_PED_TO_RAGDOLL, player, -1, -1, 0, true, true, true);
            API.setEntityData(player, "Bleedingout", null);
            TimeSpan newDate = (deadPlayers[player].AddMinutes(5) - DateTime.Now);
            TextLabel label = API.createTextLabel(string.Format("~r~Bleedout: ~b~{0}m {1}s", newDate.Minutes, newDate.Seconds), player.position, 10f, 1.0f, true);
            API.setEntityData(player, "DeathText", label);
            return;
        }

        public void actionSpawnAsDead(Client player)
        {
            if (!deadPlayers.ContainsKey(player))
            {
                deadPlayers.Add(player, DateTime.Now);
            }

            API.sendChatMessageToPlayer(player, "~y~You have died. You may ~r~/bleedout ~y~ at any time.");
            API.sendChatMessageToPlayer(player, "~y~Bleeding out will ~r~remove all ~y~of your weapons.");
            API.sendChatMessageToPlayer(player, "~y~You may also wait for an ~b~EMT ~y~ to rescue you.");
            API.sendNativeToPlayer(player, (ulong)Hash.SET_PED_TO_RAGDOLL, player, -1, -1, 0, true, true, true);
            API.setEntityData(player, "Bleedingout", null);
            TimeSpan newDate = (deadPlayers[player].AddMinutes(5) - DateTime.Now);
            TextLabel label = API.createTextLabel(string.Format("~r~Bleedout: ~b~{0}m {1}s", newDate.Minutes, newDate.Seconds), player.position, 10f, 1.0f, true);
            API.setEntityData(player, "DeathText", label);
            return;
        }

        [Command("bleedout")]
        public void cmdActionBleedOut(Client player)
        {
            if (Convert.ToBoolean(API.getEntityData(player, "Bleedingout")))
            {
                return;
            }

            if (deadPlayers.ContainsKey(player))
            {
                deadPlayers[player] = deadPlayers[player].Add(new TimeSpan(0, 0, -240));
                API.sendChatMessageToPlayer(player, "~o~You have chosen to bleed out.");
                API.sendChatMessageToPlayer(player, "~b~You will respawn momentarily");
                API.setEntityData(player, "Bleedingout", true);
            }
        }

        public void actionScanDeaths()
        {
            if (deadPlayers.Count < 1)
            {
                return;
            }

            foreach (Client player in deadPlayers.Keys)
            {
                API.sendNativeToPlayer(player, (ulong)Hash.SET_PED_TO_RAGDOLL, player, -1, -1, 0, true, true, true);

                if (Convert.ToBoolean(API.getEntityData(player, "Bleedingout")))
                {
                    TimeSpan newDate = (deadPlayers[player].AddMinutes(5) - DateTime.Now);
                    API.setTextLabelText(API.getEntityData(player, "DeathText"), string.Format("~o~Dead: ~b~{0}m {1}s", newDate.Minutes, newDate.Seconds));
                    API.setEntityPosition(API.getEntityData(player, "DeathText"), player.position);
                }
                else
                {
                    TimeSpan newDate = (deadPlayers[player].AddMinutes(5) - DateTime.Now);
                    API.setTextLabelText(API.getEntityData(player, "DeathText"), string.Format("~r~Bleedout: ~b~{0}m {1}s", newDate.Minutes, newDate.Seconds));
                    API.setEntityPosition(API.getEntityData(player, "DeathText"), player.position);
                }

                if (DateTime.Now > deadPlayers[player].AddMinutes(5))
                {
                    queueRemove.Add(player);
                    actionSendToHospital(player);
                }
            }

            foreach (Client player in queueRemove)
            {
                API.sendNativeToPlayer(player, (ulong)Hash.RESET_PED_RAGDOLL_TIMER, player);
                if (deadPlayers.ContainsKey(player))
                {
                    deadPlayers.Remove(player);
                }
            }

            queueRemove.Clear();
            return;
        }

        public void actionSendToHospital(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance == null)
            {
                return;
            }

            // Cleanup List
            if (deadPlayers.ContainsKey(player))
            {
                deadPlayers.Remove(player);
            }

            DateTime hospitalCallTime = DateTime.Now;
            bool respawnTimePassed = false;
            while (respawnTimePassed == false)
            {
                // Wait for timeout or when the player arrives at position.
                if (DateTime.Now > hospitalCallTime.AddSeconds(8))
                {
                    break;
                }
            }

            API.deleteEntity(API.getEntityData(player, "DeathText"));
            API.setEntityPosition(player, new Vector3(-449.3315, -341.0768, 34.50172));
            instance.setDead(false);
            instance.removePlayerCash(150);
            API.setEntityData(player, "Bleedingout", null);
            API.setEntityData(player, "DeathText", null);
            API.setEntityData(player, "DeathPosition", null);
            API.sendNativeToPlayer(player, (ulong)Hash.RESET_PED_RAGDOLL_TIMER, player);
            API.sendNotificationToPlayer(player, "~b~You have respawned at the hospital.");
            return;
        }
    }
}
