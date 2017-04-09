using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class Dice : Script
    {
        float range = 20f;

        public Dice() { }

        [Command("d20")]
        public void cmdD20(Client player)
        {
            double result = new Random().Next(1, 20);
            sendToInRadius(player, "d20", result);
        }

        [Command("d12")]
        public void cmdD12(Client player)
        {
            double result = new Random().Next(1, 12);
            sendToInRadius(player, "d12", result);
        }

        [Command("d10")]
        public void cmdD10(Client player)
        {
            double result = new Random().Next(1, 10);
            sendToInRadius(player, "d10", result);
        }

        [Command("d8")]
        public void cmdD8(Client player)
        {
            double result = new Random().Next(1, 8);
            sendToInRadius(player, "d8", result);
        }

        [Command("d6")]
        public void cmdD6(Client player)
        {
            double result = new Random().Next(1, 6);
            sendToInRadius(player, "d6", result);
        }

        [Command("d4")]
        public void cmdD4(Client player)
        {
            double result = new Random().Next(1, 4);
            sendToInRadius(player, "d4", result);
        }

        public void sendToInRadius(Client player, string diceType, double result)
        {
            List<Client> players = API.getAllPlayers();
            foreach (Client nearbyPlayer in players)
            {
                if (nearbyPlayer.position.DistanceTo2D(player.position) <= range)
                {
                    API.sendChatMessageToPlayer(nearbyPlayer, string.Format("~b~{0} ~w~rolled a ~b~{1} ~w~and got a ~o~{2}", player.name, diceType, result));
                }
            }
        }
    }
}
