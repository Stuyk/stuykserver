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
        Dictionary<ColShape, Shop> shopInformation = new Dictionary<ColShape, Shop>();

        public VehicleModificationHandler()
        {
            API.consoleOutput("Started: Vehicle Modification Handler");
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        // CLIENT EVENTS
        private void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "pushVehicleChanges")
            {
                // Gather all our data
                string[] varNames = { "Red", "Green", "Blue", "sRed", "sGreen", "sBlue", "Spoilers", "FrontBumper", "RearBumper", "SideSkirt", "Exhaust", "Grille", "Hood", "Fender", "RightFender", "Roof", "FrontWheels", "BackWheels", "WindowTint" };
                string before = "UPDATE PlayerVehicles SET";
                string after = string.Format("WHERE PlayerID='{0}' AND VehicleType='{1}'", Convert.ToString(API.getEntityData(player, "PlayerID")), API.getVehicleDisplayName((VehicleHash)player.vehicle.model));

                // Send all our data to generate the query and run it
                this.db.compileQuery(before, after, varNames, args);

                actionExitShop(player);
            }

            if (eventName == "leaveVehicleShop")
            {
                actionExitShop(player);
            }
        }

        // ACTIONS
        public void actionEnterShop(Client player)
        {
            if (!player.isInVehicle)
            {
                API.sendNotificationToPlayer(player, "~r~You must have a vehicle to access this.");
                return;
            }

            NetHandle playerVehicle = API.getPlayerVehicle(player);

            db.setPlayerHUD(player, false);
            API.setEntityData(player, "ReturnPosition", player.position);
            API.setEntityDimension(player.vehicle, Convert.ToInt32(API.getEntityData(player, "PlayerID")));
            API.setEntityDimension(player, Convert.ToInt32(API.getEntityData(player, "PlayerID")));

            ColShape colshape = (ColShape)API.getEntityData(player, "ColShape");
            Shop shop = (Shop)API.call("ShopHandler", "getShop", colshape);
            // Temporary Holding Collision
            API.setEntityData(player, "ExitPoint", shop.returnExitPoint());

            // Custom
            if (shop.returnCameraCenterPoint() != new Vector3(0, 0, 0) && shop.returnCameraPoint() != new Vector3(0, 0, 0))
            {
                API.setEntityPosition(player.vehicle, shop.returnCameraCenterPoint());
                API.setPlayerIntoVehicle(player, playerVehicle, -1);
                API.triggerClientEvent(player, "createCamera", shop.returnCameraPoint(), shop.returnCameraCenterPoint());
                API.triggerClientEvent(player, "openCarPanel");
                parseVehicleMods(player); // Setup Mods for Variable Use
                return;
            }

            // Default
            API.setEntityPosition(player.vehicle, new Vector3(-1156.071, -2005.241, 13.18026));
            API.setPlayerIntoVehicle(player, playerVehicle, -1);
            API.triggerClientEvent(player, "createCamera", new Vector3(-1149.901, -2006.942, 14.14681), player.vehicle.position);
            API.triggerClientEvent(player, "openCarPanel");
            parseVehicleMods(player); // Setup Mods for Variable User
            return;

            //API.setEntityDimension(player, dimension);
            //API.setPlayerIntoVehicle(player, shopInformation[collision].returnInsidePlayers()[player], -1);
        }

        public void actionExitShop(Client player)
        {
            NetHandle playerVehicle = API.getPlayerVehicle(player);

            db.setPlayerHUD(player, true);
            API.triggerClientEvent(player, "endCamera");
            player.vehicle.engineStatus = true;

            if (API.getEntityData(player, "ExitPoint") != null)
            {
                Vector3 exitPoint = (Vector3)API.getEntityData(player, "ExitPoint");
                API.setEntityPosition(player.vehicle, exitPoint);
                API.setEntityDimension(player.vehicle, 0);
                API.setEntityDimension(player, 0);
                API.setPlayerIntoVehicle(player, playerVehicle, -1);
                API.call("VehicleHandler", "initializeVehicleMods", player);
                return;
            }

            Vector3 returnPoint = (Vector3)API.getEntityData(player, "ReturnPosition");
            API.setEntityPosition(player.vehicle, returnPoint);
            API.setEntityDimension(player.vehicle, 0);
            API.setEntityDimension(player, 0);
            API.setPlayerIntoVehicle(player, playerVehicle, -1);
            API.call("VehicleHandler", "initializeVehicleMods", player);
        }

        // VEHICLE MOD HELPER
        public void parseVehicleMods(Client player)
        {
            string query = string.Format("SELECT * FROM PlayerVehicles WHERE PlayerID='{0}' AND VehicleType='{1}'", Convert.ToString(API.getEntityData(player, "PlayerID")), API.getVehicleDisplayName((VehicleHash)player.vehicle.model));
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
