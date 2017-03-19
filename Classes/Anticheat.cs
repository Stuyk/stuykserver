using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class Anticheat : Script
    {
        DateTime lastCheckTime;

        public Anticheat()
        {
            API.onResourceStart += API_onResourceStart;
            API.onUpdate += API_onUpdate;
        }

        private void API_onUpdate()
        {
            if (DateTime.Now >= lastCheckTime.AddSeconds(15))
            {
                lastCheckTime = DateTime.Now;
                checkPlayers();
            }
        }

        private void API_onResourceStart()
        {
            lastCheckTime = DateTime.Now;
        }

        public void checkPlayers()
        {
            List<Client> players = API.getAllPlayers();

            foreach (Client player in players)
            {
                Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
                if (instance == null)
                {
                    return;
                }

                if (instance.returnPlayerModel() != player.model)
                {
                    API.kickPlayer(player, "~R~ANTICHEAT");
                    return;
                }

                if (instance.returnPlayerHealth() != player.health)
                {
                    API.kickPlayer(player, "~R~ANTICHEAT");
                    return;
                }

                if (instance.returnLastPosition() != null)
                {
                    if (instance.returnLastPosition().DistanceTo(player.position) >= 250f && !player.isInVehicle)
                    {
                        API.kickPlayer(player, "~R~ANTICHEAT");
                        return;
                    }

                    if (instance.returnLastPosition().DistanceTo(player.position) >= 1800f && player.isInVehicle)
                    {
                        API.kickPlayer(player, "~R~ANTICHEAT");
                        return;
                    }
                }

                instance.setLastPosition(player);
            }
                
        }
    }
}
