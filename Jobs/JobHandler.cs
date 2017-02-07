using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Jobs
{
    public class JobHandler : Script
    {
        Main main = new Main();
        SpawnPoints spawnPoints = new SpawnPoints();
        DatabaseHandler db = new DatabaseHandler();

        public JobHandler()
        {
            API.onResourceStart += API_onResourceStart;
        }

        public void API_onResourceStart()
        {
            API.consoleOutput("Started: JobHandler");
        }

        [Command("beginjob")]
        public void cmdJobStart(Client player)
        {
            if(isInJob(player) == "True")
            {
                API.sendNotificationToPlayer(player, main.msgPrefix + "~r~You are currently on a job.");
                return;
            }
            // Start Fishing Job
            foreach (Vector3 point in spawnPoints.FishingSpawnPoints)
            {
                if (player.position.DistanceTo(point) <= 20)
                {
                    API.call("Fishing", "beginFishing", player);
                    return;
                }
            }

            // Start Pizza Job
            foreach (Vector3 point in spawnPoints.PizzaDeliveryPoints)
            {
                if (player.position.DistanceTo(point) <= 20)
                {
                    API.call("PizzaDelivery", "beginDelivery", player);
                    return;
                }
            }
        }

        public string isInJob(Client player)
        {
            string isJobStarted = db.pullDatabase("Players", "JobStarted", "Nametag", player.name);
            return isJobStarted;
        }

        [Command("CheckJobStarted")]
        public void checkJobStarted(Client player)
        {
            API.sendNotificationToPlayer(player, isInJob(player).ToString());
        }

        [Command("finishjob")]
        public void finishJob(Client player)
        {
            if (isInJob(player) == "True")
            {
                if (API.isPlayerInAnyVehicle(player))
                {
                    var x = Convert.ToSingle(db.pullDatabase("Players", "JobX", "Nametag", player.name));
                    var y = Convert.ToSingle(db.pullDatabase("Players", "JobY", "Nametag", player.name));
                    var z = Convert.ToSingle(db.pullDatabase("Players", "JobZ", "Nametag", player.name));
                    Vector3 targetPos = new Vector3(x, y, z);

                    if (player.position.DistanceTo(targetPos) <= 15)
                    {
                        string jobType = db.pullDatabase("Players", "JobType", "Nametag", player.name);

                        if (jobType == "PizzaDelivery")
                        {
                            clearJob(player);
                            API.sendNotificationToPlayer(player, "You have finished your job.");
                            db.setPlayerMoney(player, 30);
                            API.triggerClientEvent(player, "clearMarkers", player);
                        }
                    }
                    else
                    {
                        API.sendNotificationToPlayer(player, main.msgPrefix + "You are not at your target destination.");
                    }
                }
                else
                {
                    API.sendNotificationToPlayer(player, main.msgPrefix + "You cannot finish your job without the borrowed vehicle.");
                }
            }
        }

        public void clearJob(Client player)
        {
            db.updateDatabase("Players", "JobX", "0", "Nametag", player.name);
            db.updateDatabase("Players", "JobY", "0", "Nametag", player.name);
            db.updateDatabase("Players", "JobZ", "0", "Nametag", player.name);
            db.updateDatabase("Players", "JobStarted", "False", "Nametag", player.name);
            db.updateDatabase("Players", "JobType", "None", "Nametag", player.name);
            
            NetHandle tempVehicle = new NetHandle(Convert.ToInt32(db.pullDatabase("Players", "TempJobVehicle", "Nametag", player.name)));
            API.deleteEntity(tempVehicle);

            db.updateDatabase("Players", "TempJobVehicle", "None", "Nametag", player.name);

            API.triggerClientEvent(player, "clearMarkers", player);
        }
    }
}
