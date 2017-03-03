using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Util
{
    public class HouseHandler : Script
    {
        Dictionary<ColShape, HouseInformation> houseInformation = new Dictionary<ColShape, HouseInformation>();

        public enum HouseType
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
            apa_v_mp_h_07_a,
            apa_v_mp_h_07_c,
            apa_v_mp_h_07_b,
            apa_v_mp_h_08_a,
            apa_v_mp_h_08_b,
            apa_v_mp_h_08_c
        }

        int currentDimension = 10;

        public class HouseInformation
        {
            int houseID;
            ColShape houseCollision;
            string houseOwner; // The owner of the house. (player.name)
            HouseType houseType; // Type of Apartment Interior.
            List<Client> houseKeys; // List of players who currently own house keys for this session.
            List<Client> housePlayersOutside;
            List<Client> housePlayersInside;
            ColShape houseExit;
            Vector3 housePosition; // Set to housing Position.
            bool houseSale; // Set to FALSE if not for sale.
            int houseDimension;

            public void setupHouse(ColShape collision, int id, string player, HouseType type, Vector3 position, bool forSale, int dimension)
            {
                houseCollision = collision;
                houseOwner = player;
                houseType = type;
                housePosition = position;
                houseSale = forSale;
                houseID = id;
                houseKeys = new List<Client>();
                housePlayersOutside = new List<Client>();
                housePlayersInside = new List<Client>();
                houseDimension = dimension;
            }

            public ColShape returnHouseExit()
            {
                return houseExit;
            }

            public void setHouseExit(ColShape collision)
            {
                houseExit = collision;
            }

            public ColShape returnCollision()
            {
                return houseCollision;
            }

            public string returnOwner()
            {
                return houseOwner;
            }

            public void setOwner(string player)
            {
                houseOwner = player;
            }

            public int returnDimension()
            {
                return houseDimension;
            }

            public HouseType returnType()
            {
                return houseType;
            }

            public void setType(HouseType type)
            {
                houseType = type;
            }

            public List<Client> returnPlayersInside()
            {
                return housePlayersInside;
            }

            public void addPlayersInside(Client player)
            {
                if (!housePlayersInside.Contains(player))
                {
                    housePlayersInside.Add(player);
                }
            }

            public void removePlayersInside(Client player)
            {
                if (housePlayersInside.Contains(player))
                {
                    housePlayersInside.Remove(player);
                }
            }


            public List<Client> returnHouseKeys()
            {
                return houseKeys;
            }

            public void addHouseKeys(Client player)
            {
                if (!houseKeys.Contains(player))
                {
                    houseKeys.Add(player);
                }
            }

            public void removeHouseKeys(Client player)
            {
                if (houseKeys.Contains(player))
                {
                    houseKeys.Remove(player);
                }
            }

            public List<Client> returnPlayersOutside()
            {
                return housePlayersOutside;
            }

            public void addPlayerOutside(Client player)
            {
                if (!housePlayersOutside.Contains(player))
                {
                    housePlayersOutside.Add(player);
                }
            }

            public void removePlayerOutside(Client player)
            {
                if (housePlayersOutside.Contains(player))
                {
                    housePlayersOutside.Remove(player);
                }
            }

            public Vector3 returnPosition()
            {
                return housePosition;
            }

            public bool returnForSale()
            {
                return houseSale;
            }

            public void setForSale(bool value)
            {
                houseSale = value;
            }

            public int returnID()
            {
                return houseID;
            }
        }

        public HouseHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (houseInformation.ContainsKey(colshape))
                {
                    API.triggerClientEvent(player, "removeUseFunction", "House");
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (houseInformation.ContainsKey(colshape) && !player.isInVehicle)
                {
                    houseInformation[colshape].addPlayerOutside(player);
                    API.triggerClientEvent(player, "triggerUseFunction", "House");
                    // If they have keys, tell them.
                    if (houseInformation[colshape].returnOwner() == player.name || houseInformation[colshape].returnHouseKeys().Contains(player))
                    {
                        API.sendNotificationToPlayer(player, "I have keys for this place.");
                    }
                    else if (houseInformation[colshape].returnForSale())
                    {
                        API.sendNotificationToPlayer(player, "~g~This house is for sale.");
                    }
                    
                }
                else
                {
                    if (player.dimension > 9)
                    {
                        foreach (ColShape collision in houseInformation.Keys)
                        {
                            if (houseInformation[collision].returnPlayersInside().Contains(player))
                            {
                                API.triggerClientEvent(player, "triggerUseFunction", "House");
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void API_onResourceStart()
        {
            string query = string.Format("SELECT * FROM PlayerHousing");
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initialized = 0;

            foreach (DataRow row in result.Rows)
            {
                int id = Convert.ToInt32(row["ID"]);
                string player = row["Owner"].ToString();
                bool forSale = Convert.ToBoolean(row["ForSale"]);
                Vector3 position = new Vector3(Convert.ToSingle(row["PosX"]), Convert.ToSingle(row["PosY"]), Convert.ToSingle(row["PosZ"]));
                HouseType type = (HouseType) Enum.Parse(typeof(HouseType), row["Type"].ToString());
                API.consoleOutput(type.ToString());
                positionBlips(player, position, id, forSale, type);
                ++initialized;
            }

            API.consoleOutput("Houses Intialized: " + initialized.ToString());
        }

        public void positionBlips(string player, Vector3 position, int id, bool forSale, HouseType type)
        {
            HouseInformation newHouse = new HouseInformation();
            ColShape shape = API.createCylinderColShape(position, 3f, 2f);

            var newBlip = API.createBlip(position);
            if (forSale == false)
            {
                API.setBlipSprite(newBlip, 40);
            }
            else
            {
                API.setBlipSprite(newBlip, 374);
            }
            
            API.setBlipColor(newBlip, 37);

            currentDimension += 1;

            newHouse.setupHouse(shape, id, player, type, position, forSale, currentDimension);
            houseInformation.Add(shape, newHouse);
        }

        public void actionHouseControl(Client player)
        {
            foreach (ColShape collision in houseInformation.Keys)
            {
                if (houseInformation[collision].returnPlayersOutside().Contains(player) && !player.isInVehicle)
                {
                    // IF THIS IS FOR SALE / ELSE
                    if (houseInformation[collision].returnForSale())
                    {
                        actionOpenHouseSale(player);
                        break;
                    }
                    else
                    {
                        if (houseInformation[collision].returnOwner() == player.name || houseInformation[collision].returnHouseKeys().Contains(player))
                        {
                            actionEnterHouse(player, collision);
                            break;
                        }
                    }
                }
                else
                {
                    if (houseInformation[collision].returnPlayersInside().Contains(player))
                    {
                        actionLeaveHouse(player, collision);
                        break;
                    }
                }
            }
        }

        public void actionOpenHouseSale(Client player)
        {
            API.sendNotificationToPlayer(player, "Totally for sale.");
            // SALE ACTION HERE
        }

        public void actionLeaveHouse(Client player, ColShape collision)
        {
            houseInformation[collision].removePlayersInside(player);
            API.setEntityPosition(player, houseInformation[collision].returnPosition());
            API.setEntityDimension(player, 0);
        }

        [Command("gotoHouse")]
        public void cmdGOTOHOUSE(Client player)
        {
            API.setEntityPosition(player, new Vector3(-1452.51, -653.32, 29.5831));
        }

        [Command("givekeys")]
        public void cmdGiveKeys(Client player, string target)
        {
            foreach (ColShape collision in houseInformation.Keys)
            {
                if (houseInformation[collision].returnOwner() == player.name)
                {
                    houseInformation[collision].addHouseKeys(API.getPlayerFromName(target));
                }
            }
        }

        public void actionEnterHouse(Client player, ColShape collision)
        {
            HouseType type = houseInformation[collision].returnType();

            switch (type)
            {
                case HouseType.apa_v_mp_h_01_a:

                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.8663, 315.7642, 217.6385));
                    API.setEntityDimension(player, houseInformation[collision].returnDimension());
                    houseInformation[collision].removePlayerOutside(player);
                    houseInformation[collision].addPlayersInside(player);
                    houseInformation[collision].setHouseExit(API.createCylinderColShape(new Vector3(-786.8663, 315.7642, 217.6385), 3f, 2f));
                    break;
            }
        }
    }
}
