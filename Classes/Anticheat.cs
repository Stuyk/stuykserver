using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class Anticheat : Script
    {
        /*
        static string msgAntiCheat = "~y~Anticheat:";
        static string msgArmorHacks = string.Format("{0} ~o~Armor Hacks", msgAntiCheat);
        static string msgHealthHacks = string.Format("{0} ~o~Health Hacks", msgAntiCheat);
        static string msgSpeedHacks = string.Format("{0} ~o~Speed Hacks", msgAntiCheat);
        static string msgTeleportHacks = string.Format("{0} ~o~Teleport Hacks", msgAntiCheat);
        static int refreshTime = 15; // In Seconds
        DateTime lastCheckTime;

        public Anticheat()
        {
            API.onResourceStart += API_onResourceStart;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onUpdate += API_onUpdate;
        }

        // Setup EntityData for the player.
        private void API_onPlayerFinishedDownload(Client player)
        {
            API.setEntityData(player, "CHEAT_MODEL", player.model);
            API.setEntityData(player, "CHEAT_HEALTH", player.health);
            API.setEntityData(player, "CHEAT_ARMOR", player.armor);
        }

        // Depending on your refresh time, this will refresh every couple of seconds or minutes depending on what you provide.
        private void API_onUpdate()
        {
            if (DateTime.Now >= lastCheckTime.AddSeconds(refreshTime)) // After refreshTime check up on stuff.
            {
                lastCheckTime = DateTime.Now;
                scanAllPlayers();
            }
        }

        // When the resource starts, kick on the timer.
        private void API_onResourceStart()
        {
            lastCheckTime = DateTime.Now;
        }

        // Scans all players currently on the server.
        private void scanAllPlayers()
        {
            List<Client> players = API.getAllPlayers();
            foreach (Client player in players)
            {
                scanHealthChanges(player);
                scanArmorChanges(player);
            }
        }

        // Check if the player health has increased at all.
        // You can avoid a kick by setting "CHEAT_HEALTH" data before setting player health data.
        // Which can only be done SERVER SIDE.
        private void scanHealthChanges(Client player)
        {
            int oldHealth = Convert.ToInt32(API.getEntityData(player, "CHEAT_HEALTH"));
            if (player.health >= oldHealth)
            {
                API.kickPlayer(player, msgHealthHacks);
                return;
            }
        }

        // Check if the player health has increased at all.
        // You can avoid a kick by setting "CHEAT_ARMOR" data before setting player armor data.
        // Which can only be done SERVER SIDE.
        private void scanArmorChanges(Client player)
        {
            int oldArmor = Convert.ToInt32(API.getEntityData(player, "CHEAT_ARMOR"));
            if (player.armor >= oldArmor)
            {
                API.kickPlayer(player, msgArmorHacks);
                return;
            }
        }
        */

    }
}
