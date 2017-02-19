using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class ChatHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();
        DeathHandler dh = new DeathHandler();

        public ChatHandler()
        {
            API.onChatMessage += API_onChatMessage;
        }

        public void API_onChatMessage(Client player, string message, CancelEventArgs e)
        {
            if (!db.isPlayerLoggedIn(player))
            {
                e.Cancel = true;
                return;
            }
        }

    }
}
