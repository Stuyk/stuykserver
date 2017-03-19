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
                    Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
                    int atmMoney = instance.returnPlayerBank();
                    if (atmMoney > 0 && input <= atmMoney)
                    {
                        instance.addPlayerCash(input);
                        instance.removePlayerBank(input);
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
                    Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
                    int playerMoney = instance.returnPlayerCash();
                    if (playerMoney > 0 && input <= playerMoney)
                    {
                        instance.removePlayerCash(input);
                        instance.addPlayerBank(input);
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
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);

            if (instance == null)
            {
                API.triggerClientEvent(player, "killPanel");
                API.sendNotificationToPlayer(player, "~r~Something went wrong.");
                return;
            }

            int atmMoney = instance.returnPlayerBank();
            int playerMoney = instance.returnPlayerCash();

            API.triggerClientEvent(player, "refreshATM", atmMoney, playerMoney);
        }

        public void selectATM(Client player)
        {
            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            if (instance == null)
            {
                return;
            }

            API.triggerClientEvent(player, "loadATM", instance.returnPlayerCash());
        }
    }
}
