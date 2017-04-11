using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkServer;
using GTANetworkShared;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Diagnostics;
using stuykserver.Classes;

namespace stuykserver.Util
{
    public class ConnectionHandler : Script
    {
        Main main = new Main();
        DatabaseHandler db = new DatabaseHandler();
        SkinHandler skinHandler = new SkinHandler();
        ClothingHandler clothingHandler = new ClothingHandler();
        Util util = new Util();

        public ConnectionHandler()
        {
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onResourceStop += API_onResourceStop;
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: ConnectionHandler");
        }

        private void API_onResourceStop()
        {
            List<Client> playerList = API.getAllPlayers();

            foreach (Client player in playerList)
            {
                API.call("PlayerHandler", "cleanupPlayer", player);
            }
        }

        public void API_onPlayerFinishedDownload(Client player)
        {
            // If the username is invalid. Display the panel.
            if (!util.isValidUsername(player.name))
            {
                Random randum = new Random();
                int ran = randum.Next(1, 1000);
                API.setEntityDimension(player, ran);
                API.setEntityPosition(player, new Vector3(649.5031, -10.4181, 75));
                API.freezePlayer(player, true);
                db.setPlayerHUD(player, false);
                API.triggerClientEvent(player, "createCamera", new Vector3(649.5031, -10.4181, 450), new Vector3(649.5031, -10.4181, 82.78617));
                API.triggerClientEvent(player, "showInvalidName");
                return;
            }

            // If the username is valid. Move on.
            API.triggerClientEvent(player, "createCamera", new Vector3(649.5031, -10.4181, 450), new Vector3(649.5031, -10.4181, 82.78617));
            API.triggerClientEvent(player, "showLogin");
            Random random = new Random();
            int r = random.Next(1, 1000);
            API.setEntityDimension(player, r);
            API.setEntityPosition(player, new Vector3(649.5031, -10.4181, 75));
            API.freezePlayer(player, true);
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance != null)
            {
                instance.savePlayerLogOut();
            }


            API.call("PlayerHandler", "cleanupPlayer", player);
            API.call("VehicleHandler", "removeDisconnectedVehicles", player);
        }

        public void SpawnPlayer(Client player)
        {
            string[] varNames = { "ID" };
            string before = "SELECT ID, LASTX, LASTY, SocialClub, LASTZ, Dead, Money, Bank, Nametag, Health, Armor, Admin, Karma, Time, Organization, Business FROM Players WHERE";
            object[] data = { Convert.ToString(API.getEntityData(player, "PlayerID")) };
            DataTable result = db.compileSelectQuery(before, varNames, data);

            Player playerInstance = (Player)API.call("PlayerHandler", "createPlayer", result.Rows[0], player);

            // Player Setup Calls
            API.call("SkinHandler", "loadCurrentFace", player);
            API.call("ClothingHandler", "updateClothingForPlayer", player);
            API.call("VehicleHandler", "SpawnPlayerCars", player);
            API.exported.gtaocharacter.updatePlayerFace(player.handle);

            // Player Client Events
            API.triggerClientEvent(player, "serverLoginCamera", player.position, player.rotation);
            API.triggerClientEvent(player, "update_money_display", playerInstance.returnPlayerCash());
            API.setEntityData(player, "CHEAT_MODEL", player.model);

            // Player Specific Settings
            API.freezePlayer(player, false);
            API.sendNativeToPlayer(player, Hash.DISPLAY_HUD, true);
            API.sendNativeToPlayer(player, Hash.DISPLAY_RADAR, true);

            API.sendNotificationToPlayer(player, "If a menu freezes. Press F1.");

            // Organization Login Message
            if (playerInstance.returnPlayerOrganization() != 0)
            {
                string message = (string)API.call("OrganizationHandler", "fetchOrgMessage", playerInstance.returnPlayerOrganization());
                API.sendChatMessageToPlayer(player, message);
            }

            // Death Handling
            if (playerInstance.isDead())
            {
                API.call("DeathHandler", "actionSendToHospital", player);
            }

            playerInstance.setPlayerModel(player.model);
        }

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }
    }
}
