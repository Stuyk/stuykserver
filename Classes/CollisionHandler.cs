using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class CollisionHandler : Script
    {
        public CollisionHandler()
        {
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client player = API.getPlayerFromHandle(entity);

                if (!player.isInVehicle)
                {
                    // All Class Calls
                    Shop shop = (Shop)API.call("ShopHandler", "getShop", colshape);
                    House house = (House)API.call("HouseHandler", "getHouse", colshape);
                    VehicleClass veh = (VehicleClass)API.call("VehicleHandler", "getVehicle", player, colshape);

                    if (shop != null)
                    {
                        if (shop.returnShopType().ToString() == "Modification" && !player.isInVehicle)
                        {
                            return;
                        }

                        API.setEntityData(player, "Collision", shop.returnShopType().ToString());
                        API.setEntityData(player, "ColShape", colshape);
                        API.triggerClientEvent(player, "triggerUseFunction", shop.returnShopType().ToString());
                        return;
                    }

                    if (house != null)
                    {
                        API.setEntityData(player, "Collision", house.returnHouseStatus().ToString());
                        API.triggerClientEvent(player, "triggerUseFunction", house.returnHouseStatus().ToString());
                    }

                    if (veh != null)
                    {
                        API.setEntityData(player, "Collision", veh.returnCollisionType());
                        API.setEntityData(player, "NearVehicle", veh.returnVehicleID());
                        API.setEntityData(player, "ColShape", veh.returnCollision());
                        API.triggerClientEvent(player, "triggerUseFunction", veh.returnCollisionType());
                    }
                }
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (player.isInVehicle)
                {
                    return;
                }

                // Check if matching ColShape
                if (colshape == (ColShape)API.getEntityData(player, "ColShape"))
                {
                    API.setEntityData(player, "Collision", "None");
                    API.setEntityData(player, "ColShape", null);
                    API.setEntityData(player, "NearVehicle", null);
                    API.triggerClientEvent(player, "removeUseFunction");
                }
            }
        }
    }
}
