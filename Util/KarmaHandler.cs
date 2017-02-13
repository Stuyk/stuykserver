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
            int currentKarma = Convert.ToInt32(db.pullDatabase("Players", "Karma", "Nametag", player.name)); // Convert a string into a variable of an integer type.
            int newKarma = currentKarma + amount; // Add our currentKarma to the amount specified. Assign the new amount to a variable of the integer type.
            db.updateDatabase("Players", "Karma", newKarma.ToString(), "Nametag", player.name); // Update the database.
            API.sendNotificationToPlayer(player, "~g~You've gained some karma.");
        }

        public void removeKarma(Client player, int amount)
        {
            int currentKarma = Convert.ToInt32(db.pullDatabase("Players", "Karma", "Nametag", player.name));
            int newKarma = currentKarma - amount;
            db.updateDatabase("Players", "Karma", newKarma.ToString(), "Nametag", player.name);
            API.sendNotificationToPlayer(player, "~r~You have lost some karma.");
        }

        public bool checkKarma(Client player, int amount)
        {
            int currentKarma = Convert.ToInt32(db.pullDatabase("Players", "Karma", "Nametag", player.name));
            if (currentKarma >= amount)
            {
                return true;
            }
            return false;
        }
    }
}

