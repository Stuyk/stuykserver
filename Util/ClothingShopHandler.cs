using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class ClothingShopHandler : Script
    {
        ClothingHandler clothingHandler = new ClothingHandler();
        DatabaseHandler db = new DatabaseHandler();

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
            List<Client> collisionPlayers; // When a player is in the collision.
            List<Client> containedPlayers; // When a player enters a shop.

            public void setupPoint(ColShape collision, int id, Vector3 position, Blip blip)
            {
                collisionShape = collision;
                collisionID = id;
                collisionPosition = position;
                collisionBlip = blip;
                containedPlayers = new List<Client>();
                collisionPlayers = new List<Client>();
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

            public void containedPlayersAdd(Client player)
            {
                if (!containedPlayers.Contains(player))
                {
                    containedPlayers.Add(player);
                }
            }

            public void containedPlayersRemove(Client player)
            {
                if (containedPlayers.Contains(player))
                {
                    containedPlayers.Remove(player);
                }
            }

            public List<Client> returnCollisionPlayers()
            {
                return collisionPlayers;
            }

            public List<Client> returnContainedPlayers()
            {
                return containedPlayers;
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

        public ClothingShopHandler()
        {
            API.onResourceStart += API_onResourceStart;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnContainedPlayers().Contains(player))
                {
                    db.setPlayerPositionByVector(player, shopInformation[collision].returnPosition());
                    shopInformation[collision].containedPlayersRemove(player);
                }
            }
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
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "removeUseFunction");
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
                    if (!shopInformation[colshape].returnCollisionPlayers().Contains(player) && !player.isInVehicle)
                    {
                        shopInformation[colshape].collisionPlayersAdd(player);
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "Clothing");
                        API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), "There are a lot of clothes in this store.");
                    }
                }
            }
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: Clothing Shop Handler");

            string query = "SELECT * FROM ClothingShops";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                float posX = Convert.ToSingle(row["PosX"]);
                float posY = Convert.ToSingle(row["PosY"]);
                float posZ = Convert.ToSingle(row["PosZ"]);
                int id = Convert.ToInt32(row["ID"]);
                positionBlips(new Vector3(posX, posY, posZ), id);
                ++initializedObjects;
            }

            API.consoleOutput("Clothing Shops Initialized: " + initializedObjects.ToString());
        }

        public void selectClothing(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnCollisionPlayers().Contains(player) && !player.isInVehicle)
                {
                    shopInformation[collision].containedPlayersAdd(player);
                    API.setEntityDimension(player, new Random().Next(1, 1000));
                    API.setEntityPosition(player, new Vector3(-1187.994, -764.7119, 17.31953));
                    API.triggerClientEvent(player, "createCamera", new Vector3(-1190.004, -766.2875, 17.3196), player.position);
                    API.triggerClientEvent(player, "openClothingPanel");
                    clothingHandler.updateLocalClothingVariables(player);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@male_b@base", "base");
                }
            }
        }

        public void positionBlips(Vector3 position, int id)
        {
            ShopInformation newShop = new ShopInformation();
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 5f, 5f);

            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 366);
            API.setBlipColor(newBlip, 7);

            newShop.setupPoint(shape, id, new Vector3(position.X, position.Y, position.Z), newBlip);
            shopInformation.Add(shape, newShop);
        }

        public void leaveClothingShop(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnContainedPlayers().Contains(player))
                {
                    API.setEntityPosition(player, shopInformation[collision].returnPosition());
                    API.setEntityDimension(player, 0);
                    shopInformation[collision].containedPlayersRemove(player);
                    API.stopPlayerAnimation(player);
                    API.stopPedAnimation(player);
                    break;
                }
            }
        }
    }
}
