using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace stuykserver.Jobs
{
    public class Fishing : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Util.Util util = new Util.Util();

        Dictionary<Client,string> playersFishing = new Dictionary<Client, string>();
        Dictionary<Client, NetHandle> playersFishingRods = new Dictionary<Client, NetHandle>();

        public Fishing()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if(eventName == "fishingFailed")
            {
                playerFishingFailed(player);
            }

            if (eventName == "pushWordToPanel")
            {
                API.triggerClientEvent(player, "fishingPushWord", playersFishing[player]);
            }

            if (eventName == "submitWord")
            {
                string playerWord = arguments[0].ToString();
                if (playersFishing.ContainsKey(player))
                {
                    if (playerWord.ToLower() == playersFishing[player].ToLower())
                    {
                        API.sendNotificationToPlayer(player, "~b~You reel in a fish ~g~flawlessly.");
                        playersFishing.Remove(player);
                        API.triggerClientEvent(player, "stopFishing");
                        API.playSoundFrontEnd(player, "Hack_Success", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                        API.deleteEntity(playersFishingRods[player]);
                        playersFishingRods.Remove(player);
                        API.triggerClientEvent(player, "killPanel");
                        API.stopPlayerAnimation(player);
                        API.stopPedAnimation(player);
                        playerAddFish(player);
                        return;
                    }
                    else
                    {
                        playerFishingFailed(player);
                        API.triggerClientEvent(player, "killPanel");
                        return;
                    }
                }

            }
        }

        public void sellFish(Client player)
        {
            int fish = Convert.ToInt32(db.pullDatabase("PlayerInventory", "Fish", "Nametag", player.name));
            if (fish <= 0)
            {
                API.sendNotificationToPlayer(player, "~r~You don't seem to have any fish.");
                return;
            }

            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance == null)
            {
                API.sendNotificationToPlayer(player, "~r~Something went wrong.");
                return;
            }

            instance.addPlayerCash(7 * fish);
            db.updateDatabase("PlayerInventory", "Fish", 0.ToString(), "Nametag", player.name);
            API.sendNotificationToPlayer(player, string.Format("~b~You sold ~g~{0} ~b~fish.", fish));
        }

        public void startFishing(Client player)
        {
            if (playersFishing.ContainsKey(player))
            {
                API.sendNotificationToPlayer(player, "~r~ You're already fishing.");
                return;
            }

            if (Convert.ToInt32(db.pullDatabase("PlayerInventory", "Fish", "Nametag", player.name)) >= 10)
            {
                API.sendNotificationToPlayer(player, "~r~ You have too many fish.");
                return;
            }

            string word = sendWordToPlayer();
            API.triggerClientEvent(player, "startFishing", word);
            playersFishing.Add(player, word);

            var fishingrod = API.createObject(-1910604593, new Vector3(), new Vector3());
            API.attachEntityToEntity(fishingrod, player, "SKEL_L_Hand", new Vector3(0.13f, 0.1f, 0.01f), new Vector3(180f, 90f, 70f));
            playersFishingRods.Add(player, fishingrod);

            API.playPlayerAnimation(player, 1, "amb@world_human_stand_fishing@idle_a", "idle_c");
        }

        public string sendWordToPlayer()
        {
            var lines = File.ReadAllLines("resources/stuykserver/Jobs/wordlist.txt");
            Random random = new Random();
            string word = lines[random.Next(0, lines.Length)];
            return word;
        }

        public void playerFishingFailed(Client player)
        {
            API.sendNotificationToPlayer(player, "~r~Failed, the fish got away!");
            playersFishing.Remove(player);
            API.triggerClientEvent(player, "stopFishing");
            API.playSoundFrontEnd(player, "Hack_Failed", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
            API.deleteEntity(playersFishingRods[player]);
            playersFishingRods.Remove(player);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
        }

        public void playerAddFish(Client player)
        {
            int oldFish = Convert.ToInt32(db.pullDatabase("PlayerInventory", "Fish", "Nametag", player.name));
            oldFish += 1;
            db.updateDatabase("PlayerInventory", "Fish", oldFish.ToString(), "Nametag", player.name);
            API.sendNotificationToPlayer(player, string.Format("~b~Your inventory contains ~g~{0}/10 ~b~Fish.", oldFish));
        }
    }
}
