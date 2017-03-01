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

        Dictionary<ColShape, VehicleShopInformation> shopInformation = new Dictionary<ColShape, VehicleShopInformation>();

        public enum PointType
        {
            Boats,
            Commercial,
            Compacts,
            Coupes,
            Bicycles,
            Police,
            Helicopters,
            Industrial,
            Motorcycles,
            Muscle,
            OffRoad,
            Planes,
            SUVS,
            Sedans,
            Sports,
            Classic,
            Super,
            Utility,
            Vans,
            Null
        }

        class VehicleShopInformation
        {
            ColShape collisionShape;
            int collisionID;
            Vector3 collisionPosition;
            Blip collisionBlip;
            PointType collisionType; // The type of point. Van, Sedan, ETC.
            List<Client> collisionPlayers; // When a player is in the collision.
            List<Client> containedPlayers; // When a player enters a shop.
            Vector3 shopCenterPoint; // Used to determine where the player is positioned.
            Vector3 shopCameraPoint; // Used to determine where the camera is positioned.
            Vector3 shopExitPoint; // Used as an area to spawn vehicles.
            
            public void setupPoint(ColShape collision, int id, Vector3 position, Blip blip, PointType type, Vector3 centerPoint, Vector3 cameraPoint, Vector3 exitPoint)
            {
                collisionShape = collision;
                collisionID = id;
                collisionPosition = position;
                collisionBlip = blip;
                collisionType = type;
                containedPlayers = new List<Client>();
                collisionPlayers = new List<Client>();
                shopCenterPoint = centerPoint;
                shopCameraPoint = cameraPoint;
                shopExitPoint = exitPoint;
            }

            public Vector3 returnShopCenterPoint()
            {
                return shopCenterPoint;
            }

            public Vector3 returnShopCameraPoint()
            {
                return shopCameraPoint;
            }

            public Vector3 returnShopExitPoint()
            {
                return shopExitPoint;
            }

            public void setShopCenterPoint(Vector3 position)
            {
                shopCenterPoint = position;
            }

            public void setShopCameraPoint(Vector3 position)
            {
                shopCameraPoint = position;
            }

            public void setShopExitPoint(Vector3 position)
            {
                shopExitPoint = position;
            }

            public void collisionPlayersAdd(Client player)
            {
                if (!collisionPlayers.Contains(player))
                {
                    collisionPlayers.Add(player);
                }
            }

            public void collisionPlayersRemove(Client player)
            {
                if (collisionPlayers.Contains(player))
                {
                    collisionPlayers.Remove(player);
                }
            }

            public void containedPlayersAdd(Client player)
            {
                if (!containedPlayers.Contains(player))
                {
                    containedPlayers.Add(player);
                }
            }

            public void containedPlayersRemove(Client player)
            {
                if (containedPlayers.Contains(player))
                {
                    containedPlayers.Remove(player);
                }
            }

            public List<Client> returnCollisionPlayers()
            {
                return collisionPlayers;
            }

            public List<Client> returnContainedPlayers()
            {
                return containedPlayers;
            }

            public int returnID()
            {
                return collisionID;
            }

            public ColShape returnCollision()
            {
                return collisionShape;
            }

            public Vector3 returnPosition()
            {
                return collisionPosition;
            }

            public Blip returnBlip()
            {
                return collisionBlip;
            }

            public PointType returnType()
            {
                return collisionType;
            }
        }

        public VehicleShopHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
        }

        [Command("dealership")]
        public void cmdDealership(Client player, string action, string type)
        {
            if (db.isAdmin(player.name))
            {
                if (action == "create")
                {
                    db.insertDataPointPosition("VehicleShops", player);
                    API.sendNotificationToPlayer(player, "~g~Created a dealership.");
                }

                if (action == "get")
                {
                    foreach (ColShape collision in shopInformation.Keys)
                    {
                        if (shopInformation[collision].returnPosition().DistanceTo(player.position) <= 30)
                        {
                            API.sendChatMessageToPlayer(player, string.Format("~y~Dealership ID: ~w~{0}", shopInformation[collision].returnID().ToString()));
                        }
                    }
                }

                if (action == "setexit")
                {
                    foreach (ColShape collision in shopInformation.Keys)
                    {
                        if (shopInformation[collision].returnPosition().DistanceTo(player.position) <= 30)
                        {
                            db.updateDatabase("VehicleShops", "ExitPoint", player.position.ToString(), "ID", shopInformation[collision].returnID().ToString());
                            API.sendNotificationToPlayer(player, "~g~Updated Exit Point.");
                        }
                    }
                }

                if (action == "setfocus")
                {
                    foreach (ColShape collision in shopInformation.Keys)
                    {
                        if (shopInformation[collision].returnPosition().DistanceTo(player.position) <= 30)
                        {
                            db.updateDatabase("VehicleShops", "CenterPoint", player.position.ToString(), "ID", shopInformation[collision].returnID().ToString());
                            API.sendNotificationToPlayer(player, "~g~Updated Focus Point.");
                        }
                    }
                }

                if (action == "setcamera")
                {
                    foreach (ColShape collision in shopInformation.Keys)
                    {
                        if (shopInformation[collision].returnPosition().DistanceTo(player.position) <= 30)
                        {
                            db.updateDatabase("VehicleShops", "CameraPoint", player.position.ToString(), "ID", shopInformation[collision].returnID().ToString());
                            API.sendNotificationToPlayer(player, "~g~Updated Camera Point.");
                        }
                    }
                }

                if (action == "type")
                {
                    foreach (ColShape collision in shopInformation.Keys)
                    {
                        if (shopInformation[collision].returnPosition().DistanceTo(player.position) <= 30)
                        {
                            db.updateDatabase("VehicleShops", "Type", type.ToString(), "ID", shopInformation[collision].returnID().ToString());
                            API.sendNotificationToPlayer(player, "~g~Updated Type to: " + type);
                        }
                    }
                }
            }
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnContainedPlayers().Contains(player))
                {
                    db.setPlayerPositionByVector(player, shopInformation[collision].returnPosition());
                    shopInformation[collision].containedPlayersRemove(player);
                }

                if (shopInformation[collision].returnCollisionPlayers().Contains(player))
                {
                    shopInformation[collision].collisionPlayersRemove(player);
                }
            }
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "dealershipReady")
            {
                foreach (ColShape collision in shopInformation.Keys)
                {
                    if (shopInformation[collision].returnContainedPlayers().Contains(player))
                    {
                        API.triggerClientEvent(player, "createCamera", shopInformation[collision].returnShopCameraPoint(), shopInformation[collision].returnShopCenterPoint());
                        API.setEntityDimension(player, 0);
                    }
                }
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
                Client player = API.getPlayerFromHandle(entity);
                if (shopInformation.ContainsKey(colshape))
                {
                    if (shopInformation[colshape].returnCollisionPlayers().Contains(player))
                    {
                        shopInformation[colshape].collisionPlayersRemove(player);
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "removeUseFunction");
                    }
                }
            }
                
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (shopInformation.ContainsKey(colshape))
                {
                    if (!shopInformation[colshape].returnCollisionPlayers().Contains(player) && !player.isInVehicle)
                    {
                        shopInformation[colshape].collisionPlayersAdd(player);
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "Dealership");
                        string type = shopInformation[colshape].returnType().ToString();
                        API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), string.Format("This shop carries the type: {0}", type));
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

            // Setup Shops
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
                    PointType type = convertToPointType(db.pullDatabase("VehicleShops", "Type", "ID", selectedrow));
                    Vector3 centerPoint = db.convertStringToVector3(db.pullDatabase("VehicleShops", "CenterPoint", "ID", selectedrow));
                    Vector3 cameraPoint = db.convertStringToVector3(db.pullDatabase("VehicleShops", "CameraPoint", "ID", selectedrow));
                    Vector3 exitPoint = db.convertStringToVector3(db.pullDatabase("VehicleShops", "ExitPoint", "ID", selectedrow));
                    int id = Convert.ToInt32(row[column]);

                    positionBlips(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), id, type, centerPoint, cameraPoint, exitPoint);

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Vehicle Shops Initialized: " + initializedObjects.ToString());
        }

        public PointType convertToPointType(string input)
        {
            switch(input)
            {
                case "Boats":
                    return PointType.Boats;
                case "Commercial":
                    return PointType.Commercial;
                case "Compacts":
                    return PointType.Compacts;
                case "Coupes":
                    return PointType.Coupes;
                case "Bicycles":
                    return PointType.Bicycles;
                case "Police":
                    return PointType.Police;
                case "Helicopters":
                    return PointType.Helicopters;
                case "Industrial":
                    return PointType.Industrial;
                case "Motorcycles":
                    return PointType.Motorcycles;
                case "Muscle":
                    return PointType.Muscle;
                case "OffRoad":
                    return PointType.OffRoad;
                case "Planes":
                    return PointType.Planes;
                case "SUVS":
                    return PointType.SUVS;
                case "Sedans":
                    return PointType.Sedans;
                case "Sports":
                    return PointType.Sports;
                case "Classic":
                    return PointType.Classic;
                case "Super":
                    return PointType.Super;
                case "Utility":
                    return PointType.Utility;
                case "Vans":
                    return PointType.Vans;
                default:
                    return PointType.Null;
            }
        }

        // When a player tries to enter a Dealership.
        public void browseDealership(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (API.isPlayerInAnyVehicle(player))
                {
                    break;
                }

                if (shopInformation[collision].returnCollisionPlayers().Contains(player))
                {
                    shopInformation[collision].containedPlayersAdd(player);
                    API.setEntityPosition(player, shopInformation[collision].returnShopCenterPoint());
                    API.setEntityDimension(player, new Random().Next(1, 1000));
                    db.setPlayerHUD(player, false);
                    API.triggerClientEvent(player, "startBrowsing", shopInformation[collision].returnType().ToString());
                    break;
                }
            }
        }

        // When a player purchased a vehicle from a Dealership.
        public void purchaseDealershipVehicle(Client player, string vehicleType)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnContainedPlayers().Contains(player))
                {
                    db.setPlayerHUD(player, true);
                    API.setEntityDimension(player, 0);
                    API.setEntityPosition(player, shopInformation[collision].returnPosition());
                    shopInformation[collision].containedPlayersRemove(player);
                    Vector3 exitPoint = db.convertStringToVector3(db.pullDatabase("VehicleShops", "ExitPoint", "ID", shopInformation[collision].returnID().ToString()));
                    API.call("VehicleHandler", "actionSetupPurchasedCar", exitPoint, API.vehicleNameToModel(vehicleType), player);
                    break;
                }
            }
        }

        // When a player leaves a dealership without a purchase.
        public void leaveDealership(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnContainedPlayers().Contains(player))
                {
                    db.setPlayerHUD(player, true);
                    API.setEntityDimension(player, 0);
                    API.setEntityPosition(player, shopInformation[collision].returnPosition());
                    shopInformation[collision].containedPlayersRemove(player);
                    API.stopPlayerAnimation(player);
                    API.stopPedAnimation(player);
                    break;
                }
            }
        }

        public void positionBlips(Vector3 position, Vector3 rotation, int id, PointType type, Vector3 centerPoint, Vector3 cameraPoint, Vector3 exitPoint)
        {
            VehicleShopInformation newShop = new VehicleShopInformation();
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 3f, 5f);

            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));

            switch (type)
            {
                case PointType.Motorcycles:
                    API.setBlipSprite(newBlip, 226);
                    break;
                case PointType.Helicopters:
                    API.setBlipSprite(newBlip, 43);
                    break;
                case PointType.Industrial:
                    API.setBlipSprite(newBlip, 318);
                    break;
                case PointType.Commercial:
                    API.setBlipSprite(newBlip, 477);
                    break;
                case PointType.Planes:
                    API.setBlipSprite(newBlip, 251);
                    break;
                case PointType.Super:
                    API.setBlipSprite(newBlip, 147);
                    break;
                case PointType.Boats:
                    API.setBlipSprite(newBlip, 455);
                    break;
                case PointType.OffRoad:
                    API.setBlipSprite(newBlip, 512);
                    break;
                case PointType.Vans:
                    API.setBlipSprite(newBlip, 67);
                    break;
                case PointType.Bicycles:
                    API.setBlipSprite(newBlip, 348);
                    break;
                default:
                    API.setBlipSprite(newBlip, 225);
                    break;
            }

            API.setBlipColor(newBlip, 73); // Yellow

            newShop.setupPoint(shape, id, position, newBlip, type, centerPoint, cameraPoint, exitPoint);
            shopInformation.Add(shape, newShop);
        }
    }
}
