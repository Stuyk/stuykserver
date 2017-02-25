using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace stuykserver.Util
{
    public class DeathHandler : Script
    {
        ActiveShooter activeShooter = new ActiveShooter();
        KarmaHandler karmaHandler = new KarmaHandler();
        DatabaseHandler db = new DatabaseHandler();
        public List<Client> inLimbo = new List<Client>();
        Dictionary<Client, Vector3> lastPositions = new Dictionary<Client, Vector3>();

        public DeathHandler()
        {
            //API.onPlayerDeath += API_onPlayerDeath;
            API.onResourceStart += API_onResourceStart;
            //API.onPlayerRespawn += API_onPlayerRespawn;
        }

        private void API_onResourceStart()
        {
            //inLimbo.Add(API.getPlayerFromName("Example"));
        }

        /*
        [Command("killplayer")]
        public void cmdKill(Client player, string name)
        {
            if (db.isAdmin(player.name))
            {
                API.setPlayerHealth(API.getPlayerFromName(name), -1);
            }
        }

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        private void API_onPlayerRespawn(Client player)
        {
            

            inLimbo.Add(player);

            API.setEntityPosition(player, lastPositions.Get(player));
            API.sendNotificationToPlayer(player, "~r~You have died.");
            API.sendChatMessageToPlayer(player, "~g~/service EMS");
            API.sendChatMessageToPlayer(player, "~r~/tapout");

            API.freezePlayer(player, true);
            API.setEntityInvincible(player, true);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            API.playPlayerAnimation(player, (int)(AnimationFlags.StopOnLastFrame), "combat@death@from_writhe", "death_c");
            while (stopWatch.Elapsed.Seconds < 5)
            {

            }
            API.playPlayerAnimation(player, (int)(AnimationFlags.StopOnLastFrame), "combat@death@from_writhe", "death_c");
        }

        private void API_onPlayerDeath(Client player, GTANetworkShared.NetHandle entityKiller, int weapon)
        {
            db.updateDatabase("Players", "Health", "-1", "Nametag", player.name);
            db.updateDatabase("Players", "Armor", "-1", "Nametag", player.name);

            lastPositions.Add(player, player.position);
            db.updateDatabase("Players", "Dead", "1", "Nametag", player.name);

            if (!inLimbo.Contains(player))
            {
                Client killer = API.getPlayerFromHandle(entityKiller);
                if (killer != null)
                {
                    if (activeShooter.isActiveShooter(killer))
                    {
                        if (!activeShooter.isActiveShooter(player))
                        {
                            if (karmaHandler.checkKarma(killer, -100) && !karmaHandler.checkKarma(killer, 0))
                            {
                                karmaHandler.removeKarma(killer, 5);
                            }

                            if (karmaHandler.checkKarma(killer, 0) && !karmaHandler.checkKarma(killer, 100))
                            {
                                karmaHandler.removeKarma(killer, -5);
                            }
                        }
                    }
                }
            }
        }

        
        [Command("tapout")]
        public void cmdTapOut(Client player)
        {
            if (db.pullDatabase("Players", "Dead", "Nametag", player.name) == "1")
            {
                if (!inLimbo.Contains(player))
                {
                    inLimbo.Add(player);
                }
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                API.sendNotificationToPlayer(player, "You are being transported to a hospital. Approx: 30 Seconds.");
                bool fadeOut = false;
                while (stopWatch.Elapsed.Seconds < 30)
                {
                    if (!fadeOut)
                    {
                        if (stopWatch.Elapsed.Seconds > 5)
                        {
                            API.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_OUT, 10000);
                            fadeOut = true;
                        }  
                    }
                }
                stopWatch.Stop();
                queueForRespawn(player);
                return;
            }
        }

        public void queueForRespawn(Client player)
        {
            API.sendNativeToPlayer(player, Hash.RESET_PED_RAGDOLL_TIMER, player);

            Vector3 hospital = new Vector3(-449.8117, -340.2331, 34.50175);
            API.sendNativeToPlayer(player, Hash.DO_SCREEN_FADE_IN, 5000);
            API.setPlayerHealth(player, 100);
            API.setEntityPosition(player, hospital);
            API.stopPlayerAnimation(player);
            db.updateDatabase("Players", "Health", "100", "Nametag", player.name);
            API.sendNotificationToPlayer(player, "You were transported to a hospital in an unstable condition.");
            db.setPlayerMoney(player, -50);
            API.setPlayerHealth(player, 100);
            inLimbo.Remove(player);
            db.updateDatabase("Players", "Dead", "0", "Nametag", player.name);
            API.freezePlayer(player, false);
            API.setEntityInvincible(player, false);
            API.sendNativeToPlayer(player, Hash.SET_PED_GENERATES_DEAD_BODY_EVENTS, player, false);
            lastPositions.Remove(player);
        }
        */
    }
}
