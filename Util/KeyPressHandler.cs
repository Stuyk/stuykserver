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
            // Controls Vehicle Hood
            if (eventName == "vehicleHood")
            {
                API.call("VehicleHandler", "actionVehicleHood", player);
                return;
            }

            // Controls Vehicle Trunk
            if (eventName == "vehicleTrunk")
            {
                API.call("VehicleHandler", "actionVehicleTrunk", player);
                return;
            }

            // SHIFT + B FUNCTIONS
            if (eventName == "useSpecial")
            {
                string currentFunction = Convert.ToString(API.getEntityData(player, "Collision"));

                // If NONE, do nothing.
                if (currentFunction == "None")
                {
                    return;
                }

                // House Property Panel
                if (currentFunction == "House")
                {
                    API.call("HouseHandler", "actionHousePropertyPanel", player);
                    return;
                }
            }

            // B FUNCTIONS
            if (eventName == "useFunction")
            {
                // Pull Current Function off Player - Assigned by Collision Handler - These are ENUM Types converted to STRINGS.
                string currentFunction = Convert.ToString(API.getEntityData(player, "Collision"));
                API.consoleOutput("Function: {0}", currentFunction);

                // If NONE, do nothing.
                if (currentFunction == "None")
                {
                    return;
                }

                // ON FOOT FUNCTIONS
                if (!player.isInVehicle)
                {
                    // ATM
                    if (currentFunction == "Atm")
                    {
                        API.call("BankHandler", "selectATM", player);
                        return;
                    }

                    // FISHING
                    if (currentFunction == "Fishing")
                    {
                        API.call("Fishing", "startFishing", player);
                        return;
                    }

                    // FISHING SALES
                    if (currentFunction == "FishingSale")
                    {
                        API.call("Fishing", "sellFish", player);
                        return;
                    }

                    // BARBERSHOP
                    if (currentFunction == "Barbershop")
                    {
                        API.call("BarberShopHandler", "selectBarberShop", player);
                        return;
                    }

                    // CLOTHING SHOP
                    if (currentFunction == "Clothing")
                    {
                        API.call("ClothingShopHandler", "selectClothing", player);
                        return;
                    }

                    // ENTER HOUSE
                    if (currentFunction == "House")
                    {
                        API.call("HouseHandler", "actionHouseControl", player);
                    }

                    // VEHICLE LOCK
                    if (currentFunction == "Vehicle")
                    {
                        API.call("VehicleHandler", "actionLockCar", player);
                    }

                    // DEALERSHIP - BOATS TYPE
                    if (currentFunction == "Boats")
                    {
                        API.call("VehicleShopHandler", "browserDealership", player, currentFunction);
                    }
                }

                // VEHICLE FUNCTIONS
                if (player.isInVehicle)
                {
                    // VEHICLE MODIFICATIONS
                    if (currentFunction == "Modification")
                    {
                        API.call("VehicleModificationHandler", "actionEnterShop", player);
                        return;
                    }

                    // VEHICLE ENGINE
                    if (currentFunction == "InVehicle")
                    {
                        API.call("VehicleHandler", "actionVehicleEngine", player);
                    }
                }
            }
        }
    }
}
