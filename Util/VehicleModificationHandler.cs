using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class VehicleModificationHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();

        List<Vector3> shopLocations = new List<Vector3>(); // Shop Locations
        List<ColShape> shopCollisions = new List<ColShape>(); // Shop Collisions for Use Functions
        List<Client> playersInCollisions = new List<Client>(); // Players in the Collisions.
        Dictionary<Client, Vector3> playersInShop = new Dictionary<Client, Vector3>();

        public VehicleModificationHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onResourceStop += API_onResourceStop;
        }

        private void API_onResourceStop()
        {
            foreach (Client p in playersInShop.Keys)
            {
                p.position = playersInShop[p];
                db.setPlayerPositionByVector(p, playersInShop[p]);
            }
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            if (playersInShop.ContainsKey(player))
            {
                player.position = playersInShop[player];
                db.setPlayerPositionByVector(player, playersInShop[player]);
                API.consoleOutput("{0} moved outside of shop due to disconnection.", player.name);
            }
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "pushVehicleChanges")
            {
                Vehicle playerVehicle = player.vehicle;
                API.setVehicleCustomPrimaryColor(playerVehicle, Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
                API.setVehicleCustomSecondaryColor(playerVehicle, Convert.ToInt32(args[3]), Convert.ToInt32(args[4]), Convert.ToInt32(args[5]));
                API.setVehicleMod(playerVehicle, 0, Convert.ToInt32(args[6]));
                API.setVehicleMod(playerVehicle, 1, Convert.ToInt32(args[7]));
                API.setVehicleMod(playerVehicle, 2, Convert.ToInt32(args[8]));
                API.setVehicleMod(playerVehicle, 3, Convert.ToInt32(args[9]));
                API.setVehicleMod(playerVehicle, 4, Convert.ToInt32(args[10]));
                API.setVehicleMod(playerVehicle, 6, Convert.ToInt32(args[11]));
                API.setVehicleMod(playerVehicle, 7, Convert.ToInt32(args[12]));
                API.setVehicleMod(playerVehicle, 8, Convert.ToInt32(args[13]));
                API.setVehicleMod(playerVehicle, 9, Convert.ToInt32(args[14]));
                API.setVehicleMod(playerVehicle, 10, Convert.ToInt32(args[15]));
                API.setVehicleMod(playerVehicle, 23, Convert.ToInt32(args[16]));
                API.setVehicleMod(playerVehicle, 24, Convert.ToInt32(args[17]));
                API.setVehicleMod(playerVehicle, 69, Convert.ToInt32(args[18]));
            }



            if (eventName == "leaveVehicleShop")
            {
                actionExitShop(player);
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                if (shopCollisions.Contains(colshape))
                {
                    if (playersInCollisions.Contains(API.getPlayerFromHandle(entity)))
                    {
                        playersInCollisions.Remove(API.getPlayerFromHandle(entity));
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "removeUseFunction");
                    }
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                if (shopCollisions.Contains(colshape))
                {
                    if (!playersInCollisions.Contains(API.getPlayerFromHandle(entity)))
                    {
                        if (API.isPlayerInAnyVehicle(API.getPlayerFromHandle(entity)))
                        {
                            playersInCollisions.Add(API.getPlayerFromHandle(entity));
                            API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "VehicleModificationShop");
                            API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), "This place seems to have a lot of car parts.");
                        }
                        
                    }
                }
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Vehicle Modification Handler");

            string query = "SELECT ID FROM VehicleModificationShops";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                foreach (DataColumn column in result.Columns)
                {
                    string selectedrow = row[column].ToString();

                    float posX = Convert.ToSingle(db.pullDatabase("VehicleModificationShops", "PosX", "ID", selectedrow));
                    float posY = Convert.ToSingle(db.pullDatabase("VehicleModificationShops", "PosY", "ID", selectedrow));
                    float posZ = Convert.ToSingle(db.pullDatabase("VehicleModificationShops", "PosZ", "ID", selectedrow));

                    positionBlips(new Vector3(posX, posY, posZ));

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Vehicle Mod Shops Initialized: " + initializedObjects.ToString());
        }

        public void positionBlips(Vector3 position)
        {
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 446);
            API.setBlipColor(newBlip, 63);
            shopLocations.Add(new Vector3(position.X, position.Y, position.Z));
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 5f, 5f);
            shopCollisions.Add(shape);
        }

        public void actionEnterShop(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (player.isInVehicle)
                {
                    if (!playersInShop.ContainsKey(player))
                    {
                        if (playersInCollisions.Contains(player))
                        {
                            db.setPlayerHUD(player, false);
                            playersInShop.Add(player, player.position);
                            Random rand = new Random();
                            int dimension = rand.Next(1, 1000);
                            NetHandle playerVehicle = API.getPlayerVehicle(player);
                            API.setEntityPosition(playerVehicle, new Vector3(-1156.071, -2005.241, 13.18026));
                            API.setEntityDimension(playerVehicle, dimension);
                            API.setEntityDimension(player, dimension);
                            API.setPlayerIntoVehicle(player, playerVehicle, -1);
                            player.vehicle.engineStatus = false;
                            API.triggerClientEvent(player, "createCamera", new Vector3(-1149.901, -2006.942, 14.14681), player.vehicle.position);
                            API.triggerClientEvent(player, "openCarPanel");
                        }
                    }
                    else
                    {
                        API.sendNotificationToPlayer(player, "~r~You already seem to be in the shop.");
                    }
                }
            }
        }

        public void actionExitShop(Client player)
        {
            API.setEntityDimension(player.vehicle, 0);
            API.setEntityDimension(player, 0);
            API.setEntityPosition(player.vehicle, playersInShop[player]);
            API.setEntityRotation(player.vehicle, new Vector3(player.rotation.X, player.rotation.Y, player.rotation.Z + 180));
            playersInShop.Remove(player);
            player.vehicle.engineStatus = true;
            API.triggerClientEvent(player, "endCamera");
            db.setPlayerHUD(player, true);
        }

        [Command("createvehiclemodshop")]
        public void cmdCreateVehicleModShop(Client player)
        {
            if (db.isAdmin(player.name))
            {
                db.insertDataPointPosition("VehicleModificationShops", player);
            }
        }

        [Command("gotoVehiclemodShop")]
        public void cmdGoToVehicleModShop(Client player)
        {
            if (db.isAdmin(player.name))
            {
                API.setEntityPosition(player, shopLocations[0]);
            }
        }
    }
}
