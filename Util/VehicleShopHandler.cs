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
        VehicleHandler vehicleHandler = new VehicleHandler();
        Main main = new Main();
        List<Vector3> vehicleShops = new List<Vector3>();
        List<Client> playersInCollisions = new List<Client>();
        Dictionary<ColShape, string> collisionShopType = new Dictionary<ColShape, string>();
        Dictionary<ColShape, int> collisionID = new Dictionary<ColShape, int>();
        Dictionary<ColShape, Vector3> collisionShapes = new Dictionary<ColShape, Vector3>();
        Dictionary<Client, Vector3> playersInShop = new Dictionary<Client, Vector3>();

        public VehicleShopHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "dealershipReady")
            {
                API.triggerClientEvent(player, "createCamera", new Vector3(230.8, -990.1201, -99), player.position);
            }

            if (eventName == "leaveDealership")
            {
                leaveDealership(player);
                API.triggerClientEvent(player, "endCamera");
            }

            if (eventName == "purchaseVehicle")
            {
                purchaseDealershipVehicle(player, arguments[0].ToString());
                API.triggerClientEvent(player, "endCamera");
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                if (collisionShapes.ContainsKey(colshape))
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
                if (collisionShapes.ContainsKey(colshape))
                {
                    if (!playersInCollisions.Contains(API.getPlayerFromHandle(entity)))
                    {
                        if (!API.isPlayerInAnyVehicle(API.getPlayerFromHandle(entity)))
                        {
                            playersInCollisions.Add(API.getPlayerFromHandle(entity));
                            API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "Dealership");

                            if (collisionShopType.ContainsKey(colshape))
                            {
                                string type = collisionShopType[colshape];
                                API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), string.Format("This shop carries the type: {0}", type));
                            }
                        }
                    }
                }
            }
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
                    string type = db.pullDatabase("VehicleShops", "Type", "ID", selectedrow);
                    int id = Convert.ToInt32(row[column]);

                    positionBlips(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), type, id);

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Vehicle Shops Initialized: " + initializedObjects.ToString());
        }

        public void browseDealership(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (!player.isInVehicle)
                {
                    if (playersInCollisions.Contains(player))
                    {
                        if (!playersInShop.ContainsKey(player))
                        {
                            foreach (ColShape collision in collisionShopType.Keys)
                            {
                                if (player.position.DistanceTo(collisionShapes[collision]) <= 3)
                                {
                                    Random rand = new Random();
                                    int dimension = rand.Next(1, 1000);
                                    playersInShop.Add(player, player.position);
                                    API.setEntityDimension(player, dimension);

                                    string type = collisionShopType[collision];

                                    db.setPlayerHUD(player, false);
                                    API.triggerClientEvent(player, "startBrowsing", type);
                                    API.setEntityPosition(player, new Vector3(225.6238, -990, -98.99996));
                                    break;
                                }
                            }
                        }
                    }  
                }
            }
            return;
        }

        public void purchaseDealershipVehicle(Client player, string vehicleType)
        {
            db.setPlayerHUD(player, true);
            API.setEntityDimension(player, 0);
            API.setEntityPosition(player, playersInShop[player]);
            playersInShop.Remove(player);
            foreach (ColShape collision in collisionShopType.Keys)
            {
                if (player.position.DistanceTo(collisionShapes[collision]) <= 10)
                {
                    string exitPoint = db.pullDatabase("VehicleShops", "ExitPoint", "ID", collisionID[collision].ToString());
                    Vector3 newExitPoint = db.convertStringToVector3(exitPoint);
                    API.call("VehicleHandler", "actionSetupPurchasedCar", newExitPoint, API.vehicleNameToModel(vehicleType), player);
                    return;
                }
            }
        }

        public void leaveDealership(Client player)
        {
            db.setPlayerHUD(player, true);
            API.setEntityDimension(player, 0);
            API.setEntityPosition(player, playersInShop[player]);
            playersInShop.Remove(player);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
        }

        [Command("setupdealership")] // Set a dealership type.
        public void cmdSetDealership(Client player, string type)
        {
            if (db.isAdmin(player.name))
            {
                db.insertDataPointPosition("VehicleShops", player);

                string query = "SELECT ID FROM VehicleShops";
                DataTable result = API.exported.database.executeQueryWithResult(query);
                string id = result.Rows.Count.ToString();

                API.sendNotificationToPlayer(player, string.Format("The current ID is: {0}", id));
                switch (type)
                {
                    case "Boats":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Commercial":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Compacts":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Coupes":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Bicycles":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Police":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Helicopters":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Industrial":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Motorcycles":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Muscle":
                        actionSetupDealershipType(id, type);
                        break;
                    case "OffRoad":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Planes":
                        actionSetupDealershipType(id, type);
                        break;
                    case "SUVS":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Sedans":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Sports":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Classic":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Super":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Utility":
                        actionSetupDealershipType(id, type);
                        break;
                    case "Vans":
                        actionSetupDealershipType(id, type);
                        break;
                    default:
                        API.sendNotificationToPlayer(player, "Not a valid type. Removed");
                        query = string.Format("DELETE FROM VehicleShops WHERE ID={0}", id);
                        API.exported.database.executeQueryWithResult(query);
                        break;
                }

                positionBlips(player.position, player.rotation, type, Convert.ToInt32(id));
            }
        }

        public void actionSetupDealershipType(string id, string type)
        {
            db.updateDatabase("VehicleShops", "Type", type, "ID", id);
        }

        [Command("setdealershipexit")] // Set a dealership exit point.
        public void cmdSetDealershipExit(Client player, string id)
        {
            if (db.isAdmin(player.name))
            {
                db.updateDatabase("VehicleShops", "ExitPoint", player.position.ToString(), "ID", id);
                API.sendNotificationToPlayer(player, string.Format("Exit point set to: {0}", player.position));
            }
        }

        public void positionBlips(Vector3 position, Vector3 rotation, string type, int id)
        {
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 225);
            API.setBlipColor(newBlip, 47);
            vehicleShops.Add(new Vector3(position.X, position.Y, position.Z));
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 3f, 5f);
            collisionShapes.Add(shape, new Vector3(position.X, position.Y, position.Z));
            collisionShopType.Add(shape, type);
            collisionID.Add(shape, id);
            int i = 0;
            ++i;
        }
    }
}
