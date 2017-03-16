using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class RepairShopHandler : Script
    {
        public RepairShopHandler()
        {
            API.consoleOutput("Started: Repair Shop");
        }

        // Action
        public void actionRepair(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (!player.isInVehicle)
            {
                return;
            }

            int repairCost = calculateVehicleRepair(player);

            if (instance.returnPlayerCash() < repairCost)
            {
                API.sendChatMessageToPlayer(player, "~y~Repair # ~o~Not enough cash to repair your vehicle.");
                return;
            }

            // Start a Timestamp.
            DateTime preTime = DateTime.Now;
            API.sendChatMessageToPlayer(player, "~y~Repair # ~b~Do not move your vehicle out of range.");

            bool failed = false; // Set to true when the player does something wrong.
            bool firstPass = false; // 20 Seconds Remain
            bool secondPass = false; // 10 Seconds Remain
            while (DateTime.Now < preTime.AddSeconds(30))
            {
                if (!player.isInVehicle)
                {
                    cancelRepair(player, 0);
                    failed = true;
                    break;
                }

                if (DateTime.Now >= preTime.AddSeconds(10) && DateTime.Now <= preTime.AddSeconds(11) && !firstPass)
                {
                    firstPass = true;
                    API.sendChatMessageToPlayer(player, "~y~Repair # ~o~20 Seconds Remain.");
                }

                if (DateTime.Now >= preTime.AddSeconds(20) && DateTime.Now <= preTime.AddSeconds(21) && !secondPass)
                {
                    secondPass = true;
                    API.sendChatMessageToPlayer(player, "~y~Repair # ~o~10 Seconds Remain.");
                }
                
                if (API.getEntityData(player, "ColShape") == null)
                {
                    cancelRepair(player, 1);
                    failed = true;
                    break;
                }
            }

            if (failed == false)
            {
                finishRepair(player, repairCost);
                return;
            }
            return;
        }

        //Action display cost.
        public void actionDisplayCost(Client player)
        {
            if (!player.isInVehicle)
            {
                return;
            }

            API.sendChatMessageToPlayer(player, string.Format("~y~Repair # ~b~A repair will cost: {0}", calculateVehicleRepair(player)));
            return;
        }

        public int calculateVehicleRepair(Client player)
        {
            if (!player.isInVehicle)
            {
                return 0;
            }

            int vehicleHealth = Convert.ToInt32(player.vehicle.health);
            if (vehicleHealth <= 250)
            {
                return 125;
            }
            
            if (vehicleHealth >= 251 && vehicleHealth <= 500)
            {
                return 75;
            }

            if (vehicleHealth >= 501)
            {
                return 50;
            }

            return 150;
        }

        public void finishRepair(Client player, int repairCost)
        {
            if (!player.isInVehicle)
            {
                return;
            }

            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            player.vehicle.repair();
            instance.removePlayerCash(repairCost);
            API.sendChatMessageToPlayer(player, "~y~Repair # ~b~Your vehicle has been repaired.");
            return;
        }

        public void cancelRepair(Client player, int type)
        {
            if (type == 0)
            {
                API.sendChatMessageToPlayer(player, "~y~Repair # ~o~You left your vehicle, so your vehicle was not repaired.");
            }
            
            if (type == 1)
            {
                API.sendChatMessageToPlayer(player, "~y~Repair # ~o~You left the point, so your vehicle was not repaired.");
            }
            return;
        }
    }
}
