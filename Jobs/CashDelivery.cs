using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Jobs
{
    public class CashDelivery : Script
    {
        public CashDelivery()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.createMarker(1, new Vector3(start.X, start.Y, start.Z - 2), new Vector3(), new Vector3(), new Vector3(2f, 2f, 2f), 75, 0, 255, 0);
        }

        Vector3 start = new Vector3(-365.7887, -74.00308, 45.66034);
        Vector3 end = new Vector3();

        Vector3 truckSpawn = new Vector3(-359.9267, -87.88322, 45.58482);

        [Command("start")]
        public void actionStartCashDelivery(Client player)
        {
            if (player.position.DistanceTo(start) >= 5f)
            {
                return;
            }

            bool returnValue = setupDeliveryTruck(player);

            // If false cancel it.
            if (!returnValue)
            {
                API.sendChatMessageToPlayer(player, "~r~There is currently someone boarding. Please wait...");
                return;
            }

            API.sendChatMessageToPlayer(player, "~g~You are tasked to deliver this vehicle to its proper destination. Good luck.");

            // Mission Handler
        }

        private bool setupDeliveryTruck(Client player)
        {
            List<NetHandle> vehicles = API.getAllVehicles();

            foreach (NetHandle veh in vehicles)
            {
                if (API.getEntityPosition(veh).DistanceTo(truckSpawn) <= 2f)
                {
                    return false;
                }
            }

            Vehicle deliveryTruck = API.createVehicle(VehicleHash.Stockade, truckSpawn, new Vector3(), 112, 112);

            API.sendNativeToPlayer(player, (ulong)Hash.TASK_ENTER_VEHICLE, player, deliveryTruck, -1, -1, 2.0, 1, 0);

            while (player.isInVehicle == false) { }

            return true;
        }
    }
}
