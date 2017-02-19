using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class VehicleShopHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();
        List<Vector3> vehicleShops = new List<Vector3>();


        public VehicleShopHandler()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Vehicle Shop Handler");

            string query = "SELECT ID FROM VehicleShops";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                foreach (DataColumn column in result.Columns)
                {
                    string selectedrow = row[column].ToString();

                    float posX = Convert.ToSingle(db.pullDatabase("VehicleShops", "PosX", "ID", selectedrow));
                    float posY = Convert.ToSingle(db.pullDatabase("VehicleShops", "PosY", "ID", selectedrow));
                    float posZ = Convert.ToSingle(db.pullDatabase("VehicleShops", "PosZ", "ID", selectedrow));
                    float rotX = Convert.ToSingle(db.pullDatabase("VehicleShops", "RotX", "ID", selectedrow));
                    float rotY = Convert.ToSingle(db.pullDatabase("VehicleShops", "RotY", "ID", selectedrow));
                    float rotZ = Convert.ToSingle(db.pullDatabase("VehicleShops", "RotZ", "ID", selectedrow));

                    positionBlips(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ));

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Vehicle Shops Initialized: " + initializedObjects.ToString());
        }

        [Command("browse")]
        public void cmdSkin(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (!player.isInVehicle)
                {
                    foreach (Vector3 pos in vehicleShops)
                    {
                        if (player.position.DistanceTo(pos) <= 15)
                        {
                            API.sendNotificationToPlayer(player, "Dealership");
                            main.sendNotification(player, "Dealership");
                        }
                    }
                }
            }
            return;
        }

        [Command("createdealership")]
        public void cmdCreateDealership(Client player, string id)
        {
            if (db.isAdmin(player.name))
            {
                string checkID = db.pullDatabase("VehicleShops", "ID", "ID", id);

                if (checkID == null)
                {
                    string posX = player.position.X.ToString();
                    string posY = player.position.Y.ToString();
                    string posZ = player.position.Z.ToString();
                    string rotX = player.rotation.X.ToString();
                    string rotY = player.rotation.Y.ToString();
                    string rotZ = player.rotation.Z.ToString();

                    db.insertDatabase("VehicleShops", "ID", id);
                    db.updateDatabase("VehicleShops", "PosX", posX, "ID", id);
                    db.updateDatabase("VehicleShops", "PosY", posY, "ID", id);
                    db.updateDatabase("VehicleShops", "PosZ", posZ, "ID", id);
                    db.updateDatabase("VehicleShops", "RotX", rotX, "ID", id);
                    db.updateDatabase("VehicleShops", "RotY", rotY, "ID", id);
                    db.updateDatabase("VehicleShops", "RotZ", rotZ, "ID", id);

                    positionBlips(new Vector3(player.position.X, player.position.Y, player.position.Z), new Vector3(player.rotation.X, player.rotation.Y, player.rotation.Z));
                    main.sendNotification(player, "~g~A Dealership has been created.");
                    return;
                }
                else
                {
                    main.sendNotification(player, "~r~This ID already exists.");
                    return;
                }
            }
            else
            {
                API.sendNotificationToPlayer(player, "You are not an administrator.");
            }
        }

        public void positionBlips(Vector3 position, Vector3 rotation)
        {
            API.createTextLabel("~y~Usage: ~w~/browse", new Vector3(position.X, position.Y, position.Z), 20, 0.5f);
            API.createTextLabel("~w~Browse available vehicle models.", new Vector3(position.X, position.Y, position.Z - 0.4), 20, 0.5f);
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 225);
            API.setBlipColor(newBlip, 47);
            API.createMarker(1, new Vector3(position.X, position.Y, position.Z - 4), new Vector3(), new Vector3(), new Vector3(2, 2, 5), 255, 214, 151, 64, 0);
            vehicleShops.Add(new Vector3(position.X, position.Y, position.Z));
            int i = 0;
            ++i;
        }

    }
}
