using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Jobs
{
    public class Fishing : Script
    {
        Main main = new Main();
        SpawnPoints spawnPoints = new SpawnPoints();

        public Fishing()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Fishing");
        }

        public void beginFishing(Client player)
        {
            player.freeze(true);
            API.sendChatMessageToPlayer(player, main.msgPrefix + "You have started fishing.");
            API.sendNotificationToPlayer(player, "~r~To stop at any time type: /stop");
            return;
        }
    }
}
