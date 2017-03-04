using GTANetworkServer;
using GTANetworkShared;
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

        Dictionary<NetHandle, VehicleInformation> vehicleInformation = new Dictionary<NetHandle, VehicleInformation>();
        List<string> bicycles = new List<string>(new string[] {"Bmx", "Cruiser", "Fixter", "Scorcher", "TriBike" });

        public class VehicleInformation : IDisposable
        {
            NetHandle vehicleID;
            ColShape vehicleCollision;
            Vector3 vehiclePosition;
            List<Client> vehicleKeys;
            Client vehicleOwner;
            List<Client> playersInVehicle;
            string vehicleType;

            public void setupVehicle(NetHandle id, ColShape collision, Vector3 collisionPosition, Client owner, string type)
            {
                vehicleID = id;
                vehicleCollision = collision;
                vehiclePosition = collisionPosition;
                vehicleOwner = owner;
                vehicleType = type;
                vehicleKeys = new List<Client>();
                playersInVehicle = new List<Client>();
            }

            public string returnType()
            {
                return vehicleType;
            }

            public void setVehiclePosition(ColShape collision, Vector3 position)
            {
                vehicleCollision = collision;
                vehiclePosition = position;
            }

            public void vehicleKeysAdd(Client player)
            {
                if (!vehicleKeys.Contains(player))
                {
                    vehicleKeys.Add(player);
                }
            }

            public void vehicleKeysRemove(Client player)
            {
                if (vehicleKeys.Contains(player))
                {
                    vehicleKeys.Remove(player);
                }
            }

            public void playersInVehicleAdd(Client player)
            {
                if (!playersInVehicle.Contains(player))
                {
                    playersInVehicle.Add(player);
                }
            }

            public void playersInVehicleRemove(Client player)
            {
                if (playersInVehicle.Contains(player))
                {
                    playersInVehicle.Remove(player);
                }
            }

            public Vector3 returnPosition()
            {
                return vehiclePosition;
            }

            public ColShape returnCollision()
            {
                return vehicleCollision;
            }

            public List<Client> returnVehicleKeys()
            {
                return vehicleKeys;
            }

            public List<Client> returnPlayersInVehicle()
            {
                return playersInVehicle;
            }

            public Client returnOwner()
            {
                return vehicleOwner;
            }

            public NetHandle returnVehicleID()
            {
                return vehicleID;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
        }

        

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
                    vehicleInformation[vehicle].setVehiclePosition(API.createCylinderColShape(API.getEntityPosition(vehicle), 2f, 2f), API.getEntityPosition(vehicle));
                }
            }
        }

        public void removeDisconnectedVehicles(Client player)
        {
            List<NetHandle> vehicles = API.getAllVehicles();

            foreach (NetHandle vehicle in vehicles)
            {
                if (vehicleInformation[vehicle].returnOwner() == player)
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
                        if (API.getPlayerFromHandle(entity).position.DistanceTo(API.getEntityPosition(vehicle)) <= 5)
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

        [Command("spawncar")] // Admin
        public void cmdSpawnCar(Client player, VehicleHash model)
        {
            if (db.isPlayerLoggedIn(player))
            {
                var rot = API.getEntityRotation(player.handle);
                var vehicle = API.createVehicle(model, player.position, new Vector3(0, 0, rot.Z), 0, 0);
                handleVehicleSpawn(player, vehicle, player.position, API.getVehicleDisplayName(model));
                return;
            }
            return;
        }

        public Vehicle actionSetupPurchasedCar(Vector3 position, string model, Client player)
        {
            VehicleHash name = API.vehicleNameToModel(model);
            Vehicle vehicle = API.createVehicle(name, position, new Vector3(), 0, 0);
            handleVehicleSpawn(player, vehicle, position, model);
            db.insertPurchasedVehicle(player, vehicle, model);
            return vehicle;
        }

        public void handleVehicleSpawn(Client player, Vehicle vehicle, Vector3 where, string type)
        {
            VehicleInformation newVehicle = new VehicleInformation();
            ColShape collision = API.createCylinderColShape(where, 2f, 2f);
            API.setVehicleLocked(vehicle, true);
            vehicle.engineStatus = false;
            newVehicle.setupVehicle(vehicle, collision, where, player, type);
            vehicleInformation.Add(vehicle, newVehicle);
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
            foreach (NetHandle vehicle in vehicleInformation.Keys)
            {
                if (player.position.DistanceTo(vehicleInformation[vehicle].returnPosition()) <= 3 && !player.isInVehicle)
                {
                    if (vehicleInformation[vehicle].returnOwner() == player || vehicleInformation[vehicle].returnVehicleKeys().Contains(player))
                    {
                        if (API.getVehicleLocked(vehicle))
                        {
                            API.setVehicleLocked(vehicle, false);
                            API.setVehicleDoorState(vehicle, 0, true);
                            API.sendNotificationToPlayer(player, "~g~The vehicle has been unlocked.");
                            break;
                        }
                        else
                        {
                            API.setVehicleLocked(vehicle, true);
                            API.setVehicleDoorState(vehicle, 0, false);
                            API.setVehicleDoorState(vehicle, 1, false);
                            API.setVehicleDoorState(vehicle, 2, false);
                            API.setVehicleDoorState(vehicle, 3, false);
                            API.sendNotificationToPlayer(player, "~r~The vehicle has been locked.");
                            if (vehicleInformation[vehicle].returnOwner() == player)
                            {
                                Vector3 veh = API.getEntityPosition(vehicle);
                                Vector3 vehR = API.getEntityRotation(vehicle);
                                string query = string.Format("UPDATE PlayerVehicles SET PosX='{0}', PosY='{1}', PosZ='{2}', RotX='{3}', RotY='{4}', RotZ='{5}' WHERE Garage='{6}' AND VehicleType='{7}'", veh.X, veh.Y, veh.Z, vehR.X, vehR.Y, vehR.Z, player.name, vehicleInformation[vehicle].returnType());
                                API.exported.database.executeQueryWithResult(query);
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void SpawnPlayerCars(Client player)
        {
            string query = string.Format("SELECT * FROM PlayerVehicles WHERE Garage='{0}'", player.name);
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                float posX = Convert.ToSingle(row["PosX"]);
                float posY = Convert.ToSingle(row["PosY"]);
                float posZ = Convert.ToSingle(row["PosZ"]);
                float rotX = Convert.ToSingle(row["RotX"]);
                float rotY = Convert.ToSingle(row["RotY"]);
                float rotZ = Convert.ToSingle(row["RotZ"]);
                VehicleHash type = API.vehicleNameToModel(row["VehicleType"].ToString());

                Vector3 position = new Vector3(posX, posY, posZ);
                Vector3 rotation = new Vector3(rotX, rotY, rotZ);

                Vehicle vehicle = API.createVehicle(type, position, rotation, 0, 0);

                // Primary RGB
                int r = Convert.ToInt32(row["Red"]);
                int g = Convert.ToInt32(row["Green"]);
                int b = Convert.ToInt32(row["Blue"]);

                API.setVehicleCustomPrimaryColor(vehicle, r, g, b);

                // Secondary RGB
                int sr = Convert.ToInt32(row["sRed"]);
                int sg = Convert.ToInt32(row["sGreen"]);
                int sb = Convert.ToInt32(row["sBlue"]);

                API.setVehicleCustomSecondaryColor(vehicle, sr, sg, sb);

                // Mods
                API.setVehicleMod(vehicle, 0, Convert.ToInt32(row["Spoilers"])); // Spoilers
                API.setVehicleMod(vehicle, 1, Convert.ToInt32(row["FrontBumper"])); // Front Bumper
                API.setVehicleMod(vehicle, 2, Convert.ToInt32(row["RearBumper"])); // Rear Bumper
                API.setVehicleMod(vehicle, 3, Convert.ToInt32(row["SideSkirt"])); // Side Skirt
                API.setVehicleMod(vehicle, 4, Convert.ToInt32(row["Exhaust"])); // Exhaust
                API.setVehicleMod(vehicle, 6, Convert.ToInt32(row["Grille"])); // Grille
                API.setVehicleMod(vehicle, 7, Convert.ToInt32(row["Hood"])); // Hood
                API.setVehicleMod(vehicle, 8, Convert.ToInt32(row["Fender"])); // Fender
                API.setVehicleMod(vehicle, 9, Convert.ToInt32(row["RightFender"])); // Right Fender
                API.setVehicleMod(vehicle, 10, Convert.ToInt32(row["Roof"])); // Roof
                API.setVehicleMod(vehicle, 23, Convert.ToInt32(row["FrontWheels"])); // Front Wheels
                API.setVehicleMod(vehicle, 24, Convert.ToInt32(row["BackWheels"])); // Back Wheels
                API.setVehicleMod(vehicle, 69, Convert.ToInt32(row["WindowTint"])); // Window Tint

                handleVehicleSpawn(player, vehicle, position, API.getVehicleDisplayName(type));

                API.consoleOutput("Created vehicle for: " + row["Garage"].ToString());
            }
        }

        public void initializeVehicleMods(Client player)
        {
            if (player.isInVehicle)
            {
                string query = string.Format("SELECT * FROM PlayerVehicles WHERE Garage='{0}' AND VehicleType='{1}'", player.name, vehicleInformation[player.vehicle].returnType());
                DataTable result = API.exported.database.executeQueryWithResult(query);

                foreach (DataRow row in result.Rows)
                {
                    Vehicle vehicle = player.vehicle;

                    // Primary RGB
                    int r = Convert.ToInt32(row["Red"]);
                    int g = Convert.ToInt32(row["Green"]);
                    int b = Convert.ToInt32(row["Blue"]);

                    API.setVehicleCustomPrimaryColor(vehicle, r, g, b);

                    // Secondary RGB
                    int sr = Convert.ToInt32(row["sRed"]);
                    int sg = Convert.ToInt32(row["sGreen"]);
                    int sb = Convert.ToInt32(row["sBlue"]);

                    API.setVehicleCustomSecondaryColor(vehicle, sr, sg, sb);

                    // Mods
                    API.setVehicleMod(vehicle, 0, Convert.ToInt32(row["Spoilers"])); // Spoilers
                    API.setVehicleMod(vehicle, 1, Convert.ToInt32(row["FrontBumper"])); // Front Bumper
                    API.setVehicleMod(vehicle, 2, Convert.ToInt32(row["RearBumper"])); // Rear Bumper
                    API.setVehicleMod(vehicle, 3, Convert.ToInt32(row["SideSkirt"])); // Side Skirt
                    API.setVehicleMod(vehicle, 4, Convert.ToInt32(row["Exhaust"])); // Exhaust
                    API.setVehicleMod(vehicle, 6, Convert.ToInt32(row["Grille"])); // Grille
                    API.setVehicleMod(vehicle, 7, Convert.ToInt32(row["Hood"])); // Hood
                    API.setVehicleMod(vehicle, 8, Convert.ToInt32(row["Fender"])); // Fender
                    API.setVehicleMod(vehicle, 9, Convert.ToInt32(row["RightFender"])); // Right Fender
                    API.setVehicleMod(vehicle, 10, Convert.ToInt32(row["Roof"])); // Roof
                    API.setVehicleMod(vehicle, 23, Convert.ToInt32(row["FrontWheels"])); // Front Wheels
                    API.setVehicleMod(vehicle, 24, Convert.ToInt32(row["BackWheels"])); // Back Wheels
                    API.setVehicleMod(vehicle, 69, Convert.ToInt32(row["WindowTint"])); // Window Tint
                    break;
                }
            }
        }
    }
}
