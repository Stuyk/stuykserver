using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class ActiveShooter : Script
    {
        List<string> activeShooters;

        public ActiveShooter()
        {
            API.onUpdate += API_onUpdate;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            activeShooters = new List<string>();
            activeShooters.Add("example");
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "stopActivity")
            {
                API.consoleOutput("stopActivity called.");
                activeShooters.Remove(player.name);
            }

            if (eventName == "startActivity")
            {
                API.consoleOutput("I got to here.");
                if (activeShooters.Contains(player.name) == false)
                {
                    API.consoleOutput("startActivity called.");
                    activeShooters.Add(player.name);
                    API.triggerClientEvent(player, "startActiveShooter");
                }
            }
        }

        private void API_onUpdate()
        {

        }
    }
}
