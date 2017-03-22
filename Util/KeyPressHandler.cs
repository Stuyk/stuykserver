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

                if (!player.isInVehicle)
                {
                    // House Property Panel
                    if (currentFunction == "House")
                    {
                        API.call("HouseHandler", "actionHousePropertyPanel", player);
                        return;
                    }
                }
                

                // VEHICLE FUNCTIONS
                if (player.isInVehicle)
                {
                    // VEHICLE LOCK - SHIFT + B - Version
                    if (currentFunction == "VehicleEngine")
                    {
                        API.call("VehicleHandler", "actionVehicleLock", player);
                        return;
                    }
                }
            }

            // B FUNCTIONS
            if (eventName == "useFunction")
            {
                // Pull Current Function off Player - Assigned by Collision Handler - These are ENUM Types converted to STRINGS.
                string currentFunction = Convert.ToString(API.getEntityData(player, "Collision"));

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
                        if (API.getEntityData(player, "IsInInterior") == true)
                        {
                            API.call("HouseHandler", "actionHouseExit", player);
                            return;
                        }

                        API.call("HouseHandler", "actionHouseControl", player);
                        return;
                    }

                    // VEHICLE LOCK
                    if (currentFunction == "Vehicle")
                    {
                        API.call("VehicleHandler", "actionVehicleLock", player);
                        return;
                    }

                    // Dealership Switch
                    switch (currentFunction)
                    {
                        case "Boats":
                            pushDealership(player, currentFunction);
                            break;
                        case "Classic":
                            pushDealership(player, currentFunction);
                            break;
                        case "Commercial":
                            pushDealership(player, currentFunction);
                            break;
                        case "Compacts":
                            pushDealership(player, currentFunction);
                            break;
                        case "Coupes":
                            pushDealership(player, currentFunction);
                            break;
                        case "Bicycles":
                            pushDealership(player, currentFunction);
                            break;
                        case "Helicopters":
                            pushDealership(player, currentFunction);
                            break;
                        case "Industrial":
                            pushDealership(player, currentFunction);
                            break;
                        case "Motorcycles":
                            pushDealership(player, currentFunction);
                            break;
                        case "Muscle":
                            pushDealership(player, currentFunction);
                            break;
                        case "OffRoad":
                            pushDealership(player, currentFunction);
                            break;
                        case "Planes":
                            pushDealership(player, currentFunction);
                            break;
                        case "Police":
                            pushDealership(player, currentFunction);
                            break;
                        case "SUVS":
                            pushDealership(player, currentFunction);
                            break;
                        case "Sedans":
                            pushDealership(player, currentFunction);
                            break;
                        case "Sports":
                            pushDealership(player, currentFunction);
                            break;
                        case "Super":
                            pushDealership(player, currentFunction);
                            break;
                        case "Utility":
                            pushDealership(player, currentFunction);
                            break;
                        case "Vans":
                            pushDealership(player, currentFunction);
                            break;
                    }
                }

                // VEHICLE FUNCTIONS
                if (player.isInVehicle)
                {
                    switch (currentFunction)
                    {
                        case "Modification":
                            API.call("VehicleModificationHandler", "actionEnterShop", player);
                            return;
                        case "VehicleEngine":
                            API.call("VehicleHandler", "actionVehicleEngine", player);
                            return;
                        case "Repair":
                            API.call("RepairShopHandler", "actionRepair", player);
                            return;
                        case "FuelPump":
                            API.call("FuelPumpHandler", "actionUseFuel", player);
                            return;
                    }
                }
            }
        }

        public void pushDealership(Client player, string currentFunction)
        {
            API.call("VehicleShopHandler", "browseDealership", player, currentFunction);
        }
    }
}
