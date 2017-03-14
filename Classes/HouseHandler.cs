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
                    return;
                }

                Player playerInstance = (Player)API.call("PlayerHandler", "getPlayer", player);
                if (playerInstance.returnPlayerCash() >= instance.returnHousePrice())
                {
                    // CHANGE HOUSE OWNERSHIP HERE
                }
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
            API.triggerClientEvent(player, "showBuyHousing");
            API.triggerClientEvent(player, "passHousePrice", instance.returnHousePrice()); // FIX THIS
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
            API.setEntityDimension(player, instance.returnID());
            API.setEntityPosition(player, instance.returnExitPosition());
        }

        // What happens when a player exits the house.
        public void actionHouseExit(Client player)
        {
            API.call("ChatHandler", "cmdChatMe", player, "grabs the door, opens it, and exits the building.");
            API.setEntityPosition(player, (Vector3)API.getEntityData(player, "ReturnPosition"));
            API.setEntityDimension(player, 0);
            API.setEntityData(player, "ReturnPosition", null); // Set Return Position
            API.setEntityData(player, "IsInInterior", null); // Set IsInInterior
            API.setEntityData(player, "InteriorInstance", null); // Set House Instance
        }
    }
}
