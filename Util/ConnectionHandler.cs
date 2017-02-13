using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkServer;
using GTANetworkShared;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace stuykserver.Util
{
    public class ConnectionHandler : Script
    {
        Main main = new Main();
        SpawnPoints spawnPoints = new SpawnPoints();
        DatabaseHandler db = new DatabaseHandler();

        int dimension;

        public ConnectionHandler()
        {
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onPlayerBeginConnect += API_onPlayerBeginConnect;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onPlayerDeath += API_onPlayerDeath;
            API.onResourceStop += API_onResourceStop;

            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: ConnectionHandler");
        }

        private void API_onResourceStop()
        {
            List<Client> playerList = API.getAllPlayers();

            foreach (Client player in playerList)
            {
                db.updateDatabase("Players", "LoggedIn", "0", "Nametag", player.name);
                db.updateDatabase("Players", "LASTX", player.position.X.ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "LASTY", player.position.Y.ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "LASTZ", player.position.Z.ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "CurrentSkin", ((PedHash)API.getEntityModel(player)).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "Health", API.getPlayerHealth(player).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "Armor", API.getPlayerArmor(player).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "JobStarted", "False", "Nametag", player.name);
                db.updateDatabase("Players", "JobX", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobY", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobZ", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobType", "None", "Nametag", player.name);

                if (db.pullDatabase("Players", "TempJobVehicle", "Nametag", player.name) != "None")
                {
                    NetHandle tempVehicle = new NetHandle(Convert.ToInt32(db.pullDatabase("Players", "TempJobVehicle", "Nametag", player.name)));
                    API.deleteEntity(tempVehicle);
                }

                db.updateDatabase("Players", "TempJobVehicle", "None", "Nametag", player.name);
            }
        }

        public void API_onPlayerFinishedDownload(Client player)
        {
            API.triggerClientEvent(player, "createCamera", spawnPoints.ServerSpawnPoints[0], spawnPoints.ServerSpawnPoints[0]);
            API.setEntityDimension(player, ++dimension);
        }

        private void API_onPlayerBeginConnect(Client player, CancelEventArgs cancelConnection)
        {
            // Check if player has a valid username before connecting.
            if (!main.isValidUsername(player.name))
            {
                API.kickPlayer(player, "Invalid Username Format. Example: John_Doe");
                return;
            }
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            string loginPull = db.pullDatabase("Players", "LoggedIn", "Nametag", player.name);

            if (main.isPlayerLoggedIn(player))
            {
                db.updateDatabase("Players", "LASTX", player.position.X.ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "LASTY", player.position.Y.ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "LASTZ", player.position.Z.ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "LoggedIn", "0", "Nametag", player.name);
                db.updateDatabase("Players", "CurrentSkin", ((PedHash)API.getEntityModel(player)).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "Health", API.getPlayerHealth(player).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "Armor", API.getPlayerArmor(player).ToString(), "Nametag", player.name);
                db.updateDatabase("Players", "JobStarted", "False", "Nametag", player.name);
                db.updateDatabase("Players", "JobX", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobY", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobZ", "0", "Nametag", player.name);
                db.updateDatabase("Players", "JobType", "None", "Nametag", player.name);
                API.consoleOutput("[DATABASE] " + player.name.ToString() + " has disconnected.");

                if (db.pullDatabase("Players", "TempJobVehicle", "Nametag", player.name) != "None")
                {
                    NetHandle tempVehicle = new NetHandle(Convert.ToInt32(db.pullDatabase("Players", "TempJobVehicle", "Nametag", player.name)));
                    API.deleteEntity(tempVehicle);
                }

                db.updateDatabase("Players", "TempJobVehicle", "None", "Nametag", player.name);
            }
            else
            {
                return;
            }
            return;
        }

        private void API_onPlayerDeath(Client player, NetHandle entityKiller, int weapon)
        {
            db.updateDatabase("Players", "Health", "100", "Nametag", player.name);
            db.updateDatabase("Players", "Armor", "0", "Nametag", player.name);
            API.sendNotificationToPlayer(player, main.msgPrefix + "You have died, and respawned at a Hospital.");
        }

        public void SpawnPlayer(Client player)
        {
            string s = player.socialClubName;

            API.triggerClientEvent(player, "endCamera");

            var x = Convert.ToSingle(db.pullDatabase("Players", "LastX", "Nametag", player.name));
            var y = Convert.ToSingle(db.pullDatabase("Players", "LastY", "Nametag", player.name));
            var z = Convert.ToSingle(db.pullDatabase("Players", "LastZ", "Nametag", player.name));

            db.updateDatabase("Players", "JobStarted", "False", "Nametag", player.name);
            db.updateDatabase("Players", "JobX", "0", "Nametag", player.name);
            db.updateDatabase("Players", "JobY", "0", "Nametag", player.name);
            db.updateDatabase("Players", "JobZ", "0", "Nametag", player.name);
            db.updateDatabase("Players", "JobType", "None", "Nametag", player.name);

            player.freezePosition = false;
            API.setEntityPosition(player, new Vector3(x, y, z));
            API.setPlayerSkin(player, API.pedNameToModel(db.pullDatabase("Players", "CurrentSkin", "Nametag", player.name)));

            API.triggerClientEvent(player, "killPanel");
            API.setEntityDimension(player, 0);

            //API.call("VehicleHandler", "SpawnPlayerCars", player);
        }
    }
}
