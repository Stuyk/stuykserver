using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class PlayerHandler : Script
    {
        Dictionary<Client, Player> playerInformation = new Dictionary<Client, Player>();

        public PlayerHandler()
        {
            API.consoleOutput("Started: Player Handler");
        }

        public Player createPlayer(DataRow row, Client sender)
        {
            Player player = new Player(row, sender);
            playerInformation.Add(sender, player);
            return player;
        }

        public Player getPlayer(Client player)
        {
            if (playerInformation.ContainsKey(player))
            {
                return playerInformation[player];
            }
            return null;
        }

        public void cleanupPlayer(Client player)
        {
            if (playerInformation.ContainsKey(player))
            {
                playerInformation[player].savePlayerLogOut();
                playerInformation[player].Dispose();
                playerInformation.Remove(player);
            }
        }
    }
}
