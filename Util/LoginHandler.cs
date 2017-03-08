using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
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

            API.setEntityData(player, "PlayerID", Convert.ToInt32(result.Rows[0]["ID"]));
            API.call("ConnectionHandler", "SpawnPlayer", player);
        }

        public void cmdRegister(Client player, string email, string password)
        {
            string hash;

            using (MD5 md5Hash = MD5.Create())
            {
                hash = GetMd5Hash(md5Hash, password);
            }

            string[] varNamesZero = { "Email" };
            string beforeZero = "SELECT Email FROM Players WHERE";
            object[] dataZero = { email };
            DataTable result = db.compileSelectQuery(beforeZero, varNamesZero, dataZero);

            if (result.Rows.Count >= 1)
            {
                // Email Exists
                API.sendNotificationToPlayer(player, "That email already exists.");
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
            API.triggerClientEvent(player, "registerSuccessful");
        }
    }
}
