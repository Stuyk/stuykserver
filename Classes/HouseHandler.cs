using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
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
        Dictionary<ColShape, ShopInformationHandling> houseInformation = new Dictionary<ColShape, ShopInformationHandling>();
        DatabaseHandler db = new DatabaseHandler();

        int currentDimension = 10;

        public HouseHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "housePricePoint")
            {
                foreach (ColShape collision in houseInformation.Keys)
                {
                    if (houseInformation[collision].returnOutsidePlayers().Contains(player))
                    {
                        API.triggerClientEvent(player, "passHousePrice", houseInformation[collision].returnShopPrice());
                        break;
                    }
                }
            }

            if (eventName == "housePurchase")
            {
                actionPurchaseHouse(player);
            }

            if (eventName == "setHouseProperties")
            {
                bool forSale = Convert.ToBoolean(arguments[0]);

                if (forSale == true)
                {
                    if (arguments[0] != null)
                    {
                        int price = Convert.ToInt32(arguments[1]);
                        if (price <= 1000000000)
                        {
                            foreach (ColShape collision in houseInformation.Keys)
                            {
                                if (houseInformation[collision].returnOutsidePlayers().Contains(player))
                                {
                                    if (houseInformation[collision].returnShopOwner() == player.name)
                                    {
                                        houseInformation[collision].setForSale(forSale);
                                        houseInformation[collision].setShopPrice(price);
                                        string query = string.Format("UPDATE PlayerHousing SET ForSale='1', Price='{0}' WHERE ID='{1}'", price, houseInformation[collision].returnCollisionID());
                                        API.exported.database.executeQueryWithResult(query);
                                    }
                                }
                            }
                        }
                        else
                        {
                            API.sendNotificationToPlayer(player, "~r~ That's too much.");
                        }
                    }
                    else
                    {
                        API.sendNotificationToPlayer(player, "~r~ You must specify a price to sell it.");
                    }
                }
                else
                {
                    foreach (ColShape collision in houseInformation.Keys)
                    {
                        if (houseInformation[collision].returnOutsidePlayers().Contains(player))
                        {
                            if (houseInformation[collision].returnShopOwner() == player.name)
                            {
                                houseInformation[collision].setForSale(forSale);
                                houseInformation[collision].setShopPrice(0);
                                string query = string.Format("UPDATE PlayerHousing SET ForSale='0', Price='0' WHERE ID='{0}'", houseInformation[collision].returnCollisionID());
                                API.exported.database.executeQueryWithResult(query);
                            }
                        }
                    }
                }
            }
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (houseInformation.ContainsKey(colshape))
                {
                    API.triggerClientEvent(player, "removeUseFunction", "House");
                    houseInformation[colshape].removeOutsidePlayer(player);
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
                    houseInformation[colshape].addOutsidePlayer(player);
                    API.triggerClientEvent(player, "triggerUseFunction", "House");
                    // If they have keys, tell them.
                    if (houseInformation[colshape].returnShopOwner() == player.name || houseInformation[colshape].returnShopKeys().Contains(player))
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
                            if (houseInformation[collision].returnInsidePlayers().ContainsKey(player))
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
            initializeHousing();
        }

        private void initializeHousing()
        {
            if (houseInformation.Count > 1)
            {
                foreach (ColShape collision in houseInformation.Keys)
                {
                    houseInformation[collision].Dispose();
                }
                houseInformation.Clear();
            }


            string query = string.Format("SELECT * FROM PlayerHousing");
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initialized = 0;

            foreach (DataRow row in result.Rows)
            {
                int id = Convert.ToInt32(row["ID"]);
                string player = row["Owner"].ToString();
                bool forSale = Convert.ToBoolean(row["ForSale"]);
                Vector3 position = new Vector3(Convert.ToSingle(row["PosX"]), Convert.ToSingle(row["PosY"]), Convert.ToSingle(row["PosZ"]));
                ShopInformationHandling.ShopType type = (ShopInformationHandling.ShopType)Enum.Parse(typeof(ShopInformationHandling.ShopType), row["Type"].ToString());
                int price = Convert.ToInt32(row["Price"]);
                positionBlips(player, position, id, forSale, type, price);
                ++initialized;
            }

            API.consoleOutput("Houses Intialized: " + initialized.ToString());
        }

        [Command("addhouse")]
        public void cmdHouseHandlerAddHouse(Client player)
        {
            if (db.isAdmin(player.name))
            {
                string query = string.Format("INSERT INTO PlayerHousing (PosX, PosY, PosZ) VALUES ('{0}', '{1}', '{2}')", player.position.X, player.position.Y, player.position.Z);
                API.exported.database.executeQuery(query);
                initializeHousing();
            }
        }


        public void positionBlips(string player, Vector3 position, int id, bool forSale, ShopInformationHandling.ShopType type, int price)
        {
            ShopInformationHandling newShop = new ShopInformationHandling();
            ColShape shape = API.createCylinderColShape(position, 0.5f, 2f);

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
            API.setBlipShortRange(newBlip, true);

            currentDimension += 1;

            newShop.setBlip(newBlip);
            newShop.setCollisionID(id);
            newShop.setCollisionShape(shape);
            newShop.setShopOwner(player);
            newShop.setCollisionPosition(position);
            newShop.setForSale(forSale);
            newShop.setShopDimension(currentDimension);
            newShop.setShopPrice(price);
            newShop.setShopType(type);

            houseInformation.Add(shape, newShop);
        }

        public void actionHouseControl(Client player)
        {
            foreach (ColShape collision in houseInformation.Keys)
            {
                if (houseInformation[collision].returnOutsidePlayers().Contains(player) && !player.isInVehicle)
                {
                    // IF THIS IS FOR SALE / ELSE
                    if (houseInformation[collision].returnForSale())
                    {
                        actionOpenHouseSale(player);
                        break;
                    }
                    else
                    {
                        if (houseInformation[collision].returnShopOwner() == player.name || houseInformation[collision].returnShopKeys().Contains(player))
                        {
                            actionEnterHouse(player, collision, houseInformation[collision].returnShopType());
                            break;
                        }
                    }
                }
                else
                {
                    if (houseInformation[collision].returnInsidePlayers().ContainsKey(player))
                    {
                        actionLeaveHouse(player, collision);
                        break;
                    }
                }
            }
        }

        public void actionOpenHouseSale(Client player)
        {
            API.triggerClientEvent(player, "showBuyHousing");
        }

        public void actionPurchaseHouse(Client player)
        {
            foreach (ColShape collision in houseInformation.Keys)
            {
                if (houseInformation[collision].returnOutsidePlayers().Contains(player))
                {
                    if (player.position.DistanceTo(houseInformation[collision].returnCollisionPosition()) <= 2)
                    {
                        if (Convert.ToBoolean(db.pullDatabase("PlayerHousing", "ForSale", "ID", houseInformation[collision].returnCollisionID().ToString())))
                        {
                            int housePrice = Convert.ToInt32(db.pullDatabase("PlayerHousing", "Price", "ID", houseInformation[collision].returnCollisionID().ToString()));
                            if (db.getPlayerMoney(player) >= housePrice)
                            {
                                db.setPlayerMoney(player, -housePrice); // Set Minus for Buyer

                                int old = Convert.ToInt32(db.pullDatabase("Players", "Bank", "Nametag", houseInformation[collision].returnShopOwner().ToString()));
                                string newAmount = (old + housePrice.ToString());

                                db.updateDatabase("Players", "Bank", newAmount, "Nametag", houseInformation[collision].returnShopOwner().ToString());

                                houseInformation[collision].setShopOwner(player.name);
                                houseInformation[collision].setForSale(false);
                                string query = string.Format("UPDATE PlayerHousing SET ForSale='False', Owner='{0}' WHERE ID='{1}'", player.name, houseInformation[collision].returnCollisionID());
                                API.exported.database.executeQueryWithResult(query);
                                break;
                            }

                        }
                    }
                }
            }
        }

        public void actionLeaveHouse(Client player, ColShape collision)
        {
            houseInformation[collision].removeInsidePlayer(player);
            API.setEntityPosition(player, houseInformation[collision].returnCollisionPosition());
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
                if (houseInformation[collision].returnShopOwner() == player.name)
                {
                    houseInformation[collision].addShopKeys(API.getPlayerFromName(target));
                }
            }
        }

        public void actionEnterHouse(Client player, ColShape collision, ShopInformationHandling.ShopType type)
        {
            switch (type)
            {
                case ShopInformationHandling.ShopType.apa_v_mp_h_01_a:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.8663, 315.7642, 217.6385));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-786.8663, 315.7642, 217.6385), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_01_b:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-774.0126, 342.0428, 196.6864));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-774.0126, 342.0428, 196.6864), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_01_c:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.9563, 315.6229, 187.9136));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-786.9563, 315.6229, 187.9136), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_02_a:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-787.0749, 315.8198, 217.6386));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-787.0749, 315.8198, 217.6386), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_02_b:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-774.1382, 342.0316, 196.6864));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-774.1382, 342.0316, 196.6864), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_02_c:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.8195, 315.5634, 187.9137));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-786.8195, 315.5634, 187.9137), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_03_a:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.6245, 315.6175, 217.6385));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-786.6245, 315.6175, 217.6385), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_03_b:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-774.0223, 342.1718, 196.6863));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-774.0223, 342.1718, 196.6863), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_03_c:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.9584, 315.7974, 187.9135));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-786.9584, 315.7974, 187.9135), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_04_a:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-787.0902, 315.7039, 217.6384));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-787.0902, 315.7039, 217.6384), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_04_b:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-773.8976, 342.1525, 196.6863));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-773.8976, 342.1525, 196.6863), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_04_c:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-787.0155, 315.7071, 187.9135));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-787.0155, 315.7071, 187.9135), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_05_a:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.9887, 315.7393, 217.6386));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-786.9887, 315.7393, 217.6386), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_05_b:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-774.0675, 342.0773, 196.6864));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-774.0675, 342.0773, 196.6864), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_05_c:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.8809, 315.6634, 187.9136));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-786.8809, 315.6634, 187.9136), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_06_a:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-787.1423, 315.6943, 217.6384));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-787.1423, 315.6943, 217.6384), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_06_b:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-773.9552, 341.9892, 196.6862));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-773.9552, 341.9892, 196.6862), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_06_c:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-787.0961, 315.815, 187.9135));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-787.0961, 315.815, 187.9135), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_07_a:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-787.029, 315.7113, 217.6385));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-787.029, 315.7113, 217.6385), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_07_b:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-774.0109, 342.0965, 196.6863));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-774.0109, 342.0965, 196.6863), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_07_c:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-787.0574, 315.6567, 187.9135));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-787.0574, 315.6567, 187.9135), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_08_a:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.9469, 315.5655, 217.6383));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-786.9469, 315.5655, 217.6383), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_08_b:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-774.0349, 342.0296, 196.6862));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-774.0349, 342.0296, 196.6862), 3f, 2f));
                    break;
                case ShopInformationHandling.ShopType.apa_v_mp_h_08_c:
                    API.requestIpl(type.ToString());
                    API.setEntityPosition(player, new Vector3(-786.9756, 315.723, 187.9134));
                    API.setEntityDimension(player, houseInformation[collision].returnShopDimension());
                    houseInformation[collision].removeOutsidePlayer(player);
                    houseInformation[collision].addInsidePlayer(player, player.handle);
                    houseInformation[collision].setExitCollision(API.createCylinderColShape(new Vector3(-786.9756, 315.723, 187.9134), 3f, 2f));
                    break;
                default:
                    API.consoleOutput(type.ToString());
                    break;
            }
        }


        public void actionHousePropertyPanel(Client player)
        {
            foreach (ColShape collision in houseInformation.Keys)
            {
                if (player.position.DistanceTo(houseInformation[collision].returnCollisionPosition()) <= 2)
                {
                    if (houseInformation[collision].returnShopOwner() == player.name)
                    {
                        API.triggerClientEvent(player, "ShowHousePropertyPanel");
                        break;
                    }
                }
            }
        }
    }
}
