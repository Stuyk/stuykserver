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
                Shop shop = (Shop)API.call("ShopHandler", "getShop", colshape);
                House house = (House)API.call("HouseHandler", "getHouse", colshape);
                if (shop != null)
                {
                    API.setEntityData(player, "Collision", shop.returnShopType().ToString());
                    API.setEntityData(player, "ColShape", colshape);
                    API.triggerClientEvent(player, "triggerUseFunction", shop.returnShopType().ToString());
                }

                if (house != null)
                {
                    API.setEntityData(player, "Collision", house.returnHouseStatus().ToString());
                    API.triggerClientEvent(player, "triggerUseFunction", house.returnHouseStatus().ToString());
                }
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client player = API.getPlayerFromHandle(entity);
                API.setEntityData(player, "Collision", "None");
                API.setEntityData(player, "ColShape", null);
                API.triggerClientEvent(player, "removeUseFunction");
            }
        }
    }
}
