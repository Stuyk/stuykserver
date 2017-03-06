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
                string[] varNames = { "Red", "Green", "Blue", "sRed", "sGreen", "sBlue", "Spoilers", "FrontBumper", "RearBumper", "SideSkirt", "Exhaust", "Grille", "Hood", "Fender", "RightFender", "Roof", "FrontWheels", "BackWheels", "WindowTint" };

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                int i = 0;
                string query = "UPDATE PlayerVehicles SET";

                foreach (string label in varNames)
                {
                    if (i == varNames.Length - 1)
                    {
                        query = string.Format("{0} {1}=@{1}", query, label);
                    }
                    else
                    {
                        query = string.Format("{0} {1}=@{1},", query, label);
                    }
                    
                    parameters.Add(string.Format("@{0}", label), args[i].ToString());
                    ++i;
                }

                query = string.Format("{0} WHERE Garage='{1}' AND VehicleType='{2}'", query, player.name, API.getVehicleDisplayName((VehicleHash)player.vehicle.model));
                API.exported.database.executePreparedQuery(query, parameters);

                actionExitShop(player);
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
            API.setBlipShortRange(newBlip, true);

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
                    parseVehicleMods(player); // Setup Mods for Variable Use
                }
            }
        }

        public void parseVehicleMods(Client player)
        {
            string query = string.Format("SELECT * FROM PlayerVehicles WHERE Garage='{0}' AND VehicleType='{1}'", player.name, API.getVehicleDisplayName((VehicleHash)player.vehicle.model));
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                // Primary RGB
                int r = Convert.ToInt32(row["Red"]);
                int g = Convert.ToInt32(row["Green"]);
                int b = Convert.ToInt32(row["Blue"]);

                // Secondary RGB
                int sr = Convert.ToInt32(row["sRed"]);
                int sg = Convert.ToInt32(row["sGreen"]);
                int sb = Convert.ToInt32(row["sBlue"]);

                // Mods
                int spoiler = Convert.ToInt32(row["Spoilers"]); // Spoilers
                int frontBumper = Convert.ToInt32(row["FrontBumper"]); // Front Bumper
                int rearBumper = Convert.ToInt32(row["RearBumper"]); // Rear Bumper
                int sideSkirt = Convert.ToInt32(row["SideSkirt"]); // Side Skirt
                int exhaust = Convert.ToInt32(row["Exhaust"]); // Exhaust
                int grille = Convert.ToInt32(row["Grille"]); // Grille
                int hood = Convert.ToInt32(row["Hood"]); // Hood
                int fender = Convert.ToInt32(row["Fender"]); // Fender
                int rightFender = Convert.ToInt32(row["RightFender"]); // Right Fender
                int roof = Convert.ToInt32(row["Roof"]); // Roof
                int frontWheels = Convert.ToInt32(row["FrontWheels"]); // Front Wheels
                int backWheels = Convert.ToInt32(row["BackWheels"]); // Back Wheels
                int windowTint = Convert.ToInt32(row["WindowTint"]); // Window Tint

                API.triggerClientEvent(player, "passVehicleModifications", r, g, b, sr, sg, sb, spoiler, frontBumper, rearBumper, sideSkirt, exhaust, grille, hood, fender, rightFender, roof, frontWheels, backWheels, windowTint);
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
                    API.call("VehicleHandler", "initializeVehicleMods", player);
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
