using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class VehicleHandler : Script
    {
        ChatHandler ch = new ChatHandler();
        DatabaseHandler db = new DatabaseHandler();
        List<Client> playersInVehicles = new List<Client>();
        List<NetHandle> vehicleHandles = new List<NetHandle>();
        Dictionary<NetHandle, Vector3> vehiclePositions = new Dictionary<NetHandle, Vector3>();
        Dictionary<NetHandle, ColShape> vehicleCollisions = new Dictionary<NetHandle, ColShape>();
        Dictionary<NetHandle, Client> vehicleKeys = new Dictionary<NetHandle, Client>();

        public VehicleHandler()
        {
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle;
            API.onPlayerExitVehicle += API_onPlayerExitVehicle;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 1)
            {
                if (vehiclePositions.ContainsKey(entity))
                {
                    if (vehicleCollisions.ContainsKey(entity))
                    {
                        API.setEntityPosition(entity, vehiclePositions[entity]);
                        return;
                    }
                }
            }

            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                if (!API.isPlayerInAnyVehicle(API.getPlayerFromHandle(entity)))
                {
                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "removeUseFunction");
                    return;
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                if (!API.isPlayerInAnyVehicle(API.getPlayerFromHandle(entity)))
                {
                    List<NetHandle> vehicles = API.getAllVehicles();
                    foreach (NetHandle vehicle in vehicles)
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
            if (playersInVehicles.Contains(player))
            {
                playersInVehicles.Remove(player);
                API.triggerClientEvent(player, "removeUseFunction");
            }

            if (!vehicleHandles.Contains(vehicle))
            {
                vehicleHandles.Add(vehicle);
            }

            if (!vehiclePositions.ContainsKey(vehicle))
            {
                vehiclePositions.Add(vehicle, API.getEntityPosition(vehicle));
                if (!vehicleCollisions.ContainsKey(vehicle))
                {
                    vehicleCollisions.Add(vehicle, API.createCylinderColShape(vehiclePositions[vehicle], 3f, 3f));
                    API.setVehicleEngineStatus(vehicle, false);
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
            }

            if (!playersInVehicles.Contains(player))
            {
                if (API.getPlayerVehicleSeat(player) == -1)
                {
                    playersInVehicles.Add(player);
                    API.triggerClientEvent(player, "triggerUseFunction", "VehicleEngine");
                }
                else
                {
                    API.setVehicleDoorState(vehicle, API.getPlayerVehicleSeat(player) - 1, false);
                    API.triggerClientEvent(player, "removeUseFunction");
                }
            }

            if (vehiclePositions.ContainsKey(vehicle))
            {
                if (API.getPlayerVehicleSeat(player) == -1)
                {
                    vehiclePositions.Remove(vehicle);
                    if (vehicleCollisions.ContainsKey(vehicle))
                    {
                        vehicleCollisions.Remove(vehicle);
                    }
                }
            }

            if (!vehicleHandles.Contains(vehicle))
            {
                vehicleHandles.Add(vehicle);
            }
        }

        [Command("editCar")]
        public void editCar(Client player)
        {
            API.triggerClientEvent(player, "openCarPanel");
        }

        [Command("spawncar")] // Admin
        public void cmdSpawnCar(Client player, VehicleHash model)
        {
            if (db.isPlayerLoggedIn(player))
            {
                var rot = API.getEntityRotation(player.handle);
                var vehicle = API.createVehicle(model, player.position, new Vector3(0, 0, rot.Z), 0, 0);
                handleVehicleSpawn(player, vehicle);
                return;
            }
            return;
        }

        public void handleVehicleSpawn(Client player, Vehicle vehicle)
        {
            vehicleKeys.Add(vehicle.handle, player);
            vehicleHandles.Add(vehicle.handle);
            vehiclePositions.Add(vehicle.handle, API.getEntityPosition(vehicle.handle));
            vehicleCollisions.Add(vehicle.handle, API.createCylinderColShape(vehiclePositions[vehicle.handle], 2f, 2f));
            API.setVehicleLocked(vehicle, true);
            vehicle.engineStatus = false;
        }

        public void actionVehicleEngine(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (player.isInVehicle)
                {
                    if (API.getPlayerVehicleSeat(player) == -1)
                    {
                        if (vehicleKeys[player.vehicle] == player)
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
                                API.sendNotificationToPlayer(player, "You start the engine, and put on your seatbelt.");
                                return;
                            }
                        }
                        else
                        {
                            API.sendNotificationToPlayer(player, "~r~You don't have keys for this.");
                        }
                    } 
                }
            }
        }

        public void actionVehicleHood(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (player.isInVehicle)
                {
                    if (API.getPlayerVehicleSeat(player) == -1)
                    {
                        if (vehicleKeys[player.vehicle] == player)
                        {
                            if (API.getVehicleDoorState(player.vehicle, 4))
                            {
                                API.setVehicleDoorState(player.vehicle, 4, false);
                            }
                            else
                            {
                                API.setVehicleDoorState(player.vehicle, 4, true);
                            }
                        }
                    }
                }
            }
        }

        public void actionLockCar(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (!player.isInVehicle)
                {
                    foreach (NetHandle vehicle in vehicleHandles)
                    {
                        if (player.position.DistanceTo(API.getEntityPosition(vehicle)) <= 3)
                        {
                            if (vehicleKeys[vehicle] == player)
                            {
                                if (API.isVehicleDoorBroken(vehicle, 0))
                                {
                                    API.sendNotificationToPlayer(player, "Can't lock a car with a broken door.");
                                    API.setVehicleLocked(vehicle, false);
                                    break;
                                }

                                if (API.isVehicleDoorBroken(vehicle, 1))
                                {
                                    API.sendNotificationToPlayer(player, "Can't lock a car with a broken door.");
                                    API.setVehicleLocked(vehicle, false);
                                    break;
                                }

                                if (API.isVehicleDoorBroken(vehicle, 2))
                                {
                                    API.sendNotificationToPlayer(player, "Can't lock a car with a broken door.");
                                    API.setVehicleLocked(vehicle, false);
                                    break;
                                }

                                if (API.isVehicleDoorBroken(vehicle, 3))
                                {
                                    API.sendNotificationToPlayer(player, "Can't lock a car with a broken door.");
                                    API.setVehicleLocked(vehicle, false);
                                    break;
                                }

                                if (API.getVehicleLocked(vehicle))
                                {
                                    API.setVehicleLocked(vehicle, false);
                                    API.setVehicleDoorState(vehicle, 0, true);
                                    API.sendNotificationToPlayer(player, "You unlocked the vehicle.");
                                    break;
                                }
                                else
                                {
                                    API.setVehicleLocked(vehicle, true);
                                    API.setVehicleDoorState(vehicle, 0, false);
                                    API.setVehicleDoorState(vehicle, 1, false);
                                    API.setVehicleDoorState(vehicle, 2, false);
                                    API.setVehicleDoorState(vehicle, 3, false);
                                    API.sendNotificationToPlayer(player, "You locked the vehicle.");
                                    break;
                                }
                            }
                            else
                            {
                                API.sendNotificationToPlayer(player, "~r~You don't have keys for this.");
                            }
                        }
                    }
                }
            }
        }

        public void SpawnPlayerCars(Client player)
        {
            if(db.databasePlayCarSlotExists(player, 0))
            {
                Vehicle slotZero = db.databaseSpawnPlayerCar(player, 0);
                handleVehicleSpawn(player, slotZero);
                return;
            }
        }
    }
}
