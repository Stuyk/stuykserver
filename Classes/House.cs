using GTANetworkServer;
using GTANetworkShared;
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
            houseEntry = new Vector3(Convert.ToSingle(row["PosX"]), Convert.ToSingle(row["PosY"]), Convert.ToSingle(row["PosZ"]));
            houseType = (Houses)Enum.Parse(typeof(Houses), Convert.ToString(row["Type"]));
            setupCollision();
        }

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
        HouseStatus houseStatus;
        Blip houseBlip;
        ColShape houseEntryCollision;
        ColShape houseExitCollision;
        Vector3 houseEntry; // Based on player position.
        Vector3 houseExit; // Based on interior position.
        bool forSale;

        public void setupCollision()
        {
            houseEntryCollision = API.createCylinderColShape(houseEntry, 2f, 2f);
            houseBlip = API.createBlip(houseEntry);
            API.setBlipShortRange(houseBlip, true);

            if (forSale)
            {
                API.setBlipSprite(houseBlip, 374);
                houseStatus = HouseStatus.ForSale;
            }
            else
            {
                API.setBlipSprite(houseBlip, 40);
                houseStatus = HouseStatus.House;
            }
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
                houseStatus = HouseStatus.ForSale;
            }
            else
            {
                API.setBlipSprite(houseBlip, 40);
                forSale = value;
                houseStatus = HouseStatus.House;
            }
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

        public HouseStatus returnHouseStatus()
        {
            return houseStatus;
        }

        public void setHouseStatus(HouseStatus type)
        {
            houseStatus = type;
        }

        public void Dispose()
        {
            //Dispose Code
            GC.SuppressFinalize(this);
        }
    }
}
