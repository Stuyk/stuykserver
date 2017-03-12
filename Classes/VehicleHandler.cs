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
        Dictionary<ColShape, VehicleClass> vehicleColInfo = new Dictionary<ColShape, VehicleClass>();
        List<string> bicycles = new List<string>(new string[] {"Bmx", "Cruiser", "Fixter", "Scorcher", "TriBike" });

        public VehicleHandler()
        {
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle;
            API.onPlayerExitVehicle += API_onPlayerExitVehicle;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onUpdate += API_onUpdate;
            startTime = DateTime.Now;
        }

        private void API_onUpdate()
        {
            if (DateTime.Now > startTime.AddSeconds(15))
            {
                startTime = DateTime.Now;
                updateVehicleCollisions();
            }
        }

        // Return VehicleClass from Collision
        public VehicleClass getVehicle(ColShape collision)
        {
            if (vehicleColInfo.ContainsKey(collision))
            {
                return vehicleColInfo[collision];
            }
            return null;
        }

        // Update Vehicle Collisions Every 15 Seconds for Unoccupied Spawned Vehicles.
        public void updateVehicleCollisions()
        {
            List<Client> players = API.getAllPlayers();
            List<NetHandle> inVehicles = new List<NetHandle>();
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

            foreach (NetHandle vehicle in vehicleInformation.Keys)
            {
                if (!inVehicles.Contains(vehicle))
                {
                    API.deleteColShape(vehicleInformation[vehicle].returnCollision());
                    vehicleInformation[vehicle].setVehiclePosition(API.createCylinderColShape(API.getEntityPosition(vehicle), 3f, 3f), API.getEntityPosition(vehicle));
                }
            }
        }

        public void removeDisconnectedVehicles(Client player)
        {
            List<NetHandle> vehicles = API.getAllVehicles();

            foreach (NetHandle vehicle in vehicles)
            {
                if (vehicleInformation[vehicle].returnOwnerID() == Convert.ToInt32(API.getEntityData(player, "PlayerID")))
                {
                    vehicleInformation[vehicle].Dispose();
                    vehicleInformation.Remove(vehicle);
                    API.deleteEntity(vehicle);
                }
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (!API.isPlayerInAnyVehicle(player))
                {
                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "removeUseFunction");
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                if (!API.isPlayerInAnyVehicle(API.getPlayerFromHandle(entity)))
                {
                    foreach (NetHandle vehicle in vehicleInformation.Keys)
                    {
                        if (API.getPlayerFromHandle(entity).position.DistanceTo(API.getEntityPosition(vehicle)) <= 1.5f)
                        {
                            API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "VehicleLock");
                            break;
                        }
                    }
                    return;
                }
            }
        }

        private void API_onPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            if (vehicleInformation.ContainsKey(vehicle))
            {
                if (vehicleInformation[vehicle].returnOwner() == player || vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
                {
                    vehicleInformation[vehicle].setVehiclePosition(API.createCylinderColShape(player.position, 3f, 3f), player.position);
                    API.setVehicleEngineStatus(vehicle, false);
                }

                if (vehicleInformation[vehicle].returnPlayersInVehicle().Contains(player))
                {
                    vehicleInformation[vehicle].playersInVehicleRemove(player);
                }
            }
        }

        private void API_onPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            if (API.getVehicleLocked(vehicle))
            {
                API.warpPlayerOutOfVehicle(player, vehicle);
                API.setVehicleDoorState(vehicle, 0, false);
                API.setVehicleDoorState(vehicle, 1, false);
                API.setVehicleDoorState(vehicle, 2, false);
                API.setVehicleDoorState(vehicle, 3, false);
                API.sendNotificationToPlayer(player, "~r~That appears to be locked.");
                return;
            }

            if (vehicleInformation.ContainsKey(vehicle))
            {
                if (!vehicleInformation[vehicle].returnPlayersInVehicle().Contains(player))
                {
                    vehicleInformation[vehicle].playersInVehicleAdd(player);
                    if (API.getPlayerVehicleSeat(player) == -1 && !bicycles.Contains(vehicleInformation[vehicle].returnType()))
                    {
                        API.triggerClientEvent(player, "triggerUseFunction", "VehicleEngine");
                        if (vehicleInformation[vehicle].returnOwner() == player || vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
                        {
                            vehicleInformation[vehicle].setVehiclePosition(null, null);
                        }
                        return;
                    }
                    else
                    {
                        API.triggerClientEvent(player, "removeUseFunction");
                        return;
                    }
                }
            }
        }

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
            vehicleColInfo.Add(veh.returnCollision(), veh);
        }

        public void actionVehicleEngine(Client player)
        {
            if (vehicleInformation[player.vehicle].returnOwner() == player && API.getPlayerVehicleSeat(player) == -1 || vehicleInformation[player.vehicle].returnVehicleKeys().Contains(player) && API.getPlayerVehicleSeat(player) == -1)
            {
                if (player.isInVehicle)
                {
                    if (player.vehicle.engineStatus == true)
                    {
                        player.vehicle.engineStatus = false;
                        API.sendNotificationToPlayer(player, "You stop the engine.");
                        return;
                    }
                    else
                    {
                        player.vehicle.engineStatus = true;
                        player.seatbelt = true;
                        API.setVehicleDoorState(player.vehicle, 0, false);
                        API.setVehicleDoorState(player.vehicle, 1, false);
                        API.setVehicleDoorState(player.vehicle, 2, false);
                        API.setVehicleDoorState(player.vehicle, 3, false);
                        API.sendNotificationToPlayer(player, "You start the engine.");
                        return;
                    }
                }
            }
        }

        public void actionVehicleHood(Client player)
        {
            foreach (NetHandle vehicle in vehicleInformation.Keys)
            {
                if (player.position.DistanceTo(vehicleInformation[vehicle].returnPosition()) <= 3 && !player.isInVehicle)
                {
                    if (vehicleInformation[vehicle].returnOwner() == player || vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
                    {
                        if (API.getVehicleDoorState(vehicle, 4))
                        {
                            API.setVehicleDoorState(vehicle, 4, false);
                            break;
                        }
                        else
                        {
                            API.setVehicleDoorState(vehicle, 4, true);
                            break;
                        }
                    }
                }
            }
        }

        public void actionVehicleTrunk(Client player)
        {
            foreach (NetHandle vehicle in vehicleInformation.Keys)
            {
                if (player.position.DistanceTo(vehicleInformation[vehicle].returnPosition()) <= 3 && !player.isInVehicle)
                {
                    if (vehicleInformation[vehicle].returnOwner() == player || vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
                    {
                        if (API.getVehicleDoorState(vehicle, 5))
                        {
                            API.setVehicleDoorState(vehicle, 5, false);
                            break;
                        }
                        else
                        {
                            API.setVehicleDoorState(vehicle, 5, true);
                            break;
                        }
                    }
                }
            }
        }

        public void actionLockCar(Client player)
        {
            if (API.getEntityData(player, "ColShape") == null)
            {
                return;
            }

            ColShape collision = (ColShape)API.getEntityData(player, "ColShape");
            VehicleClass vehInfo = getVehicle(collision);
            NetHandle vehicle = vehInfo.returnVehicleID();
            int owner = vehInfo.returnOwnerID();

            if (!player.isInVehicle && owner == Convert.ToInt32(API.getEntityData(player, "PlayerID")))
            {
                if (API.getVehicleLocked(vehicle))
                {
                    API.setVehicleLocked(vehicle, false);
                    API.setVehicleDoorState(vehicle, 0, true);
                    API.sendNotificationToPlayer(player, "~g~The vehicle has been unlocked.");
                }
                else
                {
                    API.setVehicleLocked(vehicle, true);
                    API.setVehicleDoorState(vehicle, 0, false);
                    API.setVehicleDoorState(vehicle, 1, false);
                    API.setVehicleDoorState(vehicle, 2, false);
                    API.setVehicleDoorState(vehicle, 3, false);
                    API.sendNotificationToPlayer(player, "~r~The vehicle has been locked.");

                    // REPLACE WITH SAVE FEATURE
                    Vector3 pos = API.getEntityPosition(vehicle);
                    Vector3 rot = API.getEntityRotation(vehicle);
                    string[] varNames = { "PosX", "PosY", "PosZ", "RotX", "RotY", "RotZ" };
                    string before = "UPDATE PlayerVehicles SET";
                    object[] data = { pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z };
                    string after = string.Format("WHERE PlayerID='{0}' AND VehicleType='{1}'", API.getEntitySyncedData(player, "PlayerID"), vehicleInformation[vehicle].returnType());

                    db.compileQuery(before, after, varNames, data);
                }
            }
        }

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
                vehicleColInfo.Add(veh.returnCollision(), veh);
                API.consoleOutput("Created vehicle for: " + result.Rows[0]["Nametag"].ToString());
            }
        }

        public void initializeVehicleMods(Client player)
        {
            if (player.isInVehicle)
            {
                string[] varNames = { "ID" };
                string before = "SELECT * FROM PlayerVehicles WHERE";
                object[] data = { Convert.ToString(API.getEntityData(player, "PlayerID")) };
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
