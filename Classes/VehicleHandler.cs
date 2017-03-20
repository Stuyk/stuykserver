using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace stuykserver.Util
{
    public class VehicleHandler : Script
    {
        ChatHandler ch = new ChatHandler();
        DatabaseHandler db = new DatabaseHandler();
        DateTime startTime;

        Dictionary<NetHandle, VehicleClass> vehicleInformation = new Dictionary<NetHandle, VehicleClass>();
        List<string> bicycles = new List<string>(new string[] {"Bmx", "Cruiser", "Fixter", "Scorcher", "TriBike" });

        public VehicleHandler()
        {
            API.onUpdate += API_onUpdate;
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle;
            API.onPlayerExitVehicle += API_onPlayerExitVehicle;
            startTime = DateTime.Now; // Start Timer on Resource Start for vehicle updates.
        }

        // When the player exits a vehicle. Assign a collision to the vehicle.
        private void API_onPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            // Prevent Local Vehicles from being called.
            if (!vehicleInformation.ContainsKey(vehicle))
            {
                return;
            }

            // If Owner
            if (vehicleInformation[vehicle].returnOwnerID() == Convert.ToInt32(API.getEntityData(player, "PlayerID")))
            {
                vehicleInformation[vehicle].setVehiclePosition(API.createCylinderColShape(API.getEntityPosition(vehicle), 3f, 3f), API.getEntityPosition(vehicle));
            }

            // If Has Keys
            if (vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
            {
                vehicleInformation[vehicle].setVehiclePosition(API.createCylinderColShape(API.getEntityPosition(vehicle), 3f, 3f), API.getEntityPosition(vehicle));
            }

            // Mainly for Anti-cheat. Prevent a false-positive.
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance == null)
            {
                return;
            }

            instance.setLastPosition(player);
        }

        // When a player enters a vehicle. Assign the VehicleEngine function to the player.
        private void API_onPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            Client[] playersInCar = API.getVehicleOccupants(vehicle);

            API.sendNativeToPlayer(player, (ulong)Hash.SET_VEHICLE_RADIO_ENABLED, vehicle, false);

            // If it's locked, kick them the fuck out.
            if (API.getVehicleLocked(vehicle))
            {
                API.warpPlayerOutOfVehicle(player, vehicle);
                API.sendNotificationToPlayer(player, "~r~This vehicle is locked.");
                foreach (Client playhur in playersInCar)
                {
                    if (vehicleInformation[vehicle].returnOwnerID() == Convert.ToInt32(API.getEntityData(playhur, "PlayerID")) && API.getEntityPosition(vehicleInformation[vehicle].returnVehicleID()).DistanceTo(playhur.position) <= 5)
                    {
                        API.setPlayerIntoVehicle(playhur, vehicle, -1);
                        break;
                    }
                }
                return;
            }

            if (vehicleInformation.ContainsKey(vehicle))
            {
                // Check if they're in the driver seat. Check if it's not a bicycle.
                if (API.getPlayerVehicleSeat(player) == -1 && !bicycles.Contains(vehicleInformation[vehicle].returnType()))
                {
                    API.setPlayerIntoVehicle(player, vehicle, -1);
                    vehicleInformation[vehicle].deleteCollision();
                    API.triggerClientEvent(player, "triggerUseFunction", "VehicleEngine");
                    API.setEntityData(player, "Collision", "VehicleEngine");
                }
            }
        }

        // #########################################
        // PRIMARILY ON UPDATE METHODS FOR VEHICLES
        // #########################################
        private void API_onUpdate()
        {
            // Every 10 seconds, update vehicle collision.
            if (DateTime.Now > startTime.AddSeconds(10))
            {
                startTime = DateTime.Now;
                updateVehicleCollisions();
            }
        }

        // Update Vehicle Collisions Every 15 Seconds for Unoccupied Spawned Vehicles.
        public void updateVehicleCollisions()
        {
            List<Client> players = API.getAllPlayers();
            List<NetHandle> inVehicles = new List<NetHandle>();
            // Add vehicles with players to list.
            foreach (Client player in players)
            {
                if (player.isInVehicle)
                {
                    if (vehicleInformation.ContainsKey(player.vehicle))
                    {
                        inVehicles.Add(player.vehicle);
                    }
                }
            }

            // Check for empty vehicles in main list.
            foreach (NetHandle vehicle in vehicleInformation.Keys)
            {
                if (!inVehicles.Contains(vehicle))
                {
                    vehicleInformation[vehicle].deleteCollision();
                    vehicleInformation[vehicle].setVehiclePosition(API.createCylinderColShape(API.getEntityPosition(vehicle), 3f, 3f), API.getEntityPosition(vehicle));
                }
            }
        }

        // #########################################
        // VEHICLE FUNCTIONS
        // #########################################
        // Start + Stop Vehicle
        public void actionVehicleEngine(Client player)
        {
            if (API.getEntityData(player, "NearVehicle") == null)
            {
                return;
            }

            NetHandle vehicle = (NetHandle)API.getEntityData(player, "NearVehicle");

            if (!player.isInVehicle)
            {
                return;
            }

            // Prevent Local Vehicles from being called.
            if (!vehicleInformation.ContainsKey(vehicle))
            {
                return;
            }

            // Check if Owner
            if (vehicleInformation[vehicle].returnOwnerID() == Convert.ToInt32(API.getEntityData(player, "PlayerID")))
            {
                if (API.getVehicleEngineStatus(vehicle))
                {
                    API.setVehicleEngineStatus(vehicle, false);
                    API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~Engine Off");
                    return;
                }
                else
                {
                    API.setVehicleEngineStatus(vehicle, true);
                    API.sendChatMessageToPlayer(player, "~y~Vehicle # ~b~Engine On");
                    return;
                }
            }

            // Check for Keys
            if (vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
            {
                if (API.getVehicleEngineStatus(vehicle))
                {
                    API.setVehicleEngineStatus(vehicle, false);
                    API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~Engine Off");
                    return;
                }
                else
                {
                    API.setVehicleEngineStatus(vehicle, true);
                    API.sendChatMessageToPlayer(player, "~y~Vehicle # ~b~Engine On");
                    return;
                }
            }

            // If none of the above, tell the player he has no keys.
            API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~You don't have any keys for this vehicle.");
            return;
        }

        // Lock + Unlock Vehicle
        public void actionVehicleLock(Client player)
        {
            if (API.getEntityData(player, "NearVehicle") == null)
            {
                return;
            }

            NetHandle vehicle = (NetHandle)API.getEntityData(player, "NearVehicle");

            // Distance check for the sake of it.
            if (player.position.DistanceTo(API.getEntityPosition(vehicle)) > 3)
            {
                API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~You don't seem to be near a vehicle.");
                return;
            }

            // Prevent Local Vehicles from being called.
            if (!vehicleInformation.ContainsKey(vehicle))
            {
                return;
            }

            // Check if Owner
            if (vehicleInformation[vehicle].returnOwnerID() == Convert.ToInt32(API.getEntityData(player, "PlayerID")))
            {
                if (API.getVehicleLocked(vehicle))
                {
                    API.setVehicleLocked(vehicle, false);
                    API.sendChatMessageToPlayer(player, "~y~Vehicle # ~b~Unlocked");
                    return;
                }
                else
                {
                    API.setVehicleLocked(vehicle, true);
                    vehicleInformation[vehicle].saveVehiclePosition();
                    API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~Locked");
                    return;
                }
            }

            // Check for Keys
            if (vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
            {
                if (API.getVehicleLocked(vehicle))
                {
                    vehicleInformation[vehicle].saveVehiclePosition();
                    API.sendChatMessageToPlayer(player, "~y~Vehicle # ~b~Unlocked");
                    return;
                }
                else
                {
                    API.setVehicleLocked(vehicle, true);
                    vehicleInformation[vehicle].saveVehiclePosition();
                    API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~Locked");
                    return;
                }
            }

            // If none of the above, tell the player he has no keys.
            API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~You don't have any keys for this vehicle.");
            return;
        }

        // Action Vehicle Hood
        public void actionVehicleHood(Client player)
        {
            if (API.getEntityData(player, "NearVehicle") ==  null)
            {
                return;
            }

            NetHandle vehicle = (NetHandle)API.getEntityData(player, "NearVehicle");

            if (player.position.DistanceTo(API.getEntityPosition(vehicle)) > 3 && player.isInVehicle)
            {
                API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~You don't seem to be near a vehicle.");
                return;
            }

            // Prevent Local Vehicles from being called.
            if (!vehicleInformation.ContainsKey(vehicle))
            {
                return;
            }

            // Check for Owner
            if (vehicleInformation[vehicle].returnOwnerID() == Convert.ToInt32(API.getEntityData(player, "PlayerID")))
            {
                if (API.getVehicleDoorState(vehicle, 4))
                {
                    API.setVehicleDoorState(vehicle, 4, false);
                    return;
                }
                else
                {
                    API.setVehicleDoorState(vehicle, 4, true);
                    return;
                }
            }

            // Check for Keys
            if (vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
            {
                if (API.getVehicleDoorState(vehicle, 4))
                {
                    API.setVehicleDoorState(vehicle, 4, false);
                    return;
                }
                else
                {
                    API.setVehicleDoorState(vehicle, 4, true);
                    return;
                }
            }

            // If none of the above, tell the player he has no keys.
            API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~You don't have any keys for this vehicle.");
            return;
        }

        // Action Vehicle Trunk
        public void actionVehicleTrunk(Client player)
        {
            if (API.getEntityData(player, "NearVehicle") == null)
            {
                return;
            }

            NetHandle vehicle = (NetHandle)API.getEntityData(player, "NearVehicle");

            if (player.position.DistanceTo(API.getEntityPosition(vehicle)) > 3 && player.isInVehicle)
            {
                API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~You don't seem to be near a vehicle.");
                return;
            }

            // Prevent Local Vehicles from being called.
            if (!vehicleInformation.ContainsKey(vehicle))
            {
                return;
            }

            // Check for Owner
            if (vehicleInformation[vehicle].returnOwnerID() == Convert.ToInt32(API.getEntityData(player, "PlayerID")))
            {
                if (API.getVehicleDoorState(vehicle, 5))
                {
                    API.setVehicleDoorState(vehicle, 5, false);
                    return;
                }
                else
                {
                    API.setVehicleDoorState(vehicle, 5, true);
                    return;
                }
            }

            // Check for Keys
            if (vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
            {
                if (API.getVehicleDoorState(vehicle, 5))
                {
                    API.setVehicleDoorState(vehicle, 5, false);
                    return;
                }
                else
                {
                    API.setVehicleDoorState(vehicle, 5, true);
                    return;
                }
            }

            // If none of the above, tell the player he has no keys.
            API.sendChatMessageToPlayer(player, "~y~Vehicle # ~o~You don't have any keys for this vehicle.");
            return;
        }


        // #########################################
        // VEHICLE SETUPS
        // #########################################

        // Used in Collision Handler to return the vehicle.
        public VehicleClass getVehicle(Client player, ColShape colshape)
        {
            foreach (NetHandle vehicle in vehicleInformation.Keys)
            {
                if (API.getEntityPosition(vehicle).DistanceTo(player.position) <= 3 && vehicleInformation[vehicle].returnCollision() == colshape)
                {
                    return vehicleInformation[vehicle];
                }
            }
            return null;
        }

        // Remove Disconnected Vehicles
        public void removeDisconnectedVehicles(Client player)
        {
            List<NetHandle> vehicles = API.getAllVehicles();

            foreach (NetHandle vehicle in vehicles)
            {
                if (vehicleInformation.ContainsKey(vehicle))
                {
                    if (vehicleInformation[vehicle].returnOwnerID() == Convert.ToInt32(API.getEntityData(player, "PlayerID")))
                    {
                        vehicleInformation[vehicle].Dispose();
                    }
                }
            }
        }

        // Used when a vehicle is purchased.
        public void actionSetupPurchasedCar(Vector3 position, string model, Client player)
        {
            // Push Up
            string[] vars = { "Nametag", "PosX", "PosY", "PosZ", "VehicleType", "PlayerID" };
            string[] data = { player.name, position.X.ToString(), position.Y.ToString(), position.Z.ToString(), model, Convert.ToString(API.getEntityData(player, "PlayerID")) };
            db.compileInsertQuery("PlayerVehicles", vars, data);

            // Pull Down for Creation
            string[] varNames = { "PlayerID", "VehicleType" };
            string before = "SELECT * FROM PlayerVehicles WHERE";
            object[] dataTwo = { Convert.ToString(API.getEntityData(player, "PlayerID")), model };
            DataTable result = db.compileSelectQuery(before, varNames, dataTwo);

            if (result.Rows.Count >= 1)
            {
                API.consoleOutput("Found More Than 1");
            }
            else
            {
                API.consoleOutput("Didn't find shit.");
            }
            VehicleClass veh = new VehicleClass(result.Rows[0]);
            vehicleInformation.Add(veh.returnVehicleID(), veh);
        }

        // Used when a player joins the server.
        public void SpawnPlayerCars(Client player)
        {
            string[] varNames = { "PlayerID" };
            string before = "SELECT * FROM PlayerVehicles WHERE";
            object[] data = { Convert.ToString(API.getEntityData(player, "PlayerID")) };
            DataTable result = db.compileSelectQuery(before, varNames, data);

            if (result.Rows.Count < 1)
            {
                return;
            }

            foreach (DataRow row in result.Rows)
            {
                VehicleClass veh = new VehicleClass(row);
                vehicleInformation.Add(veh.returnVehicleID(), veh);
                API.consoleOutput("Created vehicle for: " + result.Rows[0]["Nametag"].ToString());
            }
        }

        // Used when a vehicle exits a modifiction shop.
        public void initializeVehicleMods(Client player)
        {
            if (player.isInVehicle)
            {
                if (!vehicleInformation.ContainsKey(player.vehicle))
                {
                    return;
                }

                string[] varNames = { "ID" };
                string before = "SELECT * FROM PlayerVehicles WHERE";
                object[] data = { vehicleInformation[player.vehicle].returnVehicleIDNumber().ToString() };
                DataTable result = db.compileSelectQuery(before, varNames, data);

                Vehicle vehicle = player.vehicle;

                // Primary RGB
                int r = Convert.ToInt32(result.Rows[0]["Red"]);
                int g = Convert.ToInt32(result.Rows[0]["Green"]);
                int b = Convert.ToInt32(result.Rows[0]["Blue"]);

                API.setVehicleCustomPrimaryColor(vehicle, r, g, b);

                // Secondary RGB
                int sr = Convert.ToInt32(result.Rows[0]["sRed"]);
                int sg = Convert.ToInt32(result.Rows[0]["sGreen"]);
                int sb = Convert.ToInt32(result.Rows[0]["sBlue"]);

                API.setVehicleCustomSecondaryColor(vehicle, sr, sg, sb);

                // Mods
                API.setVehicleMod(vehicle, 0, Convert.ToInt32(result.Rows[0]["Spoilers"])); // Spoilers
                API.setVehicleMod(vehicle, 1, Convert.ToInt32(result.Rows[0]["FrontBumper"])); // Front Bumper
                API.setVehicleMod(vehicle, 2, Convert.ToInt32(result.Rows[0]["RearBumper"])); // Rear Bumper
                API.setVehicleMod(vehicle, 3, Convert.ToInt32(result.Rows[0]["SideSkirt"])); // Side Skirt
                API.setVehicleMod(vehicle, 4, Convert.ToInt32(result.Rows[0]["Exhaust"])); // Exhaust
                API.setVehicleMod(vehicle, 6, Convert.ToInt32(result.Rows[0]["Grille"])); // Grille
                API.setVehicleMod(vehicle, 7, Convert.ToInt32(result.Rows[0]["Hood"])); // Hood
                API.setVehicleMod(vehicle, 8, Convert.ToInt32(result.Rows[0]["Fender"])); // Fender
                API.setVehicleMod(vehicle, 9, Convert.ToInt32(result.Rows[0]["RightFender"])); // Right Fender
                API.setVehicleMod(vehicle, 10, Convert.ToInt32(result.Rows[0]["Roof"])); // Roof
                API.setVehicleMod(vehicle, 23, Convert.ToInt32(result.Rows[0]["FrontWheels"])); // Front Wheels
                API.setVehicleMod(vehicle, 24, Convert.ToInt32(result.Rows[0]["BackWheels"])); // Back Wheels
                API.setVehicleMod(vehicle, 69, Convert.ToInt32(result.Rows[0]["WindowTint"])); // Window Tint
            }
        }
    }
}
