using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace stuykserver.Classes
{
    public class VehicleClass : Script, IDisposable
    {
        DatabaseHandler db = new DatabaseHandler();

        public VehicleClass()
        {
            API.consoleOutput("Started: Vehicle Class");
        }

        // Constructed Version
        public VehicleClass(DataRow row)
        {
            // Type, Position, Rotation * Creation
            vehicleType = Convert.ToString(row["VehicleType"]);
            vehiclePosition = new Vector3(Convert.ToSingle(row["PosX"]), Convert.ToSingle(row["PosY"]), Convert.ToSingle(row["PosZ"]));
            vehicleRotation = new Vector3(Convert.ToSingle(row["RotX"]), Convert.ToSingle(row["RotY"]), Convert.ToSingle(row["RotZ"]));
            vehicleOwner = API.getPlayerFromName(Convert.ToString(row["Nametag"]));
            vehicleIDNumber = Convert.ToInt32(row["ID"]);
            vehicle = API.createVehicle(API.vehicleNameToModel(vehicleType), vehiclePosition, vehicleRotation, 0, 0);
            vehicleID = vehicle;

            // MODS MODS MODS MODS MODS
            Spoilers = Convert.ToInt32(row["Spoilers"]);
            FrontBumper = Convert.ToInt32(row["FrontBumper"]);
            RearBumper = Convert.ToInt32(row["RearBumper"]);
            SideSkirt = Convert.ToInt32(row["SideSkirt"]);
            Exhaust = Convert.ToInt32(row["Exhaust"]);
            Frame = Convert.ToInt32(row["Frame"]);
            Grille = Convert.ToInt32(row["Grille"]);
            Hood = Convert.ToInt32(row["Hood"]);
            Fender = Convert.ToInt32(row["Fender"]);
            RightFender = Convert.ToInt32(row["RightFender"]);
            Roof = Convert.ToInt32(row["Roof"]);
            Engine = Convert.ToInt32(row["Engine"]);
            Brakes = Convert.ToInt32(row["Brakes"]);
            Transmission = Convert.ToInt32(row["Transmission"]);
            Horn = Convert.ToInt32(row["Horn"]);
            Suspension = Convert.ToInt32(row["Suspension"]);
            Armor = Convert.ToInt32(row["Armor"]);
            Turbo = Convert.ToInt32(row["Turbo"]);
            Xenon = Convert.ToInt32(row["Xenon"]);
            FrontWheels = Convert.ToInt32(row["FrontWheels"]);
            BackWheels = Convert.ToInt32(row["BackWheels"]);
            PlateHolders = Convert.ToInt32(row["PlateHolders"]);
            TrimDesign = Convert.ToInt32(row["TrimDeisgn"]);
            Ornaments = Convert.ToInt32(row["Ornaments"]);
            DialDesign = Convert.ToInt32(row["DialDesign"]);
            SteeringWheel = Convert.ToInt32(row["SteeringWheel"]);
            ShiftLever = Convert.ToInt32(row["ShiftLever"]);
            Plaques = Convert.ToInt32(row["Plaques"]);
            Hydraulics = Convert.ToInt32(row["Hydraulics"]);
            Livery = Convert.ToInt32(row["Livery"]);
            Plate = Convert.ToInt32(row["Plate"]);
            WindowTint = Convert.ToInt32(row["WindowTint"]);
            playerID = Convert.ToInt32(row["PlayerID"]);
            vehicleFuel = Convert.ToInt32(row["Fuel"]);
            
            rgb = new List<int> {
                Convert.ToInt32(row["Red"]), 
                Convert.ToInt32(row["Green"]), 
                Convert.ToInt32(row["Blue"]) 
            };

            srgb = new List<int>
            {
                Convert.ToInt32(row["sRed"]),
                Convert.ToInt32(row["sGreen"]),
                Convert.ToInt32(row["sBlue"])
            };

            vehicleCollision = API.createCylinderColShape(vehiclePosition, 3f, 3f); // Vehicle Collision
            API.setVehicleCustomPrimaryColor(vehicle, rgb[0], rgb[1], rgb[2]);
            API.setVehicleCustomSecondaryColor(vehicle, srgb[0], srgb[1], srgb[2]);
            API.setVehicleEngineStatus(vehicle, false);
            API.setVehicleLocked(vehicle, true);
            API.setVehicleMod(vehicle, 0, Spoilers);
            API.setVehicleMod(vehicle, 1, FrontBumper);
            API.setVehicleMod(vehicle, 2, RearBumper);
            API.setVehicleMod(vehicle, 3, SideSkirt);
            API.setVehicleMod(vehicle, 4, Exhaust);
            API.setVehicleMod(vehicle, 5, Frame);
            API.setVehicleMod(vehicle, 6, Grille);
            API.setVehicleMod(vehicle, 7, Hood);
            API.setVehicleMod(vehicle, 8, Fender);
            API.setVehicleMod(vehicle, 9, RightFender);
            API.setVehicleMod(vehicle, 10, Roof);
            API.setVehicleMod(vehicle, 11, Engine);
            API.setVehicleMod(vehicle, 12, Brakes);
            API.setVehicleMod(vehicle, 13, Transmission);
            API.setVehicleMod(vehicle, 14, Horn);
            API.setVehicleMod(vehicle, 15, Suspension);
            API.setVehicleMod(vehicle, 16, Armor);
            API.setVehicleMod(vehicle, 18, Turbo);
            API.setVehicleMod(vehicle, 22, Xenon);
            API.setVehicleMod(vehicle, 23, FrontWheels);
            API.setVehicleMod(vehicle, 24, BackWheels);
            API.setVehicleMod(vehicle, 25, PlateHolders);
            API.setVehicleMod(vehicle, 27, TrimDesign);
            API.setVehicleMod(vehicle, 28, Ornaments);
            API.setVehicleMod(vehicle, 30, DialDesign);
            API.setVehicleMod(vehicle, 33, SteeringWheel);
            API.setVehicleMod(vehicle, 34, ShiftLever);
            API.setVehicleMod(vehicle, 35, Plaques);
            API.setVehicleMod(vehicle, 38, Hydraulics);
            API.setVehicleMod(vehicle, 48, Livery);
            API.setVehicleMod(vehicle, 62, Plate);
            API.setVehicleMod(vehicle, 69, WindowTint);
            collisionType = "Vehicle";

            vehicleKeys = new List<Client>();
            playersInVehicle = new List<Client>();

            API.setEntityRotation(vehicle, vehicleRotation);
            API.setEntityPosition(vehicle, vehiclePosition);
            API.setEntityData(vehicle, "VehicleID", vehicleIDNumber);

            fuelTimer = new Timer();
            fuelTimer.Interval = 10000;
            fuelTimer.Enabled = true;
            fuelTimer.Elapsed += FuelTimer_Elapsed;
        }

        private void FuelTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!vehicle.engineStatus)
            {
                return;
            }

            if (vehicleFuel <= 0)
            {
                vehicle.engineStatus = false;
                return;
            }

            if (vehicle.position.DistanceTo(vehiclePosition) < 5f)
            {
                vehiclePosition = vehicle.position;
                vehicleFuel -= 0.08;
            }
            else
            {
                vehiclePosition = vehicle.position;
                vehicleFuel -= 0.1;
            }

            Client[] occupants = API.getVehicleOccupants(vehicle);
            
            foreach (Client occupant in occupants)
            {
                if (API.getPlayerVehicleSeat(occupant) == -1)
                {
                    API.triggerClientEvent(occupant, "updateFuel", vehicleFuel);
                    break;
                }
            }
        }

        Timer fuelTimer;
        Vehicle vehicle;
        NetHandle vehicleID;
        int vehicleIDNumber;
        ColShape vehicleCollision;
        Vector3 vehiclePosition;
        Vector3 vehicleRotation;
        List<Client> vehicleKeys;
        Client vehicleOwner;
        int playerID; // Vehicle Owner
        double vehicleFuel;
        List<Client> playersInVehicle; // Probably don't need this.
        string vehicleType;
        string collisionType;
        Timer deathTimer;

        // Vehicle Mods
        int Spoilers; // 0
        int FrontBumper; // 1
        int RearBumper; // 2
        int SideSkirt; // 3
        int Exhaust; // 4 
        int Frame; // 5
        int Grille; // 6
        int Hood; // 7
        int Fender; // 8
        int RightFender; // 9
        int Roof; // 10
        int Engine; // 11
        int Brakes; // 12
        int Transmission; // 13
        int Horn; // 14
        int Suspension; // 15
        int Armor; // 16
        int Turbo; // 18 
        int Xenon; // 22
        int FrontWheels; // 23
        int BackWheels; // 24
        int PlateHolders; // 25
        int TrimDesign; // 27
        int Ornaments; // 28
        int DialDesign; // 30
        int SteeringWheel; // 33
        int ShiftLever; // 34
        int Plaques; // 35
        int Hydraulics; // 38
        int Livery; // 48
        int Plate; // 62
        int WindowTint; //69

        // Custom Colors
        List<int> rgb; // First
        List<int> srgb; // Second

        public NetHandle returnVehicleHandle()
        {
            return vehicleID;
        }

        public void saveVehiclePosition()
        {
            string[] varNames = { "PosX", "PosY", "PosZ", "RotX", "RotY", "RotZ", "Fuel" };
            string before = "UPDATE PlayerVehicles SET";
            object[] data = { API.getEntityPosition(vehicleID).X, API.getEntityPosition(vehicleID).Y, API.getEntityPosition(vehicleID).Z, API.getEntityRotation(vehicleID).X, API.getEntityRotation(vehicleID).Y, API.getEntityRotation(vehicleID).Z, vehicleFuel };
            string after = string.Format("WHERE ID='{0}'", vehicleIDNumber);

            db.compileQuery(before, after, varNames, data);

            API.sendNotificationToPlayer(vehicleOwner, "~o~Vehicle Position Saved");
        }

        public int returnVehicleIDNumber()
        {
            return vehicleIDNumber;
        }

        public string returnCollisionType()
        {
            return collisionType;
        }

        public string returnType()
        {
            return vehicleType;
        }

        public int returnOwnerID()
        {
            return playerID;
        }

        public void setOwnerID(int i)
        {
            playerID = i;
        }

        public double returnFuel()
        {
            return vehicleFuel;
        }

        public void addFuel(int amount)
        {
            vehicleFuel += amount;

            if (vehicleFuel >= 100)
            {
                vehicleFuel = 100;
            }
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

        public void deleteCollision()
        {
            API.deleteColShape(vehicleCollision);
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
            fuelTimer.Dispose();
            saveVehiclePosition();
            API.deleteEntity(vehicleID);
            vehicleKeys.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
