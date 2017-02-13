using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class OrganizationHandler : Script
    {
        KarmaHandler karma = new KarmaHandler();
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();

        public OrganizationHandler()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Resource Started: Organization Handler");
        }

        [Command("createorganization", GreedyArg = true)]
        public void cmd_createOrganization(Client player, string nameOfOrganization)
        {
            bool doesOrgNameExist = db.checkIfExists("Name", "Organization", nameOfOrganization);
            if (!doesOrgNameExist)
            {
                if (main.isPlayerLoggedIn(player))
                {
                    string organization = db.pullDatabase("Players", "Organization", "Nametag", player.name);
                    if (organization == "none")
                    {
                        if (karma.checkKarma(player, 90) == true)
                        {
                            if (checkStringLength(nameOfOrganization, 5))
                            {
                                db.insertDatabase("Organization", "Owner", player.name);
                                db.updateDatabase("Organization", "Name", nameOfOrganization, "Owner", player.name);
                                db.updateDatabase("Organization", "Type", "Lawful", "Owner", player.name);
                                db.updateDatabase("Players", "Organization", nameOfOrganization, "Nametag", player.name);
                                API.sendNotificationToPlayer(player, "Organization " + nameOfOrganization + " has been created successfully.");
                                return;
                            }
                            else
                            {
                                API.sendNotificationToPlayer(player, "Organization name is not long enough.");
                                return;
                            }
                        }

                        if (karma.checkKarma(player, -90) == false)
                        {
                            if (checkStringLength(nameOfOrganization, 5))
                            {
                                db.insertDatabase("Organization", "Owner", player.name);
                                db.updateDatabase("Organization", "Name", nameOfOrganization, "Owner", player.name);
                                db.updateDatabase("Organization", "Type", "Chaotic", "Owner", player.name);
                                db.updateDatabase("Players", "Organization", nameOfOrganization, "Nametag", player.name);
                                API.sendNotificationToPlayer(player, "Organization " + nameOfOrganization + " has been created successfully.");
                                return;
                            }
                            else
                            {
                                API.sendNotificationToPlayer(player, "Organization name is not long enough.");
                                return;
                            }
                        }

                        if (!karma.checkKarma(player, 25) && karma.checkKarma(player, -25))
                        {
                            if (db.getPlayerMoney(player) >= 500000)
                            {
                                if (checkStringLength(nameOfOrganization, 5))
                                {
                                    db.insertDatabase("Organization", "Owner", player.name);
                                    db.updateDatabase("Organization", "Name", nameOfOrganization, "Owner", player.name);
                                    db.updateDatabase("Organization", "Type", "Neutral", "Owner", player.name);
                                    db.updateDatabase("Players", "Organization", nameOfOrganization, "Nametag", player.name);
                                    API.sendNotificationToPlayer(player, "Organization " + nameOfOrganization + " has been created successfully.");
                                    return;
                                }
                                else
                                {
                                    API.sendNotificationToPlayer(player, "Organization name is not long enough.");
                                    return;
                                }
                            }
                            else
                            {
                                API.sendNotificationToPlayer(player, "You do not have enough money to create a neutral organization.");
                                return;
                            }
                        }
                        API.sendNotificationToPlayer(player, "You don't have enough karma.");
                        return;
                    }
                    API.sendNotificationToPlayer(player, "You are already in an organization.");
                    return;
                }
            }
        }


        public bool checkStringLength(string name, int length)
        {
            if (name.Length >= length)
            {
                return true;
            }
            return false;
        }
    }
}
