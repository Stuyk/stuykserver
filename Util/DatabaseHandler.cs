using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace stuykserver.Util
{
    public class DatabaseHandler : Script
    {
        public string msgPrefix = "~y~[~w~STUYK~y~]~w~ ";

        public DatabaseHandler()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: DatabaseHandler");
        }

        // ########################################
        //        Login Handling / Setting
        // ########################################
        public void setPlayerLoggedIn(Client player)
        {
            updateDatabase("Players", "LoggedIn", "1", "Nametag", player.name);
            API.consoleOutput("{0} has logged in.", player.name);
        }

        public void setPlayerLoggedOut(Client player)
        {
            updateDatabase("Players", "LoggedIn", "0", "Nametag", player.name);
            API.consoleOutput("{0} has logged out.", player.name);
        }

        public bool isPlayerLoggedIn(Client player)
        {
            return Convert.ToBoolean(pullDatabase("Players", "LoggedIn", "Nametag", player.name));
        }

        // ########################################
        //        Player Specific Settings
        // ########################################
        public void setPlayerPosition(Client player)
        {
            updateDatabase("Players", "LASTX", player.position.X.ToString(), "Nametag", player.name);
            updateDatabase("Players", "LASTY", player.position.Y.ToString(), "Nametag", player.name);
            updateDatabase("Players", "LASTZ", player.position.Z.ToString(), "Nametag", player.name);
        }

        public void setPlayerPositionByVector(Client player, Vector3 vector3)
        {
            updateDatabase("Players", "LASTX", vector3.X.ToString(), "Nametag", player.name);
            updateDatabase("Players", "LASTY", vector3.Y.ToString(), "Nametag", player.name);
            updateDatabase("Players", "LASTZ", vector3.Z.ToString(), "Nametag", player.name);
        }

        public void setPlayerHUD(Client player, bool setting)
        {
            API.sendNativeToPlayer(player, Hash.DISPLAY_HUD, setting);
            API.sendNativeToPlayer(player, Hash.DISPLAY_RADAR, setting);
        }


        // Check if player is an admin.
        public bool isAdmin(string playerName)
        {
            bool result = Convert.ToBoolean(pullDatabase("Players", "Admin", "Nametag", playerName));
            if (result)
            {
                return true;
            }
            return false;
        }

        // Create a Vector3 List of Database Postions
        public List<Vector3> vectorPositions(string tableName)
        {
            List<Vector3> vectorList = new List<Vector3>();
            DataTable result = API.exported.database.executeQueryWithResult("SELECT * FROM " + tableName);

            foreach (DataRow row in result.Rows)
            {
                float x = Convert.ToSingle(row[1]);
                float y = Convert.ToSingle(row[2]);
                float z = Convert.ToSingle(row[3]);

                API.sendNotificationToAll(x.ToString() + y.ToString() + z.ToString());

                vectorList.Add(new Vector3(x, y, z));
            }
            return vectorList;
        }


        // Check if the Nametag already exists.
        public bool usernameExists(string playerName)
        {
            string query = "SELECT Nametag FROM Players";
            DataTable result = API.exported.database.executeQueryWithResult(query);
            bool exists = result.Select().ToList().Exists(row => row["Nametag"].ToString() == playerName);
            if (exists)
            {
                return true;
            }
            return false;
        }

        public bool checkIfExists(string what, string where, string compareTo)
        {
            string query = "SELECT " + what + " FROM " + where;
            DataTable result = API.exported.database.executeQueryWithResult(query);
            bool exists = result.Select().ToList().Exists(row => row["what"].ToString() == compareTo);
            if (exists)
            {
                return true;
            }
            return false;
        }

        // Check if the organization owner has an organization.
        public bool checkIfOwnerHasOrgnization(string playerName)
        {
            string query = "SELECT Owner FROM Organization";
            DataTable result = API.exported.database.executeQueryWithResult(query);
            bool exists = result.Select().ToList().Exists(row => row["Owner"].ToString() == playerName);
            if (exists)
            {
                if (pullDatabase("Players", "Organization", "Nametag", playerName) == "none")
                {
                    return true;
                }
            }
            return false;
        }

        //Update a piece of the PlayerDatabase
        public void updateDatabase(string tableName, string databaseColumn, string value, string where, string what)
        {
            string query = "UPDATE " + tableName + " SET " + databaseColumn + "='" + value + "' WHERE " + where + "='" + what + "'";
            API.exported.database.executeQueryWithResult(query);
        }

        //Pull string of the Database
        public string pullDatabase(string tableName, string databaseColumn, string where, string what)
        {
            string query = "SELECT " + databaseColumn + " FROM " + tableName + " WHERE " + where + "='" + what + "'";
            DataTable queryResult = API.exported.database.executeQueryWithResult(query);

            if (queryResult != null)
            {
                if (queryResult.Rows.Count > 0)
                {
                    string returnString = queryResult.Rows[0][0].ToString();
                    return returnString;
                } 
            }
            return null;
        }

        public void insertDataPointPosition(string tablename, Client player)
        {
            string query = string.Format("INSERT INTO {0} (PosX, PosY, PosZ) VALUES ({1}, {2}, {3})", tablename, player.position.X, player.position.Y, player.position.Z);
            API.exported.database.executeQueryWithResult(query);
            API.sendNotificationToPlayer(player, "Created");
        }

        //Create a piece of the Database
        public void insertDatabase(string tableName, string databaseColumn, string value)
        {
            string query = "INSERT INTO " + tableName + " (" + databaseColumn + ") VALUES ('" + value + "')";
            API.exported.database.executeQueryWithResult(query);
        }

        //Get player money from Database
        public int getPlayerMoney(Client player)
        {
            int returnInt = Convert.ToInt32(pullDatabase("Players", "Money", "Nametag", player.name));
            return returnInt;
        }

        //Set player money to Database
        public void setPlayerMoney(Client player, int value)
        {
            int oldMoney = getPlayerMoney(player);
            int newMoney = oldMoney + value;

            updateDatabase("Players", "Money", newMoney.ToString(), "Nametag", player.name);

            API.triggerClientEvent(player, "update_money_display", newMoney);
            return;
        }

        //Get player money from Database
        public int getPlayerAtmMoney(Client player)
        {
            int returnInt = Convert.ToInt32(pullDatabase("Players", "Bank", "Nametag", player.name));
            return returnInt;
        }

        //Set atm money to Database
        public void setPlayerAtmMoney(Client player, int value)
        {
            int oldMoney = getPlayerAtmMoney(player);
            int newMoney = oldMoney + value;

            updateDatabase("Players", "Bank", newMoney.ToString(), "Nametag", player.name);
            return;
        }

        public bool databasePlayCarSlotExists(Client player, int slot)
        {
            string vehicle = pullDatabase("PlayerVehicles", "VehicleType" + slot.ToString(), "Garage", player.name);
            if (vehicle != "" || vehicle != null)
            {
                return true;
            }
            return false;
        }

        public void updateVehiclePosition(Client player, int slot)
        {
            updateDatabase("PlayerVehicles", slot.ToString() + "PosX", player.vehicle.position.X.ToString(), "Garage", player.name);
            updateDatabase("PlayerVehicles", slot.ToString() + "PosY", player.vehicle.position.Y.ToString(), "Garage", player.name);
            updateDatabase("PlayerVehicles", slot.ToString() + "PosZ", player.vehicle.position.Z.ToString(), "Garage", player.name);
        }

        public bool isDead(Client player)
        {
            if(player.health <= 0)
            {
                return true;
            }
            return false;
        }

        public Vector3 convertStringToVector3(string pass)
        {
            string xpos = pass.Replace("X:", string.Empty);
            xpos = xpos.Replace("Y:", string.Empty);
            xpos = xpos.Replace("Z:", string.Empty);
            string[] coords = xpos.Split(' ');
            Vector3 finalVector = new Vector3(Convert.ToSingle(coords[1]), Convert.ToSingle(coords[3]), Convert.ToSingle(coords[5]));
            return finalVector;
        }

        public void insertPurchasedVehicle(Client player, Vehicle vehicle, VehicleHash vehicletype)
        {
            Vector3 pos = API.getEntityPosition(vehicle);
            Vector3 rot = API.getEntityRotation(vehicle);
            string query = string.Format("INSERT INTO PlayerVehicles (Garage, PosX, PosY, PosZ, RotX, RotY, RotZ, VehicleType) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", player.name, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, API.getVehicleDisplayName(vehicletype));
            API.exported.database.executeQueryWithResult(query);
        }
    }
}
