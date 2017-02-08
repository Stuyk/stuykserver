using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class SkinHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();
        SpawnPoints sp = new SpawnPoints();

        public SkinHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        [Command("skin")] //Purchase Clothing
        public void cmdSkin(Client player)
        {
            if (main.isPlayerLoggedIn(player))
            {
                if (!player.isInVehicle) // If player is not in Vehicle
                {
                    foreach (Vector3 clothingShop in sp.ClothingSpawnPoints)
                    {
                        if (player.position.DistanceTo(clothingShop) <= 15)
                        {
                            if (db.getPlayerMoney(player) >= 30)
                            {
                                API.triggerClientEvent(player, "openSkinPanel");
                                return;
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(player, main.msgPrefix + "Not enough money.");
                                return;
                            }
                        }
                    }
                }
            }
            return;
        }

        public void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "clientSkinSelected")
            {
                API.sendNotificationToPlayer(player, "~g~Your model has been changed to: " + arguments[0]);
                API.setPlayerSkin(player, API.pedNameToModel(arguments[0].ToString()));
                db.updateDatabase("Players", "CurrentSkin", arguments[0].ToString(), "Nametag", player.name);
                db.setPlayerMoney(player, -30);
            }
        }
    }
}
