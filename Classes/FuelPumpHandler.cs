using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace stuykserver.Classes
{
    public class FuelPumpHandler : Script
    {
        Dictionary<Client, Timer> gettingFuel = new Dictionary<Client, Timer>();
        List<Client> stopFuel = new List<Client>();

        public FuelPumpHandler()
        {
            API.onPlayerDisconnected += API_onPlayerDisconnected;
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            if (gettingFuel.ContainsKey(player))
            {
                gettingFuel[player].Dispose();
                gettingFuel.Remove(player);
            }
        }

        public void actionUseFuel(Client player)
        {
            if (!player.isInVehicle)
            {
                return;
            }

            VehicleClass vehicle = (VehicleClass)API.call("VehicleHandler", "getVehicleByPlayer", player);

            if (vehicle == null)
            {
                return;
            }

            if (gettingFuel.ContainsKey(player))
            {
                API.sendChatMessageToPlayer(player, "~r~You're already getting fuel.");
                return;
            }

            ColShape colshape = (ColShape)API.getEntityData(player, "ColShape");
            Shop shop = (Shop)API.call("ShopHandler", "getShop", colshape);

            if (shop.returnShopUnits() <= 0)
            {
                API.sendChatMessageToPlayer(player, "~b~FuelPump: ~o~There is no fuel available.");
                return;
            }

            Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Enabled = true;
            timer.Elapsed += FuelTimer;

            gettingFuel.Add(player, timer);

            API.sendChatMessageToPlayer(player, "~b~FuelPump: ~o~You started pumping fuel.");

            API.setVehicleEngineStatus(player.vehicle, false);
        }

        private void addToFuelStop(Client player)
        {
            if (!stopFuel.Contains(player))
            {
                stopFuel.Add(player);
                API.sendChatMessageToPlayer(player, "~b~FuelPump: ~o~You have stopped pumping fuel.");
                API.setVehicleEngineStatus(player.vehicle, true);
                return;
            }
        }

        private void FuelTimer(object sender, ElapsedEventArgs e)
        {
            foreach (Client player in gettingFuel.Keys)
            {
                Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);

                if (API.getEntityData(player, "Collision") == "None")
                {
                    addToFuelStop(player);
                }

                if (instance.returnPlayerCash() <= 0)
                {
                    addToFuelStop(player);
                    API.sendChatMessageToPlayer(player, "~b~FuelPump: ~o~You don't have enough money.");
                    API.setVehicleEngineStatus(player.vehicle, true);
                }

                instance.removePlayerCash(3);

                if (!player.isInVehicle)
                {
                    addToFuelStop(player);
                    API.setVehicleEngineStatus(player.vehicle, true);
                }

                VehicleClass vehicle = (VehicleClass)API.call("VehicleHandler", "getVehicleByPlayer", player);

                vehicle.addFuel(5);

                ColShape colshape = (ColShape)API.getEntityData(player, "ColShape");

                if (colshape == null)
                {
                    addToFuelStop(player);
                }

                Shop shop = (Shop)API.call("ShopHandler", "getShop", colshape);

                if (shop == null)
                {
                    addToFuelStop(player);
                }

                if (shop.returnShopUnits() <= 0)
                {
                    addToFuelStop(player);
                    API.sendNotificationToPlayer(player, "~b~FuelPump: ~o~Has run out of fuel.");
                }

                shop.removeShopUnits(5);

                API.triggerClientEvent(player, "updateFuel", vehicle.returnFuel());

                if (vehicle.returnFuel() >= 100)
                {
                    addToFuelStop(player);
                    API.sendNotificationToPlayer(player, "~b~FuelPump: ~o~Your tank is full.");
                }
            }

            foreach (Client player in stopFuel)
            {
                if (gettingFuel.ContainsKey(player))
                {
                    gettingFuel[player].Enabled = false;
                    gettingFuel[player].Dispose();
                    gettingFuel.Remove(player);
                    API.setVehicleEngineStatus(player.vehicle, true);
                    return;
                }
            }

            stopFuel.Clear();
        }
    }
}
