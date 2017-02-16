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

            API.triggerClientEvent(player, "clothingLocalVariableUpdate", torso, top, topcolor, undershirt, undershirtcolor, legs, legscolor, hat, hatcolor, shoes, shoescolor);
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
            API.setPlayerAccessory(player, 0, hat, hatcolor);
            API.setPlayerClothes(player, 6, shoes, shoescolor);
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            // CLOTHING TOP
            if (eventName == "clothingTopChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTop", "Nametag", player.name));
                int color = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTopColor", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerClothes(player, 11, amount, color);
                db.updateDatabase("PlayerClothing", "clothingTop", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING TOP COLOR
            if (eventName == "clothingTopColorChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTopColor", "Nametag", player.name));
                int top = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTop", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerClothes(player, 11, top, amount);
                db.updateDatabase("PlayerClothing", "clothingTopColor", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING UNDERSHIRT
            if (eventName == "clothingUndershirtChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingUndershirt", "Nametag", player.name));
                int color = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingUndershirtColor", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerClothes(player, 8, amount, color);
                db.updateDatabase("PlayerClothing", "clothingUndershirt", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING UNDERSHIRT COLOR
            if (eventName == "clothingUndershirtColorChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingUndershirtColor", "Nametag", player.name));
                int undershirt = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingUndershirt", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerClothes(player, 8, undershirt, amount);
                db.updateDatabase("PlayerClothing", "clothingUndershirtColor", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING LEGS
            if (eventName == "clothingLegsChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingLegs", "Nametag", player.name));
                int color = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingLegsColor", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerClothes(player, 4, amount, color);
                db.updateDatabase("PlayerClothing", "clothingLegs", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING LEGS COLOR
            if (eventName == "clothingLegsColorChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingLegsColor", "Nametag", player.name));
                int legs = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingLegs", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerClothes(player, 4, legs, amount);
                db.updateDatabase("PlayerClothing", "clothingLegsColor", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING HAT
            if (eventName == "clothingHatChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingHat", "Nametag", player.name));
                int color = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingHatColor", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerAccessory(player, 0, amount, color);
                db.updateDatabase("PlayerClothing", "clothingHat", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING HAT COLOR
            if (eventName == "clothingHatColorChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingHatColor", "Nametag", player.name));
                int hat = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingHat", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerAccessory(player, 0, hat, amount);
                db.updateDatabase("PlayerClothing", "clothingHatColor", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING SHOES
            if (eventName == "clothingShoesChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingShoes", "Nametag", player.name));
                int color = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingShoesColor", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerClothes(player, 6, amount, color);
                db.updateDatabase("PlayerClothing", "clothingShoes", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING SHOES COLOR
            if (eventName == "clothingShoesColorChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingShoesColor", "Nametag", player.name));
                int shoes = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingShoes", "Nametag", player.name));

                if (amount == -1)
                {
                    amount = 0;
                }

                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerClothes(player, 6, shoes, amount);
                db.updateDatabase("PlayerClothing", "clothingShoesColor", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            // CLOTHING TORSO
            if (eventName == "clothingTorsoChange")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerClothing", "clothingTorso", "Nametag", player.name));

                if (amount < 0)
                {
                    amount = 0;
                }

                // Base Amount = Base Amount + or - 1
                amount = amount + Convert.ToInt32(arguments[0]);
                API.setPlayerClothes(player, 0, 0, 0);
                API.setPlayerClothes(player, 3, amount, 0);
                db.updateDatabase("PlayerClothing", "clothingTorso", amount.ToString(), "Nametag", player.name);
                updateLocalClothingVariables(player);
            }

            if (eventName == "clothingSave")
            {
                API.triggerClientEvent(player, "killPanel");
                API.triggerClientEvent(player, "endCamera");
                API.call("ClothingShopHandler", "leaveClothingShop", player);
            }
        }
    }
}
