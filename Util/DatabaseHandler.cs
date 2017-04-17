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

        public DataTable compileSelectQuery(string before, string[] vars, object[] data, string after = "")
        {
            int i = 0;
            string query;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            //Add the beginning of our query
            query = string.Format("{0}", before);

            //format and add our params
            foreach (string label in vars)
            {
                if (i == vars.Length - 1)
                {
                    query = string.Format("{0} {1}=@{1}", query, label);
                }
                else
                {
                    query = string.Format("{0} {1}=@{1} AND", query, label);
                }

                parameters.Add(string.Format("@{0}", label), data[i].ToString());
                ++i;
            }

            //Add anything after the data formatting
            query = string.Format("{0} {1}", query, after);

            //Execute it
            return API.exported.database.executePreparedQueryWithResult(query, parameters);
        }

        public void compileQuery(string before, string after, string[] vars, object[] data)
        {
            int i = 0;
            string query;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            
            //Add the beginning of our query
            query = string.Format("{0}", before);

            //format and add our params
            foreach (string label in vars)
            {
                if (i == vars.Length - 1)
                {
                    query = string.Format("{0} {1}=@{1}", query, label);
                }
                else
                {
                    query = string.Format("{0} {1}=@{1},", query, label);
                }

                parameters.Add(string.Format("@{0}", label), data[i].ToString());
                ++i;
            }

            //Add anything after the data formatting
            query = string.Format("{0} {1}", query, after);

            //Execute it
            API.exported.database.executePreparedQuery(query, parameters);
        }

        public void compileInsertQuery(string tableName, string[] vars, object[] data)
        {
            int i = 0;
            string query;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            //Add the beginning of our query
            query = string.Format("INSERT INTO {0} (", tableName);

            //format and add our params
            foreach (string label in vars)
            {
                if (i == vars.Length - 1)
                {
                    query = string.Format("{0} {1}) VALUES (", query, label);
                }
                else
                {
                    query = string.Format("{0} {1},", query, label);
                }

                parameters.Add(string.Format("@{0}", label), data[i].ToString());
                ++i;
            }

            i = 0;
            foreach (string label in vars)
            {
                if (i == vars.Length - 1)
                {
                    query = string.Format("{0} @{1})", query, label);
                }
                else
                {
                    query = string.Format("{0} @{1},", query, label);
                }
                ++i;
            }

            //Execute it
            API.exported.database.executePreparedQuery(query, parameters);
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: DatabaseHandler");
        }

        public void setPlayerHUD(Client player, bool setting)
        {
            API.sendNativeToPlayer(player, Hash.DISPLAY_HUD, setting);
            API.sendNativeToPlayer(player, Hash.DISPLAY_RADAR, setting);
        }

        // Just Set Player Position by Vector3
        public void setPlayerPositionByVector(Client player, Vector3 position)
        {
            string[] varNames = { "X", "Y", "Z" };
            string before = "UPDATE Players SET";
            object[] data = { position.X.ToString(), position.Y.ToString(), position.Z.ToString() };
            string after = string.Format("WHERE ID='{0}'", Convert.ToString(API.getEntityData(player, "PlayerID")));

            // Send all our data to generate the query and run it
            compileQuery(before, after, varNames, data);
        }

        // Return if player is logged in.
        public bool isPlayerLoggedIn(Client player)
        {
            int id = Convert.ToInt32(API.getEntityData(player, "PlayerID"));

            if (id > 0)
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


        public void insertPurchasedVehicle(Client player, Vehicle vehicle, string displayName)
        {
            Vector3 pos = API.getEntityPosition(vehicle);
            Vector3 rot = API.getEntityRotation(vehicle);
            string query = string.Format("INSERT INTO PlayerVehicles (Garage, PosX, PosY, PosZ, RotX, RotY, RotZ, VehicleType) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", player.name, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, displayName);
            API.exported.database.executeQueryWithResult(query);
        }
    }
}
