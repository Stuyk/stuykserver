using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class Object_Placement_Handler : Script
    {
        public Object_Placement_Handler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (!player.hasData("Placing_Shop_Object"))
            {
                return;
            }

            if (!Convert.ToBoolean(player.getData("Placing_Shop_Object")))
            {
                return;
            }

            if (eventName == "Object_Placement")
            {
                API.call("ShopHandler", "actionPlaceDownShopObject", player, arguments[0], arguments[1]);
                return;
            }
        }
    }
}
