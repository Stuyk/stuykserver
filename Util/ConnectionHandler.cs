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
        KarmaHandler kh = new KarmaHandler();
        Util util = new Util();

        public ConnectionHandler()
        {
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onPlayerBeginConnect += API_onPlayerBeginConnect;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onResourceStop += API_onResourceStop;
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: ConnectionHandler");
        }

        private void storePlayer(Client player)
        {
            DateTime endTime = DateTime.Now;
            DateTime startTime = Convert.ToDateTime(API.getEntityData(player, "Session"));
            TimeSpan span = (endTime - startTime);

            int timePlayed = Convert.ToInt32(span.Minutes);
            int lastTime = Convert.ToInt32(API.getEntityData(player, "Time"));
            timePlayed += lastTime;

            // Gather all our data
            string[] varNames = { "LASTX", "LASTY", "LASTZ", "LoggedIn", "CurrentSkin", "Health", "Armor", "Time" };
            string before = "UPDATE Players SET";
            object[] data = { player.position.X, player.position.Y, player.position.Z, "0", ((PedHash)API.getEntityModel(player)).ToString(), API.getPlayerHealth(player).ToString(), API.getPlayerArmor(player).ToString(), timePlayed };
            string after = string.Format("WHERE ID='{0}'", API.getEntityData(player, "PlayerID"));

            // Send all our data to generate the query and run it
            db.compileQuery(before, after, varNames, data);
        }

        private void API_onResourceStop()
        {
            List<Client> playerList = API.getAllPlayers();

            foreach (Client player in playerList)
            {
                if (db.isPlayerLoggedIn(player))
                {
                    storePlayer(player);
                }
            }
        }

        public void API_onPlayerFinishedDownload(Client player)
        {
            if (!util.isValidUsername(player.name))
            {
                db.setPlayerHUD(player, false);
                API.triggerClientEvent(player, "createCamera", new Vector3(-1605.505, -1089.018, 30), new Vector3(40, 0, 0));
                API.triggerClientEvent(player, "showInvalidName");
                Random randum = new Random();
                int ran = randum.Next(1, 1000);
                API.setEntityDimension(player, ran);
                API.setEntityPosition(player, new Vector3(-1605.505, -1089.018, 13.01836));
                return;
            }

            db.setPlayerHUD(player, false);
            API.triggerClientEvent(player, "createCamera", new Vector3(-1605.505, -1089.018, 30), new Vector3(40, 0, 0));
            API.triggerClientEvent(player, "showLogin");
            Random random = new Random();
            int r = random.Next(1, 1000);
            API.setEntityDimension(player, r);
            API.setEntityPosition(player, new Vector3(-1605.505, -1089.018, 13.01836));
        }

        private void API_onPlayerBeginConnect(Client player, CancelEventArgs cancelConnection)
        {
            // Check if player has a valid username before connecting.
            
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            if (db.isPlayerLoggedIn(player))
            {
                this.storePlayer(player);
                API.consoleOutput("[DATABASE] " + player.name.ToString() + " has disconnected.");
                API.call("VehicleHandler", "removeDisconnectedVehicles", player);
            }
        }

        public void SpawnPlayer(Client player)
        {
            string[] varNames = { "ID" };
            string before = "SELECT LASTX, LASTY, LASTZ, Dead, Money, Nametag, Health, Armor, Admin, Karma, Time, Organization, Business FROM Players WHERE";
            object[] data = { Convert.ToString(API.getEntityData(player, "PlayerID")) };
            DataTable result = db.compileSelectQuery(before, varNames, data);
            
            // Player Setup Calls
            API.call("SkinHandler", "loadCurrentFace", player);
            API.call("ClothingHandler", "updateClothingForPlayer", player);
            API.call("KarmaHandler", "updateKarma", player);
            API.call("VehicleHandler", "SpawnPlayerCars", player);
            API.exported.gtaocharacter.updatePlayerFace(player.handle);

            // Player Client Events
            API.triggerClientEvent(player, "update_money_display", Convert.ToInt32(result.Rows[0]["Money"]));
            API.triggerClientEvent(player, "killPanel");
            API.triggerClientEvent(player, "endCamera");

            // Player Specific Settings
            API.freezePlayer(player, false);
            API.setEntityPosition(player, util.convertToVector3(result.Rows[0]["LASTX"], result.Rows[0]["LASTY"], result.Rows[0]["LASTZ"]));
            API.setPlayerHealth(player, Convert.ToInt32(result.Rows[0]["Health"]));
            API.setPlayerArmor(player, Convert.ToInt32(result.Rows[0]["Armor"]));
            API.setEntityDimension(player, 0);
            API.sendNativeToPlayer(player, Hash.DISPLAY_HUD, true);
            API.sendNativeToPlayer(player, Hash.DISPLAY_RADAR, true);
            API.sendNotificationToPlayer(player, "If a menu freezes. Press F1.");
            API.setEntityData(player, "Admin", Convert.ToBoolean(result.Rows[0]["Admin"]));
            API.setEntityData(player, "Karma", Convert.ToInt32(result.Rows[0]["Karma"]));
            API.setEntityData(player, "Session", DateTime.Now);
            API.setEntityData(player, "Time", Convert.ToInt32(result.Rows[0]["Time"]));
            API.setEntityData(player, "Organization", Convert.ToInt32(result.Rows[0]["Organization"]));
            API.setEntityData(player, "Business", Convert.ToInt32(result.Rows[0]["Business"]));

            // Organization Login Message
            if (Convert.ToInt32(result.Rows[0]["Organization"]) != 0)
            {
                object message = API.call("OrganizationHandler", "fetchOrgMessage", Convert.ToInt32(result.Rows[0]["Organization"]));
                API.sendChatMessageToPlayer(player, string.Format("{0}", message));
            }

            // Death Handling
            if (result.Rows[0]["Dead"].ToString() == "1")
            {
                API.sendNotificationToPlayer(player, "~r~You have died.");
                API.sendChatMessageToPlayer(player, "~g~/service EMS");
                API.sendChatMessageToPlayer(player, "~r~/tapout");
                API.playPlayerAnimation(player, (int)(AnimationFlags.StopOnLastFrame), "combat@death@from_writhe", "death_c");
            }

            string dateTime = DateTime.Now.ToString();

            API.consoleOutput("{0} logged in || Admin = {1} || On: {2}", player.name, Convert.ToBoolean(API.getEntitySyncedData(player, "Admin")), dateTime);
            string[] logNames = { "LoggedIn" };
            string logBefore = "UPDATE Players SET";
            object[] logData = { "1" };
            string after = string.Format("WHERE ID='{0}'", Convert.ToString(API.getEntityData(player, "PlayerID")));

            // Send all our data to generate the query and run it
            db.compileQuery(logBefore, after, logNames, logData);
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
