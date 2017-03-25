using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Jobs
{
    public class TradeHandler : Script
    {
        [Command("accept")]
        public void acceptOffer(Client player)
        {
            if (!API.hasEntityData(player, "TradeType"))
            {
                return;
            }

            string type = API.getEntityData(player, "TradeType");

            switch(type)
            {
                case "BodyGuard":
                    API.call("BodyGuard", "actionAccept", player);
                    break;
                default:
                    API.sendChatMessageToPlayer(player, "~r~Invalid trade offer.");
                    break;
                // More here.
            }
        }
    }
}
