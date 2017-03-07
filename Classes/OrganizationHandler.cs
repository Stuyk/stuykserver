using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class OrganizationHandler : Script, IDisposable
    {
        public OrganizationHandler()
        {
            primaryJobAddons = new List<JobAddons>();
            primaryModularAddons = new List<ModularAddons>();
            organizationEmployees = new Dictionary<string, int>();
            organizationRanks = new Dictionary<int, string>();
        }

        // #############################
        // #### Organization Information
        // #############################
        string organizationOwner;
        Dictionary<string, int> organizationEmployees; // player.name * int rank
        Dictionary<int, string> organizationRanks; // Rank # - String Rank / 0 is lowest.

        // Organization Owner
        public void setOrganizationOwner(string name)
        {
            organizationOwner = name;
        }

        public string returnOrganizationOwner()
        {
            return organizationOwner;
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
            Standard, // Pool Cue, Switchblade, Golf Club, Flashlight, Pipe Wrench
            Utility, // Baseball Bat, Crowbar, Golf Club, Hammer, Knife, Hatchet, Machete, Flashlight, Pipe Wrench
            MeleeOnly, // Baseballbat, Golf Club, Crowbar, Hammer, Bottle, Knuckle, Machete, Switchblade, Pool Cue
            Police, // Flare Gun, Heavy Revolver, Pump Shotgun, Assault Shotgun, Carbine, Baton, Stun Gun
            SelfDefense, // Knife, Pistol, Duster, Switchblade, Pool Cue 
            Explosive, // RPG, Grenade, Molotov, Sticky Bomb
            GangStandard, // Double Barrel, Pistol, Micro SMG, Mini SMG, Compact Rifle
            OutOfTime, // Hatchet, BattleAxe, Musket
            Militia // Marksman, Sniper, Advanced Rifle
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
            UtilityVehicles // Farming, Tow Truck, Caddies, etc.
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
            CashDelivery, // PVP - Deliver cash between banks. [+Karma]
            CashDeliveryHeist, // PVP - Stop delivery of cash between banks. [-Karma]
            TruckDriving, // Delivery Driver - Point to Point Bullshit. [+Karma]
            VehicleHeist, // Target vehicles, steal them, deliver to warehouses. [-Karma]
            VehiclePartsHeist, // Target Emergency / Police vehicles. Steal parts. Sell to convenience stores. [-Karma]
            VehicleReposession, // Target player vehicles via police. [+Karma]
            Mechanic, // Fix cars, buff parts. [Neutral]
            Lift, // Need a lyft? General Taxi / Bus Corps [Neutral]
            EMT, // Recieve distress signals. [+Karma]
            BlackEMT, // Recieve downed person notifications. #OrganHarvest [-Karma]
            Police, // Recieve civilain distress signals. [+Karma]
            Grinder, // Recieve hits from gangs. Bounty Board System. [-Karma]
            PrisonTransfer, // PVP - Transfer Prisoners from Police to Prison. [+Karma]
            PrisonHeist, // PVP - Stop that bus! [-Karma]
            PinkSlip, // Start Races for Pink Slips [-Karma]
            Government, // Controls Taxrate of Goods [-Karma]
            Bartender, // Get Turnt [Neutral]
            Hooker, // Because why not? [Neutral]
            Farming, // Do some crop stuff. [+Karma]
            Lumberjack, // Do some woodsman stuff. [+Karma]
            Hunting // PVP - Hunter vs. Animals (You Guys) [Neutral]
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
