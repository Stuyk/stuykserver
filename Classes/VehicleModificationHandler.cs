using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
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
        Dictionary<ColShape, ShopInformationHandling> shopInformation = new Dictionary<ColShape, ShopInformationHandling>();

        public VehicleModificationHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
        }

        // CLIENT DISCONNECTS
        private void API_onPlayerDisconnected(Client player, string reason)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnInsidePlayers().ContainsKey(player))
                {
                    db.setPlayerPositionByVector(player, shopInformation[collision].returnCollisionPosition());
                }
            }
        }

        // CLIENT EVENTS
        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "pushVehicleChanges")
            {
                // Gather all our data
                string[] varNames = { "Red", "Green", "Blue", "sRed", "sGreen", "sBlue", "Spoilers", "FrontBumper", "RearBumper", "SideSkirt", "Exhaust", "Grille", "Hood", "Fender", "RightFender", "Roof", "FrontWheels", "BackWheels", "WindowTint" };
                string before = "UPDATE PlayerVehicles SET";
                string after = string.Format("WHERE Garage='{0}' AND VehicleType='{1}'", player.name, API.getVehicleDisplayName((VehicleHash)player.vehicle.model));

                // Send all our data to generate the query and run it
                this.db.compileQuery(before, after, varNames, args);

                actionExitShop(player);
            }

            if (eventName == "leaveVehicleShop")
            {
                actionExitShop(player);
            }
        }


        // COLSHAPES
        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (shopInformation.ContainsKey(colshape))
                {
                    if (shopInformation[colshape].returnOutsidePlayers().Contains(player))
                    {
                        shopInformation[colshape].removeOutsidePlayer(player);
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
                    if (!shopInformation[colshape].returnOutsidePlayers().Contains(player) && player.isInVehicle)
                    {
                        shopInformation[colshape].addOutsidePlayer(player);
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "VehicleModificationShop");
                        API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), "This place seems to have a lot of car parts.");
                    }
                }
            }
        }

        // RESOURCE START
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

        // POSITION BLIPS
        public void positionBlips(Vector3 position, int id)
        {
            ShopInformationHandling newShop = new ShopInformationHandling();
            ColShape collision = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 5f, 5f);

            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 402);
            API.setBlipColor(newBlip, 59);
            API.setBlipShortRange(newBlip, true);

            newShop.setCollisionShape(collision);
            newShop.setBlip(newBlip);
            newShop.setCollisionID(id);
            newShop.setCollisionPosition(position);
            newShop.setShopType(ShopInformationHandling.ShopType.Modification);

            shopInformation.Add(collision, newShop);
        }

        // ACTIONS
        public void actionEnterShop(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnOutsidePlayers().Contains(player) && player.isInVehicle)
                {
                    int dimension = new Random().Next(1, 1000);
                    shopInformation[collision].addInsidePlayer(player, player.vehicle);
                    db.setPlayerHUD(player, false);
                    API.setEntityPosition(player.vehicle, new Vector3(-1156.071, -2005.241, 13.18026));
                    API.setEntityDimension(player.vehicle, dimension);
                    API.setEntityDimension(player, dimension);
                    API.setPlayerIntoVehicle(player, shopInformation[collision].returnInsidePlayers()[player], -1);
                    player.vehicle.engineStatus = false;
                    API.triggerClientEvent(player, "createCamera", new Vector3(-1149.901, -2006.942, 14.14681), player.vehicle.position);
                    API.triggerClientEvent(player, "openCarPanel");
                    parseVehicleMods(player); // Setup Mods for Variable Use
                }
            }
        }

        public void actionExitShop(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnInsidePlayers().ContainsKey(player))
                {
                    API.setEntityPosition(player.vehicle, shopInformation[collision].returnCollisionPosition());
                    API.setPlayerIntoVehicle(player, shopInformation[collision].returnInsidePlayers()[player], -1);
                    API.setEntityDimension(player.vehicle, 0);
                    API.setEntityDimension(player, 0);

                    shopInformation[collision].removeInsidePlayer(player);
                    player.vehicle.engineStatus = true;
                    API.triggerClientEvent(player, "endCamera");
                    db.setPlayerHUD(player, true);
                    API.call("VehicleHandler", "initializeVehicleMods", player);
                }
            }
        }

        // VEHICLE MOD HELPER
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

        
    }
}
