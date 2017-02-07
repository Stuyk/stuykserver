using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Jobs
{
    public class PizzaDelivery : Script
    {
        Main main = new Main();
        DatabaseHandler db = new DatabaseHandler();
        JobHandler jh = new JobHandler();
        SpawnPoints sp = new SpawnPoints();

        public PizzaDelivery()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: PizzaDelivery");
        }

        public void beginDelivery(Client player)
        {
            player.freeze(false);
            API.sendChatMessageToPlayer(player, main.msgPrefix + "A resident is requesting a delivery. Drive to the designated waypoint.");
            var x = Convert.ToSingle(db.pullDatabase("PizzaDelivery", "PosX", "ID", "0"));
            var y = Convert.ToSingle(db.pullDatabase("PizzaDelivery", "PosY", "ID", "0"));
            var z = Convert.ToSingle(db.pullDatabase("PizzaDelivery", "PosZ", "ID", "0"));

            API.triggerClientEvent(player, "markonmap", x, y, z);
            API.triggerClientEvent(player, "createTextLabel", "/ringdoorbell", x, y, z);
            API.sendNotificationToPlayer(player, "~r~To stop at any time type: /stop");

            db.updateDatabase("Players", "JobStarted", "True", "Nametag", player.name);
            db.updateDatabase("Players", "JobX", x.ToString(), "Nametag", player.name);
            db.updateDatabase("Players", "JobY", y.ToString(), "Nametag", player.name);
            db.updateDatabase("Players", "JobZ", z.ToString(), "Nametag", player.name);
            db.updateDatabase("Players", "JobType", "PizzaDelivery", "Nametag", player.name);

            var rot = API.getEntityRotation(player.handle);
            Vehicle veh = API.createVehicle(API.vehicleNameToModel("Rhapsody"), sp.PizzaDeliveryCarSpawns[0], player.rotation, 46, 46);
            API.setPlayerIntoVehicle(player, veh, -1);
            db.updateDatabase("Players", "TempJobVehicle", veh.Value.ToString(), "Nametag", player.name);
        }

        public void rerouteDelivery(Client player)
        {
            API.triggerClientEvent(player, "clearMarkers", player);

            var x = sp.PizzaDeliveryCarSpawns[0].X;
            var y = sp.PizzaDeliveryCarSpawns[0].Y;
            var z = sp.PizzaDeliveryCarSpawns[0].Z;

            db.updateDatabase("Players", "JobX", x.ToString(), "Nametag", player.name);
            db.updateDatabase("Players", "JobY", y.ToString(), "Nametag", player.name);
            db.updateDatabase("Players", "JobZ", z.ToString(), "Nametag", player.name);

            API.triggerClientEvent(player, "markonmap", x, y, z);
            API.triggerClientEvent(player, "createTextLabel", "/finishjob", x, y, z);

            API.sendNotificationToPlayer(player, main.msgPrefix + "Finish your delivery by returning your vehicle.");
        }

        [Command("ringdoorbell")]
        public void cmdFinishDelivery(Client player)
        {
            var x = Convert.ToSingle(db.pullDatabase("Players", "JobX", "Nametag", player.name));
            var y = Convert.ToSingle(db.pullDatabase("Players", "JobY", "Nametag", player.name));
            var z = Convert.ToSingle(db.pullDatabase("Players", "JobZ", "Nametag", player.name));
            Vector3 targetPosition = new Vector3(x, y, z);

            if (!API.isPlayerInAnyVehicle(player))
            {
                if (player.position.DistanceTo(targetPosition) <= 15)
                {
                    API.triggerClientEvent(player, "clear");
                    rerouteDelivery(player);
                } 
            }
            else
            {
                API.sendNotificationToPlayer(player, main.msgPrefix + "You must exit your vehicle to finish your delivery.");
            }
        }


    }
}
