using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace stuykserver
{
    public class Main : Script
    {
        DatabaseHandler db = new DatabaseHandler();

        public string msgPrefix = "~y~[~w~STUYK~y~]~w~ ";

        public Main()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.setGamemodeName("~r~Stuyk ~w~Premier ~b~Roleplay");
            API.consoleOutput("Started: Main");
        }

        [Command("resetdimension")]
        public void cmdResetDimension(Client player)
        {
            player.dimension = 0;
        }

        [Command("goto")]
        public void cmdGoTo(Client player, string target)
        {
            API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
            player.position = API.getPlayerFromName(target).position;
        }

        [Command("getpos")] //Temporary
        public void cmdGetPos(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                string xyz = API.getEntityPosition(player).ToString();
                string rot = API.getEntityRotation(player).ToString();
                API.sendNotificationToPlayer(player, "POS: " + xyz);
                API.sendNotificationToPlayer(player, "ROT: " + rot);
                return;
            }
            return;
        }

        // Removes current active checkpoints, blips, etc.
        [Command("clear")]
        public void cmdClearBlips(Client player)
        {
            API.triggerClientEvent(player, "removeBlips");
        }

        // Modified send notificaiton with clientside noise.
        public void sendNotification(Client player, string message)
        {
            API.playSoundFrontEnd(player, "Menu_Accept", "Phone_SoundSet_Default");
            API.sendNotificationToPlayer(player, message);
        }

        [Command("fakemarker")]
        public void cmdFakeMarker(Client player, int type)
        {
            API.createMarker(type, new Vector3(player.position.X, player.position.Y, player.position.Z - 1.5), new Vector3(), new Vector3(), new Vector3(3, 3, 10), 50, 0, 255, 0);
        }
    }
}
