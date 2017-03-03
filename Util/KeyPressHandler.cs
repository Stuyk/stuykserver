using GTANetworkServer;
using GTANetworkShared;
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
            if (eventName == "useFunction")
            {
                switch(args[0].ToString())
                {
                    case "VehicleModificationShop":
                        API.call("VehicleModificationHandler", "actionEnterShop", player);
                        break;

                    case "FishingSpot":
                        API.call("Fishing", "startFishing", player);
                        break;

                    case "FishingSaleSpot":
                        API.call("Fishing", "sellFish", player);
                        break;

                    case "BarberShop":
                        API.call("BarberShopHandler", "selectBarberShop", player);
                        break;

                    case "Bank":
                        API.call("BankHandler", "selectATM", player);
                        break;

                    case "Clothing":
                        API.call("ClothingShopHandler", "selectClothing", player);
                        break;

                    case "Dealership":
                        API.call("VehicleShopHandler", "browseDealership", player);
                        break;

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
                        API.call("HouseHandler", "actionHouseControl", player);
                        break;
                    case "HouseOwnershipPanel":
                        API.call("HouseHandler", "actionHousePropertyPanel", player);
                        break;
                }
            }
        }
    }
}
