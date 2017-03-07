using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace stuykserver.Jobs
{
    public class Fishing : Script
    {
        DatabaseHandler db = new DatabaseHandler();

        Dictionary<Client,string> playersFishing = new Dictionary<Client, string>();
        Dictionary<Client, NetHandle> playersFishingRods = new Dictionary<Client, NetHandle>();

        Dictionary<ColShape, ShopInformationHandling> fishingPoints = new Dictionary<ColShape, ShopInformationHandling>();

        public Fishing()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Fishing Handler");

            string query = "SELECT * FROM Fishing";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            // Setup Fishing Points
            foreach (DataRow row in result.Rows)
            {
                float posX = Convert.ToSingle(row["PosX"]);
                float posY = Convert.ToSingle(row["PosY"]);
                float posZ = Convert.ToSingle(row["PosZ"]);
                int id = Convert.ToInt32(row["ID"]);

                positionBlips(new Vector3(posX, posY, posZ), id, ShopInformationHandling.ShopType.Fishing);

                ++initializedObjects;
            }

            API.consoleOutput("Fishing Points Intialized: " + initializedObjects.ToString());

            query = "SELECT * FROM FishingSalePoints";
            result = API.exported.database.executeQueryWithResult(query);

            initializedObjects = 0;

            // Setup Fishing Sale Points
            foreach (DataRow row in result.Rows)
            {
                float posX = Convert.ToSingle(row["PosX"]);
                float posY = Convert.ToSingle(row["PosY"]);
                float posZ = Convert.ToSingle(row["PosZ"]);
                int id = Convert.ToInt32(row["ID"]);

                positionBlips(new Vector3(posX, posY, posZ), id, ShopInformationHandling.ShopType.FishingSale);
                ++initializedObjects;
            }

            API.consoleOutput("Fishing Sale Points Intialized: " + initializedObjects.ToString());
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            var entityType = API.getEntityType(entity);
            if (Convert.ToInt32(entityType) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (fishingPoints.ContainsKey(colshape))
                {
                    if (fishingPoints[colshape].returnShopType() == ShopInformationHandling.ShopType.Fishing)
                    {
                        API.triggerClientEvent(player, "removeUseFunction");
                        fishingPoints[colshape].removeOutsidePlayer(player);
                        return;
                    }
                    
                    if (fishingPoints[colshape].returnShopType() == ShopInformationHandling.ShopType.FishingSale)
                    {
                        API.triggerClientEvent(player, "removeUseFunction");
                        fishingPoints[colshape].removeOutsidePlayer(player);
                        return;
                    }
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            var entityType = API.getEntityType(entity);
            if (Convert.ToInt32(entityType) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (fishingPoints.ContainsKey(colshape) && !player.isInVehicle)
                {
                    if (fishingPoints[colshape].returnShopType() == ShopInformationHandling.ShopType.Fishing)
                    {
                        API.triggerClientEvent(player, "triggerUseFunction", "FishingSpot");
                        API.sendNotificationToPlayer(player, "~b~There seem to be a lot of fish in this area.");
                        fishingPoints[colshape].addOutsidePlayer(player);
                        return;
                    }

                    if (fishingPoints[colshape].returnShopType() == ShopInformationHandling.ShopType.FishingSale)
                    {
                        API.triggerClientEvent(player, "triggerUseFunction", "FishingSaleSpot");
                        API.sendNotificationToPlayer(player, "~b~The cooks here seem to appreciate fresh fish.");
                        fishingPoints[colshape].addOutsidePlayer(player);
                        return;
                    }
                }
            }
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            if (playersFishing.ContainsKey(player))
            {
                playersFishing.Remove(player);
            }

            if (playersFishingRods.ContainsKey(player))
            {
                API.deleteEntity(playersFishingRods[player]);
                playersFishingRods.Remove(player);
            }

            foreach (ColShape collision in fishingPoints.Keys)
            {
                if (fishingPoints[collision].returnOutsidePlayers().Contains(player))
                {
                    fishingPoints[collision].removeOutsidePlayer(player);
                    break;
                }
            }
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if(eventName == "fishingFailed")
            {
                playerFishingFailed(player);
            }

            if (eventName == "pushWordToPanel")
            {
                API.triggerClientEvent(player, "fishingPushWord", playersFishing[player]);
            }

            if (eventName == "submitWord")
            {
                string playerWord = arguments[0].ToString();
                if (playersFishing.ContainsKey(player))
                {
                    if (playerWord.ToLower() == playersFishing[player].ToLower())
                    {
                        API.sendNotificationToPlayer(player, "~b~You reel in a fish ~g~flawlessly.");
                        playersFishing.Remove(player);
                        API.triggerClientEvent(player, "stopFishing");
                        API.playSoundFrontEnd(player, "Hack_Success", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                        API.deleteEntity(playersFishingRods[player]);
                        playersFishingRods.Remove(player);
                        API.triggerClientEvent(player, "killPanel");
                        API.stopPlayerAnimation(player);
                        API.stopPedAnimation(player);
                        playerAddFish(player);
                        return;
                    }
                    else
                    {
                        playerFishingFailed(player);
                        API.triggerClientEvent(player, "killPanel");
                        return;
                    }
                }

            }
        }

        [Command("createfishingpoint")]
        public void cmdCreateFishingPoint(Client player, string id)
        {
            if (db.isAdmin(player.name))
            {
                db.insertDataPointPosition("Fishing", player);
                API.sendNotificationToPlayer(player, "~g~A fishing point has been created.");
                return;
            }
        }

        [Command("createfishingsalepoint")]
        public void cmdCreateFishingSellPoint(Client player)
        {
            if (db.isAdmin(player.name))
            {
                db.insertDataPointPosition("FishingSalePoints", player);
                API.sendNotificationToPlayer(player, "~g~A fishing sale point has been created.");
            }
        }

        public void sellFish(Client player)
        {
            foreach (ColShape collision in fishingPoints.Keys)
            {
                // Check if player is in the collision and the collision type is of the 'Sale Type'
                if (fishingPoints[collision].returnOutsidePlayers().Contains(player) && fishingPoints[collision].returnShopType() == ShopInformationHandling.ShopType.FishingSale && !player.isInVehicle)
                {
                    int fish = Convert.ToInt32(db.pullDatabase("PlayerInventory", "Fish", "Nametag", player.name));
                    if (fish <= 0)
                    {
                        API.sendNotificationToPlayer(player, "~r~You don't seem to have any fish.");
                        return;
                    }
                    else
                    {
                        db.setPlayerMoney(player, 7 * fish);
                        db.updateDatabase("PlayerInventory", "Fish", 0.ToString(), "Nametag", player.name);
                        API.sendNotificationToPlayer(player, string.Format("~b~You sold ~g~{0} ~b~fish.", fish));
                    }
                }
            }
        }

        public void startFishing(Client player)
        {
            foreach (ColShape collision in fishingPoints.Keys)
            {
                // Check if player is in the collision and the collision type is of the 'Fishing Type'
                if (fishingPoints[collision].returnOutsidePlayers().Contains(player) && fishingPoints[collision].returnShopType() == ShopInformationHandling.ShopType.Fishing && !player.isInVehicle)
                {
                    // Check if Inventory contains less than 10 fish. Check if they're already fishing.
                    if (!playersFishing.ContainsKey(player))
                    {
                        if (Convert.ToInt32(db.pullDatabase("PlayerInventory", "Fish", "Nametag", player.name)) >= 10)
                        {
                            API.sendNotificationToPlayer(player, "~r~ You have too many fish.");
                            break;
                        }

                        string word = sendWordToPlayer();
                        API.triggerClientEvent(player, "startFishing", word);
                        playersFishing.Add(player, word);

                        var fishingrod = API.createObject(-1910604593, new Vector3(), new Vector3());
                        API.attachEntityToEntity(fishingrod, player, "SKEL_L_Hand", new Vector3(0.13f, 0.1f, 0.01f), new Vector3(180f, 90f, 70f));
                        playersFishingRods.Add(player, fishingrod);

                        API.playPlayerAnimation(player, 1, "amb@world_human_stand_fishing@idle_a", "idle_c");
                        break;
                    }

                    break;
                }
            }
        }

        public void positionBlips(Vector3 position, int id, ShopInformationHandling.ShopType point)
        {
            ShopInformationHandling newPoint = new ShopInformationHandling();
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 10f, 5f);
           
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));

            if (point == ShopInformationHandling.ShopType.Fishing)
            {
                API.setBlipSprite(newBlip, 68);
                API.setBlipColor(newBlip, 42);
                API.setBlipShortRange(newBlip, true);
            }

            if (point == ShopInformationHandling.ShopType.FishingSale)
            {
                API.setBlipSprite(newBlip, 431);
                API.setBlipColor(newBlip, 42);
                API.setBlipShortRange(newBlip, true);
            }

            newPoint.setCollisionShape(shape);
            newPoint.setCollisionID(id);
            newPoint.setCollisionPosition(position);
            newPoint.setBlip(newBlip);
            newPoint.setShopType(point);

            fishingPoints.Add(shape, newPoint);
        }

        public string sendWordToPlayer()
        {
            var lines = File.ReadAllLines("resources/stuykserver/Jobs/wordlist.txt");
            Random random = new Random();
            string word = lines[random.Next(0, lines.Length)];
            return word;
        }

        public void playerFishingFailed(Client player)
        {
            API.sendNotificationToPlayer(player, "~r~Failed, the fish got away!");
            playersFishing.Remove(player);
            API.triggerClientEvent(player, "stopFishing");
            API.playSoundFrontEnd(player, "Hack_Failed", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
            API.deleteEntity(playersFishingRods[player]);
            playersFishingRods.Remove(player);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
        }

        public void playerAddFish(Client player)
        {
            int oldFish = Convert.ToInt32(db.pullDatabase("PlayerInventory", "Fish", "Nametag", player.name));
            oldFish += 1;
            db.updateDatabase("PlayerInventory", "Fish", oldFish.ToString(), "Nametag", player.name);
            API.sendNotificationToPlayer(player, string.Format("~b~Your inventory contains ~g~{0}/10 ~b~Fish.", oldFish));
        }
    }
}
