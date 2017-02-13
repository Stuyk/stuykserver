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

        public ChatHandler()
        {
            API.onChatMessage += API_onChatMessage;
        }

        public void API_onChatMessage(Client player, string message, CancelEventArgs e)
        {
            if (main.isPlayerLoggedIn(player))
            {
                sendCloseMessage(player, 15.0f, "~#ffffff~", API.getPlayerName(player) + " says: " + message);
                e.Cancel = true;
                return;
            }
        }

        public void sendCloseMessage(Client player, float radius, string sender, string message)
        {
            List<Client> nearPlayers = API.getPlayersInRadiusOfPlayer(radius, player);
            foreach (Client target in nearPlayers)
            {
                API.sendChatMessageToPlayer(player, sender, message);
            }
        }

        [Command("me", GreedyArg = true)] // help command 
        public void cmdChatMe(Client player, string message)
        {
            if (main.isPlayerLoggedIn(player))
            {
                sendCloseMessage(player, 15.0f, "~#C2A2DA~", API.getPlayerName(player) + " " + message);
                return;
            }
        }

        [Command("do", GreedyArg = true)] // do command 
        public void cmdChatDo(Client player, string message)
        {
            sendCloseMessage(player, 15.0f, "~#C2A2DA~", message + " ((" + API.getPlayerName(player) + "))");
        }

        [Command("b", GreedyArg = true)] // ooc chat command
        public void Command_b(Client player, string message)
        {
            sendCloseMessage(player, 15.0f, "~#ffffff~", API.getPlayerName(player) + ": " + "(( " + message + " ))");
        }

        [Command("s", Alias = "shout", GreedyArg = true)] // shout command
        public void cmdChatShout(Client player, string message)
        {
            sendCloseMessage(player, 25.0f, "~#ffffff~", API.getPlayerName(player) + " shouts: " + message);
        }

        [Command("w", Alias = "whisper", GreedyArg = true)] // whisper command
        public void cmdChatWhisper(Client player, string message)
        {
            sendCloseMessage(player, 7.5f, "~#ffffff~", API.getPlayerName(player) + " whispers: " + message);
        }

        [Command("b", Alias = "ooc", GreedyArg = true)] // whisper command
        public void cmdChatOOC(Client player, string message)
        {
            sendCloseMessage(player, 8.0f, "~#C4C4C4~", "((OOC)) " + API.getPlayerName(player) + ": " + message);
        }
    }
}
