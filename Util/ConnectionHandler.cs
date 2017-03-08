﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkServer;
using GTANetworkShared;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Diagnostics;

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
            // Gather all our data
            string[] varNames = { "LASTX", "LASTY", "LASTZ", "LoggedIn", "CurrentSkin", "Health", "Armor" };
            string before = "UPDATE Players SET";
            object[] data = { player.position.X, player.position.Y, player.position.Z, "0", ((PedHash)API.getEntityModel(player)).ToString(), API.getPlayerHealth(player).ToString(), API.getPlayerArmor(player).ToString() };
            string after = string.Format("WHERE Nametag='{0}'", player.name);

            // Send all our data to generate the query and run it
            this.db.compileQuery(before, after, varNames, data);
        }

        private void API_onResourceStop()
        {
            List<Client> playerList = API.getAllPlayers();

            foreach (Client player in playerList)
            {
                if (db.isPlayerLoggedIn(player))
                {
                    this.storePlayer(player);
                }
            }
        }

        public void API_onPlayerFinishedDownload(Client player)
        {
            db.setPlayerHUD(player, false);
            API.triggerClientEvent(player, "createCamera", new Vector3(-1605.505, -1089.018, 30), new Vector3(40, 0, 0));
            Random random = new Random();
            int r = random.Next(1, 1000);
            API.setEntityDimension(player, r);
            API.setEntityPosition(player, new Vector3(-1605.505, -1089.018, 13.01836));
        }

        private void API_onPlayerBeginConnect(Client player, CancelEventArgs cancelConnection)
        {
            // Check if player has a valid username before connecting.
            if (!main.isValidUsername(player.name))
            {
                API.kickPlayer(player, "Invalid Username Format. Example: John_Doe");
                return;
            }
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
            db.setPlayerLoggedIn(player);
            string query = string.Format("SELECT LASTX, LASTY, LASTZ, Dead, Money, Nametag, Health, Armor FROM Players WHERE ID='{0}'", API.getEntitySyncedData(player, "PlayerID"));
            DataTable result = API.exported.database.executeQueryWithResult(query);
            
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

            // Death Handling
            if (result.Rows[0]["Dead"].ToString() == "1")
            {
                API.sendNotificationToPlayer(player, "~r~You have died.");
                API.sendChatMessageToPlayer(player, "~g~/service EMS");
                API.sendChatMessageToPlayer(player, "~r~/tapout");
                API.playPlayerAnimation(player, (int)(AnimationFlags.StopOnLastFrame), "combat@death@from_writhe", "death_c");
            }
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
