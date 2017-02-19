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
        SpawnPoints spawnPoints = new SpawnPoints();

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
            int i = API.exported.doormanager.registerDoor(1780022985, new Vector3(-1201.435, -776.8566, 17.99184));
            API.exported.doormanager.setDoorState(i, true, 1);
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
            if (!db.isPlayerLoggedIn(player))
            {
                cancel.Cancel = true;
                return;
            }
        }

        [Command("resetdimension")]
        public void cmdResetDimension(Client player)
        {
            player.dimension = 0;
        }

        [Command("goto")]
        public void cmdGoTo(Client player, string target)
        {
            player.position = API.getPlayerFromName(target).position;
        }

        [Command("getpos")] //Temporary
        public void cmdGetPos(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                string xyz = API.getEntityPosition(player).ToString();
                string rot = API.getEntityRotation(player).ToString();
                API.sendNotificationToPlayer(player, "POS: " + xyz);
                API.sendNotificationToPlayer(player, "ROT: " + rot);
                return;
            }
            return;
        }

        [Command("nametag")] //Temporary
        public void nameTag(Client player, string name)
        {
            if (db.isPlayerLoggedIn(player))
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

        [Command("inventory")]
        public void cmdInventory(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                API.triggerClientEvent(player, "openInventory", player.name);
                return;
            }
        }

        // Used to check if the player is logged in.
        

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
