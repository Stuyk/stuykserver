using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class BankHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();

        Dictionary<ColShape, ShopInformationHandling> shopInformation = new Dictionary<ColShape, ShopInformationHandling>();

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public BankHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (shopInformation.ContainsKey(colshape))
                {
                    if (shopInformation[colshape].returnOutsidePlayers().Contains(player))
                    {
                        shopInformation[colshape].removeOutsidePlayer(player);
                        API.triggerClientEvent(player, "removeUseFunction");
                    }
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (shopInformation.ContainsKey(colshape))
                {
                    if (!shopInformation[colshape].returnOutsidePlayers().Contains(player))
                    {
                        shopInformation[colshape].addOutsidePlayer(player);
                        API.triggerClientEvent(player, "triggerUseFunction", "Bank");
                        API.sendNotificationToPlayer(player, "This will let me deposit my cash.");
                    }
                }
            }
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "stopAnimation")
            {
                player.stopAnimation();
            }

            if (eventName == "withdrawATM_Server")
            {
                int input = Convert.ToInt32(arguments[0]);
                if (input > 0)
                {
                    int atmMoney = db.getPlayerAtmMoney(player);
                    if (atmMoney > 0 && input <= atmMoney)
                    {
                        db.setPlayerMoney(player, +input);
                        db.setPlayerAtmMoney(player, -input);
                        updateATMDisplay(player);
                        API.triggerClientEvent(player, "displayWithdrawSuccess");
                    }
                    else
                    {
                        API.triggerClientEvent(player, "displayNotThatMuch");
                    }
                }
            }

            if (eventName == "balanceNotDisplayed")
            {
                updateATMDisplay(player);
            }

            if (eventName == "depositATM_Server")
            {
                int input = Convert.ToInt32(arguments[0]);
                if (input > 0)
                {
                    int playerMoney = db.getPlayerMoney(player);
                    if (playerMoney > 0 && input <= playerMoney)
                    {
                        db.setPlayerMoney(player, -input);
                        db.setPlayerAtmMoney(player, +input);
                        updateATMDisplay(player);
                        API.triggerClientEvent(player, "depositAlertSuccess");
                    }
                    else
                    {
                        API.triggerClientEvent(player, "displayNotThatMuch");
                    }
                }
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Bank Handler");

            string query = "SELECT * FROM Banks";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                float posX = Convert.ToSingle(row["PosX"]);
                float posY = Convert.ToSingle(row["PosY"]);
                float posZ = Convert.ToSingle(row["PosZ"]);
                float rotX = Convert.ToSingle(row["RotX"]);
                float rotY = Convert.ToSingle(row["RotY"]);
                float rotZ = Convert.ToSingle(row["RotZ"]);
                int id = Convert.ToInt32(row["ID"]);

                positionBlips(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), id);
                ++initializedObjects;
            }

            API.consoleOutput("Banks Initialized: " + initializedObjects.ToString());
        }

        public void updateATMDisplay(Client player)
        {
            int atmMoney = db.getPlayerAtmMoney(player);
            int playerMoney = db.getPlayerMoney(player);

            API.triggerClientEvent(player, "refreshATM", atmMoney, playerMoney);
        }

        public void selectATM(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnOutsidePlayers().Contains(player) && !player.isInVehicle)
                {
                    API.triggerClientEvent(player, "loadATM", db.getPlayerAtmMoney(player));
                    break;
                }
            }
        }

        // Positions the objects, blips, and text when initialized or created.
        public void positionBlips(Vector3 position, Vector3 rotation, int id)
        {
            ShopInformationHandling newShop = new ShopInformationHandling();
            ColShape collision = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 2f, 2f);
            List<GTANetworkServer.Object> shopObjects = new List<GTANetworkServer.Object>();

            GTANetworkServer.Object atmObject = API.createObject(-870868698, new Vector3(position.X, position.Y, position.Z - 1), new Vector3(rotation.X, rotation.Y, rotation.Z - 180));

            shopObjects.Add(atmObject);

            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 108);
            API.setBlipColor(newBlip, 2);
            API.setBlipShortRange(newBlip, true);

            newShop.setBlip(newBlip);
            newShop.setCollisionID(id);
            newShop.setCollisionPosition(position);
            newShop.setCollisionShape(collision);
            newShop.setShopObjects(shopObjects);
            newShop.setShopType(ShopInformationHandling.ShopType.Atm);

            shopInformation.Add(collision, newShop);
        }
    }
}
