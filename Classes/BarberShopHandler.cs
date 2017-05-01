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

        public void selectBarberShop(Client player, bool free = false)
        {
            if (!player.isInVehicle)
            {
                API.setEntityData(player, "ReturnPosition", player.position);
                API.triggerClientEvent(player, "setupBarberShop");
                API.delay(5000, true, () =>
                {
                    API.setEntitySyncedData(player, "StopDraws", true);
                    API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
                    API.setEntityDimension(player, Convert.ToInt32(API.getEntityData(player, "PlayerID")));
                    API.setEntityPosition(player, new Vector3(-35.1, -153.3, 57));
                    API.setEntityRotation(player, new Vector3(0, 0, 70.6908));
                    API.playPlayerAnimation(player, (int)(AnimationFlags.StopOnLastFrame), "misshair_shop@barbers", "player_base");
                    API.freezePlayer(player, true);
                });
            }
        }

        public void leaveBarberShop(Client player)
        {
            API.freezePlayer(player, false);
            API.setEntitySyncedData(player, "StopDraws", false);
            Vector3 returnPosition = (Vector3)API.getEntityData(player, "ReturnPosition");
            API.setEntityDimension(player, 0);
            API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
            API.setEntityPosition(player, returnPosition);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
            API.setEntityData(player, "ReturnPosition", null);
        }
    }
}
