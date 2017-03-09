using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using stuykserver.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class KeyPressHandler : Script
    {
        ClothingShopHandler clothingShopHandler = new ClothingShopHandler();

        public KeyPressHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "useSpecial")
            {
                if (API.getEntitySyncedData(player, "Collision") == null)
                {
                    return;
                }

                Shop.ShopType type = (Shop.ShopType)Enum.Parse(typeof(Shop.ShopType), Convert.ToString(API.getEntityData(player, "Collision")));
                switch (type)
                {
                    case Shop.ShopType.House:
                        API.call("HouseHandler", "actionHousePropertyPanel", player);
                        break;
                }
            }

            if (eventName == "useFunction")
            {
                if (API.getEntityData(player, "Collision") == null)
                {
                    return;
                }

                Shop.ShopType type = (Shop.ShopType)Enum.Parse(typeof(Shop.ShopType), Convert.ToString(API.getEntityData(player, "Collision")));
                switch (type)
                {
                    case Shop.ShopType.Atm:
                        API.call("BankHandler", "selectATM", player);
                        break;
                    case Shop.ShopType.Fishing:
                        API.call("Fishing", "startFishing", player);
                        break;
                    case Shop.ShopType.Modification:
                        API.call("VehicleModificationHandler", "actionEnterShop", player);
                        break;
                    case Shop.ShopType.FishingSale:
                        API.call("Fishing", "sellFish", player);
                        break;
                    case Shop.ShopType.Barbershop:
                        API.call("BarberShopHandler", "selectBarberShop", player);
                        break;
                    case Shop.ShopType.Clothing:
                        API.call("ClothingShopHandler", "selectClothing", player);
                        break;
                    case Shop.ShopType.House:
                        API.call("HouseHandler", "actionHouseControl", player);
                        break;
                    // This has to be done for all Vehicle Types.
                    case Shop.ShopType.Bicycles:
                        API.call("VehicleShopHandler", "browseDealership", player);
                        break;
                }

                /*
                 * 
                 * NEED TO DO A NEW VERSION OF THIS FOR UP ABOVE
                switch (args[0].ToString())
                {

                    case "Dealership":
                        

                    case "VehicleEngine":
                        API.call("VehicleHandler", "actionVehicleEngine", player);
                        break;

                    case "VehicleLock":
                        API.call("VehicleHandler", "actionLockCar", player);
                        break;

                    case "ActionVehicleHood":
                        API.call("VehicleHandler", "actionVehicleHood", player);
                        break;

                    case "ActionVehicleTrunk":
                        API.call("VehicleHandler", "actionVehicleTrunk", player);
                        break;

                    case "House":
                        
                    case "HouseOwnershipPanel":
                        
                }
                */
            }
        }
    }
}
