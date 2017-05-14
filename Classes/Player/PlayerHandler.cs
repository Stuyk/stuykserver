using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace stuykserver.Classes
{
    public class PlayerHandler : Script
    {
        Dictionary<Client, Player> playerInformation = new Dictionary<Client, Player>();

        public PlayerHandler()
        {
            API.consoleOutput("Started: Player Handler");
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            Timer timer = new Timer();
            timer.Interval = 60000;
            timer.Elapsed += SaveAllPlayers;
            timer.Enabled = true;
        }

        private void SaveAllPlayers(object sender, ElapsedEventArgs e)
        {
            API.consoleOutput("Saved all players.");

            foreach (Client player in playerInformation.Keys)
            {
                playerInformation[player].savePlayer();
            }
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
