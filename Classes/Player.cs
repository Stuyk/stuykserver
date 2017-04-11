using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class Player : Script, IDisposable
    {
        DatabaseHandler db = new DatabaseHandler();

        bool admin;

        public Player()
        {
            API.consoleOutput("Started: Player Class");
        }

        public Player(DataRow row, Client player)
        {
            playerName = row["Nametag"].ToString();
            playerNameTag = player.nametag;
            playerID = Convert.ToInt32(row["ID"]);
            playerClient = player;
            playerNetHandle = player.handle;
            playerSocialClub = row["SocialClub"].ToString();
            nametagVisible = true;
            playerKarma = Convert.ToInt32(row["Karma"]);
            playerCash = Convert.ToInt32(row["Money"]);
            playerBank = Convert.ToInt32(row["Bank"]);
            playerHealth = Convert.ToInt32(row["Health"]);
            playerArmor = Convert.ToInt32(row["Armor"]);
            playerOrganization = Convert.ToInt32(row["Organization"]);
            playerBusiness = Convert.ToInt32(row["Business"]);
            playerOldTime = Convert.ToInt32(row["Time"]);
            dead = Convert.ToBoolean(row["Dead"]);
            admin = Convert.ToBoolean(row["Admin"]);
            playerSession = DateTime.Now;
            Vector3 position = new Vector3(Convert.ToSingle(row["LASTX"]), Convert.ToSingle(row["LASTY"]), Convert.ToSingle(row["LASTZ"]));
            playerWeapons = new Dictionary<WeaponHash, int>();
            loadPlayer(position);
            updateKarma();
            setPlayerHealth(playerHealth);
            setPlayerArmor(playerArmor);
            // WEAPONS
        }

        DateTime playerSession; // When the account logged in.
        string playerName; // Player Name
        string playerNameTag; // Player Nametag = player.nametag
        int playerID; // Row ID
        int playerOldTime; // Time Logged from Database.
        Client playerClient; // Current Client Session
        NetHandle playerNetHandle; // Current NetHandle Session.
        string playerSocialClub; // Player SocialClub.
        bool nametagVisible; // is the nametag visible.
        bool dead; // Are you dead?
        int playerKarma; // Player Karma
        int playerCash; // Player Cash
        int playerBank; // Player Bank
        int playerHealth; // Player Health
        int playerArmor; // Player Armor
        int playerOrganization; // Player Organization by INT ID
        int playerBusiness; // Player Business by INT ID
        int playerModel; // INT ID of Player Model
        Dictionary<WeaponHash, int> playerWeapons; // Weapon + Ammo
        Vector3 lastKnownPosition;

        public void savePlayer()
        {
            Vector3 pos;

            if (API.getEntityData(playerClient, "ReturnPosition") == null)
            {
                pos = playerClient.position;
            }
            else
            {
                pos = (Vector3)API.getEntityData(playerClient, "ReturnPosition");
            }

            string[] varNames = { "LASTX", "LASTY", "LASTZ", "Money", "Bank", "Nametag", "Karma", "Health", "Armor", "Organization", "Business", "Time", "Dead" };
            string before = "UPDATE Players SET";
            object[] data = { pos.X.ToString(), pos.Y.ToString(), pos.Z.ToString(), playerCash, playerBank, playerName, playerKarma, playerClient.health.ToString(), playerClient.armor.ToString(), playerOrganization.ToString(), playerBusiness.ToString(), getSessionTime().ToString(), Convert.ToInt32(dead) };
            string after = string.Format("WHERE ID='{0}'", playerID);

            // Send all our data to generate the query and run it
            db.compileQuery(before, after, varNames, data);
        }

        public void loadPlayer(Vector3 position)
        {
            playerClient.nametagVisible = nametagVisible;
            playerClient.name = playerName;
            playerClient.nametag = playerNameTag;

            API.consoleOutput("{0} logged in || Admin = {1} || On: {2}", playerName, admin, playerSession);
            string[] logNames = { "LoggedIn" };
            string logBefore = "UPDATE Players SET";
            object[] logData = { "1" };
            string after = string.Format("WHERE ID='{0}'", playerID);

            // Send all our data to generate the query and run it
            db.compileQuery(logBefore, after, logNames, logData);

            API.setEntityDimension(playerClient, 0);
            API.setEntityPosition(playerClient, position);
            API.setEntityData(playerClient, "PlayerID", playerID);
            API.setEntityData(playerClient, "AlreadyDied", false);
            API.setEntityData(playerClient, "ActiveShooter", false);
            API.setEntityData(playerClient, "CHEAT_ALLOW_TELEPORT", false);

            API.setEntitySyncedData(playerClient, "StopDraws", false); // Tells main level drawing to draw or not.
            API.setEntityData(playerClient, "Instance", this); // Setup our instance with this.
        }

        public void savePlayerLogOut()
        {
            Vector3 pos;

            if (API.getEntityData(playerClient, "ReturnPosition") == null)
            {
                pos = playerClient.position;
            }
            else
            {
                pos = (Vector3)API.getEntityData(playerClient, "ReturnPosition");
            }

            string[] varNames = { "LASTX", "LASTY", "LASTZ", "Money", "Bank", "Nametag", "Karma", "Health", "Armor", "Organization", "Business", "Time", "LoggedIn", "Dead" };
            string before = "UPDATE Players SET";
            object[] data = { pos.X.ToString(), pos.Y.ToString(), pos.Z.ToString(), playerCash, playerBank, playerName, playerKarma, playerClient.health.ToString(), playerClient.armor.ToString(), playerOrganization.ToString(), playerBusiness.ToString(), getSessionTime().ToString(), "0", Convert.ToInt32(dead) };
            string after = string.Format("WHERE ID='{0}'", playerID);

            // Send all our data to generate the query and run it
            db.compileQuery(before, after, varNames, data);
        }

        public void updateKarma()
        {
            if (playerKarma == 0)
            {
                string displayText = "~w~Pure Neutral";
                API.triggerClientEvent(playerClient, "updateKarma", displayText);
            }
            if (playerKarma > 0 && playerKarma < 25)
            {
                string displayText = "~b~Good";
                API.triggerClientEvent(playerClient, "updateKarma", displayText);
            }

            if (playerKarma > 25 && playerKarma < 90)
            {
                string displayText = "~g~Great";
                API.triggerClientEvent(playerClient, "updateKarma", displayText);
            }

            if (playerKarma > 90 && playerKarma <= 250)
            {
                string displayText = "~p~Perfect";
                API.triggerClientEvent(playerClient, "updateKarma", displayText);
            }

            if (playerKarma >= 500)
            {
                string displayText = "~p~Lawful";
                API.triggerClientEvent(playerClient, "updateKarma", displayText);
            }

            if (playerKarma < 0 && playerKarma > -25)
            {
                string displayText = "~y~Scum";
                API.triggerClientEvent(playerClient, "updateKarma", displayText);
            }

            if (playerKarma < -25 && playerKarma > -95)
            {
                string displayText = "~o~Terrible";
                API.triggerClientEvent(playerClient, "updateKarma", displayText);
            }

            if (playerKarma < -90 && playerKarma >= -250)
            {
                string displayText = "~r~Feared";
                API.triggerClientEvent(playerClient, "updateKarma", displayText);
            }

            if (playerKarma >= -500 && playerKarma <= -251)
            {
                string displayText = "~r~Chaotic";
                API.triggerClientEvent(playerClient, "updateKarma", displayText);
            }
        }

        public void setPlayerModel(int model)
        {
            playerModel = model;
        }

        public int returnPlayerModel()
        {
            return playerModel;
        }

        public void setDead(bool value)
        {
            dead = value;
            savePlayer();
        }

        public bool isDead()
        {
            if (dead)
            {
                return true;
            }
            return false;
        }

        public bool isAdmin()
        {
            if (admin)
            {
                return true;
            }
            return false;
        }

        public int getSessionTime()
        {
            DateTime endTime = DateTime.Now;
            DateTime startTime = Convert.ToDateTime(playerSession);
            TimeSpan span = (endTime - startTime);
            int timePlayed = Convert.ToInt32(span.Minutes);;

            return timePlayed += playerOldTime;
        }

        public string returnPlayerName()
        {
            return playerName;
        }

        public string returnPlayerNameTag()
        {
            return playerNameTag;
        }

        public int returnPlayerID()
        {
            return playerID;
        }

        public NetHandle returnPlayerNetHandle()
        {
            return playerNetHandle;
        }

        public string returnSocialClub()
        {
            return playerSocialClub;
        }

        public int returnPlayerKarma()
        {
            return playerKarma;
        }

        public int returnPlayerCash()
        {
            return playerCash;
        }

        public void setPlayerKarma(int amount)
        {
            playerKarma = amount;
        }

        public void addPlayerKarma(int amount)
        {
            playerKarma += amount;
            updateKarma();
        }

        public void addPlayerBank(int amount)
        {
            playerBank += amount;
        }

        public void removePlayerBank(int amount)
        {
            playerBank -= amount;
        }

        public void addPlayerCash(int amount)
        {
            playerCash += amount;
            API.triggerClientEvent(playerClient, "update_money_display", playerCash);
            API.sendNotificationToPlayer(playerClient, string.Format("~g~$ +{0}", amount));
        }

        public void removePlayerCash(int amount)
        {
            playerCash -= amount;
            API.triggerClientEvent(playerClient, "update_money_display", playerCash);
            API.sendNotificationToPlayer(playerClient, string.Format("~g~$ ~r~-{0}", amount));
        }

        public void removePlayerKarma(int amount)
        {
            playerKarma -= amount;
            updateKarma();
        }

        public int returnPlayerBank()
        {
            return playerBank;
        }

        public void setPlayerHealth(int amount)
        {
            API.setEntityData(playerClient, "CHEAT_HEALTH", amount);
            API.setPlayerHealth(playerClient, amount);
            playerClient.health = amount;
        }

        public void setPlayerArmor(int amount)
        {
            API.setEntityData(playerClient, "CHEAT_ARMOR", amount);
            API.setPlayerArmor(playerClient, amount);
            playerClient.armor = amount;
        }

        public int returnPlayerHealth()
        {
            return playerHealth;
        }

        public int returnPlayerArmor()
        {
            return playerArmor;
        }

        public int returnPlayerOrganization()
        {
            return playerOrganization;
        }

        public void setPlayerOrganization(int number)
        {
            playerOrganization = number;
        }

        public void setPlayerWeaponAndAmmo(WeaponHash weapon, int ammunition)
        {
            if (!playerWeapons.ContainsKey(weapon))
            {
                playerWeapons.Add(weapon, ammunition);
            }
            else
            {
                playerWeapons[weapon] = ammunition;
            }
        }

        public void removePlayerWeaponAndAmmo(WeaponHash weapon)
        {
            if (playerWeapons.ContainsKey(weapon))
            {
                playerWeapons.Remove(weapon);
            }
        }

        public Dictionary<WeaponHash, int> returnPlayerWeaponAndAmmo()
        {
            return playerWeapons;
        }

        public void setLastPosition(Client player)
        {
            lastKnownPosition = player.position;
        }

        public Vector3 returnLastPosition()
        {
            return lastKnownPosition;
        }

        public void Dispose()
        {
            API.consoleOutput("{0} has disconnected.", playerName);
            playerName = null;
            playerNameTag = null;
            playerID = -1;
            if (API.getEntityData(playerClient, "DeathText") != null)
            {
                API.deleteEntity(API.getEntityData(playerClient, "DeathText"));
            }
            playerClient = null;
            playerSocialClub = null;
            playerKarma = -1;
            playerCash = -1;
            playerHealth = -1;
            playerArmor = -1;
            playerOrganization = -1;
            playerBusiness = -1;
            playerWeapons.Clear();
            admin = false;
            playerOldTime = -1;
            
            GC.SuppressFinalize(this);
        }
    }
}
