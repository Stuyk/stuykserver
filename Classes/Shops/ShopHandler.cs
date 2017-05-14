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
    public class ShopHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Dictionary<ColShape, Shop> shopInfo = new Dictionary<ColShape, Shop>();
        Dictionary<Client, int> selectedShop = new Dictionary<Client, int>();

        public ShopHandler()
        {
            API.consoleOutput("Started: Shop Handler");
            API.onResourceStart += API_onResourceStart;
            initializeShops();
        }

        private void API_onResourceStart()
        {
            Timer timer = new Timer();
            timer.Interval = 300000;
            timer.Enabled = true;
            timer.Elapsed += SaveAllShops;
        }

        private void SaveAllShops(object sender, ElapsedEventArgs e)
        {
            foreach (Shop shop in shopInfo.Values)
            {
                shop.saveShop();
            }

            API.consoleOutput("Saved All Shops");
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
            }
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
            if (!instance.isAdmin())
            {
                return;
            }

            GTANetworkServer.Object obj = API.createObject(-1652821467, player.position, player.rotation, player.dimension);
            API.triggerClientEvent(player, "attachObject", obj);
            player.setData("Placing_Shop_Object", true);
            player.setData("Placing_Shop_Object_Reference", obj);
            player.setData("Placing_Shop_Object_Type", type);
        }

        public void actionPlaceDownShopObject(Client player, Vector3 position, Vector3 rotation)
        {
            var obj = player.getData("Placing_Shop_Object_Reference");
            if (API.doesEntityExist(obj))
            {
                API.deleteEntity(obj);
            }

            string[] vars = { "PosX", "PosY", "PosZ", "Type", "RotX", "RotY", "RotZ" };
            string tableName = "Shops";
            string[] data = { position.X.ToString(), position.Y.ToString(), position.Z.ToString(), Convert.ToString(player.getData("Placing_Shop_Object_Type")), rotation.X.ToString(), rotation.Y.ToString(), rotation.Z.ToString() };
            db.compileInsertQuery(tableName, vars, data);

            initializeShops();
        }

        [Command("selectshop")]
        public void cmdSelectShop(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                foreach(ColShape shop in shopInfo.Keys)
                {
                    if (shopInfo[shop].returnCollisionPosition().DistanceTo(player.position) <= 2)
                    {
                        if (!selectedShop.ContainsKey(player))
                        {
                            selectedShop.Add(player, shopInfo[shop].returnShopID());
                            API.sendChatMessageToPlayer(player, string.Format("~g~Selected shop. ~w~ID: {0}", shopInfo[shop].returnShopID()));
                        }
                        else
                        {
                            selectedShop.Set(player, shopInfo[shop].returnShopID());
                            API.sendChatMessageToPlayer(player, string.Format("~g~Selected shop. ~w~ID: {0}", shopInfo[shop].returnShopID()));
                        }
                        break;
                    }
                }
            }
        }

        [Command("setshoprange")]
        public void cmdSetShopRange(Client player, string range)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedShop.ContainsKey(player))
                {
                    string[] varNames = { "Radius" };
                    string before = "UPDATE Shops SET";
                    object[] data = { range };
                    string after = string.Format("WHERE ID='{0}'", selectedShop[player]);
                    db.compileQuery(before, after, varNames, data);
                    initializeShops();
                    API.sendNotificationToPlayer(player, "~g~Updated Range");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a shop first. ~w~/selectshop");
                    return;
                }     
            }
        }

        [Command("setshopheight")]
        public void cmdSetShopHeight(Client player, string height)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedShop.ContainsKey(player))
                {
                    string[] varNames = { "Height" };
                    string before = "UPDATE Shops SET";
                    object[] data = { height };
                    string after = string.Format("WHERE ID='{0}'", selectedShop[player]);
                    db.compileQuery(before, after, varNames, data);
                    initializeShops();
                    API.sendNotificationToPlayer(player, "~g~Updated height.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a shop first. ~w~/selectshop");
                    return;
                }
            }
        }

        [Command("setshoptype")]
        public void cmdSetShopType(Client player, string type)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedShop.ContainsKey(player))
                {
                    string[] varNames = { "Type" };
                    string before = "UPDATE Shops SET";
                    object[] data = { type.ToString() };
                    string after = string.Format("WHERE ID='{0}'", selectedShop[player]);
                    db.compileQuery(before, after, varNames, data);
                    initializeShops();
                    API.sendNotificationToPlayer(player, "~g~Updated type.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a shop first. ~w~/selectshop");
                    return;
                }
            }
        }

        [Command("setshopexit")]
        public void cmdSetShopExit(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedShop.ContainsKey(player))
                {
                    string[] varNames = { "ExitX", "ExitY", "ExitZ" };
                    string before = "UPDATE Shops SET";
                    object[] data = { player.position.X.ToString(), player.position.Y.ToString(), player.position.Z.ToString() };
                    string after = string.Format("WHERE ID='{0}'", selectedShop[player]);
                    db.compileQuery(before, after, varNames, data);
                    initializeShops();
                    API.sendNotificationToPlayer(player, "~g~Updated Shop Exit.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a shop first. ~w~/selectshop");
                    return;
                }
            }
        }

        [Command("setshopcam")]
        public void cmdSetShopCam(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedShop.ContainsKey(player))
                {
                    string[] varNames = { "CamX", "CamY", "CamZ" };
                    string before = "UPDATE Shops SET";
                    object[] data = { player.position.X.ToString(), player.position.Y.ToString(), player.position.Z.ToString() };
                    string after = string.Format("WHERE ID='{0}'", selectedShop[player]);
                    db.compileQuery(before, after, varNames, data);
                    initializeShops();
                    API.sendNotificationToPlayer(player, "~g~Updated Shop Camera.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a shop first. ~w~/selectshop");
                    return;
                }
            }
        }

        [Command("setshopfocus")]
        public void cmdSetShopFocus(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedShop.ContainsKey(player))
                {
                    string[] varNames = { "FocusX", "FocusY", "FocusZ" };
                    string before = "UPDATE Shops SET";
                    object[] data = { player.position.X.ToString(), player.position.Y.ToString(), player.position.Z.ToString() };
                    string after = string.Format("WHERE ID='{0}'", selectedShop[player]);
                    db.compileQuery(before, after, varNames, data);
                    initializeShops();
                    API.sendNotificationToPlayer(player, "~g~Updated Shop Focus Camera.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a shop first. ~w~/selectshop");
                    return;
                }
            }
        }



        [Command("deleteshop")]
        public void cmdDeleteShop(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedShop.ContainsKey(player))
                {
                    string query = string.Format("DELETE FROM Shops WHERE ID='{0}'", selectedShop[player]);
                    API.exported.database.executeQuery(query);
                    initializeShops();
                    API.sendNotificationToPlayer(player, "~r~Deleted shop.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a shop first. ~w~/selectshop");
                    return;
                }
            }
        }
    }
}
