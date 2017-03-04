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

        public ChatHandler()
        {
            API.onChatMessage += API_onChatMessage;
        }

        public string replaceUnderscore(string message)
        {
            return message.Replace("_", " ");
        }


        public void API_onChatMessage(Client player, string message, CancelEventArgs e)
        {
            if (db.isPlayerLoggedIn(player))
            {
                int dimension = API.getEntityDimension(player);
                sendProximityMessage(player, message, dimension);
                e.Cancel = true;
                return;
            }
            e.Cancel = true;
            return;
        }

        public void sendProximityMessage(Client player, string message, int dimension)
        {
            var players = API.getPlayersInRadiusOfPlayer(10, player);
            foreach (Client p in players)
            {
                if (p.position.DistanceTo(player.position) <= 10 && API.getEntityDimension(p) == dimension)
                {
                    API.sendChatMessageToPlayer(p, "~#49A1F4~", replaceUnderscore(player.name) + " says: ~w~" + message);
                    API.consoleOutput(string.Format("[{0}] {1}: {2}", dimension, player.name, message));
                }
            }
        }

        [Command("me", GreedyArg = true)]
        public void cmdChatMe(Client player, string message)
        {
            int dimension = API.getEntityDimension(player);
            List<Client> players = API.getPlayersInRadiusOfPlayer(10, player);
            if (db.isPlayerLoggedIn(player))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].position.DistanceTo(player.position) <= 10 && players[i].dimension == dimension)
                    {
                        API.sendChatMessageToPlayer(players[i], "~#D1B3D8~", string.Format("{0} {1}", replaceUnderscore(player.name), message));
                    }
                }
            }
            
        }

        [Command("do", GreedyArg = true)]
        public void cmdChatDo(Client player, string message)
        {
            int dimension = API.getEntityDimension(player);
            List<Client> players = API.getPlayersInRadiusOfPlayer(10, player);
            if (db.isPlayerLoggedIn(player))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].position.DistanceTo(player.position) <= 10 && players[i].dimension == dimension)
                    {
                        API.sendChatMessageToPlayer(players[i], "~#D1B3D8~", message + " [" + replaceUnderscore(player.name) + "]");
                    }
                }
            }
            
        }

        [Command("shout", GreedyArg = true, Alias = "s")]
        public void cmdChatShout(Client player, string message)
        {
            int dimension = API.getEntityDimension(player);
            List<Client> players = API.getPlayersInRadiusOfPlayer(15, player);
            if (db.isPlayerLoggedIn(player))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].position.DistanceTo(player.position) <= 15 && players[i].dimension == dimension)
                    {
                        API.sendChatMessageToPlayer(players[i], "~#F7FC99~", replaceUnderscore(player.name) + " shouts: ~w~" + message);
                        API.consoleOutput(string.Format("[{0}] {1}: {2}", dimension, player.name, message));
                    }
                }
            }
            
        }

        [Command("whisper", GreedyArg = true, Alias = "w")]
        public void cmdChatWhisper(Client player, string message)
        {
            int dimension = API.getEntityDimension(player);
            List<Client> players = API.getPlayersInRadiusOfPlayer(15, player);
            if (db.isPlayerLoggedIn(player))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].position.DistanceTo(player.position) <= 15 && players[i].dimension == dimension)
                    {
                        API.sendChatMessageToPlayer(players[i], "~#A6B7C1~", replaceUnderscore(player.name) + " whispers: ~w~" + message);
                        API.consoleOutput(string.Format("[{0}] {1}: {2}", dimension, player.name, message));
                    }
                }
            }
            
        }

        [Command("ooc", GreedyArg = true, Alias = "b")]
        public void cmdChatOOC(Client player, string message)
        {
            int dimension = API.getEntityDimension(player);
            List<Client> players = API.getPlayersInRadiusOfPlayer(15, player);
            if (db.isPlayerLoggedIn(player))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].position.DistanceTo(player.position) <= 15 && players[i].dimension == dimension)
                    {
                        API.sendChatMessageToPlayer(players[i], "~#ADADAD~", string.Format("(({0}: {1}))", replaceUnderscore(player.name), message));
                        API.consoleOutput(string.Format("[{0} - OOC] {1}: {2}", dimension, player.name, message));
                    }
                }
            }
        }
    }
}
