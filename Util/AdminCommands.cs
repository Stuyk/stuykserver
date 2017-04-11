using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

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
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                List<Client> players = API.getAllPlayers();
                foreach (Client p in players)
                {
                    API.kickPlayer(p, "Restarting server.");
                }
            }
        }

        [Command("dropcamera")]
        public void cmdDropCamera(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                API.triggerClientEvent(player, "createEntityCamera", player.position, player);
            }
        }

        [Command("setcamera")]
        public void cmdSetCamera(Client player, double amount)
        {
            if (amount == 0)
            {
                return;
            }

            API.triggerClientEvent(player, "createCameraAtHeadHeight", new Vector3(player.position.X, player.position.Y, player.position.Z + amount), player.rotation);
        }

        [Command("killcamera")]
        public void cmdKillCamera(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                API.triggerClientEvent(player, "endCamera");
            }
        }

        [Command("setautorunposition")]
        public void cmdAutoRunPosition(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                API.setEntityData(player, "AutoRunPos", player.position);
            }
        }

        [Command("runtoposition")]
        public void cmdRunToPosition(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                Vector3 pos = API.getEntityData(player, "AutoRunPos");
                API.sendNativeToPlayer(player, (ulong)Hash.TASK_GO_STRAIGHT_TO_COORD, player, pos.X, pos.Y, pos.Z, 10f, -1, 0f, 0f);
            }
        }

        [Command("killtask")]
        public void cmdKillTask(Client player)
        {
            API.sendNativeToPlayer(player, (ulong)Hash.CLEAR_PED_TASKS_IMMEDIATELY, player);
        }

        [Command("spawn")] //Temporary
        public void cmdSpawn(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
                API.setEntityPosition(player, new Vector3(649.5031, -10.4181, 82.7862));
            }
        }

        [Command("spectate")] // Admin Command
        public void cmdSpectate(Client player, string target)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
                API.setPlayerToSpectatePlayer(player, API.getPlayerFromName(target));
                API.sendNotificationToPlayer(player, "You are now spectating " + target);
            }
        }

        [Command("stopspectate")] // Admin Command
        public void cmdStopSpectate(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
                API.unspectatePlayer(player);
            }

        }

        [Command("giveweapon")] // Temporary
        public void WeaponCommand(Client player, WeaponHash hash)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                API.givePlayerWeapon(player, hash, 500, true, true);
            }
        }

        [Command("getweapon")] // Temporary
        public void WeaponCommandGet(Client player, WeaponHash hash)
        {
            Player instance = (Player)player.getData("Instance");
            if (instance.isAdmin())
            {
                API.givePlayerWeapon(player, hash, 500, true, true);
            }
        }

        [Command("giveadminmoney")] // Admin Command
        public void cmdGiveMoney(Client player, int amount)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                instance.addPlayerCash(amount);
            }
        }

        [Command("adminkick")] // Kick a player.
        public void cmdKick(Client player, string target)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                API.kickPlayer(API.getPlayerFromName(target), "You were kicked by an admin.");
                API.sendNotificationToPlayer(player, target + " has been kicked.");
            }
        }

        [Command("adminban")]
        public void cmdBan(Client player, string target)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                API.banPlayer(API.getPlayerFromName(target));
                API.sendNotificationToPlayer(player, target + " has been banned.");
            }
        }

        [Command("mx")]
        public void cmdMX(Client player)
        {
            Player instance = (Player)player.getData("Instance");
            if (instance.isAdmin())
            {
                API.setEntityPosition(player, new Vector3(player.position.X + 5, player.position.Y, player.position.Z));
            }
        }

        [Command("my")]
        public void cmdMY(Client player)
        {
            Player instance = (Player)player.getData("Instance");
            if (instance.isAdmin())
            {
                API.setEntityPosition(player, new Vector3(player.position.X, player.position.Y + 5, player.position.Z));
            }
        }

        [Command("addKarma")] // Only for Self
        public void cmdAdminAddKarma(Client player, int amount)
        {
            Player instance = (Player)player.getData("Instance");
            if (instance.isAdmin())
            {
                instance.addPlayerKarma(amount);
            }
        }

        [Command("removeKarma")] // Only for Self
        public void cmdAdminRemoveKarma(Client player, int amount)
        {
            Player instance = (Player)player.getData("Instance");
            if (instance.isAdmin())
            {
                instance.removePlayerKarma(amount);
            }
        }

        [Command("getKarma")] // Only for Self
        public void cmdAdminGetKarma(Client player)
        {
            Player instance = (Player)player.getData("Instance");
            if (instance.isAdmin())
            {
                API.sendNotificationToPlayer(player, string.Format("{0}'s Karma Is: {1}", player.name, instance.returnPlayerKarma()));
            }
        }

        [Command("spawncar")]
        public void cmdSpawnCar(Client player, VehicleHash model)
        {
            Player instance = (Player)player.getData("Instance");
            if (instance.isAdmin())
            {
                var rot = API.getEntityRotation(player.handle);
                var vehicle = API.createVehicle(model, player.position, new Vector3(0, 0, rot.Z), 0, 0);
            }
        }

        [Command("coords")]
        public void cmdCoords(Client player, float x=0, float y=0, float z=0)
        {
            Player instance = (Player)player.getData("Instance");
            if (instance.isAdmin())
            {
                Vector3 pos = new Vector3(x, y, z);
                API.setEntityPosition(player, pos);
                API.sendChatMessageToPlayer(player, "~g~SSSSCCHHHWWWIIIIIING!");
                return;
            }
        }

        [Command("wandervehicle")]
        public void cmdCommandWander(Client player)
        {
            Player instance = (Player)player.getData("Instance");
            if (instance.isAdmin())
            {
                API.sendNativeToPlayer(player, (ulong)Hash.TASK_VEHICLE_DRIVE_WANDER, player, player.vehicle, 40f, 1074528293);
            }
        }
    }
}
