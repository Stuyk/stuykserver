using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using BCr = BCrypt.Net;

namespace stuykserver.Util
{
    public class LoginHandler : Script
    {
        Main main = new Main();
        DatabaseHandler db = new DatabaseHandler();

        public LoginHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "clientLogin")
            {
                cmdLogin(player, arguments[0].ToString(), arguments[1].ToString());
            }

            if (eventName == "clientRegistration")
            {
                cmdRegister(player, arguments[0].ToString(), arguments[1].ToString());
            }

            if (eventName == "localPullName")
            {
                API.triggerClientEvent(player, "updateNameVariable", player.name);
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: LoginHandler");
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            } 

            return sBuilder.ToString();
        }

        public void cmdLogin(Client player, string email, string password)
        {
            string hash;

            using (MD5 md5Hash = MD5.Create())
            {
                hash = GetMd5Hash(md5Hash, password);
            }

            string[] varNames = { "Email", "Password" };
            string before = "SELECT ID, LoggedIn FROM Players WHERE";
            object[] data = { email, hash };
            DataTable result = db.compileSelectQuery(before, varNames, data);

            if (result.Rows.Count != 1)
            {
                API.triggerClientEvent(player, "passwordDoesNotMatch");
                return;
            }

            if (Convert.ToBoolean(result.Rows[0]["LoggedIn"]))
            {
                API.triggerClientEvent(player, "alreadyLoggedIn");
                return;
            }

            API.setEntitySyncedData(player, "PlayerID", Convert.ToInt32(result.Rows[0]["ID"]));
            API.call("ConnectionHandler", "SpawnPlayer", player);
        }

        public void cmdRegister(Client player, string email, string password)
        {
            // Check if Email already exists in the database.
            string query = "SELECT Email FROM Players";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            bool exists = result.Select().ToList().Exists(row => row["Email"].ToString() == email);

            if (!exists)
            {
                if (!db.usernameExists(player.name.ToString()))
                {
                    var hash = BCr.BCrypt.HashPassword(password, BCr.BCrypt.GenerateSalt(12));
                    db.insertDatabase("Players", "Email", email);
                    db.updateDatabase("Players", "Nametag", player.name, "Email", email);
                    db.updateDatabase("Players", "Password", hash, "Nametag", player.name);
                    db.updateDatabase("Players", "IP", player.address, "Nametag", player.name);
                    db.updateDatabase("Players", "SocialClub", player.socialClubName, "Nametag", player.name);
                    db.insertDatabase("PlayerInventory", "Nametag", player.name);
                    db.insertDatabase("PlayerSkins", "Nametag", player.name);
                    db.insertDatabase("PlayerClothing", "Nametag", player.name);
                    // ...  Add More Here
                    API.triggerClientEvent(player, "registerSuccessful");
                }
                else
                {
                    // Feedback if username already exists.
                    API.sendNotificationToPlayer(player, "~r~Sorry, that username exists.");
                    return;
                }
            }
            else
            {
                // Feedback if email already exists.
                API.sendNotificationToPlayer(player, "~r~Sorry, that email exists.");
                return;
            }
            return;
        }
    }
}
