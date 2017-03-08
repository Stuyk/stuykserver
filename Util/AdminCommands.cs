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
        Util util = new Util();

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
            if (util.isAdmin(player))
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
            if (util.isAdmin(player))
            {
                API.setEntityPosition(player, new Vector3(-1537.53, -942.0224, 12));
            }
        }

        [Command("spectate")] // Admin Command
        public void cmdSpectate(Client player, string target)
        {
            if (util.isAdmin(player))
            {
                API.setPlayerToSpectatePlayer(player, API.getPlayerFromName(target));
                API.sendNotificationToPlayer(player, "You are now spectating " + target);
            }
        }

        [Command("stopspectate")] // Admin Command
        public void cmdStopSpectate(Client player)
        {
            if (util.isAdmin(player))
            {
                API.unspectatePlayer(player);
            }

        }

        [Command("giveweapon")] // Temporary
        public void WeaponCommand(Client player, WeaponHash hash)
        {
            if (util.isAdmin(player))
            {
                API.givePlayerWeapon(player, hash, 500, true, true);
            }
        }

        [Command("giveadminmoney")] // Admin Command
        public void cmdGiveMoney(Client player, int amount)
        {
            if (util.isAdmin(player))
            {
                db.setPlayerMoney(player, amount);
            }
        }

        [Command("adminkick")] // Kick a player.
        public void cmdKick(Client player, string target)
        {
            if (util.isAdmin(player))
            {
                API.kickPlayer(API.getPlayerFromName(target), "You were kicked by an admin.");
                API.sendNotificationToPlayer(player, target + " has been kicked.");
            }
        }

        [Command("adminban")]
        public void cmdBan(Client player, string target)
        {
            if (util.isAdmin(player))
            {
                API.banPlayer(API.getPlayerFromName(target));
                API.sendNotificationToPlayer(player, target + " has been banned.");
            }
        }

        [Command("mx")]
        public void cmdMX(Client player)
        {
            if (util.isAdmin(player))
            {
                API.setEntityPosition(player, new Vector3(player.position.X + 5, player.position.Y, player.position.Z));
            }
        }

        [Command("my")]
        public void cmdMY(Client player)
        {
            if (util.isAdmin(player))
            {
                API.setEntityPosition(player, new Vector3(player.position.X, player.position.Y + 5, player.position.Z));
            }
        }

        [Command("addKarma")]
        public void cmdAdminAddKarma(Client player, string target, int amount)
        {
            if (util.isAdmin(player))
            {
                API.call("KarmaHandler", "addKarma", API.getPlayerFromName(target), amount);
            }
        }

        [Command("removeKarma")]
        public void cmdAdminRemoveKarma(Client player, string target, int amount)
        {
            if (util.isAdmin(player))
            {
                API.call("KarmaHandler", "removeKarma", API.getPlayerFromName(target), amount);
            }
        }

        [Command("getKarma")]
        public void cmdAdminGetKarma(Client player, string target)
        {
            if (util.isAdmin(player))
            {
                API.sendNotificationToPlayer(player, string.Format("{0}'s Karma Is: {1}", target, API.getEntitySyncedData(API.getPlayerFromName(target), "Karma")));
            }
        }

        [Command("spawncar")]
        public void cmdSpawnCar(Client player, VehicleHash model)
        {
            if (util.isAdmin(player))
            {
                var rot = API.getEntityRotation(player.handle);
                var vehicle = API.createVehicle(model, player.position, new Vector3(0, 0, rot.Z), 0, 0);
                return;
            }
            return;
        }
    }
}
