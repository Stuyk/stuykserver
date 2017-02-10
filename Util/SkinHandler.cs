using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class SkinHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();
        SpawnPoints sp = new SpawnPoints();
        ClothingShopHandler csh = new ClothingShopHandler();

        public SkinHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
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
