using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class Organization : Script, IDisposable
    {
        public Organization()
        {
            API.consoleOutput("Started: Organization");
        }

        public Organization(DataRow row)
        {
            // Initialize Lists
            primaryJobAddons = new List<JobAddons>();
            primaryModularAddons = new List<ModularAddons>();
            organizationEmployees = new Dictionary<string, int>();
            organizationRanks = new Dictionary<int, string>();

            // Set Primary
            primary = (Primary)Convert.ToInt32(row["Primary"]);
            primaryWeaponAddon = (WeaponAddons)Convert.ToInt32(row["WeaponAddon"]);
            primaryVehicleAddon = (VehicleAddons)Convert.ToInt32(row["VehicleAddon"]);

            // Initialize Basic Information
            organizationID = Convert.ToInt32(row["ID"]);
            organizationOwner = Convert.ToInt32(row["PlayerID"]);
            organizationName = Convert.ToString(row["Name"]);
            organizationBank = Convert.ToInt32(row["Bank"]);
            organizationUnits = Convert.ToInt32(row["Units"]);
            organizationKarma = Convert.ToInt32(row["Karma"]);
            API.consoleOutput("[{0}] {1} added with ${2}, {3} Units, {4} Karma. Type: {5}", organizationID, organizationName, organizationBank, organizationUnits, organizationKarma, primary.ToString());

            // Initialize Jobs
            object[] jobs = Convert.ToString(row["JobAddons"]).Split(',');
            for (int i = 0; i < jobs.Length; i++)
            {
                primaryJobAddons.Add((JobAddons)Convert.ToInt32(jobs[i]));
                API.consoleOutput("Added Job: {0}", primaryJobAddons[i].ToString());
            }

            // Initialize Modulars
            object[] modulars = Convert.ToString(row["ModularAddons"]).Split(',');
            for (int i = 0; i < modulars.Length; i++)
            {
                primaryModularAddons.Add((ModularAddons)Convert.ToInt32(modulars[i]));
                API.consoleOutput("Added Modular: {0}", primaryModularAddons[i].ToString());
            }

            // Messages and Such
            organizationMessage = organizationName = Convert.ToString(row["Message"]);
        }

        // #############################
        // #### Organization Information
        // #############################
        int organizationID;
        int organizationOwner;
        string organizationName;
        int organizationBank;
        int organizationUnits;
        int organizationKarma;
        Dictionary<string, int> organizationEmployees; // player.name * int rank
        Dictionary<int, string> organizationRanks; // Rank # - String Rank / 0 is lowest.
        string organizationMessage;

        // Set Organization Message
        public void setOrganizationMessage(string message)
        {
            organizationMessage = message;
        }

        public string returnOrganizationMessage()
        {
            return organizationMessage;
        }

        // Set Organization ID
        public void setOrganizationID(int id)
        {
            organizationID = id;
        }

        // Organization Owner
        public void setOrganizationOwner(int playerid)
        {
            organizationOwner = playerid;
        }

        public int returnOrganizationOwner()
        {
            return organizationOwner;
        }

        // Organization Name
        public void setOrganizationName(string name)
        {
            organizationName = name;
        }

        public string returnOrganizationName()
        {
            return organizationName;
        }

        // Organization Bank
        public void setOrganizationBank(int value)
        {
            organizationBank = value;
        }

        public void addOrganizationBank(int value)
        {
            if (value > 0)
            {
                organizationBank += value;
            }
        }

        public void removeOrganizationBank(int value)
        {
            if (value > 0)
            {
                organizationBank -= value;
            }
        }

        public int returnOrganizationBank()
        {
            return organizationUnits;
        }

        // Organization Units
        public void setOrganizationUnits(int value)
        {
            organizationUnits = value;
        }

        public void addOrganizationUnits(int value)
        {
            if (value > 0)
            {
                organizationUnits += value;
            }
        }

        public void removeOrganizationUnits(int value)
        {
            if (value > 0)
            {
                organizationUnits -= value;
            }
        }

        public int returnOrganizationUnits()
        {
            return organizationUnits;
        }

        // Organization Karma
        public void setOrganizationKarma(int value)
        {
            organizationKarma = value;
        }

        public void addOrganizationKarma(int value)
        {
            if (value > 0)
            {
                organizationKarma += value;
            }
        }

        public void removeOrganizationKarma(int value)
        {
            if (value > 0)
            {
                organizationKarma -= value;
            }
        }

        public int returnOrganizationKarma()
        {
            return organizationKarma;
        }


        // Organization Employees
        public void addOrganizationEmployee(Client player)
        {
            if (!organizationEmployees.ContainsKey(player.name))
            {
                organizationEmployees.Add(player.name, 0); // Set to lowest rank by default.
            }
        }

        public void removeOrganizationEmployee(string player)
        {
            if (organizationEmployees.ContainsKey(player))
            {
                organizationEmployees.Remove(player);
            }
        }

        public void clearOrganizationEmployees()
        {
            organizationEmployees.Clear();
        }

        public void setOrganizationEmployeeRank(Client player, int rank)
        {
            if (organizationEmployees.ContainsKey(player.name))
            {
                if (rank <= organizationRanks.Count - 1)
                {
                    organizationEmployees[player.name] = rank;
                }
            }
        }

        public Dictionary<string, int> returnOrganizationEmployees()
        {
            return organizationEmployees;
        }

        // Organization Ranks
        public void addOrganizationRank(int rank, string name)
        {
            if (!organizationRanks.ContainsKey(rank))
            {
                organizationRanks.Add(rank, name);
            }
            else
            {
                organizationRanks[rank] = name;
            }
        }

        public void setOrganizationRank(int rank, string name)
        {
            if (organizationRanks.ContainsKey(rank))
            {
                organizationRanks[rank] = name;
            }
        }

        public void removeOrganizationRank(int rank)
        {
            if (organizationRanks.ContainsKey(rank))
            {
                organizationRanks.Remove(rank);
            }
        }

        public Dictionary<int, string> returnOrganizationRanks()
        {
            return organizationRanks;
        }


        // #############################
        // #### PRIMARY ADDONS
        // #############################
        public enum Primary
        {
            StreetGang, // Primary for Street Gangs [-Karma]
            PrisonGang, // Primary for Prison Gangs [-Karma]
            Mafia, // Primary for Mafia Gangs [-Karma]
            Racing, // Primary for Car Clubs. [+Karma / -Karma]
            Service, // Mechanics, Taxis, Towing etc. [+Karma]
            BlackService, // Hitmen, Towing, etc. [-Karma]
            PoliceService, // Police Factions, Prison [+Karma]
            MilitaryService, // Military Factions [+Karma]
            BlackMilitary, // Private Military [-Karma]
            MedicalService, // Firefighters, EMS, EMT, etc. [+Karma]
            BlackMedical, // Harvesting, Organ Selling [-Karma]
            Biker // Exactly how it sounds. [+Karma / -Karma]
        }

        Primary primary;

        public void setPrimaryFunction(Primary type)
        {
            primary = type;
        }

        public Primary returnPrimaryFunction()
        {
            return primary;
        }

        // #############################
        // #### WEAPON ADDONS
        // #############################
        public enum WeaponAddons
        {
            Standard, // Pool Cue, Switchblade, Golf Club, Flashlight, Pipe Wrench, Pistol
            Utility, // Baseball Bat, Crowbar, Golf Club, Hammer, Knife, Hatchet, Machete, Flashlight, Pipe Wrench
            MeleeOnly, // Baseballbat, Golf Club, Crowbar, Hammer, Bottle, Knuckle, Machete, Switchblade, Pool Cue
            Police, // Flare Gun, Heavy Revolver, Pump Shotgun, Assault Shotgun, Carbine, Baton, Stun Gun
            SelfDefense, // Knife, Pistol, Duster, Switchblade, Pool Cue 
            Explosive, // RPG, Grenade, Molotov, Sticky Bomb
            GangStandard, // Double Barrel, Pistol, Micro SMG, Mini SMG, Compact Rifle
            OutOfTime, // Hatchet, BattleAxe, Musket
            Militia, // Marksman, Sniper, Advanced Rifle
            None
        }

        WeaponAddons primaryWeaponAddon;

        public void setPrimaryWeaponAddon(WeaponAddons type)
        {
            primaryWeaponAddon = type;
        }

        public WeaponAddons returnPrimaryWeaponAddon()
        {
            return primaryWeaponAddon;
        }

        // #############################
        // #### VEHICLE ADDONS
        // #############################
        public enum VehicleAddons
        {
            PoliceVehicles, // Police
            CommercialVehicles, // Trucking + Trailers
            EmergencyVehicles, // EMT and such.
            MilitaryVehicles, // Military
            IndustrialVehicles, // Construction
            PlaneVehicles, // Pedestrian Planes
            ServiceVehicles, // Busses, Taxis, Trash, etc.
            UtilityVehicles, // Farming, Tow Truck, Caddies, etc.
            None
        }

        VehicleAddons primaryVehicleAddon;

        public void setPrimaryVehicleAddon(VehicleAddons type)
        {
            primaryVehicleAddon = type;
        }

        public VehicleAddons returnPrimaryVehicleAddon()
        {
            return primaryVehicleAddon;
        }

        // #############################
        // #### JOB ADDONS
        // #############################
        public enum JobAddons
        {
            CashDelivery = 0, // PVP - Deliver cash between banks. [+Karma]
            CashDeliveryHeist = 1, // PVP - Stop delivery of cash between banks. [-Karma]
            TruckDriving = 2, // Delivery Driver - Point to Point Bullshit. [+Karma]
            VehicleHeist = 3, // Target vehicles, steal them, deliver to warehouses. [-Karma]
            VehiclePartsHeist = 4, // Target Emergency / Police vehicles. Steal parts. Sell to convenience stores. [-Karma]
            VehicleReposession = 5, // Target player vehicles via police. [+Karma]
            Mechanic = 6, // Fix cars, buff parts. [Neutral]
            Lift = 7, // Need a lyft? General Taxi / Bus Corps [Neutral]
            EMT = 8, // Recieve distress signals. [+Karma]
            BlackEMT = 9, // Recieve downed person notifications. #OrganHarvest [-Karma]
            Police = 10, // Recieve civilain distress signals. [+Karma]
            Grinder = 11, // Recieve hits from gangs. Bounty Board System. [-Karma]
            PrisonTransfer = 12, // PVP - Transfer Prisoners from Police to Prison. [+Karma]
            PrisonHeist = 13, // PVP - Stop that bus! [-Karma]
            PinkSlip = 14, // Start Races for Pink Slips [-Karma]
            Government = 15, // Controls Taxrate of Goods [-Karma]
            Bartender = 16, // Get Turnt [Neutral]
            Hooker = 17, // Because why not? [Neutral]
            Farming = 18, // Do some crop stuff. [+Karma]
            Lumberjack = 19, // Do some woodsman stuff. [+Karma]
            Hunting = 20 // PVP - Hunter vs. Animals (You Guys) [Neutral]
        }

        List<JobAddons> primaryJobAddons;

        public void addPrimaryJobAddon(JobAddons type)
        {
            if (!primaryJobAddons.Contains(type))
            {
                primaryJobAddons.Add(type);
            }
        }

        public void removePrimaryJobAddon(JobAddons type)
        {
            if (primaryJobAddons.Contains(type))
            {
                primaryJobAddons.Remove(type);
            }
        }

        public List<JobAddons> returnPrimaryJobAddons()
        {
            return primaryJobAddons;
        }

        // #############################
        // #### MODULAR ADDONS
        // #############################
        public enum ModularAddons
        {
            Radio, // Access to a group specific radio, but no Radio Scanner.
            RadioScanner, // Scan other radios, but no access to Radio.
            RadioEncrypt, // Radio Encryption - Scanner makes it undetected.
            Mask, // Access to masks, hide your identity.
            PoliceTraining, // Access to Police Actions [Arresting, Pulling, etc.]
            BulletVests, // Access to Bulletproof Vests
            ServiceSkins, // Access to set to a Service Skin [Military & Police & Service Only]
            MechanicShop, // Access to Vehicle Engine Upgrades [Racing & Service Only]
            DrugHandling // Anything Drug Related
        }

        List<ModularAddons> primaryModularAddons;

        public void addPrimaryModularAddon(ModularAddons type)
        {
            if (!primaryModularAddons.Contains(type))
            {
                primaryModularAddons.Add(type);
            }
        }

        public void removePrimaryModularAddon(ModularAddons type)
        {
            if (primaryModularAddons.Contains(type))
            {
                primaryModularAddons.Remove(type);
            }
        }

        public List<ModularAddons> returnPrimaryModularAddons()
        {
            return primaryModularAddons;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
