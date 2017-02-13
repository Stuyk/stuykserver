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
            if (value > 0)
            {
                API.sendNotificationToPlayer(player, msgPrefix + "~g~+ $" + value.ToString());
            }
            else
            {
                API.sendNotificationToPlayer(player, msgPrefix + "~r~- $" + value.ToString().Remove(0, 1));
            }

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
            if (value > 0)
            {
                API.sendNotificationToPlayer(player, msgPrefix + "~y~Bank: ~g~+ $" + value.ToString());
            }
            else
            {
                API.sendNotificationToPlayer(player, msgPrefix + "~y~Bank: ~r~- $" + value.ToString().Remove(0, 1));
            }

            int oldMoney = getPlayerAtmMoney(player);
            int newMoney = oldMoney + value;

            updateDatabase("Players", "Bank", newMoney.ToString(), "Nametag", player.name);
            API.sendNotificationToPlayer(player, msgPrefix + "~y~Bank Balance: ~g~" + getPlayerAtmMoney(player).ToString());
            API.playSoundFrontEnd(player, "Menu_Accept", "Phone_SoundSet_Default");
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

        public NetHandle databaseSpawnPlayerCar(Client player, int slot)
        {
            VehicleHash vehicle = API.vehicleNameToModel(pullDatabase("PlayerVehicles", "VehicleType" + slot.ToString(), "Garage", player.name));
            float x = Convert.ToSingle(pullDatabase("PlayerVehicles", slot.ToString() + "PosX", "Garage", player.name));
            float y = Convert.ToSingle(pullDatabase("PlayerVehicles", slot.ToString() + "PosY", "Garage", player.name));
            float z = Convert.ToSingle(pullDatabase("PlayerVehicles", slot.ToString() + "PosZ", "Garage", player.name));
            Vector3 position = new Vector3(x, y, z);
            x = Convert.ToSingle(pullDatabase("PlayerVehicles", slot.ToString() + "RotX", "Garage", player.name));
            y = Convert.ToSingle(pullDatabase("PlayerVehicles", slot.ToString() + "RotY", "Garage", player.name));
            z = Convert.ToSingle(pullDatabase("PlayerVehicles", slot.ToString() + "RotZ", "Garage", player.name));
            Vector3 rotation = new Vector3(x, y, z);
            NetHandle veh = API.createVehicle(vehicle, position, rotation, 0, 0).handle;
            return veh;
        }

        public void updateVehiclePosition(Client player, int slot)
        {
            updateDatabase("PlayerVehicles", slot.ToString() + "PosX", player.vehicle.position.X.ToString(), "Garage", player.name);
            updateDatabase("PlayerVehicles", slot.ToString() + "PosY", player.vehicle.position.Y.ToString(), "Garage", player.name);
            updateDatabase("PlayerVehicles", slot.ToString() + "PosZ", player.vehicle.position.Z.ToString(), "Garage", player.name);
        }
    }
}
