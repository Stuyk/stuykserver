using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using GTANetworkServer;
using GTANetworkShared;

public class Database : Script
{

    /* Variables */
    string connStr;
    Dictionary<string, MySqlDataAdapter> dataAdapters;

    /* Constructor */

    public Database()
    {
        API.onResourceStart += onResourceStart;
        API.onResourceStop += onResourceStop;
    }

    /* Exports */

    public DataTable executeQueryWithResult(string sql)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();
                MySqlDataReader rdr = cmd.ExecuteReader();
                DataTable results = new DataTable();
                results.Load(rdr);
                rdr.Close();
                return results;
            }
            catch (Exception ex)
            {
                API.consoleOutput("DATABASE: [ERROR] " + ex.ToString());
                return null;
            }
        }
    }

    public DataTable executePreparedQueryWithResult(string sql, Dictionary<string, string> parameters)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();

                foreach (KeyValuePair<string, string> entry in parameters)
                {
                    cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                }

                MySqlDataReader rdr = cmd.ExecuteReader();
                DataTable results = new DataTable();
                results.Load(rdr);
                rdr.Close();
                return results;
            }
            catch (Exception ex)
            {
                API.consoleOutput("DATABASE: [ERROR] " + ex.ToString());
                return null;
            }
        }
    }

    public void executeQuery(string sql)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                API.consoleOutput("DATABASE: [ERROR] " + ex.ToString());
            }
        }
    }

    public void executePreparedQuery(string sql, Dictionary<string, string> parameters)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();
                foreach (KeyValuePair<string, string> entry in parameters)
                {
                    cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                API.consoleOutput("DATABASE: [ERROR] " + ex.ToString());
            }
        }
    }

    public DataTable createDataTable(string sql, string unique_name)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                MySqlDataAdapter dataAdapter;
                DataTable dataTable;
                dataAdapter = new MySqlDataAdapter(sql, conn);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(dataAdapter);
                dataAdapters[unique_name] = dataAdapter;
                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception ex)
            {
                API.consoleOutput("DATABASE: [ERROR] " + ex.ToString());
                return null;
            }
        }
    }

    public void updateDataTable(string unique_name, DataTable updatedTable)
    {
        try
        {
            dataAdapters[unique_name].Update(updatedTable);
        }
        catch (Exception ex)
        {
            API.consoleOutput("DATABASE: [ERROR] " + ex.ToString());
        }
    }

    public void closeDataTable(string unique_name)
    {
        try
        {
            MySqlDataAdapter data = dataAdapters[unique_name];
            dataAdapters.Remove(unique_name);
            data.Dispose();
        }
        catch (Exception ex)
        {
            API.consoleOutput("DATABASE: [ERROR] " + ex.ToString());
        }
    }

    /* Hooks */

    public void onResourceStart()
    {
        dataAdapters = new Dictionary<string, MySqlDataAdapter>();
        var authentication = "";
        if (API.getSetting<bool>("database_using_ssl"))
        {
            authentication = ";CertificateFile=" + API.getSetting<string>("database_ssl_pfx_file") +
                             ";CertificatePassword=" + API.getSetting<string>("database_ssl_pfx_password");
        }
        else
        {
            authentication = ";password=" + API.getSetting<string>("database_password");
        }
        connStr = "server=" + API.getSetting<string>("database_server") +
                  ";user=" + API.getSetting<string>("database_user") +
                  ";database=" + API.getSetting<string>("database_database") +
                  ";port=" + API.getSetting<string>("database_port") +
                  authentication +
                  ";min pool size=" + API.getSetting<string>("database_min_pool_size") +
                  ";max pool size=" + API.getSetting<int>("database_max_pool_size") + ";";
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            try
            {
                API.consoleOutput("DATABASE: [INFO] Attempting connecting to MySQL");
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    API.consoleOutput("DATABASE: [INFO] Connected to MySQL");
                }
            }
            catch (Exception ex)
            {
                API.consoleOutput("DATABASE: [ERROR] " + ex.ToString());
            }

        }
    }

    public void onResourceStop()
    {
        API.consoleOutput("DATABASE: [INFO] MySQL connection closed");
    }
}

