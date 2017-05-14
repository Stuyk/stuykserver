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
        DatabaseHandler db = new DatabaseHandler();
        Dictionary<ColShape, House> houseInformation = new Dictionary<ColShape, House>();
        Dictionary<Client, int> selectedHouse = new Dictionary<Client, int>();

        public HouseHandler()
        {
            API.consoleOutput("Started: House Handler");
            initializeHouses();
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        // Initialize Housing
        public void initializeHouses()
        {
            foreach (ColShape house in houseInformation.Keys)
            {
                houseInformation[house].Dispose();
            }

            houseInformation.Clear();

            string query = "SELECT * FROM PlayerHousing";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int i = 0;

            foreach (DataRow row in result.Rows)
            {
                House house = new House(row);
                ColShape shape = house.returnEntryCollision();
                houseInformation.Add(shape, house);
                i++;
            }

            API.consoleOutput("Houses Initialized: {0}", i.ToString());
        }

        // ################################
        // PRIMARY HOUSE HANDLER FUNCTIONS
        // ################################
        public House getHouse(ColShape collision)
        {
            if (houseInformation.ContainsKey(collision))
            {
                return houseInformation[collision];
            }
            return null;
        }

        [Command("createhouse")]
        public void cmdAdminCreateHouse(Client player, int id)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                Vector3 pos = player.position;
                string[] vars = { "PosX", "PosY", "PosZ", "Type" };
                string tableName = "PlayerHousing";
                string[] data = { pos.X.ToString(), pos.Y.ToString(), pos.Z.ToString(), id.ToString() };
                db.compileInsertQuery(tableName, vars, data);

                initializeHouses();
            }
        }

        [Command("selecthouse")]
        public void cmdAdminSelectHouse(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                foreach (ColShape shop in houseInformation.Keys)
                {
                    if (houseInformation[shop].returnEntryPosition().DistanceTo(player.position) <= 2)
                    {
                        if (!selectedHouse.ContainsKey(player))
                        {
                            selectedHouse.Add(player, houseInformation[shop].returnID());
                            API.sendChatMessageToPlayer(player, string.Format("~g~Selected house. ~w~ID: {0}", houseInformation[shop].returnID()));
                        }
                        else
                        {
                            selectedHouse.Set(player, houseInformation[shop].returnID());
                            API.sendChatMessageToPlayer(player, string.Format("~g~Selected house. ~w~ID: {0}", houseInformation[shop].returnID()));
                        }
                        break;
                    }
                }
            }
        }

        [Command("deletehouse")]
        public void cmdDeleteSelectedHouse(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedHouse.ContainsKey(player))
                {
                    string query = string.Format("DELETE FROM PlayerHousing WHERE ID='{0}'", selectedHouse[player]);
                    API.exported.database.executeQuery(query);
                    initializeHouses();
                    API.sendNotificationToPlayer(player, "~r~Deleted house.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a house first. ~w~/selecthouse");
                    return;
                }
            }
        }

        [Command("lockhouse")]
        public void cmdForceLockHouse(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedHouse.ContainsKey(player))
                {
                    ColShape collision = (ColShape)API.getEntityData(player, "ColShape");
                    houseInformation[collision].setLockStatus(true);
                    API.sendChatMessageToPlayer(player, "~y~House # ~o~Locked");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a house first. ~w~/selecthouse");
                    return;
                }
            }
        }

        [Command("unlockhouse")]
        public void cmdForceUnlockHouse(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                if (selectedHouse.ContainsKey(player))
                {
                    ColShape collision = (ColShape)API.getEntityData(player, "ColShape");
                    houseInformation[collision].setLockStatus(false);
                    API.sendChatMessageToPlayer(player, "~y~House # ~b~Unlocked");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(player, "~r~You must select a house first. ~w~/selecthouse");
                    return;
                }
            }
        }

        [Command("reloadhouses")]
        public void cmdReloadHouses(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance.isAdmin())
            {
                initializeHouses();
            }
        }

        // ################################
        // CLIENT EVENT FUNCTIONS
        // ################################
        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "housePurchase")
            {
                House instance = (House)API.getEntityData(player, "SelectedHouse");
                if (instance == null)
                {
                    return;
                }

                if (!instance.returnForSale())
                {
                    API.sendNotificationToPlayer(player, "~r~Seems that house isn't for sale anymore.");
                    API.triggerClientEvent(player, "killPanel");
                    return;
                }

                Player playerInstance = (Player)API.call("PlayerHandler", "getPlayer", player);
                if (playerInstance.returnPlayerCash() >= instance.returnHousePrice())
                {
                    int houseOwnerID = instance.returnHouseOwner();
                    string[] varNames = { "ID" };
                    string before = "SELECT Bank FROM Players WHERE";
                    object[] data = { houseOwnerID.ToString() };
                    DataTable result = db.compileSelectQuery(before, varNames, data);

                    if (result.Rows.Count >= 1)
                    {
                        int oldBank = Convert.ToInt32(result.Rows[0]["Bank"]);
                        oldBank += instance.returnHousePrice();

                        string[] varNamesUpdate = { "Bank" };
                        string beforeUpdate = "UPDATE Players SET";
                        object[] dataUpdate = { oldBank };
                        string after = string.Format("WHERE ID='{0}'", houseOwnerID);
                        db.compileQuery(beforeUpdate, after, varNamesUpdate, dataUpdate);
                    }

                    playerInstance.removePlayerCash(instance.returnHousePrice());
                    instance.changeHouseOwnership(player);
                    playerInstance.savePlayer();
                }
            }

            if (eventName == "housePricePoint")
            {
                House instance = (House)API.getEntityData(player, "SelectedHouse");
                if (instance == null)
                {
                    API.triggerClientEvent(player, "killPanel");
                    return;
                }

                if (!instance.returnForSale())
                {
                    API.sendNotificationToPlayer(player, "~r~Seems that house isn't for sale anymore.");
                    API.triggerClientEvent(player, "killPanel");
                    return;
                }

                API.triggerClientEvent(player, "passHousePrice", instance.returnHousePrice());
                return;
            }

            if (eventName == "setHouseProperties")
            {
                House instance = (House)API.getEntityData(player, "SelectedHouse");
                if (instance == null)
                {
                    return;
                }

                if (instance.returnHouseOwner() != Convert.ToInt32(API.getEntityData(player, "PlayerID")))
                {
                    return;
                }

                if (Convert.ToBoolean(arguments[0]))
                {
                    instance.setupForSale(Convert.ToInt32(arguments[1]));
                }
                else
                {
                    instance.setForSale(false);
                }
            }

            if (eventName == "setHouseLock")
            {
                House instance = (House)API.getEntityData(player, "SelectedHouse");
                if (instance == null)
                {
                    return;
                }

                if (instance.returnHouseOwner() != Convert.ToInt32(API.getEntityData(player, "PlayerID")))
                {
                    return;
                }

                instance.setLockStatus(Convert.ToBoolean(arguments[0]));

                API.sendChatMessageToPlayer(player, string.Format("~y~House # ~b~Lock: ~o~{0}", Convert.ToBoolean(arguments[0]).ToString()));
            }
        }

        // ################################
        // PRIMARY HOUSE HANDLER ACTIONS
        // ################################
        public void actionHouseControl(Client player)
        {
            ColShape collision = (ColShape)API.getEntityData(player, "ColShape");
            // Check if the player has the Collision Assigned.
            if (collision == null)
            {
                return;
            }

            // Check if the house exists in the list.
            if (!houseInformation.ContainsKey(collision))
            {
                return;
            }

            // Assign the House Instance
            House instance = houseInformation[collision];

            if (instance.returnForSale())
            {
                actionHouseIsForSale(player, instance);
                return;
            }

            actionHouseEnter(player, instance);
            return;
        }

        // What happens when the house is for sale.
        public void actionHouseIsForSale(Client player, House instance)
        {
            API.triggerClientEvent(player, "showBuyHousing", instance.returnHousePrice());
            API.setEntityData(player, "SelectedHouse", instance);
            return;
        }

        // What happens when a player enters the house.
        public void actionHouseEnter(Client player, House instance)
        {
            // Grab PlayerID
            int playerID = Convert.ToInt32(API.getEntityData(player, "PlayerID"));

            if (instance.returnLocked())
            {
                API.call("ChatHandler", "cmdChatMe", player, "jiggles the handle.");
                API.call("ChatHandler", "cmdChatDo", player, "The door seems to feel locked.");
                return;
            }

            API.call("ChatHandler", "cmdChatMe", player, "grabs the door, opens it, and enters the building.");
            API.setEntityData(player, "ReturnPosition", player.position); // Set Return Position
            API.setEntityData(player, "IsInInterior", true); // Set IsInInterior
            API.setEntityData(player, "InteriorInstance", instance); // Set House Instance
            API.requestIpl(instance.returnType().ToString());
            API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
            API.setEntityDimension(player, instance.returnID());
            API.setEntityPosition(player, instance.returnExitPosition());
        }

        // What happens when a player exits the house.
        public void actionHouseExit(Client player)
        {
            API.call("ChatHandler", "cmdChatMe", player, "grabs the door, opens it, and exits the building.");
            API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
            API.setEntityPosition(player, (Vector3)API.getEntityData(player, "ReturnPosition"));
            API.setEntityDimension(player, 0);
            API.setEntityData(player, "ReturnPosition", null); // Set Return Position
            API.setEntityData(player, "IsInInterior", null); // Set IsInInterior
            API.setEntityData(player, "InteriorInstance", null); // Set House Instance
        }

        //What happens when a player hits Shift + B
        public void actionHousePropertyPanel(Client player)
        {
            ColShape collision = (ColShape)API.getEntityData(player, "ColShape");
            if (houseInformation[collision].returnHouseOwner() == Convert.ToInt32(API.getEntityData(player, "PlayerID")))
            {
                API.triggerClientEvent(player, "ShowHousePropertyPanel");
                API.setEntityData(player, "SelectedHouse", houseInformation[collision]);
                return;
            }
        }
    }
}
