using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using BCr = BCrypt.Net;

namespace stuykserver.Util
{
    public class LoginHandler : Script
    {
        Main main = new Main();
        DatabaseHandler db = new DatabaseHandler();
        SpawnPoints sp = new SpawnPoints();

        public LoginHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "clientLogin")
            {
                API.sendNotificationToPlayer(player, "~g~Attempting login...");
                cmdLogin(player, arguments[0].ToString(), arguments[1].ToString());
            }

            if (eventName == "clientRegistration")
            {
                API.sendNotificationToPlayer(player, "~g~Attempting registration...");
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

        public void cmdLogin(Client player, string email, string password)
        {
            API.triggerClientEvent(player, "killPanel");
            API.triggerClientEvent(player, "loadLogin");

            if (main.isPlayerLoggedIn(player))
            {
                API.kickPlayer(player, main.msgPrefix + "You are already logged in.");
                return;
            }

            // Pull Password Information
            string passwordHash = db.pullDatabase("Players", "Password", "Nametag", player.name);

            if (passwordHash == null)
            {
                API.sendNotificationToPlayer(player, "~r~Oops, that account doesn't exist.");
                return;
            }
            bool isPasswordCorrect = BCr.BCrypt.Verify(password, passwordHash);

            // Check for Password Correctness.
            if (isPasswordCorrect)
            {
                API.call("ConnectionHandler", "SpawnPlayer", player);
                int money = Convert.ToInt32(db.pullDatabase("Players", "Money", "Nametag", player.name));
                API.triggerClientEvent(player, "update_money_display", money);
                db.updateDatabase("Players", "LoggedIn", "1", "Nametag", player.name);
                return;
            }
            else
            {
                API.sendNotificationToPlayer(player, "~r~Wrong password.");
                API.kickPlayer(player, "~r~Incorrect password");
                return;
            }
        }

        public void cmdRegister(Client player, string email, string password)
        {
            // Check if Email already exists in the database.
            string query = "SELECT Email FROM Players";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            bool exists = result.Select().ToList().Exists(row => row["Email"].ToString() == email);

            if (!exists)
            {
                // Check if UserName already exists in the database.
                if (!db.usernameExists(player.name.ToString()))
                {
                    // Register player.
                    var hash = BCr.BCrypt.HashPassword(password, BCr.BCrypt.GenerateSalt(12));
                    // First create the player.
                    db.insertDatabase("Players", "Email", email);
                    // Assign the Nametag.
                    db.updateDatabase("Players", "Nametag", player.name, "Email", email);
                    // Assign rest of values by Nametag.
                    db.updateDatabase("Players", "Password", hash, "Nametag", player.name);
                    db.updateDatabase("Players", "IP", player.address, "Nametag", player.name);
                    db.updateDatabase("Players", "SocialClub", player.socialClubName, "Nametag", player.name);
                    db.updateDatabase("Players", "LASTX", sp.ServerSpawnPoints[0].X.ToString(), "Nametag", player.name);
                    db.updateDatabase("Players", "LASTY", sp.ServerSpawnPoints[0].Y.ToString(), "Nametag", player.name);
                    db.updateDatabase("Players", "LASTZ", sp.ServerSpawnPoints[0].Z.ToString(), "Nametag", player.name);
                    db.updateDatabase("Players", "CurrentSkin", "BevHills01AMY", "Nametag", player.name);
                    db.updateDatabase("Players", "Health", "100", "Nametag", player.name);
                    db.updateDatabase("Players", "Armor", "0", "Nametag", player.name);
                    db.updateDatabase("Players", "Money", "150", "Nametag", player.name);
                    db.updateDatabase("Players", "JobStarted", "False", "Nametag", player.name);
                    db.updateDatabase("Players", "JobX", "0", "Nametag", player.name);
                    db.updateDatabase("Players", "JobY", "0", "Nametag", player.name);
                    db.updateDatabase("Players", "JobZ", "0", "Nametag", player.name);
                    db.updateDatabase("Players", "JobType", "None", "Nametag", player.name);
                    db.updateDatabase("Players", "TempJobVehicle", "None", "Nametag", player.name);
                    db.insertDatabase("PlayerVehicles", "Garage", player.name);
                    db.insertDatabase("PlayerInventory", "Nametag", player.name);
                    // ...  Add More Here
                    API.triggerClientEvent(player, "registerSuccessful");
                    API.sendNotificationToPlayer(player, "~g~Registration complete.");
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
