using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class BankHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();

        Dictionary<ColShape, Shop> shopInformation = new Dictionary<ColShape, Shop>();

        public BankHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }


        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "stopAnimation")
            {
                player.stopAnimation();
            }

            if (eventName == "withdrawATM_Server")
            {
                int input = Convert.ToInt32(arguments[0]);
                if (input > 0)
                {
                    int atmMoney = db.getPlayerAtmMoney(player);
                    if (atmMoney > 0 && input <= atmMoney)
                    {
                        db.setPlayerMoney(player, +input);
                        db.setPlayerAtmMoney(player, -input);
                        updateATMDisplay(player);
                        API.triggerClientEvent(player, "displayWithdrawSuccess");
                    }
                    else
                    {
                        API.triggerClientEvent(player, "displayNotThatMuch");
                    }
                }
            }

            if (eventName == "balanceNotDisplayed")
            {
                updateATMDisplay(player);
            }

            if (eventName == "depositATM_Server")
            {
                int input = Convert.ToInt32(arguments[0]);
                if (input > 0)
                {
                    int playerMoney = db.getPlayerMoney(player);
                    if (playerMoney > 0 && input <= playerMoney)
                    {
                        db.setPlayerMoney(player, -input);
                        db.setPlayerAtmMoney(player, +input);
                        updateATMDisplay(player);
                        API.triggerClientEvent(player, "depositAlertSuccess");
                    }
                    else
                    {
                        API.triggerClientEvent(player, "displayNotThatMuch");
                    }
                }
            }
        }

        public void updateATMDisplay(Client player)
        {
            int atmMoney = db.getPlayerAtmMoney(player);
            int playerMoney = db.getPlayerMoney(player);

            API.triggerClientEvent(player, "refreshATM", atmMoney, playerMoney);
        }

        public void selectATM(Client player)
        {
            API.triggerClientEvent(player, "loadATM", db.getPlayerAtmMoney(player));
        }
    }
}
