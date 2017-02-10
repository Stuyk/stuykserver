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
            return;
        }
    }
}
