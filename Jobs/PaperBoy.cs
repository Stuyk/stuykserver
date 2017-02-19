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
    public class PaperBoy : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        List<Vector3> paperboyRoute = new List<Vector3>();
        Vector3 startPoint;
        SphereColShape currentTask;
        Dictionary<Client, Vector3> routedPlayers = new Dictionary<Client, Vector3>();

        int playerLookup;

        public PaperBoy()
        {
            string query = "SELECT ID FROM PaperBoyPoints";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                foreach (DataColumn column in result.Columns)
                {
                    string selectedrow = row[column].ToString();
                    if (selectedrow == "1")
                    {
                        float posX = Convert.ToSingle(db.pullDatabase("PaperBoyPoints", "PosX", "ID", selectedrow));
                        float posY = Convert.ToSingle(db.pullDatabase("PaperBoyPoints", "PosY", "ID", selectedrow));
                        float posZ = Convert.ToSingle(db.pullDatabase("PaperBoyPoints", "PosZ", "ID", selectedrow));
                        startPoint = new Vector3(posX, posY, posZ);
                    }
                    else
                    {
                        float posX = Convert.ToSingle(db.pullDatabase("PaperBoyPoints", "PosX", "ID", selectedrow));
                        float posY = Convert.ToSingle(db.pullDatabase("PaperBoyPoints", "PosY", "ID", selectedrow));
                        float posZ = Convert.ToSingle(db.pullDatabase("PaperBoyPoints", "PosZ", "ID", selectedrow));
                        paperboyRoute.Add(new Vector3(posX, posY, posZ));
                    }
                }
            }

            API.onEntityEnterColShape += API_onEntityEnterColShape;
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle player)
        {
            if (currentTask == colshape)
            {
                API.sendNotificationToPlayer(API.getPlayerFromHandle(player), "Oh shit you in it now dog.");
            }
        }

        [Command("startpaper")]
        public void cmdStartPaper(Client player)
        {
            if (player.position.DistanceTo(startPoint) <= 5)
            {
                var bike = API.createVehicle(API.vehicleNameToModel("Fixter"), player.position, new Vector3(), 2, 2, 0);
                API.setPlayerIntoVehicle(player, bike, -1);
                Vector3 startPosition = new Vector3(paperboyRoute[0].X, paperboyRoute[0].Y, paperboyRoute[0].Z - 0.2);
                API.triggerClientEvent(player, "job_create_marker", startPosition);
                currentTask = API.createSphereColShape(paperboyRoute[0], 5f);
                routedPlayers.Add(player, player.position);
            }
            else if(db.isAdmin(player.name))
            {
                API.setEntityPosition(player, startPoint);
            }
        }

        [Command("createpaperpoint")]
        public void cmdCreatePaperPoint(Client player)
        {
            if (db.isAdmin(player.name))
            {
                db.insertDataPointPosition("PaperBoyPoints", player);
            }
        }
    }
}
