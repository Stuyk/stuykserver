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
                if (shop != null)
                {
                    API.setEntityData(player, "Collision", shop.returnShopType());
                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "a");
                }
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client player = API.getPlayerFromHandle(entity);
                API.setEntityData(player, "Collision", null);
                API.triggerClientEvent(API.getPlayerFromHandle(entity), "removeUseFunction");
            }
        }
    }
}
