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
        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        DateTime deathScanTime; // Time since last Death Scan.
        Dictionary<Client, int> deadPlayers; // Player and Time to Wait.

        public DeathHandler()
        {
            API.onPlayerDeath += API_onPlayerDeath;
            API.onPlayerRespawn += API_onPlayerRespawn;
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            deathScanTime = DateTime.Now;
            deadPlayers = new Dictionary<Client, int>();

            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(checkDeaths);
            timer.Interval = 1000; // 1 Second
            timer.Enabled = true; // Enable the timer
        }

        public void checkDeaths(object source, ElapsedEventArgs e)
        {
            List<Client> players = new List<Client>();

            // Add all of our client info here.
            foreach (Client player in deadPlayers.Keys)
            {
                players.Add(player);
            }

            // Subtract our values before going into the real edit.
            foreach (Client player in players)
            {
                if (deadPlayers.ContainsKey(player))
                {
                    deadPlayers[player] = deadPlayers[player] - 1;
                }
            }

            players.Clear();

            foreach (Client player in deadPlayers.Keys)
            {
                API.sendNativeToPlayer(player, (ulong)Hash.SET_PED_TO_RAGDOLL, player, -1, -1, 0, true, true, true);

                // Check if the player has decided to bleed out or not.
                if (Convert.ToBoolean(API.getEntityData(player, "Bleedingout")))
                {
                    API.setTextLabelText(API.getEntityData(player, "DeathText"), string.Format("~o~Dead: ~b~{0}s", deadPlayers[player]));
                    API.setEntityPosition(API.getEntityData(player, "DeathText"), player.position);
                }
                else
                {
                    API.setTextLabelText(API.getEntityData(player, "DeathText"), string.Format("~r~Bleedout: ~b~{0}s", deadPlayers[player]));
                    API.setEntityPosition(API.getEntityData(player, "DeathText"), player.position);
                }

                // If the players timer is 0 send them to the hospital.
                if (deadPlayers[player] < 0)
                {
                    actionSendToHospital(player);
                }

                // If the players health is less than 10, force them to the hospital.
                // This makes double tap a lot easier.
                if (player.health < 10)
                {
                    API.setEntityData(player, "AlreadyDied", true);
                }
            }
        }

        private void actionKarmaRemove(Client killer)
        {
            Player killerInstance = (Player)API.call("PlayerHandler", "getPlayer", killer);

            // If above 0, remove Karma.
            if (killerInstance.returnPlayerKarma() > 0)
            {
                killerInstance.removePlayerKarma(25);
                // Set to 0 if they drop below.
                if (killerInstance.returnPlayerKarma() <= 0)
                {
                    killerInstance.setPlayerKarma(0);
                }
            }

            // If below 0, add Karma.
            if (killerInstance.returnPlayerKarma() < 0)
            {
                killerInstance.addPlayerKarma(25);

                // Set to 0 if they go above.
                if (killerInstance.returnPlayerKarma() >= 0)
                {
                    killerInstance.setPlayerKarma(0);
                }
            }
        }

        // When the player dies set their player instance to dead and update the database incase of logout.
        private void API_onPlayerDeath(Client player, NetHandle entityKiller, int weapon)
        {
            API.setEntityData(player, "DeathPosition", player.position);

            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            instance.setDead(true);

            // Check if the player who died is an active shooter. If they aren't, remove Karma from the killer.
            if (API.getEntityData(player, "ActiveShooter") == false)
            {
                Client killer = API.getPlayerFromHandle(entityKiller);
                actionKarmaRemove(killer);
            }
        }

        // When the player respawns, move them to their last location and force ragdoll.
        // Also setup a date when they died and add it to the dead player list.
        private void API_onPlayerRespawn(Client player)
        {
            if (API.getEntityData(player, "AlreadyDied"))
            {
                actionSendToHospital(player);
                return;
            }

            API.setPlayerHealth(player, 100);

            if (API.getEntityData(player, "DeathPosition") == null)
            {
                return;
            }

            // Set the player to their death position.
            Vector3 position = (Vector3)API.getEntityData(player, "DeathPosition");
            API.setEntityPosition(player, position);

            // Set bleedingout to null for later usage.
            API.setEntityData(player, "Bleedingout", null);

            // Send some chat messages.
            API.sendChatMessageToPlayer(player, "~y~You have died. You may ~r~/bleedout ~y~ at any time.");
            API.sendChatMessageToPlayer(player, "~y~Bleeding out will ~r~remove all ~y~of your weapons.");
            API.sendChatMessageToPlayer(player, "~y~You may also wait for an ~b~EMT ~y~ to rescue you.");

            // Add them to the list and force them into ragdoll mode.
            if (!deadPlayers.ContainsKey(player))
            {
                deadPlayers.Add(player, 300);
            }

            TextLabel label = API.createTextLabel(string.Format("~r~Bleedout: {0}s", deadPlayers[player]), player.position, 10f, 1.0f, true);
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
                deadPlayers[player] = deadPlayers[player] - 240;
                API.sendChatMessageToPlayer(player, "~o~You have chosen to bleed out.");
                API.sendChatMessageToPlayer(player, "~b~You will respawn momentarily");
                API.setEntityData(player, "Bleedingout", true);
            }
        }

        public void actionSendToHospital(Client player)
        {
            if (deadPlayers.ContainsKey(player))
            {
                deadPlayers.Remove(player);
            }

            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance == null)
            {
                return;
            }

            // Cleanup List
            if (API.getEntityData(player, "DeathText") != null)
            {
                API.deleteEntity((TextLabel)API.getEntityData(player, "DeathText"));
            }

            API.setEntityData(player, "Bleedingout", null);
            API.setEntityData(player, "DeathText", null);
            API.setEntityData(player, "DeathPosition", null);
            API.setEntityData(player, "AlreadyDied", false);
            
            API.sendNotificationToPlayer(player, "~b~You have respawned at the hospital.");
            instance.setDead(false);
            instance.removePlayerCash(150);

            API.sendNativeToPlayer(player, (ulong)Hash.RESET_PED_RAGDOLL_TIMER, player);
            API.setEntityPosition(player, new Vector3(-449.3315, -341.0768, 34.50172));
            return;
        }
    }
}
