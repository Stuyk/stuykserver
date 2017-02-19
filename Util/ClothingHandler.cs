using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
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
            int torso = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTorso", "Nametag", player.name));
            int top = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTop", "Nametag", player.name));
            int topcolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTopColor", "Nametag", player.name));
            int undershirt = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingUndershirt", "Nametag", player.name));
            int undershirtcolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingUndershirtColor", "Nametag", player.name));
            int legs = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingLegs", "Nametag", player.name));
            int legscolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingLegsColor", "Nametag", player.name));
            int hat = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingHat", "Nametag", player.name));
            int hatcolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingHatColor", "Nametag", player.name));
            int shoes = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingShoes", "Nametag", player.name));
            int shoescolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingShoesColor", "Nametag", player.name));

            API.triggerClientEvent(player, "clothingLocalVariableUpdate", torso, top, topcolor, undershirt, undershirtcolor, legs, legscolor, shoes, shoescolor);
        }

        public void updateClothingForPlayer(Client player)
        {
            int torso = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTorso", "Nametag", player.name));
            int top = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTop", "Nametag", player.name));
            int topcolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTopColor", "Nametag", player.name));
            int undershirt = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingUndershirt", "Nametag", player.name));
            int undershirtcolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingUndershirtColor", "Nametag", player.name));
            int legs = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingLegs", "Nametag", player.name));
            int legscolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingLegsColor", "Nametag", player.name));
            int hat = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingHat", "Nametag", player.name));
            int hatcolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingHatColor", "Nametag", player.name));
            int shoes = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingShoes", "Nametag", player.name));
            int shoescolor = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingShoesColor", "Nametag", player.name));

            API.setPlayerClothes(player, 3, torso, 0);
            API.setPlayerClothes(player, 11, top, topcolor);
            API.setPlayerClothes(player, 4, legs, legscolor);
            API.setPlayerClothes(player, 8, undershirt, undershirtcolor);
            API.setPlayerClothes(player, 6, shoes, shoescolor);
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "clothingSave")
            {
                API.stopPlayerAnimation(player);
                API.triggerClientEvent(player, "killPanel");
                API.triggerClientEvent(player, "endCamera");

                db.updateDatabase("PlayerClothing", "clothingTop", args[0].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingTopColor", args[1].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingUndershirt", args[2].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingUndershirtColor", args[3].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingTorso", args[4].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingLegs", args[5].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingLegsColor", args[6].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingShoes", args[7].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerClothing", "clothingShoesColor", args[8].ToString(), "Nametag", player.name);

                API.call("ClothingShopHandler", "leaveClothingShop", player);
                updateClothingForPlayer(player);
            }
        }
    }
}
