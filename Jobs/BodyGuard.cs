using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Jobs
{
    public class BodyGuard : Script
    {
        const string notEnoughUnits = "~y~Organization ~b~# ~o~Not enough units.";
        const string notAJob = "~y~Organization ~b~# ~o~Not a organization job.";
        const string targetDoesNotExist = "~y~Organization ~b~# ~o~Target player does not exist.";
        const string targetHasTradeOffer = "~y~Trade Offer ~b~# ~r~Target has a trade offer already.";
        const string targetHasArmor = "~y~Organization ~b~# ~o~Target player already has armor.";

        public bool organizationCheck(Player player, Organization organization)
        {
            if (player == null)
            {
                return false;
            }

            if (organization == null)
            {
                return false;
            }

            return true;
        }

        public bool hasTradeOffer(Client player)
        {
            if (API.hasEntityData(player, "TradeType"))
            {
                return true;
            }
            return false;
        }

        [Command("guard")]
        public void actionGuard(Client player, string target, int price = 100)
        {
            if (price < 0)
            {
                return;
            }

            Player instance = (Player)API.call("PlayerHandler", "getPlayer", player);
            Organization organization = (Organization)API.call("OrganizationHandler", "returnOrganization", instance.returnPlayerOrganization());

            Client foundPlayer = null;
            List<Client> players = API.getPlayersInRadiusOfPlayer(10f, player);
            foreach (Client targetPlayer in players)
            {
                if (targetPlayer.name == target)
                {
                    foundPlayer = targetPlayer;
                    break;
                }
            }

            // Check if the player was actually found.
            if (foundPlayer == null)
            {
                API.sendChatMessageToPlayer(player, targetDoesNotExist);
                return;
            }

            // If the player already has a trade offer.
            if (hasTradeOffer(foundPlayer))
            {
                API.sendChatMessageToPlayer(player, targetHasTradeOffer);
                return;
            }

            if (foundPlayer.armor == 100)
            {
                API.sendChatMessageToPlayer(player, targetHasArmor);
                return;
            }

            // Check for nulls.
            if (!organizationCheck(instance, organization))
            {
                return;
            }

            // Check if the organization has the job.
            if (!organization.returnPrimaryJobAddons().Contains(Organization.JobAddons.BodyGuard))
            {
                API.sendChatMessageToPlayer(player, notAJob);
                return;
            }

            // Check if the organization has the units.
            if (organization.returnOrganizationUnits() < 100)
            {
                API.sendChatMessageToPlayer(player, notEnoughUnits);
                return;
            }

            API.setEntityData(foundPlayer, "TradeType", "BodyGuard"); // What they are offering.
            API.setEntityData(foundPlayer, "TradePrice", price); // What the price is of the trade.
            API.setEntityData(foundPlayer, "Trader", player); // Who initiated the trade.

            API.sendChatMessageToPlayer(player, string.Format("~y~Trade Offer ~b~# ~g~You have offered your target some body armor."));
            API.sendChatMessageToPlayer(foundPlayer, string.Format("~y~Trade Offer ~b~# ~o~You have been offered some body armor for ~g~${0}", price));
            API.sendChatMessageToPlayer(foundPlayer, string.Format("~y~Trade Offer ~b~# ~r~This offer will expire in 15 seconds.", price));
            API.sendChatMessageToPlayer(foundPlayer, string.Format("~y~Trade Offer ~b~# ~o~You may /accept at any time, or let it expire.", price));

            API.delay(15000, true, () =>
            {
                if (API.hasEntityData(player, "TradeType"))
                {
                    resetTradeData(foundPlayer);
                    API.sendChatMessageToPlayer(foundPlayer, string.Format("~y~Trade Offer ~b~# ~r~Offer has expired"));
                }
            });
        }

        public void actionAccept(Client player)
        {
            // If they don't have the offer, expire this.
            if (!hasTradeOffer(player))
            {
                return;
            }

            Player playerInstance = (Player)API.call("PlayerHandler", "getPlayer", player);
            Client trader = (Client)API.getEntityData(player, "Trader");
            double price = Convert.ToDouble(API.getEntityData(player, "TradePrice"));

            Player traderInstance = (Player)API.call("PlayerHandler", "getPlayer", trader);
            Organization organization = (Organization)API.call("OrganizationHandler", "returnOrganization", traderInstance.returnPlayerOrganization());

            if (trader.position.DistanceTo(player.position) > 10f)
            {
                API.sendChatMessageToPlayer(player, string.Format("~b~{0} ~r~is not within range of you.", trader.name));
                return;
            }

            // Check for nulls.
            if (!organizationCheck(traderInstance, organization))
            {
                resetTradeData(player);
                return;
            }

            if (playerInstance == null)
            {
                resetTradeData(player);
                return;
            }

            if (organization.returnOrganizationUnits() < 100)
            {
                API.sendChatMessageToPlayer(trader, notEnoughUnits);
                API.sendChatMessageToPlayer(player, "~r~They appear to have run out of armor.");
                resetTradeData(player);
                return;
            }

            if (playerInstance.returnPlayerCash() < price)
            {
                API.sendChatMessageToPlayer(player, "~r~You do not have enough money.");
                return;
            }

            playerInstance.removePlayerCash(Convert.ToInt32(price));
            playerInstance.setPlayerArmor(100);

            organization.removeOrganizationUnits(100); // Organization removes units.
            traderInstance.addPlayerCash(Convert.ToInt32(price * 0.4)); // Trader takes 40%
            organization.addOrganizationBank(Convert.ToInt32(price * 0.6)); // Organization takes 60%

            resetTradeData(player);
        }

        public void resetTradeData(Client player)
        {
            API.resetEntityData(player, "TradeType");
            API.resetEntityData(player, "TradePrice");
            API.resetEntityData(player, "Trader");
        }
    }
}
