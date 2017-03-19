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
        public DeathHandler()
        {
            API.onPlayerDeath += API_onPlayerDeath;
            API.onPlayerRespawn += API_onPlayerRespawn;
        }

        private void API_onPlayerRespawn(Client player)
        {
            actionHandleDeath(player);
        }

        private void API_onPlayerDeath(Client player, NetHandle entityKiller, int weapon)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance == null)
            {
                return;
            }

            instance.setDead(true);
            API.setEntityData(player, "Death", player.position);
        }

        public void actionHandleDeath(Client player)
        {
            if (API.getEntityData(player, "Death") == null)
            {
                return;
            }

            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance == null)
            {
                return;
            }

            Vector3 deathPos = (Vector3)API.getEntityData(player, "Death");
            API.setEntityPosition(player, deathPos);
            API.setEntityData(player, "Rescued", false);

            API.sendChatMessageToPlayer(player, "~y~Death # ~b~You died. ~o~You may bleed out at anytime. ~b~Command: /bleedout");

            DateTime timeOfDeath = DateTime.Now;
            bool firstPass = false;
            while (DateTime.Now <= timeOfDeath.AddMinutes(2) && !Convert.ToBoolean(API.getEntityData(player, "Rescued")))
            {
                API.sendNativeToPlayer(player, (ulong)Hash.SET_PED_TO_RAGDOLL, player, -1, -1, 0, true, true, true);
                // Do nothing for two minutes.
                if (DateTime.Now >= timeOfDeath.AddSeconds(60) && DateTime.Now <= timeOfDeath.AddSeconds(61) && !firstPass)
                {
                    firstPass = true;
                    API.sendNotificationToPlayer(player, "~y~1 minute before ~r~bleedout.");
                }

                if (Convert.ToBoolean(API.getEntityData(player, "Bleedout")))
                {
                    break;
                }
            }

            if (!Convert.ToBoolean(API.getEntityData(player, "Rescued")))
            {
                actionForceRespawn(player);
                return;
            }

            if (!Convert.ToBoolean(API.getEntityData(player, "Rescued")))
            {
                API.sendChatMessageToPlayer(player, "~y~Please wait while the EMS take care of you.");
                return;
            }
        }

        [Command("bleedout")]
        public void actionBleedOut(Client player)
        {
            if (API.getEntityData(player, "Death") != null)
            {
                API.sendNotificationToPlayer(player, "~y~60 Seconds before ~r~respawn.");
                API.sendNotificationToPlayer(player, "~y~Logging out will reset your timer.");
                API.setEntityData(player, "Bleedout", true);
                DateTime timeOfDeath = DateTime.Now;
                while (DateTime.Now <= timeOfDeath.AddMinutes(1))
                {
                    API.sendNativeToPlayer(player, (ulong)Hash.SET_PED_TO_RAGDOLL, player, -1, -1, 0, true, true, true);
                    // Literally do nothing for a whole minute.
                }
                actionForceRespawn(player);
                return;
            }
        }

        public void actionForceRespawn(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance == null)
            {
                return;
            }

            API.setEntityPosition(player, new Vector3(-449.3315, -341.0768, 34.50172));
            instance.setDead(false);
            API.setEntityData(player, "Death", null);
            API.setEntityData(player, "Rescued", false);
            API.setEntityData(player, "Bleedout", false);
            API.freezePlayer(player, false);
            API.sendNativeToPlayer(player, (ulong)Hash.RESET_PED_RAGDOLL_TIMER, player);
            return;
        }
    }
}
