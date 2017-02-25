using GTANetworkServer;
using GTANetworkShared;
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
        SpawnPoints spawnPoints = new SpawnPoints();
        DatabaseHandler db = new DatabaseHandler();
        List<Vector3> fishingPoints = new List<Vector3>();
        List<Vector3> fishingSalePoints = new List<Vector3>();

        Dictionary<Client,string> playersFishing = new Dictionary<Client, string>();
        Dictionary<Client, NetHandle> playersFishingRods = new Dictionary<Client, NetHandle>();

        List<ColShape> collisionShapes = new List<ColShape>();
        List<NetHandle> playersInCollision = new List<NetHandle>();

        List<ColShape> collisionSaleShapes = new List<ColShape>();
        List<NetHandle> playersInCollisionSale = new List<NetHandle>();

        public Fishing()
        {
            API.onResourceStart += API_onResourceStart;
            API.onChatMessage += API_onChatMessage;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Fishing Handler");

            string query = "SELECT ID FROM Fishing";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                foreach (DataColumn column in result.Columns)
                {
                    string selectedrow = row[column].ToString();

                    float posX = Convert.ToSingle(db.pullDatabase("Fishing", "PosX", "ID", selectedrow));
                    float posY = Convert.ToSingle(db.pullDatabase("Fishing", "PosY", "ID", selectedrow));
                    float posZ = Convert.ToSingle(db.pullDatabase("Fishing", "PosZ", "ID", selectedrow));

                    positionBlips(new Vector3(posX, posY, posZ), new Vector3());

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Fishing Points Intialized: " + initializedObjects.ToString());

            query = "SELECT ID FROM FishingSalePoints";
            result = API.exported.database.executeQueryWithResult(query);

            initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                foreach (DataColumn column in result.Columns)
                {
                    string selectedrow = row[column].ToString();

                    float posX = Convert.ToSingle(db.pullDatabase("FishingSalePoints", "PosX", "ID", selectedrow));
                    float posY = Convert.ToSingle(db.pullDatabase("FishingSalePoints", "PosY", "ID", selectedrow));
                    float posZ = Convert.ToSingle(db.pullDatabase("FishingSalePoints", "PosZ", "ID", selectedrow));

                    positionSaleBlips(new Vector3(posX, posY, posZ), new Vector3());

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Fishing Sale Points Intialized: " + initializedObjects.ToString());
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            var entityType = API.getEntityType(entity);
            if (Convert.ToInt32(entityType) == 6)
            {
                if (collisionShapes.Contains(colshape))
                {
                    if (playersInCollision.Contains(entity))
                    {
                        Client p = API.getPlayerFromHandle(entity);
                        playersInCollision.Remove(entity);
                        API.triggerClientEvent(p, "removeUseFunction");
                        return;
                    }
                }
                else if (collisionSaleShapes.Contains(colshape)) 
                {
                    if (playersInCollisionSale.Contains(entity))
                    {
                        Client p = API.getPlayerFromHandle(entity);
                        playersInCollisionSale.Remove(entity);
                        API.triggerClientEvent(p, "removeUseFunction");
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            var entityType = API.getEntityType(entity);
            if (Convert.ToInt32(entityType) == 6)
            {
                if (collisionShapes.Contains(colshape))
                {
                    if (!playersInCollision.Contains(entity))
                    {
                        playersInCollision.Add(entity);
                        Client p = API.getPlayerFromHandle(entity);
                        API.triggerClientEvent(p, "triggerUseFunction", "FishingSpot");
                        API.sendNotificationToPlayer(p, "~b~There seem to be a lot of fish in this area.");
                        return;
                    }
                }
                else if (collisionSaleShapes.Contains(colshape))
                {
                    if (!playersInCollisionSale.Contains(entity))
                    {
                        Client p = API.getPlayerFromHandle(entity);
                        playersInCollisionSale.Add(entity);
                        API.triggerClientEvent(p, "triggerUseFunction", "FishingSaleSpot");
                        API.sendNotificationToPlayer(p, "~b~The cooks here seem to appreciate fresh fish.");
                        return;
                    }
                }
            }
            else
            {
                return;
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
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if(eventName == "fishingFailed")
            {
                playerFishingFailed(player);
            }
        }

        private void API_onChatMessage(Client player, string message, CancelEventArgs e)
        {
            if (playersFishing.ContainsKey(player))
            {
                if (message.ToLower() == playersFishing[player].ToLower())
                {
                    API.sendNotificationToPlayer(player, "~b~You reel in a fish ~g~flawlessly.");
                    playersFishing.Remove(player);
                    API.triggerClientEvent(player, "stopFishing");
                    API.playSoundFrontEnd(player, "Hack_Success", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                    API.deleteEntity(playersFishingRods[player]);
                    playersFishingRods.Remove(player);
                    API.stopPlayerAnimation(player);
                    API.stopPedAnimation(player);
                    playerAddFish(player);
                    e.Cancel = true;
                    return;
                }
                playerFishingFailed(player);
                e.Cancel = true;
                return;
            }
        }

        [Command("createfishingpoint")]
        public void cmdCreateFishingPoint(Client player, string id)
        {
            if (db.isAdmin(player.name))
            {
                db.insertDatabase("Fishing", "ID", id);
                db.updateDatabase("Fishing", "PosX", player.position.X.ToString(), "ID", id);
                db.updateDatabase("Fishing", "PosY", player.position.Y.ToString(), "ID", id);
                db.updateDatabase("Fishing", "PosZ", player.position.Z.ToString(), "ID", id);

                positionBlips(new Vector3(player.position.X, player.position.Y, player.position.Z), new Vector3());
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
                positionSaleBlips(new Vector3(player.position.X, player.position.Y, player.position.Z), new Vector3());
                API.sendNotificationToPlayer(player, "~g~A fishing sale point has been created.");
            }
        }

        public void sellFish(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (playersInCollisionSale.Contains(player))
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
            if (playersInCollision.Contains(player))
            {
                if (!playersFishing.ContainsKey(player))
                {
                    if (Convert.ToInt32(db.pullDatabase("PlayerInventory", "Fish", "Nametag", player.name)) < 10)
                    {
                        string word = sendWordToPlayer();
                        API.triggerClientEvent(player, "startFishing", word);
                        playersFishing.Add(player, word);
                        try
                        {
                            var fishingrod = API.createObject(-1910604593, new Vector3(), new Vector3());
                            API.attachEntityToEntity(fishingrod, player, "SKEL_L_Hand", new Vector3(0.13f, 0.1f, 0.01f), new Vector3(180f, 90f, 70f));
                            playersFishingRods.Add(player, fishingrod);
                        }
                        finally
                        {
                            //stfu
                        }
                        
                        API.playPlayerAnimation(player, 1, "amb@world_human_stand_fishing@idle_a", "idle_c");
                        return;
                    }
                    else
                    {
                        API.sendNotificationToPlayer(player, "~r~You have too many fish.");
                        return;
                    }
                }
                else
                {
                    API.sendNotificationToPlayer(player, "~r~You seem to already be fishing.");
                    return;
                }
            }
        }

        public void positionSaleBlips(Vector3 position, Vector3 rotation)
        {
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 88);
            API.setBlipColor(newBlip, 63);
            fishingSalePoints.Add(new Vector3(position.X, position.Y, position.Z));
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 3f, 5f);
            collisionSaleShapes.Add(shape);
            int i = 0;
            ++i;
        }


        public void positionBlips(Vector3 position, Vector3 rotation)
        {
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 68);
            API.setBlipColor(newBlip, 63);
            fishingPoints.Add(new Vector3(position.X, position.Y, position.Z));
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 10f, 5f);
            collisionShapes.Add(shape);
            int i = 0;
            ++i;
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
