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
            API.triggerClientEvent(player, "clothingLocalVariableUpdate", player.handle);
        }

        public void updateClothingForPlayer(Client player)
        {
            string[] varNames = { "PlayerID" };
            string before = "SELECT * FROM PlayerClothing WHERE";
            object[] data = { Convert.ToString(API.getEntityData(player, "PlayerID")) };
            DataTable result = db.compileSelectQuery(before, varNames, data);

            // Assign all clothing to Variables.
            int torso = Convert.ToInt32(result.Rows[0]["clothingTorso"]);
            int top = Convert.ToInt32(result.Rows[0]["clothingTop"]);
            int topcolor = Convert.ToInt32(result.Rows[0]["clothingTopColor"]);
            int undershirt = Convert.ToInt32(result.Rows[0]["clothingUndershirt"]);
            int undershirtcolor = Convert.ToInt32(result.Rows[0]["clothingUndershirtColor"]);
            int legs = Convert.ToInt32(result.Rows[0]["clothingLegs"]);
            int legscolor = Convert.ToInt32(result.Rows[0]["clothingLegsColor"]);
            int hat = Convert.ToInt32(result.Rows[0]["clothingHat"]);
            int hatcolor = Convert.ToInt32(result.Rows[0]["clothingHatColor"]);
            int shoes = Convert.ToInt32(result.Rows[0]["clothingShoes"]);
            int shoescolor = Convert.ToInt32(result.Rows[0]["clothingShoesColor"]);
            int accessory = Convert.ToInt32(result.Rows[0]["clothingAccessory"]);
            int accessorycolor = Convert.ToInt32(result.Rows[0]["clothingAccessoryColor"]);
            int glasses = Convert.ToInt32(result.Rows[0]["clothingGlasses"]);
            int glassescolor = Convert.ToInt32(result.Rows[0]["clothingGlassesColor"]);
            int bag = Convert.ToInt32(result.Rows[0]["clothingBag"]);
            int bagcolor = Convert.ToInt32(result.Rows[0]["clothingBagColor"]);
            int mask = Convert.ToInt32(result.Rows[0]["clothingMask"]);
            int maskcolor = Convert.ToInt32(result.Rows[0]["clothingMaskColor"]);
            int torsocolor = Convert.ToInt32(result.Rows[0]["clothingTorsoColor"]);
            int bodyarmor = Convert.ToInt32(result.Rows[0]["clothingBodyArmor"]);
            int bodyarmorcolor = Convert.ToInt32(result.Rows[0]["clothingBodyArmorColor"]);
            int decal = Convert.ToInt32(result.Rows[0]["clothingDecal"]);
            int decalcolor = Convert.ToInt32(result.Rows[0]["clothingDecalColor"]);

            // Set Clothes
            API.setPlayerClothes(player, 1, mask, maskcolor);
            API.setPlayerClothes(player, 3, torso, torsocolor);
            API.setPlayerClothes(player, 11, top, topcolor);
            API.setPlayerClothes(player, 4, legs, legscolor);
            API.setPlayerClothes(player, 5, bag, bagcolor);
            API.setPlayerClothes(player, 8, undershirt, undershirtcolor);
            API.setPlayerClothes(player, 6, shoes, shoescolor);
            API.setPlayerClothes(player, 7, accessory, accessorycolor);
            API.setPlayerClothes(player, 9, bodyarmor, bodyarmorcolor);
            API.setPlayerClothes(player, 10, decal, decalcolor);
            API.setPlayerAccessory(player, 1, glasses, glassescolor);
            API.setPlayerAccessory(player, 0, hat, hatcolor);

            // Set Sync Data
            API.setEntitySyncedData(player.handle, "clothingTorso", torso);
            API.setEntitySyncedData(player.handle, "clothingTorsoColor", torsocolor);
            API.setEntitySyncedData(player.handle, "clothingTop", top);
            API.setEntitySyncedData(player.handle, "clothingTopColor", topcolor);
            API.setEntitySyncedData(player.handle, "clothingUndershirt", undershirt);
            API.setEntitySyncedData(player.handle, "clothingUndershirtColor", undershirtcolor);
            API.setEntitySyncedData(player.handle, "clothingLegs", legs);
            API.setEntitySyncedData(player.handle, "clothingLegsColor", legscolor);
            API.setEntitySyncedData(player.handle, "clothingHat", hat);
            API.setEntitySyncedData(player.handle, "clothingHatColor", hatcolor);
            API.setEntitySyncedData(player.handle, "clothingShoes", shoes);
            API.setEntitySyncedData(player.handle, "clothingShoesColor", shoescolor);
            API.setEntitySyncedData(player.handle, "clothingAccessory", accessory);
            API.setEntitySyncedData(player.handle, "clothingAccessoryColor", accessorycolor);
            API.setEntitySyncedData(player.handle, "clothingGlasses", glasses);
            API.setEntitySyncedData(player.handle, "clothingGlassesColor", glassescolor);
            API.setEntitySyncedData(player.handle, "clothingBag", bag);
            API.setEntitySyncedData(player.handle, "clothingBagColor", bagcolor);
            API.setEntitySyncedData(player.handle, "clothingBodyArmor", bodyarmor);
            API.setEntitySyncedData(player.handle, "clothingBodyArmorColor", bodyarmorcolor);
            API.setEntitySyncedData(player.handle, "clothingDecal", decal);
            API.setEntitySyncedData(player.handle, "clothingDecalColor", decalcolor);
        }

        public void actionSaveClothing(Client player, params object[] args)
        {
            string[] varNames = { "clothingTop", "clothingTopColor", "clothingUndershirt", "clothingUndershirtColor", "clothingTorso", "clothingLegs", "clothingLegsColor", "clothingShoes", "clothingShoesColor", "clothingAccessory", "clothingGlasses", "clothingGlassesColor", "clothingBag", "clothingBagColor", "clothingMask", "clothingMaskColor", "clothingTorsoColor", "clothingBodyArmor", "clothingBodyArmorColor", "clothingAccessoryColor", "clothingDecal", "clothingDecalColor", "clothingHat", "clothingHatColor" };
            string before = "UPDATE PlayerClothing SET";
            string after = string.Format("WHERE PlayerID='{0}'", Convert.ToString(API.getEntityData(player, "PlayerID")));
            db.compileQuery(before, after, varNames, args);
            updateClothingForPlayer(player);

            API.call("ClothingShopHandler", "leaveClothingShop", player);
            API.triggerClientEvent(player, "endCamera");
            API.stopPedAnimation(player);
            API.stopPlayerAnimation(player);
        }


        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "clothingSave")
            {
                actionSaveClothing(player, args);
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
