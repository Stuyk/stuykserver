using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class KarmaHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler(); // Database Instance + Calls that DatabaseHandler.cs, I can now refer to it as db.

        public KarmaHandler()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: KarmaHandler");
        }

        public void addKarma(Client player, int amount) // player is a variable, amount is a variable.
        {
            int currentKarma = Convert.ToInt32(API.getEntityData(player, "Karma"));
            currentKarma += amount;
            API.setEntityData(player, "Karma", currentKarma);
            pushKarma(player);
            API.sendNotificationToPlayer(player, "~g~You've gained some karma.");
        }

        public void removeKarma(Client player, int amount)
        {
            int currentKarma = Convert.ToInt32(API.getEntityData(player, "Karma"));
            currentKarma -= amount;
            API.setEntityData(player, "Karma", currentKarma);
            pushKarma(player);
            API.sendNotificationToPlayer(player, "~r~You have lost some karma.");
        }

        public bool checkKarma(Client player, int amount)
        {
            int currentKarma = Convert.ToInt32(API.getEntityData(player, "Karma"));
            if (currentKarma >= amount)
            {
                return true;
            }
            return false;
        }

        public void pushKarma(Client player)
        {
            string[] varNames = { "Karma" };
            string before = "UPDATE Players SET";
            string after = string.Format("WHERE ID='{0}'", Convert.ToInt32(API.getEntityData(player, "ID")));
            object[] args = { Convert.ToInt32(API.getEntityData(player, "Karma")) };
            db.compileQuery(before, after, varNames, args);
            updateKarma(player);
        }

        public void updateKarma(Client player)
        {
            int currentKarma = Convert.ToInt32(API.getEntityData(player, "Karma"));
            if (currentKarma == 0)
            {
                string displayText = "Pure Neutral";
                API.triggerClientEvent(player, "updateKarma", displayText);
            }
            if (currentKarma > 0 && currentKarma < 25)
            {
                string displayText = "Good";
                API.triggerClientEvent(player, "updateKarma", displayText);
            }

            if (currentKarma > 25 && currentKarma < 90)
            {
                string displayText = "Great";
                API.triggerClientEvent(player, "updateKarma", displayText);
            }

            if (currentKarma > 90 && currentKarma < 100)
            {
                string displayText = "Perfect";
                API.triggerClientEvent(player, "updateKarma", displayText);
            }

            if (currentKarma < 0 && currentKarma > -25)
            {
                string displayText = "Scum";
                API.triggerClientEvent(player, "updateKarma", displayText);
            }

            if (currentKarma < -25 && currentKarma > -95)
            {
                string displayText = "Terrible";
                API.triggerClientEvent(player, "updateKarma", displayText);
            }

            if (currentKarma < -90 && currentKarma > -100)
            {
                string displayText = "Feared";
                API.triggerClientEvent(player, "updateKarma", displayText);
            }
        }
    }
}

