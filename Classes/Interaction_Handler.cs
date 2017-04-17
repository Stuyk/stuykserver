using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class Interaction_Handler : Script
    {
        public Interaction_Handler()
        {
            API.onClientEventTrigger += Interaction_Management;
        }

        private void Interaction_Management(Client player, string eventName, params object[] arguments)
        {
            if (eventName != "Interaction_Update")
            {
                return;
            }

            if (arguments.Length < 1)
            {
                player.setData("Selected", null);
                return;
            }

            player.setData("Selected", arguments[0]);
        }
    }
}
