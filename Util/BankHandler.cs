using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class BankHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();
        List<Vector3> atmsList = new List<Vector3>();

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public BankHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "stopAnimation")
            {
                player.stopAnimation();
            }

            if (eventName == "withdrawATM_Server")
            {
                int input = Convert.ToInt32(arguments[0]);
                if (input > 0)
                {
                    int atmMoney = db.getPlayerAtmMoney(player);
                    if (atmMoney > 0 && input <= atmMoney)
                    {
                        db.setPlayerMoney(player, +input);
                        db.setPlayerAtmMoney(player, -input);
                        API.triggerClientEvent(player, "killPanel");
                        API.stopPlayerAnimation(player);
                    }
                }
                else
                {
                    API.sendNotificationToPlayer(player, "~r~Invalid number.");
                }
            }

            if (eventName == "depositATM_Server")
            {
                int input = Convert.ToInt32(arguments[0]);
                if (input > 0)
                {
                    int playerMoney = db.getPlayerMoney(player);
                    if (playerMoney > 0 && input <= playerMoney)
                    {
                        db.setPlayerMoney(player, -input);
                        db.setPlayerAtmMoney(player, +input);
                        API.triggerClientEvent(player, "killPanel");
                        API.stopPlayerAnimation(player);
                    }  
                }
                else
                {
                    API.sendNotificationToPlayer(player, "~r~Invalid number.");
                }
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Bank Handler");

            string query = "SELECT ID FROM Banks";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                foreach (DataColumn column in result.Columns)
                {
                    string selectedrow = row[column].ToString();

                    float posX = Convert.ToSingle(db.pullDatabase("Banks", "PosX", "ID", selectedrow));
                    float posY = Convert.ToSingle(db.pullDatabase("Banks", "PosY", "ID", selectedrow));
                    float posZ = Convert.ToSingle(db.pullDatabase("Banks", "PosZ", "ID", selectedrow));
                    float rotX = Convert.ToSingle(db.pullDatabase("Banks", "RotX", "ID", selectedrow));
                    float rotY = Convert.ToSingle(db.pullDatabase("Banks", "RotY", "ID", selectedrow));
                    float rotZ = Convert.ToSingle(db.pullDatabase("Banks", "RotZ", "ID", selectedrow));

                    positionBlips(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ));

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Banks Initialized: " + initializedObjects.ToString());
        }

        public void selectATM(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (!player.isInVehicle) // If player is not in Vehicle
                {
                    foreach (Vector3 pos in atmsList)
                    {
                        if (player.position.DistanceTo(pos) <= 5)
                        {

                            API.triggerClientEvent(player, "loadATM", db.getPlayerAtmMoney(player));
                            API.playPlayerAnimation(player, (int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Cancellable), "amb@prop_human_atm@male@enter", "enter");
                            API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.Cancellable), "amb@prop_human_atm@male@base", "base");
                            return;
                        }
                    }
                }
            }
            return;
        }

        // Spawns an ATM at the player. Must face away from wall.
        [Command("createatm")]
        public void cmdCreateBank(Client player, string id)
        {
            if(db.isAdmin(player.name))
            {
                string checkID = db.pullDatabase("Banks", "ID", "ID", id);

                if (checkID == null)
                {
                    string posX = player.position.X.ToString();
                    string posY = player.position.Y.ToString();
                    string posZ = player.position.Z.ToString();
                    string rotX = player.rotation.X.ToString();
                    string rotY = player.rotation.Y.ToString();
                    string rotZ = player.rotation.Z.ToString();

                    db.insertDatabase("Banks", "ID", id);
                    db.updateDatabase("Banks", "PosX", posX, "ID", id);
                    db.updateDatabase("Banks", "PosY", posY, "ID", id);
                    db.updateDatabase("Banks", "PosZ", posZ, "ID", id);
                    db.updateDatabase("Banks", "RotX", rotX, "ID", id);
                    db.updateDatabase("Banks", "RotY", rotY, "ID", id);
                    db.updateDatabase("Banks", "RotZ", rotZ, "ID", id);

                    positionBlips(new Vector3(player.position.X, player.position.Y, player.position.Z), new Vector3(player.rotation.X, player.rotation.Y, player.rotation.Z));
                    API.sendNotificationToPlayer(player, "~g~An atm has been created.");
                    return;
                }
                else
                {
                    API.sendNotificationToPlayer(player, "~r~This ID already exists.");
                    return;
                }
            }
            else
            {
                API.sendNotificationToPlayer(player, "You are not an administrator.");
            }
        }

        // Positions the objects, blips, and text when initialized or created.
        public void positionBlips(Vector3 position, Vector3 rotation)
        {
            API.createTextLabel("~y~[~w~Keypress: ~g~F~y~]", new Vector3(position.X, position.Y, position.Z + 1.2), 20, 0.5f);
            API.createTextLabel("~w~Access your bank account.", new Vector3(position.X, position.Y, position.Z + 1.0), 20, 0.5f);
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 108);
            API.setBlipColor(newBlip, 2);
            API.createObject(-870868698, new Vector3(position.X, position.Y, position.Z - 1), new Vector3(rotation.X, rotation.Y, rotation.Z - 180));

            atmsList.Add(new Vector3(position.X, position.Y, position.Z));
        }
    }
}
