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
    public class BarberShopHandler : Script
    {
        SkinHandler skinHandler = new SkinHandler();
        DatabaseHandler db = new DatabaseHandler();

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public BarberShopHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            Vector3 returnPosition = (Vector3)API.getEntityData(player, "ReturnPosition");
            API.setEntityDimension(player, 0);
        }

        // Pull from Database find positions.
        private void API_onResourceStart()
        {
            API.consoleOutput("Started: BarberShop Handler");
        }

        public void selectBarberShop(Client player)
        {
            if (!player.isInVehicle)
            {
                db.setPlayerHUD(player, false);
                API.setEntityData(player, "ReturnPosition", player.position);
                API.setEntityDimension(player, Convert.ToInt32(API.getEntityData(player, "PlayerID")));

                ColShape colshape = (ColShape)API.getEntityData(player, "ColShape");
                Shop shop = (Shop)API.call("ShopHandler", "getShop", colshape);
                // IF CUSTOM CAMERA
                if (shop.returnCameraCenterPoint() != new Vector3(0, 0, 0) && shop.returnCameraPoint() != new Vector3(0, 0, 0))
                {
                    API.setEntityPosition(player, shop.returnCameraCenterPoint());
                    API.triggerClientEvent(player, "createCamera", shop.returnCameraPoint(), player.position);
                    API.triggerClientEvent(player, "openSkinPanel", player.handle);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@male_b@base", "base");
                    player.rotation = new Vector3(0, 0, 88.95126);
                    return;
                }

                // ELSE DEFAULT CAMERA
                API.setEntityPosition(player, new Vector3(-1279.177, -1118.023, 6.990117));
                API.triggerClientEvent(player, "createCamera", new Vector3(-1281.826, -1118.141, 7.5), player.position);
                API.triggerClientEvent(player, "openSkinPanel", player.handle);
                API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@male_b@base", "base");
                player.rotation = new Vector3(0, 0, 88.95126);
            }
        }

        public void leaveBarberShop(Client player)
        {
            Vector3 returnPosition = (Vector3)API.getEntityData(player, "ReturnPosition");
            db.setPlayerHUD(player, true);
            API.setEntityDimension(player, 0);
            API.setEntityPosition(player, returnPosition);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
            API.setEntityData(player, "ReturnPosition", null);
        }
    }
}
