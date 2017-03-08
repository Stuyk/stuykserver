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
            API.onPlayerHealthChange += API_onPlayerHealthChange;
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.setGamemodeName("~r~Stuyk ~w~Premier ~b~Roleplay");
            API.consoleOutput("Started: Main");
        }


        private void API_onPlayerHealthChange(Client player, int oldValue)
        {
            db.updateDatabase("Players", "Health", player.health.ToString(), "Nametag", player.name);
        }

        [Command("drive")]
        public void cmdDrive(Client player)
        {
            var ped = API.createPed(PedHash.Agent14, new Vector3(-1639.599, -773.7659, 12.04596), 1f);

            Blip blip = API.createBlip(ped);
            blip.attachTo(ped, "", new Vector3(), new Vector3());

            var vehicle = API.createVehicle(VehicleHash.BfInjection, new Vector3(-1640, -773.7659, 10.04596), new Vector3(), 0, 0);
            API.sendNativeToAllPlayers((ulong)Hash.SET_PED_INTO_VEHICLE, ped, vehicle, -1);
            API.sendNativeToAllPlayers((ulong)Hash.TASK_VEHICLE_DRIVE_TO_COORD, ped, vehicle, -1639.599, -773.7659, 10.04596, 10f, 0f, (VehicleHash)vehicle.model, 2883621, 1f, true);
        }

        [Command("resetdimension")]
        public void cmdResetDimension(Client player)
        {
            player.dimension = 0;
        }

        [Command("goto")]
        public void cmdGoTo(Client player, string target)
        {
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

        [Command("inventory")]
        public void cmdInventory(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                API.triggerClientEvent(player, "openInventory", player.name);
                return;
            }
        }

        // Modified send notificaiton with clientside noise.
        public void sendNotification(Client player, string message)
        {
            API.playSoundFrontEnd(player, "Menu_Accept", "Phone_SoundSet_Default");
            API.sendNotificationToPlayer(player, message);
        }
    }
}
