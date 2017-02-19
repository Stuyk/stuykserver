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
        Dictionary<NetHandle, Client> vehicleOwner = new Dictionary<NetHandle, Client>();
        DatabaseHandler db = new DatabaseHandler();

        public VehicleHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle;
            API.onPlayerExitVehicle += API_onPlayerExitVehicle;
            API.onVehicleDoorBreak += API_onVehicleDoorBreak;
        }

        private void API_onVehicleDoorBreak(NetHandle vehicle, int index)
        {
            
        }

        private void API_onPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            if (vehicleOwner.ContainsKey(vehicle))
            {
                if (vehicleOwner[vehicle] == player)
                {
                    API.setVehicleLocked(vehicle, true);
                    API.setVehicleEngineStatus(vehicle, false);
                    //ch.sendCloseMessage(player, 15.0f, "~#C2A2DA~", API.getPlayerName(player) + " stops the engine, and exits the vehicle.");

                    API.sendNotificationToPlayer(player, "~g~You lock the car as you exit the vehicle");
                }
            }
        }

        private void API_onPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            if (API.getVehicleLocked(vehicle))
            {
                API.warpPlayerOutOfVehicle(player, vehicle);
                //main.sendNotification(player, "~r~That seems to be locked.");
            }
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            
        }

        [Command("spawncar")] // Admin
        public void cmdSpawnCar(Client player, VehicleHash model)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (db.isAdmin(player.name))
                {
                    var rot = API.getEntityRotation(player.handle);
                    var veh = API.createVehicle(model, player.position, new Vector3(0, 0, rot.Z), 0, 0);
                    veh.engineStatus = false;
                    // vehicleOwner.Add(veh.handle, player);
                    return;
                }
            }
            return;
        }

        [Command("car")]
        public void cmdCar(Client player, string action)
        {
            if (player.isInVehicle && API.getPlayerVehicleSeat(player) == -1)
            {
                if (action == "engine" || action == "Engine")
                {
                    if (vehicleOwner.ContainsKey(player.vehicle.handle))
                    {
                        if (vehicleOwner[player.vehicle.handle] == player)
                        {
                            if (player.vehicle.engineStatus == true)
                            {
                                player.vehicle.engineStatus = false;
                                //ch.sendCloseMessage(player, 15.0f, "~#C2A2DA~", API.getPlayerName(player) + " stops the engine.");
                            }
                            else
                            {
                                player.vehicle.engineStatus = true;
                                //ch.sendCloseMessage(player, 15.0f, "~#C2A2DA~", API.getPlayerName(player) + " starts the engine.");
                            }
                        }
                    }
                    else if (player.vehicle.engineStatus == true)
                    {
                        vehicleOwner[player.vehicle.handle] = player;
                        player.vehicle.engineStatus = false;
                        API.sendNotificationToPlayer(player, "~g~Looks like they left the keys in the ignition.");
                    }
                    else
                    {
                        API.sendNotificationToPlayer(player, "~r~Doesn't appear to be any keys.");
                        player.vehicle.engineStatus = true;
                    }
                }
                if (action == "lights" || action == "headlights")
                {
                    if (player.vehicle.specialLight == false)
                    {
                        player.vehicle.specialLight = true;
                    }
                    else
                    {
                        player.vehicle.specialLight = false;
                    }
                }
                if (action == "visor")
                {
                    //ch.sendCloseMessage(player, 6.0f, "~#C2A2DA~", API.getPlayerName(player) + " adjusts the visor and winks.");
                }
                if (action == "trunk")
                {
                    if (API.getVehicleDoorState(player.vehicle, 5))
                    {
                        API.setVehicleDoorState(player.vehicle, 5, false);
                    }
                    else
                    {
                        API.setVehicleDoorState(player.vehicle, 5, true);
                        //ch.sendCloseMessage(player, 10.0f, "~#C2A2DA~", API.getPlayerName(player) + " opens the trunk.");
                    }
                }
                if (action == "hood")
                {
                    if (API.getVehicleDoorState(player.vehicle, 4))
                    {
                        API.setVehicleDoorState(player.vehicle, 4, false);
                    }
                    else
                    {
                        API.setVehicleDoorState(player.vehicle, 4, true);
                        //ch.sendCloseMessage(player, 10.0f, "~#C2A2DA~", API.getPlayerName(player) + " pops the hood.");
                    }
                }
            }
            if (action == "lock")
            {
                List<NetHandle> vehicles = API.getAllVehicles();
                foreach (NetHandle vehicle in vehicles)
                {
                    Vector3 position = API.getEntityPosition(vehicle);
                    if (player.position.DistanceTo(position) <= 5.0f)
                    {
                        if (vehicleOwner.ContainsKey(vehicle))
                        {
                            if (vehicleOwner[vehicle] == player)
                            {
                                if (API.getVehicleLocked(vehicle))
                                {
                                    API.setVehicleLocked(vehicle, false);
                                    //ch.sendCloseMessage(player, 15.0f, "~#C2A2DA~", API.getPlayerName(player) + " unlocks the doors.");
                                }
                                else
                                {
                                    API.setVehicleLocked(vehicle, true);
                                    //ch.sendCloseMessage(player, 15.0f, "~#C2A2DA~", API.getPlayerName(player) + " locks the doors.");
                                }
                            }
                        }
                        else if (player.isInVehicle && API.getPlayerVehicleSeat(player) == -1)
                        {
                            if (API.getVehicleLocked(vehicle))
                            {
                                API.setVehicleLocked(vehicle, false);
                                //ch.sendCloseMessage(player, 15.0f, "~#C2A2DA~", API.getPlayerName(player) + " unlocks the doors.");
                                API.sendNotificationToPlayer(player, "~r~This isn't your car, but you unlock the doors anyway.");
                            }
                            else
                            {
                                API.setVehicleLocked(vehicle, true);
                                //ch.sendCloseMessage(player, 15.0f, "~#C2A2DA~", API.getPlayerName(player) + " locks the doors.");
                                API.sendNotificationToPlayer(player, "~r~This isn't your car, but you lock the doors anyway.");
                            }
                        }
                        else
                        {
                            API.sendNotificationToPlayer(player, "~r~This isn't your car.");
                        }
                        break;
                    }
                }
            }
        }

        [Command("seatbelt")]
        public void cmdSeatBelt(Client player)
        {
            if (player.isInVehicle)
            {
                API.setPlayerSeatbelt(player, true);
                //ch.sendCloseMessage(player, 15.0f, "~#C2A2DA~", API.getPlayerName(player) + " puts on their seatbelt.");
            }
        }

        [Command("park")] // temp
        public void cmdPark(Client player)
        {
            if (player.isInVehicle)
            {
                API.consoleOutput("In vehicle");
                if (vehicleOwner.ContainsKey(player.vehicle.handle))
                {
                    API.consoleOutput("Contains Key");
                    if (vehicleOwner[player.vehicle.handle] == player)
                    {
                        VehicleHash carPull = API.vehicleNameToModel(db.pullDatabase("PlayerVehicles", "VehicleType0", "Garage", player.name));
                        var vehicle = player.vehicle;
                        API.consoleOutput(carPull.ToString());
                        if (vehicle.model == Convert.ToInt32(carPull))
                        {
                            db.updateVehiclePosition(player, 0);
                            API.sendNotificationToPlayer(player, "Updated");
                        }
                    }
                }
            }
        }

        public void SpawnPlayerCars(Client player)
        {
            if(db.databasePlayCarSlotExists(player, 0))
            {
                vehicleOwner.Add(db.databaseSpawnPlayerCar(player, 0), player);
            }
        }
    }
}
