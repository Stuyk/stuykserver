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
    public class ShopHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Dictionary<ColShape, Shop> shopInfo = new Dictionary<ColShape, Shop>();

        public ShopHandler()
        {
            API.consoleOutput("Started: Shop Handler");
            initializeShops();
        }

        private void initializeShops()
        {
            foreach (ColShape shop in shopInfo.Keys)
            {
                shopInfo[shop].Dispose();
            }

            shopInfo.Clear();

            string query = "SELECT * FROM Shops";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                Shop shop = new Shop(row);
                ColShape shape = shop.returnCollisionShape();
                shopInfo.Add(shape, shop);
            }
        }

        public Shop getShop(ColShape collision)
        {
            if (shopInfo.ContainsKey(collision))
            {
                return shopInfo[collision];
            }
            return null;
        }

        [Command("reloadshops")]
        public void cmdInitializeShops(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                initializeShops();
            }
        }


        [Command("createshop")]
        public void cmdCreateShop(Client player, int type)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                Vector3 pos = player.position;
                string[] vars = { "PosX", "PosY", "PosZ", "Type" };
                string tableName = "Shops";
                string[] data = { pos.X.ToString(), pos.Y.ToString(), pos.Z.ToString(), type.ToString() };
                db.compileInsertQuery(tableName, vars, data);

                initializeShops();
            }
        }

        [Command("setshopexit")]
        public void cmdSetShopExit(Client player, int id)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                
            }
        }

        [Command("deleteshop")]
        public void cmdDeleteShop(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                foreach (ColShape shop in shopInfo.Keys)
                {
                    if (shopInfo[shop].returnCollisionPosition().DistanceTo(player.position) <= 10)
                    {
                        string query = string.Format("DELETE FROM Shops WHERE ID='{0}'", shopInfo[shop].returnShopID());
                        API.exported.database.executeQuery(query);
                        initializeShops();
                        API.sendNotificationToPlayer(player, "~r~Deleted shop.");
                        break;
                    }
                }
            }
        }
    }
}
