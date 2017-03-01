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

        Dictionary<ColShape, ShopInformation> shopInformation = new Dictionary<ColShape, ShopInformation>();

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        class ShopInformation
        {
            ColShape collisionShape;
            int collisionID;
            Vector3 collisionPosition;
            Blip collisionBlip;
            GTANetworkServer.Object attachedATM;
            List<Client> collisionPlayers;

            public void setupPoint(ColShape collision, int id, Vector3 position, Blip blip, GTANetworkServer.Object atm)
            {
                collisionShape = collision;
                collisionID = id;
                collisionPosition = position;
                collisionBlip = blip;
                collisionPlayers = new List<Client>();
                attachedATM = atm;
            }

            public void collisionPlayersAdd(Client player)
            {
                if (!collisionPlayers.Contains(player))
                {
                    collisionPlayers.Add(player);
                }
            }

            public void collisionPlayersRemove(Client player)
            {
                if (collisionPlayers.Contains(player))
                {
                    collisionPlayers.Remove(player);
                }
            }

            public List<Client> returnCollisionPlayers()
            {
                return collisionPlayers;
            }

            public int returnID()
            {
                return collisionID;
            }

            public ColShape returnCollision()
            {
                return collisionShape;
            }

            public Vector3 returnPosition()
            {
                return collisionPosition;
            }

            public Blip returnBlip()
            {
                return collisionBlip;
            }
        }

        public BankHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (shopInformation.ContainsKey(colshape))
                {
                    if (shopInformation[colshape].returnCollisionPlayers().Contains(player))
                    {
                        shopInformation[colshape].collisionPlayersRemove(player);
                        API.triggerClientEvent(player, "removeUseFunction");
                    }
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (shopInformation.ContainsKey(colshape))
                {
                    if (!shopInformation[colshape].returnCollisionPlayers().Contains(player))
                    {
                        shopInformation[colshape].collisionPlayersAdd(player);
                        API.triggerClientEvent(player, "triggerUseFunction", "Bank");
                        API.sendNotificationToPlayer(player, "This will let me deposit my cash.");
                    }
                }
            }
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
                        updateATMDisplay(player);
                        API.triggerClientEvent(player, "displayWithdrawSuccess");
                    }
                    else
                    {
                        API.triggerClientEvent(player, "displayNotThatMuch");
                    }
                }
            }

            if (eventName == "balanceNotDisplayed")
            {
                updateATMDisplay(player);
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
                        updateATMDisplay(player);
                        API.triggerClientEvent(player, "depositAlertSuccess");
                    }
                    else
                    {
                        API.triggerClientEvent(player, "displayNotThatMuch");
                    }
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
                    int id = Convert.ToInt32(row[column]);

                    positionBlips(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), id);

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("Banks Initialized: " + initializedObjects.ToString());
        }

        public void updateATMDisplay(Client player)
        {
            int atmMoney = db.getPlayerAtmMoney(player);
            int playerMoney = db.getPlayerMoney(player);

            API.triggerClientEvent(player, "refreshATM", atmMoney, playerMoney);
        }

        public void selectATM(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnCollisionPlayers().Contains(player) && !player.isInVehicle)
                {
                    API.triggerClientEvent(player, "loadATM", db.getPlayerAtmMoney(player));
                    break;
                }
            }
        }

        // Positions the objects, blips, and text when initialized or created.
        public void positionBlips(Vector3 position, Vector3 rotation, int id)
        {
            ShopInformation newShop = new ShopInformation();
            ColShape collision = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 2f, 2f);

            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 108);
            API.setBlipColor(newBlip, 2);

            GTANetworkServer.Object atmObject = API.createObject(-870868698, new Vector3(position.X, position.Y, position.Z - 1), new Vector3(rotation.X, rotation.Y, rotation.Z - 180));

            newShop.setupPoint(collision, id, new Vector3(position.X, position.Y, position.Z), newBlip, atmObject);
            shopInformation.Add(collision, newShop);
        }
    }
}
