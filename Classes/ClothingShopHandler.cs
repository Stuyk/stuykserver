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
    public class ClothingShopHandler : Script
    {
        ClothingHandler clothingHandler = new ClothingHandler();
        DatabaseHandler db = new DatabaseHandler();

        Dictionary<ColShape, Shop> shopInformation = new Dictionary<ColShape, Shop>();

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public ClothingShopHandler()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Clothing Shop Handler");
        }

        public void selectClothing(Client player)
        {
            if (!player.isInVehicle)
            {
                db.setPlayerHUD(player, false);
                API.setEntityData(player, "ReturnPosition", player.position);
                API.setEntityDimension(player, Convert.ToInt32(API.getEntityData(player, "PlayerID")));

                ColShape colshape = (ColShape)API.getEntityData(player, "ColShape");
                Shop shop = (Shop)API.call("ShopHandler", "getShop", colshape);
                if (shop.returnCameraCenterPoint() != new Vector3(0, 0, 0) && shop.returnCameraPoint() != new Vector3(0, 0, 0))
                {
                    API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
                    API.setEntityPosition(player, shop.returnCameraCenterPoint());
                    //API.triggerClientEvent(player, "createCamera", shop.returnCameraPoint(), player.position);
                    //API.triggerClientEvent(player, "openClothingPanel");
                    API.triggerClientEvent(player, "setupClothingMode", shop.returnCameraPoint());
                    //clothingHandler.updateLocalClothingVariables(player);
                    //API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@male_b@base", "base");
                    return;
                }
                API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
                API.setEntityPosition(player, new Vector3(-1187.994, -764.7119, 17.31953));
                //API.triggerClientEvent(player, "createCamera", new Vector3(-1190.004, -766.2875, 17.3196), player.position);
                //API.triggerClientEvent(player, "openClothingPanel", );
                API.triggerClientEvent(player, "setupClothingMode", new Vector3(-1190.004, -766.2875, 17.3196));
                //clothingHandler.updateLocalClothingVariables(player);
                //API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@male_b@base", "base");
                return;
            }     
        }

        public void leaveClothingShop(Client player)
        {
            Vector3 returnPosition = (Vector3)API.getEntityData(player, "ReturnPosition");
            db.setPlayerHUD(player, true);
            API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
            API.setEntityPosition(player, returnPosition);
            API.setEntityDimension(player, 0);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
            API.setEntityData(player, "ReturnPosition", null);
        }
    }
}
