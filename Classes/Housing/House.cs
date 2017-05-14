using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class House : Script, IDisposable
    {
        DatabaseHandler db = new DatabaseHandler();

        public House()
        {
            // Do nothing.
        }

        public House(DataRow row)
        {
            houseID = Convert.ToInt32(row["ID"]);
            housePrice = Convert.ToInt32(row["Price"]);
            houseOwner = Convert.ToInt32(row["PlayerID"]);
            houseDimension = Convert.ToInt32(row["ID"]);
            houseOwnerName = Convert.ToString(row["Owner"]);
            forSale = Convert.ToBoolean(row["ForSale"]);
            locked = Convert.ToBoolean(row["Locked"]);
            houseEntry = new Vector3(Convert.ToSingle(row["PosX"]), Convert.ToSingle(row["PosY"]), Convert.ToSingle(row["PosZ"]));
            houseExit = new Vector3(Convert.ToSingle(row["ExitX"]), Convert.ToSingle(row["ExitY"]), Convert.ToSingle(row["ExitY"]));
            houseType = (Houses)Convert.ToInt32(row["Type"]);
            houseStatus = "House";
            setupCollision();
        }

        public Dictionary<Houses, Vector3> HouseLocations = new Dictionary<Houses, Vector3>()
        {
            { Houses.apa_v_mp_h_01_a, new Vector3 (-786.8663,315.7642,217.6385) },
            { Houses.apa_v_mp_h_01_c, new Vector3 (-786.9563,315.6229,187.9136) },
            { Houses.apa_v_mp_h_01_b, new Vector3 (-774.0126,342.0428,196.6864) },
            { Houses.apa_v_mp_h_02_a, new Vector3 (-787.0749,315.8198,217.6386) },
            { Houses.apa_v_mp_h_02_c, new Vector3 (-786.8195,315.5634,187.9137) },
            { Houses.apa_v_mp_h_02_b, new Vector3 (-774.1382,342.0316,196.6864) },
            { Houses.apa_v_mp_h_03_a, new Vector3 (-786.6245,315.6175,217.6385) },
            { Houses.apa_v_mp_h_03_c, new Vector3 (-786.9584,315.7974,187.1350) },
            { Houses.apa_v_mp_h_03_b, new Vector3 (-774.0223,342.1718,196.8630) },
            { Houses.apa_v_mp_h_04_a, new Vector3 (-787.0902,315.7039,217.6384) },
            { Houses.apa_v_mp_h_04_c, new Vector3 (-787.0155,315.7071,187.9135) },
            { Houses.apa_v_mp_h_04_b, new Vector3 (-773.8976,342.1525,196.6863) },
            { Houses.apa_v_mp_h_05_a, new Vector3 (-786.9887,315.7393,217.6386) },
            { Houses.apa_v_mp_h_05_c, new Vector3 (-786.8809,315.6634,187.9136) },
            { Houses.apa_v_mp_h_05_b, new Vector3 (-774.0675,342.0773,196.6864) },
            { Houses.apa_v_mp_h_06_a, new Vector3 (-787.1423,315.6943,217.6384) },
            { Houses.apa_v_mp_h_06_c, new Vector3 (-787.0961,315.8150,187.1350) },
            { Houses.apa_v_mp_h_06_b, new Vector3 (-773.9552,341.9892,196.6862) },
            { Houses.apa_v_mp_h_07_a, new Vector3 (-787.0290,315.7113,217.6385) },
            { Houses.apa_v_mp_h_07_c, new Vector3 (-787.0574,315.6567,187.9135) },
            { Houses.apa_v_mp_h_07_b, new Vector3 (-774.0109,342.0965,196.6863) },
            { Houses.apa_v_mp_h_08_a, new Vector3 (-786.9469,315.5655,217.6383) },
            { Houses.apa_v_mp_h_08_c, new Vector3 (-786.9756,315.7230,187.9134) },
            { Houses.apa_v_mp_h_08_b, new Vector3 (-774.0349,342.0296,196.6862) }
        };

        public enum Houses
        {
            apa_v_mp_h_01_a,
            apa_v_mp_h_01_c,
            apa_v_mp_h_01_b,
            apa_v_mp_h_02_a,
            apa_v_mp_h_02_c,
            apa_v_mp_h_02_b,
            apa_v_mp_h_03_a,
            apa_v_mp_h_03_c,
            apa_v_mp_h_03_b,
            apa_v_mp_h_04_a,
            apa_v_mp_h_04_b,
            apa_v_mp_h_04_c,
            apa_v_mp_h_05_a,
            apa_v_mp_h_05_c,
            apa_v_mp_h_05_b,
            apa_v_mp_h_06_a,
            apa_v_mp_h_06_b,
            apa_v_mp_h_06_c,
            apa_v_mp_h_07_a,
            apa_v_mp_h_07_c,
            apa_v_mp_h_07_b,
            apa_v_mp_h_08_a,
            apa_v_mp_h_08_b,
            apa_v_mp_h_08_c
        }

        public enum HouseStatus
        {
            ForSale,
            House
        }

        int houseID;
        int housePrice;
        int houseOwner;
        int houseDimension;
        string houseOwnerName;
        Houses houseType;
        string houseStatus;
        Blip houseBlip;
        ColShape houseEntryCollision;
        ColShape houseExitCollision;
        Vector3 houseEntry; // Based on player position.
        Vector3 houseExit; // Based on interior position.
        bool forSale;
        bool locked;

        public void setupCollision()
        {
            // House Entrance
            houseEntryCollision = API.createCylinderColShape(houseEntry, 1f, 1f);
            houseEntryCollision.setData("Type", "House");
            houseEntryCollision.setData("Instance", this);

            // Blip
            houseBlip = API.createBlip(houseEntry);
            API.setBlipShortRange(houseBlip, true);

            // House Exit
            houseExit = HouseLocations[houseType];
            houseExitCollision = API.createCylinderColShape(houseExit, 1f, 1f);
            houseExitCollision.setData("Type", "House");
            houseExitCollision.setData("Instance", this);

            // Change Blip Marker
            if (forSale)
            {
                API.setBlipSprite(houseBlip, 374);
            }
            else
            {
                API.setBlipSprite(houseBlip, 40);
            }
        }

        public void saveHouse()
        {
            string[] varNames = { "PlayerID", "ForSale", "Price", "Locked" };
            string before = "UPDATE PlayerHousing SET";
            object[] data = { houseOwner, forSale.ToString(), housePrice.ToString(), locked.ToString() };
            string after = string.Format("WHERE ID='{0}'", houseID);

            // Send all our data to generate the query and run it
            db.compileQuery(before, after, varNames, data);
        }

        public void changeHouseOwnership(Client player)
        {
            houseOwner = Convert.ToInt32(API.getEntityData(player, "PlayerID"));
            setForSale(false);
            housePrice = 10000000;

            saveHouse();

            API.consoleOutput("House ownership changed for HouseID: {0}", houseID);
        }

        public void setupForSale(int price)
        {
            setForSale(true);
            housePrice = price;

            saveHouse();
        }

        public bool returnLocked()
        {
            return locked;
        }

        public void setLockStatus(bool value)
        {
            locked = value;
        }

        public int returnID()
        {
            return houseID;
        }

        public int returnHousePrice()
        {
            return housePrice;
        }

        public int returnHouseOwner()
        {
            return houseOwner;
        }

        public string returnOwnerName()
        {
            return houseOwnerName;
        }

        public Houses returnType()
        {
            return houseType;
        }

        public ColShape returnEntryCollision()
        {
            return houseEntryCollision;
        }

        public ColShape returnExitCollision()
        {
            return houseExitCollision;
        }

        public Vector3 returnEntryPosition()
        {
            return houseEntry;
        }

        public Vector3 returnExitPosition()
        {
            return houseExit;
        }

        public bool returnForSale()
        {
            return forSale;
        }

        public void setForSale(bool value)
        {
            if (value)
            {
                API.setBlipSprite(houseBlip, 374);
                forSale = value;
            }
            else
            {
                API.setBlipSprite(houseBlip, 40);
                forSale = value;
            }

            saveHouse();
        }

        public void setHouseOwner(int id)
        {
            houseOwner = id;
        }

        public void setHouseOwnerName(Client player)
        {
            houseOwnerName = player.name;
        }

        public void setHouseType(Houses type)
        {
            houseType = type;
        }

        public string returnHouseStatus()
        {
            return houseStatus;
        }

        public void Dispose()
        {
            API.deleteEntity(houseBlip);
            API.deleteColShape(houseEntryCollision);
            API.deleteColShape(houseExitCollision);
            GC.SuppressFinalize(this);
        }
    }
}
