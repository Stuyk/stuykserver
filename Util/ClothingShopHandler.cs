﻿using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class ClothingShopHandler : Script
    {
        ClothingHandler clothingHandler = new ClothingHandler();
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();
        List<Vector3> clothingShops = new List<Vector3>();
        Dictionary<Client, Vector3> playersInClothingShop = new Dictionary<Client, Vector3>();

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public ClothingShopHandler()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Clothing Shop Handler");

            string query = "SELECT ID FROM ClothingShops";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                foreach (DataColumn column in result.Columns)
                {
                    string selectedrow = row[column].ToString();

                    float posX = Convert.ToSingle(db.pullDatabase("ClothingShops", "PosX", "ID", selectedrow));
                    float posY = Convert.ToSingle(db.pullDatabase("ClothingShops", "PosY", "ID", selectedrow));
                    float posZ = Convert.ToSingle(db.pullDatabase("ClothingShops", "PosZ", "ID", selectedrow));

                    positionBlips(new Vector3(posX, posY, posZ));

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Clothing Shops Initialized: " + initializedObjects.ToString());
        }

        public void selectClothing(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (!player.isInVehicle) // If player is not in Vehicle
                {
                    foreach (Vector3 pos in clothingShops)
                    {
                        if (player.position.DistanceTo(pos) <= 5)
                        {
                            if (db.getPlayerMoney(player) >= 60)
                            {
                                playersInClothingShop.Add(player, player.position);
                                Random rand = new Random();
                                int dimension = rand.Next(1, 1000);
                                API.setEntityDimension(player, dimension);
                                API.consoleOutput(player.name + " is in dimension " + API.getEntityDimension(player).ToString());
                                API.setEntityPosition(player, new Vector3(-1187.994, -764.7119, 17.31953));
                                API.triggerClientEvent(player, "createCamera", new Vector3(-1190.004, -766.2875, 17.3196), player.position);
                                API.triggerClientEvent(player, "openClothingPanel");
                                
                                API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@female_arms_crossed@base", "base");
                                clothingHandler.updateLocalClothingVariables(player);
                                return;
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(player, main.msgPrefix + "Not enough money.");
                                return;
                            }
                        }
                    }
                }
            }
            return;
        }

        [Command("createclothingshop")]
        public void cmdCreateClothingShop(Client player, string id)
        {
            if (db.isAdmin(player.name))
            {
                string checkID = db.pullDatabase("ClothingShops", "ID", "ID", id);

                if (checkID == null)
                {
                    string posX = player.position.X.ToString();
                    string posY = player.position.Y.ToString();
                    string posZ = player.position.Z.ToString();

                    db.insertDatabase("ClothingShops", "ID", id);
                    db.updateDatabase("ClothingShops", "PosX", posX, "ID", id);
                    db.updateDatabase("ClothingShops", "PosY", posY, "ID", id);
                    db.updateDatabase("ClothingShops", "PosZ", posZ, "ID", id);

                    positionBlips(new Vector3(player.position.X, player.position.Y, player.position.Z));
                    API.sendNotificationToPlayer(player, "~g~A clothing shop has been created.");
                    return;
                }
                else
                {
                    API.sendNotificationToPlayer(player, "~r~The ID already exists.");
                    return;
                }
            }
            else
            {
                API.sendNotificationToPlayer(player, "You are not an administrator.");
            }
        }

        public void positionBlips(Vector3 position)
        {
            API.createTextLabel("~y~[~w~Keypress: ~g~F~y~]", new Vector3(position.X, position.Y, position.Z), 20, 0.5f);
            API.createTextLabel("~w~Change your clothing.", new Vector3(position.X, position.Y, position.Z - 0.2), 20, 0.5f);
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 73);
            API.setBlipColor(newBlip, 12);
            clothingShops.Add(new Vector3(position.X, position.Y, position.Z));
            int i = 0;
            ++i;
        }

        public void leaveClothingShop(Client player)
        {
            Vector3 leavePosition = playersInClothingShop[player];
            API.setEntityDimension(player, 0);
            API.setEntityPosition(player, leavePosition);
            playersInClothingShop.Remove(player);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
        }
    }
}
