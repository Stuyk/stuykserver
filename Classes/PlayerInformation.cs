using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class PlayerInformation : Script, IDisposable
    {
        string playerName;
        string playerNameTag;
        int playerID;
        Client playerClient;
        NetHandle playerNetHandle;
        string playerSocialClub;
        bool nametagVisible;
        int playerKarma;
        int playerCash;
        int playerHealth;
        int playerArmor;
        int playerOrganization;
        Dictionary<WeaponHash, int> playerWeapons; // Weapon + Ammo

        public void setupPlayer(Client player, int id)
        {
            playerClient = player;
            playerName = player.name;
            playerNameTag = player.nametag;
            playerID = id;
            playerNetHandle = player.handle;
            playerSocialClub = player.socialClubName;
            nametagVisible = true;
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

        public void addPlayerKarma(int amount)
        {
            playerKarma += amount;
        }

        public void addPlayerCash(int amount)
        {
            playerCash += amount;
        }

        public void removePlayerCash(int amount)
        {
            playerCash -= amount;
        }

        public void removePlayerKarma(int amount)
        {
            playerKarma -= amount;
        }

        public void setPlayerHealth(int amount)
        {
            API.setPlayerHealth(playerClient, amount);
        }

        public void setPlayerArmor(int amount)
        {
            API.setPlayerArmor(playerClient, amount);
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

        public void Dispose()
        {
            playerName = null;
            playerNameTag = null;
            playerID = -1;
            playerClient = null;
            playerSocialClub = null;
            playerKarma = -1;
            playerCash = -1;
            playerHealth = -1;
            playerArmor = -1;
            playerOrganization = -1;
            playerWeapons.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
