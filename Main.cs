using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace stuykserver
{
    public class Main : Script
    {
        DatabaseHandler db = new DatabaseHandler();
           
        // Class Pulls
        SpawnPoints spawnPoints = new SpawnPoints();

        // Prebuilt Messages
        public string msgPrefix = "~y~[~w~STUYK~y~]~w~ ";

        public Main()
        {
            API.onChatMessage += API_onChatMessage;
            API.onPlayerHealthChange += API_onPlayerHealthChange;
            API.onUpdate += API_onUpdate;
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Main");
        }

        private void API_onUpdate()
        {
        }

        private void API_onPlayerHealthChange(Client player, int oldValue)
        {
            db.updateDatabase("Players", "Health", player.health.ToString(), "Nametag", player.name);
        }

        public void API_onChatMessage(Client player, string message, CancelEventArgs cancel)
        {
            if (!isPlayerLoggedIn(player))
            {
                cancel.Cancel = true;
                return;
            }
        }

        [Command("getpos")] //Temporary
        public void cmdGetPos(Client player)
        {
            if (isPlayerLoggedIn(player))
            {
                string xyz = API.getEntityPosition(player).ToString();
                API.sendNotificationToPlayer(player, msgPrefix + xyz);
                return;
            }
            return;
        }

        [Command("nametag")] //Temporary
        public void nameTag(Client player, string name)
        {
            if (isPlayerLoggedIn(player))
            {
                if (isValidUsername(name))
                {
                    if (!db.usernameExists(name))
                    {
                        API.setPlayerNametag(player, name);
                        API.setPlayerName(player, name);
                        db.updateDatabase("Players", "Nametag", name, "Nametag", player.name);
                        sendNotification(player, msgPrefix + "Your name has been changed to:" + name);
                        return;
                    }
                    else
                    {
                        API.sendNotificationToPlayer(player, "~r~That username already exists.");
                        return;
                    } 
                }
                else
                {
                    API.sendNotificationToPlayer(player, "~r~Not a valid username format. EX: Johnny_Ringo");
                    return;
                }
            }
            return;
        }

        [Command("spawn")] //Temporary
        public void cmdSpawn(Client player)
        {
            API.setEntityPosition(player, spawnPoints.ServerSpawnPoints[0]);
            return;
        }

        [Command("inventory")] // Temporary?
        public void cmdInventory(Client player)
        {
            API.triggerClientEvent(player, "openInventory", player.name);
            return;
        }

        [Command("spectate")] // Temporary
        public void cmdSpectate(Client player, string target)
        {
            API.setPlayerToSpectatePlayer(player, API.getPlayerFromName(target));
        }

        [Command("stopspectate")]
        public void cmdStopSpectate(Client player)
        {
            API.setPlayerHealth(player, 0);
        }

        [Command("weapon")] // Temporary
        public void WeaponCommand(Client sender, WeaponHash hash)
        {
            API.givePlayerWeapon(sender, hash, 500, true, true);
        }

        // Used to check if the player is logged in.
        public bool isPlayerLoggedIn(Client player)
        {
            string loginPull = db.pullDatabase("Players", "LoggedIn", "Nametag", player.name);
            bool loginBool = Convert.ToBoolean(loginPull);
            if (loginBool == true)
            {
                return true;
            }
            else if (loginBool == false)
            {
                return false;
            }
            return false;
        }

        // Used to check if the string is a valid username.
        public bool isValidUsername(string input)
        {
            string pattern = "^(([A-Z][a-z]+)(([ _][A-Z][a-z]+)|([ _][A-z]+[ _][A-Z][a-z]+)))$";
            bool returnBool = Regex.IsMatch(input, pattern);
            return returnBool;
        }

        // Modified send notificaiton with clientside noise.
        public void sendNotification(Client player, string message)
        {
            API.playSoundFrontEnd(player, "Menu_Accept", "Phone_SoundSet_Default");
            API.sendNotificationToPlayer(player, message);
        }

    }
}
