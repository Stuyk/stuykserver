using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class AdminCommands : Script
    {
        DatabaseHandler db = new DatabaseHandler();

        public AdminCommands()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Admin Commands");
        }

        [Command("stopserver")]
        public void cmdStopServer(Client player)
        {
            if (db.isAdmin(player.name))
            {
                List<Client> players = API.getAllPlayers();
                foreach (Client p in players)
                {
                    API.kickPlayer(p, "Restarting server.");
                }
            }
        }

        [Command("spawn")] //Temporary
        public void cmdSpawn(Client player)
        {
            if (db.isAdmin(player.name))
            {
                API.setEntityPosition(player, new Vector3(-1537.53, -942.0224, 12));
            }
        }

        [Command("spectate")] // Admin Command
        public void cmdSpectate(Client player, string target)
        {
            if (db.isAdmin(player.name))
            {
                API.setPlayerToSpectatePlayer(player, API.getPlayerFromName(target));
                API.sendNotificationToPlayer(player, "You are now spectating " + target);
            }
        }

        [Command("stopspectate")] // Admin Command
        public void cmdStopSpectate(Client player)
        {
            if (db.isAdmin(player.name))
            {
                API.unspectatePlayer(player);
            }

        }

        [Command("giveweapon")] // Temporary
        public void WeaponCommand(Client player, WeaponHash hash)
        {
            if (db.isAdmin(player.name))
            {
                API.givePlayerWeapon(player, hash, 500, true, true);
            }
        }

        [Command("giveadminmoney")] // Admin Command
        public void cmdGiveMoney(Client player, int amount)
        {
            if (db.isAdmin(player.name))
            {
                db.setPlayerMoney(player, amount);
            }
        }

        [Command("adminkick")] // Kick a player.
        public void cmdKick(Client player, string target)
        {
            if (db.isAdmin(player.name))
            {
                API.kickPlayer(API.getPlayerFromName(target), "You were kicked by an admin.");
                API.sendNotificationToPlayer(player, target + " has been kicked.");
            }
        }

        [Command("adminban")]
        public void cmdBan(Client player, string target)
        {
            if (db.isAdmin(player.name))
            {
                API.banPlayer(API.getPlayerFromName(target));
                API.sendNotificationToPlayer(player, target + " has been banned.");
            }
        }

        [Command("mx")]
        public void cmdMX(Client player)
        {
            if (db.isAdmin(player.name))
            {
                API.setEntityPosition(player, new Vector3(player.position.X + 5, player.position.Y, player.position.Z));
            }
        }

        [Command("my")]
        public void cmdMY(Client player)
        {
            if (db.isAdmin(player.name))
            {
                API.setEntityPosition(player, new Vector3(player.position.X, player.position.Y + 5, player.position.Z));
            }
        }
    }
}
