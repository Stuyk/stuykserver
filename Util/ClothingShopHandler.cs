using GTANetworkServer;
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
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();
        List<Vector3> clothingShops = new List<Vector3>();

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
                    float rotX = Convert.ToSingle(db.pullDatabase("ClothingShops", "RotX", "ID", selectedrow));
                    float rotY = Convert.ToSingle(db.pullDatabase("ClothingShops", "RotY", "ID", selectedrow));
                    float rotZ = Convert.ToSingle(db.pullDatabase("ClothingShops", "RotZ", "ID", selectedrow));

                    positionBlips(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ));

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Clothing Shops Initialized: " + initializedObjects.ToString());
        }

        [Command("skin")] //Purchase Clothing
        public void cmdSkin(Client player)
        {
            if (main.isPlayerLoggedIn(player))
            {
                if (!player.isInVehicle) // If player is not in Vehicle
                {
                    foreach (Vector3 pos in clothingShops)
                    {
                        if (player.position.DistanceTo(pos) <= 15)
                        {
                            if (db.getPlayerMoney(player) >= 30)
                            {
                                API.triggerClientEvent(player, "openSkinPanel");
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
                    string rotX = player.rotation.X.ToString();
                    string rotY = player.rotation.Y.ToString();
                    string rotZ = player.rotation.Z.ToString();

                    db.insertDatabase("ClothingShops", "ID", id);
                    db.updateDatabase("ClothingShops", "PosX", posX, "ID", id);
                    db.updateDatabase("ClothingShops", "PosY", posY, "ID", id);
                    db.updateDatabase("ClothingShops", "PosZ", posZ, "ID", id);
                    db.updateDatabase("ClothingShops", "RotX", rotX, "ID", id);
                    db.updateDatabase("ClothingShops", "RotY", rotY, "ID", id);
                    db.updateDatabase("ClothingShops", "RotZ", rotZ, "ID", id);

                    positionBlips(new Vector3(player.position.X, player.position.Y, player.position.Z), new Vector3(player.rotation.X, player.rotation.Y, player.rotation.Z));
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

        public void positionBlips(Vector3 position, Vector3 rotation)
        {
            API.createTextLabel("~y~Usage: ~w~/skin", new Vector3(position.X, position.Y, position.Z), 20, 0.5f);
            API.createTextLabel("~w~Change your character model.", new Vector3(position.X, position.Y, position.Z - 0.4), 20, 0.5f);
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 73);
            API.setBlipColor(newBlip, 18);
            API.createMarker(1, new Vector3(position.X, position.Y, position.Z - 4), new Vector3(), new Vector3(), new Vector3(2, 2, 5), 255, 154, 211, 224, 0);
            clothingShops.Add(new Vector3(position.X, position.Y, position.Z));
            int i = 0;
            ++i;
        }
    }
}
