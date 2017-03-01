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

        Dictionary<NetHandle, VehicleInformation> vehicleInformation = new Dictionary<NetHandle, VehicleInformation>();

        class VehicleInformation
        {
            NetHandle vehicleID;
            ColShape vehicleCollision;
            Vector3 vehiclePosition;
            List<Client> vehicleKeys;
            Client vehicleOwner;
            List<Client> playersInVehicle;

            public void setupVehicle(NetHandle id, ColShape collision, Vector3 collisionPosition, Client owner)
            {
                vehicleID = id;
                vehicleCollision = collision;
                vehiclePosition = collisionPosition;
                vehicleOwner = owner;
                vehicleKeys = new List<Client>();
                playersInVehicle = new List<Client>();
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
        }

        public VehicleHandler()
        {
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle;
            API.onPlayerExitVehicle += API_onPlayerExitVehicle;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {

            // Force Vehicle to stay in position after locking.
            if (Convert.ToInt32(API.getEntityType(entity)) == 1)
            {
                if (vehicleInformation.ContainsKey(entity))
                {
                    if (vehicleInformation[entity].returnPosition() != null)
                    {
                        API.setEntityPosition(entity, vehicleInformation[entity].returnPosition());
                    }
                }
            }

            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
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
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
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
                    if (API.getPlayerVehicleSeat(player) == -1)
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
                handleVehicleSpawn(player, vehicle, player.position);
                return;
            }
            return;
        }

        public Vehicle actionSetupPurchasedCar(Vector3 position, VehicleHash model, Client player)
        {
            Vehicle vehicle = API.createVehicle(model, position, new Vector3(), 0, 0);
            handleVehicleSpawn(player, vehicle, position);
            return vehicle;
        }

        public void handleVehicleSpawn(Client player, Vehicle vehicle, Vector3 where)
        {
            VehicleInformation newVehicle = new VehicleInformation();
            ColShape collision = API.createCylinderColShape(where, 2f, 2f);
            API.setVehicleLocked(vehicle, true);
            vehicle.engineStatus = false;
            newVehicle.setupVehicle(vehicle, collision, where, player);
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
                            API.sendNativeToPlayersInRange(API.getEntityPosition(vehicle), 10f, (ulong)Hash.START_VEHICLE_HORN, vehicle, true);
                            API.sendNativeToPlayersInRange(API.getEntityPosition(vehicle), 10f, (ulong)Hash.START_VEHICLE_HORN, vehicle, false);
                            break;
                        }
                        else
                        {
                            API.setVehicleLocked(vehicle, true);
                            API.setVehicleDoorState(vehicle, 0, false);
                            API.setVehicleDoorState(vehicle, 1, false);
                            API.setVehicleDoorState(vehicle, 2, false);
                            API.setVehicleDoorState(vehicle, 3, false);
                            API.sendNativeToPlayersInRange(API.getEntityPosition(vehicle), 10f, (ulong)Hash.START_VEHICLE_HORN, vehicle, true);
                            API.sendNativeToPlayersInRange(API.getEntityPosition(vehicle), 10f, (ulong)Hash.START_VEHICLE_HORN, vehicle, false);
                            break;
                        }
                    }
                }
            }
        }

        /*public void SpawnPlayerCars(Client player)
        {
            if(db.databasePlayCarSlotExists(player, 0))
            {
                Vehicle slotZero = db.databaseSpawnPlayerCar(player, 0);
                //handleVehicleSpawn(player, slotZero);
                return;
            }
        }*/
    }
}
