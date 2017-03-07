using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

        public void cmdLogin(Client player, string email, string password)
        {
            string[] varNames = { "ID" };
            string before = "SELECT ID FROM Players WHERE";
            object[] data = { email, BCr.BCrypt.HashPassword(password) };
            string after = string.Format("")



            string query = string.Format("SELECT ID FROM Players WHERE Email='{0}' AND Password='{1}'", email, BCr.BCrypt.HashPassword(password));
            DataTable result = API.exported.database.executeQueryWithResult(query);

            if (result != null)
            {

            }




            if (player.name == db.pullDatabase("Players", "Nametag", "Email", email))
            {
                if (passwordHash != null)
                {
                    if (BCr.BCrypt.Verify(password, passwordHash))
                    {
                        if (!db.isPlayerLoggedIn(player))
                        {
                            API.call("ConnectionHandler", "SpawnPlayer", player);
                            int money = Convert.ToInt32(db.pullDatabase("Players", "Money", "Nametag", player.name));
                            API.triggerClientEvent(player, "update_money_display", money);
                            db.setPlayerLoggedIn(player);
                            return;
                        }
                        API.kickPlayer(player, "You are already logged in.");
                        return;
                    }
                    else
                    {
                        API.triggerClientEvent(player, "passwordDoesNotMatch");
                        return;
                    }
                }
                API.triggerClientEvent(player, "passwordDoesNotMatch");
                return;
            }
            API.triggerClientEvent(player, "doesNotMatchAccount");
            return;
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
