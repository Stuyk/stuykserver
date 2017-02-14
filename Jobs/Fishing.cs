using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Jobs
{
    public class Fishing : Script
    {
        Main main = new Main();
        SpawnPoints spawnPoints = new SpawnPoints();
        DatabaseHandler db = new DatabaseHandler();
        List<Vector3> fishingPoints = new List<Vector3>();

        public Fishing()
        {
            API.onResourceStart += API_onResourceStart;
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
            foreach (Vector3 point in fishingPoints)
            {
                if (player.position.DistanceTo(point) <= 20)
                {
                    
                    return;
                }
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
    }
}
