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
            if (eventName == "useController")
            {
                if (!player.isInVehicle)
                {
                    API.call("Fishing", "startFishing", player);
                    API.call("Fishing", "sellFish", player);
                    API.call("BarberShopHandler", "selectBarberShop", player);
                    API.call("BankHandler", "selectATM", player);
                    API.call("ClothingShopHandler", "selectClothing", player);
                    API.call("VehicleShopHandler", "browseDealership", player);
                    API.call("VehicleHandler", "actionLockCar", player);
                }
            }

            if (eventName == "vehicleController")
            {
                if (player.isInVehicle)
                {
                    API.call("VehicleHandler", "actionVehicleEngine", player);
                }
            }
        }
    }
}
