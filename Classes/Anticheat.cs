using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace stuykserver.Classes
{
    public class Anticheat : Script
    {
        static string msgAntiCheat = "~y~Anticheat:";
        static string msgArmorHacks = string.Format("{0} ~o~Armor Hacks", msgAntiCheat);
        static string msgHealthHacks = string.Format("{0} ~o~Health Hacks", msgAntiCheat);
        static string msgSpeedHacks = string.Format("{0} ~o~Speed Hacks", msgAntiCheat);
        static string msgTeleportHacks = string.Format("{0} ~o~Teleport Hacks", msgAntiCheat);

        public Anticheat()
        {
            API.onResourceStart += API_onResourceStart;
            API.onPlayerHealthChange += API_onPlayerHealthChange;
            API.onPlayerArmorChange += API_onPlayerArmorChange;
            API.onPlayerExitVehicle += API_onPlayerExitVehicle;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "ServeCheetos")
            {
                API.kickPlayer(sender);
            }
        }

        private void API_onPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            API.setEntityData(player, "CHEAT_LAST_POS", player.position);
        }

        private void API_onPlayerArmorChange(Client player, int oldValue)
        {
            if (API.hasEntityData(player, "CHEAT_ARMOR"))
            {
                if (player.armor > Convert.ToInt32(API.getEntityData(player, "CHEAT_ARMOR")))
                {
                    API.setPlayerArmor(player, Convert.ToInt32(API.getEntityData(player, "CHEAT_ARMOR")));
                    return;
                }

                API.setEntityData(player, "CHEAT_ARMOR", player.armor);
                API.sendNativeToPlayer(player, (long)Hash.SET_ENTITY_INVINCIBLE, player, false);
            }
        }

        private void API_onPlayerHealthChange(Client player, int oldValue)
        {
            if (API.hasEntityData(player, "CHEAT_HEALTH"))
            {
                if (player.health > Convert.ToInt32(API.getEntityData(player, "CHEAT_HEALTH")))
                {
                    API.setPlayerHealth(player, Convert.ToInt32(API.getEntityData(player, "CHEAT_HEALTH")));
                    return;
                }

                API.setEntityData(player, "CHEAT_HEALTH", player.health);
                API.sendNativeToPlayer(player, (long)Hash.SET_ENTITY_INVINCIBLE, player, false);
            }
        }

        // When the resource starts, kick on the timer.
        private void API_onResourceStart()
        {
            Timer timer = new Timer();
            timer.Interval = 2000;
            timer.Enabled = true;
            timer.Elapsed += Timer_ScanAllPlayers;
        }

        private void Timer_ScanAllPlayers(object sender, ElapsedEventArgs e)
        {
            List<Client> players = API.getAllPlayers();
            foreach (Client player in players)
            {
                scanTeleportChanges(player);
                scanVelocity(player);
                scanModelChanges(player);
                
                if (player.isInVehicle)
                {
                    scanHighVehicleHealth(player);
                }
            }
        }

        // Vehicle High Health
        private void scanHighVehicleHealth(Client player)
        {
            if (player.vehicle.health > 1000)
            {
                API.kickPlayer(player);
            }
        }

        // Model Check
        private void scanModelChanges(Client player)
        {
            if (API.hasEntityData(player, "CHEAT_MODEL"))
            {
                if (player.model != Convert.ToInt32(API.getEntityData(player, "CHEAT_MODEL")))
                {
                    API.setPlayerSkin(player, (PedHash)Convert.ToInt32(API.getEntityData(player, "CHEAT_MODEL")));
                    API.kickPlayer(player);
                    return;
                }
            }
        }

        // Velocity Check
        private void scanVelocity(Client player)
        {
            if (player.isInVehicle)
            {
                Vector3 velocity = API.getPlayerVelocity(player);
                float squaredSpeed = Convert.ToSingle(Math.Sqrt((velocity.X * velocity.X) + (velocity.Y * velocity.Y) + (velocity.Z * velocity.Z)));

                if (squaredSpeed > API.getVehicleMaxSpeed((VehicleHash)player.vehicle.model) + 15)
                {
                    API.warpPlayerOutOfVehicle(player, player.vehicle);
                    return;
                }
            }

            if (!player.isInVehicle)
            {
                Vector3 velocity = API.getPlayerVelocity(player);

                if (velocity.X > 10f)
                {
                    Vector3 lastPos = API.getEntityData(player, "CHEAT_LAST_POS");
                    API.setEntityPosition(player, lastPos);
                    return;
                }

                if (velocity.Y > 10f)
                {
                    Vector3 lastPos = API.getEntityData(player, "CHEAT_LAST_POS");
                    API.setEntityPosition(player, lastPos);
                    return;
                }

                if (velocity.Z > 10f)
                {
                    Vector3 lastPos = API.getEntityData(player, "CHEAT_LAST_POS");
                    API.setEntityPosition(player, lastPos);
                    return;
                }
            }
        }

        private void scanTeleportChanges(Client player)
        {
            if (API.hasEntityData(player, "CHEAT_ALLOW_TELEPORT"))
            {
                if (!API.getEntityData(player, "CHEAT_ALLOW_TELEPORT"))
                {
                    if (!player.isInVehicle)
                    {
                        if (!API.hasEntityData(player, "CHEAT_LAST_POS"))
                        {
                            API.setEntityData(player, "CHEAT_LAST_POS", player.position);
                            return;
                        }

                        if (API.hasEntityData(player, "CHEAT_LAST_POS"))
                        {
                            Vector3 lastPos = API.getEntityData(player, "CHEAT_LAST_POS");
                            if (lastPos.DistanceTo2D(player.position) >= 50f)
                            {
                                API.setEntityPosition(player, lastPos);
                                return;
                            }

                            API.setEntityData(player, "CHEAT_LAST_POS", player.position);
                            return;
                        }
                    }
                }
                else
                {
                    API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", false);
                    API.setEntityData(player, "CHEAT_LAST_POS", player.position);
                }
            }
            
        }
    }
}
