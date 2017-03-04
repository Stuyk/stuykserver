using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class ClothingHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();

        public ClothingHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        public void updateLocalClothingVariables(Client player)
        {
            string query = string.Format("SELECT * FROM PlayerClothing WHERE Nametag='{0}'", player.name);
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                int torso = Convert.ToInt32(row["clothingTorso"]);
                int top = Convert.ToInt32(row["clothingTop"]);
                int topcolor = Convert.ToInt32(row["clothingTopColor"]);
                int undershirt = Convert.ToInt32(row["clothingUndershirt"]);
                int undershirtcolor = Convert.ToInt32(row["clothingUndershirtColor"]);
                int legs = Convert.ToInt32(row["clothingLegs"]);
                int legscolor = Convert.ToInt32(row["clothingLegsColor"]);
                int hat = Convert.ToInt32(row["clothingHat"]);
                int hatcolor = Convert.ToInt32(row["clothingHatColor"]);
                int shoes = Convert.ToInt32(row["clothingShoes"]);
                int shoescolor = Convert.ToInt32(row["clothingShoesColor"]);

                API.triggerClientEvent(player, "clothingLocalVariableUpdate", torso, top, topcolor, undershirt, undershirtcolor, legs, legscolor, shoes, shoescolor);
            }
        }

        public void updateClothingForPlayer(Client player)
        {
            string query = string.Format("SELECT * FROM PlayerClothing WHERE Nametag='{0}'", player.name);
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                int torso = Convert.ToInt32(row["clothingTorso"]);
                int top = Convert.ToInt32(row["clothingTop"]);
                int topcolor = Convert.ToInt32(row["clothingTopColor"]);
                int undershirt = Convert.ToInt32(row["clothingUndershirt"]);
                int undershirtcolor = Convert.ToInt32(row["clothingUndershirtColor"]);
                int legs = Convert.ToInt32(row["clothingLegs"]);
                int legscolor = Convert.ToInt32(row["clothingLegsColor"]);
                int hat = Convert.ToInt32(row["clothingHat"]);
                int hatcolor = Convert.ToInt32(row["clothingHatColor"]);
                int shoes = Convert.ToInt32(row["clothingShoes"]);
                int shoescolor = Convert.ToInt32(row["clothingShoesColor"]);

                API.setPlayerClothes(player, 3, torso, 0);
                API.setPlayerClothes(player, 11, top, topcolor);
                API.setPlayerClothes(player, 4, legs, legscolor);
                API.setPlayerClothes(player, 8, undershirt, undershirtcolor);
                API.setPlayerClothes(player, 6, shoes, shoescolor);
            }
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "clothingSave")
            {
                db.updateDatabase("PlayerClothing", "clothingTop", args[0].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingTopColor", args[1].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingUndershirt", args[2].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingUndershirtColor", args[3].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingTorso", args[4].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingLegs", args[5].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingLegsColor", args[6].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingShoes", args[7].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingShoesColor", args[8].ToString(), "Nametag", player.name);
                updateClothingForPlayer(player);
                API.stopPedAnimation(player);
                API.stopPlayerAnimation(player);

                API.call("ClothingShopHandler", "leaveClothingShop", player);
                API.triggerClientEvent(player, "killPanel");
                API.triggerClientEvent(player, "endCamera");
            }

            if (eventName == "exitClothingShop")
            {
                API.call("ClothingShopHandler", "leaveClothingShop", player);
                API.triggerClientEvent(player, "killPanel");
                API.triggerClientEvent(player, "endCamera");
                updateClothingForPlayer(player);
            }
        }
    }
}
