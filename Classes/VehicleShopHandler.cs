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
    public class VehicleShopHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();
        Util util = new Util();

        Dictionary<ColShape, ShopInformationHandling> shopInformation = new Dictionary<ColShape, ShopInformationHandling>();

        public VehicleShopHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnInsidePlayers().ContainsKey(player))
                {
                    db.setPlayerPositionByVector(player, shopInformation[collision].returnCollisionPosition());
                    shopInformation[collision].removeInsidePlayer(player);
                }

                if (shopInformation[collision].returnOutsidePlayers().Contains(player))
                {
                    shopInformation[collision].removeOutsidePlayer(player);
                }
            }
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "dealershipReady")
            {
                foreach (ColShape collision in shopInformation.Keys)
                {
                    if (shopInformation[collision].returnInsidePlayers().ContainsKey(player))
                    {
                        API.triggerClientEvent(player, "createCamera", shopInformation[collision].returnCameraCenterPoint(), shopInformation[collision].returnCameraPoint());
                        API.setEntityDimension(player, 0);
                    }
                }
            }

            if (eventName == "leaveDealership")
            {
                leaveDealership(player);
                API.triggerClientEvent(player, "endCamera");
            }

            if (eventName == "purchaseVehicle")
            {
                purchaseDealershipVehicle(player, arguments[0].ToString());
                API.triggerClientEvent(player, "endCamera");
            }
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
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "removeUseFunction");
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
                    if (!shopInformation[colshape].returnOutsidePlayers().Contains(player) && !player.isInVehicle)
                    {
                        shopInformation[colshape].addOutsidePlayer(player);
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "Dealership");
                        string type = shopInformation[colshape].returnShopType().ToString();
                        API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), string.Format("This shop carries the type: {0}", type));
                    }
                }
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Vehicle Shop Handler");

            string query = "SELECT * FROM VehicleShops";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            // Setup Shops
            foreach (DataRow row in result.Rows)
            {
                float posX = Convert.ToSingle(row["PosX"]);
                float posY = Convert.ToSingle(row["PosY"]);
                float posZ = Convert.ToSingle(row["PosZ"]);
                float rotX = Convert.ToSingle(row["RotX"]);
                float rotY = Convert.ToSingle(row["RotY"]);
                float rotZ = Convert.ToSingle(row["RotZ"]);
                int id = Convert.ToInt32(row["ID"]);
                ShopInformationHandling.ShopType type = (ShopInformationHandling.ShopType) Enum.Parse(typeof(ShopInformationHandling.ShopType), row["Type"].ToString());
                API.consoleOutput(type.ToString());
                Vector3 centerPoint = db.convertStringToVector3(row["CenterPoint"].ToString());
                Vector3 cameraPoint = db.convertStringToVector3(row["CameraPoint"].ToString());
                Vector3 exitPoint = db.convertStringToVector3(row["ExitPoint"].ToString());

                positionBlips(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), id, type, centerPoint, cameraPoint, exitPoint);
                ++initializedObjects;
            }

            API.consoleOutput("Vehicle Shops Initialized: " + initializedObjects.ToString());
        }

        // When a player tries to enter a Dealership.
        public void browseDealership(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (API.isPlayerInAnyVehicle(player))
                {
                    break;
                }

                if (shopInformation[collision].returnOutsidePlayers().Contains(player))
                {
                    int rand = new Random().Next(1, 1000);
                    shopInformation[collision].addInsidePlayer(player, player.handle);
                    API.setEntityPosition(player, shopInformation[collision].returnCameraCenterPoint());
                    API.setEntityDimension(player, rand);
                    db.setPlayerHUD(player, false);
                    API.triggerClientEvent(player, "startBrowsing", shopInformation[collision].returnShopType().ToString(), rand);
                    break;
                }
            }
        }

        // When a player purchased a vehicle from a Dealership.
        public void purchaseDealershipVehicle(Client player, string vehicleType)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnInsidePlayers().ContainsKey(player))
                {
                    db.setPlayerHUD(player, true);
                    API.setEntityDimension(player, 0);
                    API.setEntityPosition(player, shopInformation[collision].returnCollisionPosition());
                    shopInformation[collision].removeInsidePlayer(player);
                    API.call("VehicleHandler", "actionSetupPurchasedCar", shopInformation[collision].returnExitPoint(), vehicleType, player);
                    break;
                }
            }
        }

        // When a player leaves a dealership without a purchase.
        public void leaveDealership(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnInsidePlayers().ContainsKey(player))
                {
                    db.setPlayerHUD(player, true);
                    API.setEntityDimension(player, 0);
                    API.setEntityPosition(player, shopInformation[collision].returnCollisionPosition());
                    shopInformation[collision].removeInsidePlayer(player);
                    API.stopPlayerAnimation(player);
                    API.stopPedAnimation(player);
                    break;
                }
            }
        }

        public void positionBlips(Vector3 position, Vector3 rotation, int id, ShopInformationHandling.ShopType type, Vector3 centerPoint, Vector3 cameraPoint, Vector3 exitPoint)
        {
            ShopInformationHandling newShop = new ShopInformationHandling();
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 3f, 5f);

            newShop.setCollisionID(id);
            newShop.setCollisionPosition(position);
            newShop.setCollisionShape(shape);
            newShop.setShopType(type);
            newShop.setCameraCenterPoint(centerPoint);
            newShop.setCameraPoint(cameraPoint);
            newShop.setExitPoint(exitPoint);
            newShop.setupBlip();

            shopInformation.Add(shape, newShop);
        }
    }
}
