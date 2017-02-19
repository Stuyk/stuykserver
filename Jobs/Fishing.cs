using GTANetworkServer;
using GTANetworkShared;
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
        SpawnPoints spawnPoints = new SpawnPoints();
        DatabaseHandler db = new DatabaseHandler();
        List<Vector3> fishingPoints = new List<Vector3>();
        Dictionary<Client,string> playersFishing = new Dictionary<Client, string>();
        Dictionary<Client, NetHandle> playersFishingRods = new Dictionary<Client, NetHandle>();

        public Fishing()
        {
            API.onResourceStart += API_onResourceStart;
            API.onChatMessage += API_onChatMessage;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if(eventName == "fishingFailed")
            {
                playerFishingFailed(player);
            }
        }

        private void API_onChatMessage(Client player, string message, CancelEventArgs e)
        {
            if (playersFishing.ContainsKey(player))
            {
                if (message == playersFishing[player])
                {
                    API.sendNotificationToPlayer(player, "Correct!");
                    playersFishing.Remove(player);
                    API.triggerClientEvent(player, "stopFishing");
                    API.playSoundFrontEnd(player, "Hack_Success", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                    API.deleteEntity(playersFishingRods[player]);
                    playersFishingRods.Remove(player);
                    API.stopPlayerAnimation(player);
                    API.stopPedAnimation(player);
                    e.Cancel = true;
                    return;
                }
                playerFishingFailed(player);
                e.Cancel = true;
                return;
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Fishing Handler");

            string query = "SELECT ID FROM Fishing";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                foreach (DataColumn column in result.Columns)
                {
                    string selectedrow = row[column].ToString();

                    float posX = Convert.ToSingle(db.pullDatabase("Fishing", "PosX", "ID", selectedrow));
                    float posY = Convert.ToSingle(db.pullDatabase("Fishing", "PosY", "ID", selectedrow));
                    float posZ = Convert.ToSingle(db.pullDatabase("Fishing", "PosZ", "ID", selectedrow));

                    positionBlips(new Vector3(posX, posY, posZ), new Vector3());

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Fishing Points Intialized: " + initializedObjects.ToString());
        }

        [Command("createfishingpoint")]
        public void cmdCreateFishingPoint(Client player, string id)
        {
            if (db.isAdmin(player.name))
            {
                db.insertDatabase("Fishing", "ID", id);
                db.updateDatabase("Fishing", "PosX", player.position.X.ToString(), "ID", id);
                db.updateDatabase("Fishing", "PosY", player.position.Y.ToString(), "ID", id);
                db.updateDatabase("Fishing", "PosZ", player.position.Z.ToString(), "ID", id);

                positionBlips(new Vector3(player.position.X, player.position.Y, player.position.Z), new Vector3());
                API.sendNotificationToPlayer(player, "~g~A fishing point has been created.");
                return;
            }
        }

        [Command("fish")]
        public void cmdFish(Client player)
        {
            if (!playersFishing.ContainsKey(player))
            {
                foreach (Vector3 point in fishingPoints)
                {
                    if (player.position.DistanceTo(point) <= 20)
                    {
                        string word = sendWordToPlayer();
                        API.triggerClientEvent(player, "startFishing", word);
                        playersFishing.Add(player, word);
                        var fishingrod = API.createObject(-1910604593, new Vector3(), new Vector3());
                        API.attachEntityToEntity(fishingrod, player, "SKEL_L_Hand", new Vector3(0.13f, 0.1f, 0.01f), new Vector3(180f, 90f, 70f));
                        playersFishingRods.Add(player, fishingrod);
                        API.playPlayerAnimation(player, 1, "amb@world_human_stand_fishing@idle_a", "idle_c");
                        return;
                    }
                }
            }
            else
            {
                API.sendNotificationToPlayer(player, "You seem to already be fishing.");
            }
        }

        public void beginFishing(Client player)
        {
            
            return;
        }

        public void positionBlips(Vector3 position, Vector3 rotation)
        {
            API.createTextLabel("~y~Usage: ~w~/fish", new Vector3(position.X, position.Y, position.Z), 20, 0.5f);
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 68);
            API.setBlipColor(newBlip, 63);
            fishingPoints.Add(new Vector3(position.X, position.Y, position.Z));
            int i = 0;
            ++i;
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
        }
    }
}
