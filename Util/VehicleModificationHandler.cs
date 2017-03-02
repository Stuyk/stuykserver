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

        Dictionary<ColShape, ShopInformation> shopInformation = new Dictionary<ColShape, ShopInformation>();

        class ShopInformation
        {
            ColShape collisionShape;
            int collisionID;
            Vector3 collisionPosition;
            Blip collisionBlip;
            List<Client> collisionPlayers; // When a player is in the collision.
            Dictionary<Client, NetHandle> containedPlayers; // When a player enters a shop.

            public void setupPoint(ColShape collision, int id, Vector3 position, Blip blip)
            {
                collisionShape = collision;
                collisionID = id;
                collisionPosition = position;
                collisionBlip = blip;
                containedPlayers = new Dictionary<Client, NetHandle>();
                collisionPlayers = new List<Client>();
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

            public void containedPlayersAdd(Client player, NetHandle vehicle)
            {
                if (!containedPlayers.ContainsKey(player))
                {
                    containedPlayers.Add(player, vehicle);
                }
            }

            public void containedPlayersRemove(Client player)
            {
                if (containedPlayers.ContainsKey(player))
                {
                    containedPlayers.Remove(player);
                }
            }

            public List<Client> returnCollisionPlayers()
            {
                return collisionPlayers;
            }

            public Dictionary<Client, NetHandle> returnContainedPlayers()
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
        }

        public VehicleModificationHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnContainedPlayers().ContainsKey(player))
                {
                    db.setPlayerPositionByVector(player, shopInformation[collision].returnPosition());
                }
            }
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "pushVehicleChanges")
            {
                Vehicle playerVehicle = player.vehicle;
                API.setVehicleCustomPrimaryColor(playerVehicle, Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
                API.setVehicleCustomSecondaryColor(playerVehicle, Convert.ToInt32(args[3]), Convert.ToInt32(args[4]), Convert.ToInt32(args[5]));
                string query = string.Format("UPDATE PlayerVehicles SET Red='{0}', Green='{1}', Blue='{2}', sRed='{3}', sGreen='{4}', sBlue='{5}' WHERE Garage='{6}' AND VehicleType='{7}'", args[0], args[1], args[2], args[3], args[4], args[5], player.name, API.getVehicleDisplayName((VehicleHash)player.vehicle.model));
                API.exported.database.executeQueryWithResult(query);

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
                    if (!shopInformation[colshape].returnCollisionPlayers().Contains(player) && player.isInVehicle)
                    {
                        shopInformation[colshape].collisionPlayersAdd(player);
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "VehicleModificationShop");
                        API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), "This place seems to have a lot of car parts.");
                    }
                }
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Vehicle Modification Handler");

            string query = "SELECT * FROM VehicleModificationShops";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                float posX = Convert.ToSingle(row["PosX"]);
                float posY = Convert.ToSingle(row["PosY"]);
                float posZ = Convert.ToSingle(row["PosZ"]);
                int id = Convert.ToInt32(row["ID"]);
                positionBlips(new Vector3(posX, posY, posZ), id);

                ++initializedObjects;
            }

            API.consoleOutput("Vehicle Mod Shops Initialized: " + initializedObjects.ToString());
        }

        public void positionBlips(Vector3 position, int id)
        {
            ShopInformation newShop = new ShopInformation();
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 5f, 5f);

            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 402);
            API.setBlipColor(newBlip, 59);

            newShop.setupPoint(shape, id, new Vector3(position.X, position.Y, position.Z), newBlip);
            shopInformation.Add(shape, newShop);
        }

        public void actionEnterShop(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnCollisionPlayers().Contains(player) && player.isInVehicle)
                {
                    int dimension = new Random().Next(1, 1000);
                    shopInformation[collision].containedPlayersAdd(player, player.vehicle);
                    db.setPlayerHUD(player, false);
                    API.setEntityPosition(player.vehicle, new Vector3(-1156.071, -2005.241, 13.18026));
                    API.setEntityDimension(player.vehicle, dimension);
                    API.setEntityDimension(player, dimension);
                    API.setPlayerIntoVehicle(player, shopInformation[collision].returnContainedPlayers()[player], -1);
                    player.vehicle.engineStatus = false;
                    API.triggerClientEvent(player, "createCamera", new Vector3(-1149.901, -2006.942, 14.14681), player.vehicle.position);
                    API.triggerClientEvent(player, "openCarPanel");
                }
            }
        }

        public void actionExitShop(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnContainedPlayers().ContainsKey(player))
                {
                    API.setEntityPosition(player.vehicle, shopInformation[collision].returnPosition());
                    API.setPlayerIntoVehicle(player, shopInformation[collision].returnContainedPlayers()[player], -1);
                    API.setEntityDimension(player.vehicle, 0);
                    API.setEntityDimension(player, 0);
                    shopInformation[collision].containedPlayersRemove(player);
                    player.vehicle.engineStatus = true;
                    API.triggerClientEvent(player, "endCamera");
                    db.setPlayerHUD(player, true);
                }
            }
        }

        [Command("createvehiclemodshop")]
        public void cmdCreateVehicleModShop(Client player)
        {
            if (db.isAdmin(player.name))
            {
                db.insertDataPointPosition("VehicleModificationShops", player);
            }
        }
    }
}
