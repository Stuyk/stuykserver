using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace stuykserver.Util
{
    public class ActiveShooter : Script
    {
        Dictionary<Client, double> activeShooters = new Dictionary<Client, double>();
        List<Client> dummyShooters = new List<Client>();

        public ActiveShooter()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onResourceStart += API_onResourceStart;
        }

        // Start the timer.
        private void API_onResourceStart()
        {
            Timer timer = new Timer();
            timer.Interval = 10000; // Every 10 Seconds.
            timer.Enabled = true;
            timer.Elapsed += ActiveShooterTimer;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName != "PedIsShooting")
            {
                return;
            }

            addActiveShooter(player);
        }

        public void addActiveShooter(Client player)
        {
            if (activeShooters.ContainsKey(player))
            {
                activeShooters[player] = 300; // Set the players active shooter cooldown again if they already shot.
                API.triggerClientEvent(player, "setActiveShooter", true);
                return;
            }

            activeShooters.Add(player, 300);
            API.triggerClientEvent(player, "setActiveShooter", true);
        }

        private void ActiveShooterTimer(object sender, ElapsedEventArgs e)
        {
            // If active shooters is zero, don't iterate.
            if (activeShooters.Count == 0)
            {
                return;
            }

            // Add to the dummy list for dictionary modification.
            foreach(Client player in activeShooters.Keys)
            {
                dummyShooters.Add(player);
            }

            // Iterate over the dummy shooters and pull active shooters and modify their time.
            foreach(Client player in dummyShooters)
            {
                if (activeShooters.ContainsKey(player))
                {
                    // Minutes 10 seconds from the player.
                    activeShooters[player] -= 10;
                    API.triggerClientEvent(player, "setActiveShooter", true);
                    API.setEntityData(player, "ActiveShooter", true);

                    // If the players shooting time is zero.
                    if (activeShooters[player] <= 0)
                    {
                        activeShooters.Remove(player);
                        API.triggerClientEvent(player, "setActiveShooter", false);
                        API.setEntityData(player, "ActiveShooter", false);
                    }
                }
            }

            dummyShooters.Clear(); // Clear the dummy list once we're done with it.
        }
    }
}
