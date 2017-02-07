using GTANetworkServer;
using GTANetworkShared;
using System;
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
            string returnString = queryResult.Rows[0][0].ToString();
            return returnString;
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
            string query = "SELECT Money FROM Players WHERE Nametag='" + player.name + "'";
            DataTable queryResult = API.exported.database.executeQueryWithResult(query);
            int returnInt = Convert.ToInt32(queryResult.Rows[0][0]);
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
    }
}
