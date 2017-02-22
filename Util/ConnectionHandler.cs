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
        SpawnPoints spawnPoints = new SpawnPoints();
        DatabaseHandler db = new DatabaseHandler();
        SkinHandler skinHandler = new SkinHandler();
        ClothingHandler clothingHandler = new ClothingHandler();
        KarmaHandler kh = new KarmaHandler();

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

        private void API_onResourceStop()
        {
            List<Client> playerList = API.getAllPlayers();

            foreach (Client player in playerList)
            {
                db.setPlayerLoggedOut(player);
                db.setPlayerPosition(player);
                db.updateDatabase("Players", "CurrentSkin", ((PedHash)API.getEntityModel(player)).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "Health", API.getPlayerHealth(player).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "Armor", API.getPlayerArmor(player).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "JobStarted", "False", "Nametag", player.name);
                db.updateDatabase("Players", "JobX", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobY", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobZ", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobType", "None", "Nametag", player.name);

                if (db.pullDatabase("Players", "TempJobVehicle", "Nametag", player.name) != "None")
                {
                    NetHandle tempVehicle = new NetHandle(Convert.ToInt32(db.pullDatabase("Players", "TempJobVehicle", "Nametag", player.name)));
                    API.deleteEntity(tempVehicle);
                }

                db.updateDatabase("Players", "TempJobVehicle", "None", "Nametag", player.name);
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
                db.setPlayerPosition(player);
                db.setPlayerLoggedOut(player);
                db.updateDatabase("Players", "CurrentSkin", ((PedHash)API.getEntityModel(player)).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "Health", API.getPlayerHealth(player).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "Armor", API.getPlayerArmor(player).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "JobStarted", "False", "Nametag", player.name);
                db.updateDatabase("Players", "JobX", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobY", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobZ", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobType", "None", "Nametag", player.name);
                API.consoleOutput("[DATABASE] " + player.name.ToString() + " has disconnected.");
                if (db.pullDatabase("Players", "TempJobVehicle", "Nametag", player.name) != "None")
                {
                    NetHandle tempVehicle = new NetHandle(Convert.ToInt32(db.pullDatabase("Players", "TempJobVehicle", "Nametag", player.name)));
                    API.deleteEntity(tempVehicle);
                }
                db.updateDatabase("Players", "TempJobVehicle", "None", "Nametag", player.name);
            }
        }

        public void SpawnPlayer(Client player)
        {
            string s = player.socialClubName;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var x = Convert.ToSingle(db.pullDatabase("Players", "LastX", "Nametag", player.name));
            var y = Convert.ToSingle(db.pullDatabase("Players", "LastY", "Nametag", player.name));
            var z = Convert.ToSingle(db.pullDatabase("Players", "LastZ", "Nametag", player.name));

            db.updateDatabase("Players", "JobStarted", "False", "Nametag", player.name);
            db.updateDatabase("Players", "JobX", "0", "Nametag", player.name);
            db.updateDatabase("Players", "JobY", "0", "Nametag", player.name);
            db.updateDatabase("Players", "JobZ", "0", "Nametag", player.name);
            db.updateDatabase("Players", "JobType", "None", "Nametag", player.name);

            player.freezePosition = false;

            // Set / Load Player Data
            API.setEntityPosition(player, new Vector3(x, y, z));
            skinHandler.loadCurrentFace(player);
            clothingHandler.updateClothingForPlayer(player);
            kh.updateKarma(player);

            // Kill any open panels.
            API.triggerClientEvent(player, "killPanel");
            API.setEntityDimension(player, 0);

            //API.call("VehicleHandler", "SpawnPlayerCars", player);

            if (db.pullDatabase("Players", "Dead", "Nametag", player.name) == "1")
            {
                API.sendNotificationToPlayer(player, "~r~You have died.");
                API.sendChatMessageToPlayer(player, "~g~/service EMS");
                API.sendChatMessageToPlayer(player, "~r~/tapout");
                API.playPlayerAnimation(player, (int)(AnimationFlags.StopOnLastFrame), "combat@death@from_writhe", "death_c");
            }

            API.exported.gtaocharacter.updatePlayerFace(player.handle);

            API.triggerClientEvent(player, "endCamera");
            API.sendNativeToPlayer(player, Hash.DISPLAY_HUD, true);
            API.sendNativeToPlayer(player, Hash.DISPLAY_RADAR, true);
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
