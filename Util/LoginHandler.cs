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

            // Set all logins to "0".
            string[] varNames = { "LoggedIn" };
            string before = "UPDATE Players SET";
            object[] data = { "0" };
            string after = string.Format("");
            db.compileQuery(before, after, varNames, data);
        }

        public void cmdLogin(Client player, string email, string password)
        {
            string[] varNames = { "Email" };
            string before = "SELECT ID, Password FROM Players WHERE";
            object[] data = { email };
            DataTable result = db.compileSelectQuery(before, varNames, data);

            if (result.Rows.Count < 1)
            {
                API.triggerClientEvent(player, "passwordDoesNotMatch");
                return;
            }

            bool verify = BCr.BCrypt.Verify(password, Convert.ToString(result.Rows[0]["Password"]));
            if (!verify)
            {
                API.triggerClientEvent(player, "passwordDoesNotMatch");
                return;
            }

            API.setEntityData(player, "PlayerID", Convert.ToInt32(result.Rows[0]["ID"]));
            API.call("ConnectionHandler", "SpawnPlayer", player);
        }

        public void cmdRegister(Client player, string email, string password)
        {
            var hash = BCr.BCrypt.HashPassword(password, BCr.BCrypt.GenerateSalt(12));

            string[] varNamesZero = { "Email" };
            string beforeZero = "SELECT Email FROM Players WHERE";
            object[] dataZero = { email };
            DataTable result = db.compileSelectQuery(beforeZero, varNamesZero, dataZero);

            if (result.Rows.Count >= 1)
            {
                // Email Exists
                API.sendNotificationToPlayer(player, "That username already exists.");
                return;
            }

            string[] varNamesOne = { "Nametag" };
            string beforeOne = "SELECT Nametag FROM Players WHERE";
            object[] dataOne = { player.name };
            result = db.compileSelectQuery(beforeOne, varNamesOne, dataOne);

            if (result.Rows.Count >= 1)
            {
                // Nametag Exists
                API.sendNotificationToPlayer(player, "That nametag already exists.");
                return;
            }

            DateTime date = DateTime.Now;

            string[] varNamesTwo = { "Email", "Nametag", "Password", "SocialClub", "IP", "Health", "Armor", "RegisterDate" };
            string tableName = "Players";
            string[] dataTwo = { email, player.name, hash, player.socialClubName, player.address, "100", "0", date.ToString("yyyy-MM-dd HH:mm:ss") };
            db.compileInsertQuery(tableName, varNamesTwo, dataTwo);

            result = API.exported.database.executeQueryWithResult("SELECT ID FROM Players ORDER BY ID DESC LIMIT 1");
            string playerID = result.Rows[0]["ID"].ToString();

            string[] varNamesThree = { "PlayerID", "Nametag" };
            tableName = "PlayerInventory";
            string[] dataThree = { playerID, player.name };
            db.compileInsertQuery(tableName, varNamesThree, dataThree);

            tableName = "PlayerSkins";
            db.compileInsertQuery(tableName, varNamesThree, dataThree);

            tableName = "PlayerClothing";
            db.compileInsertQuery(tableName, varNamesThree, dataThree);

            API.consoleOutput("Registered {0} at {1}.", player.name, playerID);
            API.setEntityData(player, "FirstTimeLogin", true);
            cmdLogin(player, email, password);
        }
    }
}
