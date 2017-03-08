using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class BarberShopHandler : Script
    {
        SkinHandler skinHandler = new SkinHandler();
        DatabaseHandler db = new DatabaseHandler();

        Dictionary<ColShape, ShopInformationHandling> shopInformation = new Dictionary<ColShape, ShopInformationHandling>();

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public BarberShopHandler()
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
                if (shopInformation[collision].returnInsidePlayers().ContainsKey(player))
                {
                    db.setPlayerPositionByVector(player, shopInformation[collision].returnCollisionPosition());
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
                    shopInformation[colshape].removeOutsidePlayer(player);
                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "removeUseFunction");
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                Client player = API.getPlayerFromHandle(entity);
                if (shopInformation.ContainsKey(colshape) && !player.isInVehicle)
                {
                    shopInformation[colshape].addOutsidePlayer(player);
                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction", "BarberShop");
                    API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), "A classic looking barbershop.");
                }
            }
        }

        // Pull from Database find positions.
        private void API_onResourceStart()
        {
            API.consoleOutput("Started: BarberShop Handler");

            string query = "SELECT * FROM BarberShops";
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

            API.consoleOutput("BarberShops Initialized: " + initializedObjects.ToString());
        }

        // Place blips and text labels for interactions.
        public void positionBlips(Vector3 position, int id)
        {
            ShopInformationHandling newShop = new ShopInformationHandling();
            ColShape shape = API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 5f, 5f);

            newShop.setCollisionShape(shape);
            newShop.setCollisionID(id);
            newShop.setCollisionPosition(position);
            newShop.setShopType(ShopInformationHandling.ShopType.Barbershop);
            newShop.setupBlip();

            shopInformation.Add(shape, newShop);
        }

        public void selectBarberShop(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnOutsidePlayers().Contains(player) && !player.isInVehicle)
                {
                    db.setPlayerHUD(player, false);
                    shopInformation[collision].addInsidePlayer(player, player.handle);
                    API.setEntityDimension(player, new Random().Next(1, 1000));
                    API.triggerClientEvent(player, "openSkinPanel", player.handle);
                    API.setEntityPosition(player, new Vector3(-1279.177, -1118.023, 6.990117));
                    API.triggerClientEvent(player, "createCamera", new Vector3(-1281.826, -1118.141, 7.5), player.position);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@male_b@base", "base");
                    player.rotation = new Vector3(0, 0, 88.95126);
                }
            }
        }

        public void leaveBarberShop(Client player)
        {
            foreach (ColShape collision in shopInformation.Keys)
            {
                if (shopInformation[collision].returnInsidePlayers().ContainsKey(player))
                {
                    db.setPlayerHUD(player, true);
                    API.setEntityDimension(player, 0);
                    API.setEntityPosition(player, shopInformation[collision].returnCollisionPosition());
                    shopInformation[collision].removeInsidePlayer(player);
                    API.stopPlayerAnimation(player);
                    API.stopPedAnimation(player);
                }
            }
        }
    }
}
