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
                // Convert out entity into a player handle.
                Client player = API.getPlayerFromHandle(entity);

                // If our collision doesn't have an instance return.
                if (!colshape.hasData("Instance") && !colshape.hasData("Type"))
                {
                    return;
                }

                // SHOP COLLISION TYPES
                // If our Collision is of the Shop Type
                if (colshape.getData("Type") == "Shop")
                {
                    // Pull our shop instance.
                    Shop shop = (Shop)colshape.getData("Instance");
                    // Let the player know what type of shop this is.
                    API.sendNotificationToPlayer(player, shop.returnShopMessage());

                    // If the player is not in a vehicle and they're near one of the follow shops, don't let them use it.
                    if (!player.isInVehicle)
                    {
                        switch (shop.returnShopType())
                        {
                            case Shop.ShopType.Modification:
                                API.sendNotificationToPlayer(player, "~r~You must be in a vehicle to access this.");
                                return;
                            case Shop.ShopType.Repair:
                                API.sendNotificationToPlayer(player, "~r~You must be in a vehicle to access this.");
                                return;
                            case Shop.ShopType.FuelPump:
                                API.sendNotificationToPlayer(player, "~r~You must be in a vehicle to access this.");
                                return;
                        }
                    }

                    // Move on to the rest of the shop handling...
                    API.setEntityData(player, "Collision", Convert.ToString(shop.returnShopType()));
                    API.setEntityData(player, "ColShape", colshape);
                    API.triggerClientEvent(player, "triggerUseFunction", shop.returnShopType().ToString());

                    if (shop.returnShopOwner() == API.getEntityData(player, "PlayerID"))
                    {
                        API.sendNotificationToPlayer(player, "~g~This shop belongs to you.");
                    }
                }

                // HOUSE COLLISION TYPES
                // If our collision is of the House Type
                if (colshape.getData("Type") == "House")
                {
                    // If the player is in a vehicle, tell them to fuck off.
                    if (player.isInVehicle)
                    {
                        return;
                    }

                    // Get our house instance.
                    House house = (House)colshape.getData("Instance");

                    // When a player is inside an interior and they enter a collision space.
                    if (API.getEntityData(player, "IsInInterior") == true)
                    {
                        API.setEntityData(player, "Collision", house.returnHouseStatus());
                        API.triggerClientEvent(player, "triggerUseFunction", "House");
                        return;
                    }

                    API.setEntityData(player, "Collision", house.returnHouseStatus());
                    API.setEntityData(player, "ColShape", house.returnEntryCollision());
                    API.triggerClientEvent(player, "triggerUseFunction", house.returnHouseStatus());

                    if (house.returnHouseOwner() == API.getEntityData(player, "PlayerID"))
                    {
                        API.sendNotificationToPlayer(player, "~g~This house belongs to you.");
                    }

                    return;
                }
                
                // VEHICLE COLLISION TYPES
                // If our collision is of the Vehicle Type
                if (colshape.getData("Type") == "Vehicle")
                {
                    VehicleClass veh = (VehicleClass)colshape.getData("Instance");

                    API.setEntityData(player, "Collision", veh.returnCollisionType());
                    API.setEntityData(player, "NearVehicle", veh.returnVehicleID());
                    API.setEntityData(player, "ColShape", veh.returnCollision());
                    API.triggerClientEvent(player, "triggerUseFunction", veh.returnCollisionType());

                    if (veh.returnOwner() == player)
                    {
                        API.sendNotificationToPlayer(player, "~g~This vehicle belongs to you.");
                    }

                    return;
                }
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client player = API.getPlayerFromHandle(entity);

                // Check if matching ColShape
                if (colshape == (ColShape)API.getEntityData(player, "ColShape"))
                {
                    API.setEntityData(player, "Collision", "None");
                    API.setEntityData(player, "ColShape", null);
                    API.setEntityData(player, "SelectedHouse", null);
                    API.triggerClientEvent(player, "removeUseFunction");
                    if (player.isInVehicle)
                    {
                        API.triggerClientEvent(player, "triggerSilentUseFunction", "VehicleEngine");
                        API.setEntityData(player, "Collision", "VehicleEngine");
                        return;
                    }
                    API.setEntityData(player, "NearVehicle", null);
                    return;
                }

                if (API.getEntityData(player, "IsInInterior") == true)
                {
                    API.setEntityData(player, "ColShape", null);
                    API.setEntityData(player, "NearVehicle", null);
                    API.triggerClientEvent(player, "removeUseFunction");
                    return;
                }
            }
        }
    }
}
