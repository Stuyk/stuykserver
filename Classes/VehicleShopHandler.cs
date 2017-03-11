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

        Dictionary<ColShape, Shop> shopInformation = new Dictionary<ColShape, Shop>();

        public VehicleShopHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "dealershipReady")
            {
                // Fuckin Nothihng
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

        // When a player tries to enter a Dealership.
        public void browseDealership(Client player, string type)
        {
            if (!player.isInVehicle)
            {
                db.setPlayerHUD(player, false);
                API.setEntityData(player, "ReturnPosition", player.position);
                API.setEntityDimension(player, Convert.ToInt32(API.getEntityData(player, "PlayerID")));

                ColShape colshape = (ColShape)API.getEntityData(player, "ColShape");
                Shop shop = (Shop)API.call("ShopHandler", "getShop", colshape);
                // Temporary Holding Collision
                API.setEntityData(player, "ExitPoint", shop.returnExitPoint());
                // Custom Camera
                if (shop.returnCameraCenterPoint() != new Vector3(0, 0, 0) && shop.returnCameraPoint() != new Vector3(0, 0, 0))
                {
                    API.setEntityPosition(player, shop.returnCameraCenterPoint());
                    API.triggerClientEvent(player, "startBrowsing", Convert.ToString(API.getEntityData(player, "Collision")), Convert.ToInt32(API.getEntityData(player, "PlayerID")), shop.returnCameraCenterPoint());
                    API.triggerClientEvent(player, "createCamera", shop.returnCameraPoint(), shop.returnCameraCenterPoint());
                    return;
                }

                // Default Camera
                API.setEntityPosition(player, new Vector3(198.9816, -1000.94, -98));
                API.triggerClientEvent(player, "startBrowsing", Convert.ToString(API.getEntityData(player, "Collision")), Convert.ToInt32(API.getEntityData(player, "PlayerID")), new Vector3(198.9816, -1000.94, -98));
                API.triggerClientEvent(player, "createCamera", new Vector3(206.339, -1000.967, -98.5), new Vector3(198.9816, -1000.94, -98));
                return;
            }     
        }

        // When a player purchased a vehicle from a Dealership.
        public void purchaseDealershipVehicle(Client player, string vehicleType)
        {
            Vector3 exitPoint = (Vector3)API.getEntityData(player, "ExitPoint");
            API.call("VehicleHandler", "actionSetupPurchasedCar", exitPoint, vehicleType, player);
            leaveDealership(player);
        }

        // When a player leaves a dealership without a purchase.
        public void leaveDealership(Client player)
        {
            Vector3 returnPosition = (Vector3)API.getEntityData(player, "ReturnPosition");
            db.setPlayerHUD(player, true);
            API.setEntityPosition(player, returnPosition);
            API.setEntityDimension(player, 0);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
            API.setEntityData(player, "ReturnPosition", null);
            API.setEntityData(player, "ExitPoint", null);
            API.triggerClientEvent(player, "killPanel");
        }
    }
}
