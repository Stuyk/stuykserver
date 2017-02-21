using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class BarberShopHandler : Script
    {
        Main main = new Main();
        SkinHandler skinHandler = new SkinHandler();
        DatabaseHandler db = new DatabaseHandler();
        List<Vector3> barberShops = new List<Vector3>(); // All barbershops from the database are pulled into here.
        Dictionary<Client, Vector3> playersInBarbershop = new Dictionary<Client, Vector3>();
        List<ColShape> collisionShapes = new List<ColShape>();
        List<Client> playersInCollisions = new List<Client>();

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
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                if (collisionShapes.Contains(colshape))
                {
                    if (playersInCollisions.Contains(API.getPlayerFromHandle(entity)))
                    {
                        playersInCollisions.Remove(API.getPlayerFromHandle(entity));
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "removeUseFunction");
                    }
                }
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (Convert.ToInt32(API.getEntityType(entity)) == 6)
            {
                if (collisionShapes.Contains(colshape))
                {
                    if (!playersInCollisions.Contains(API.getPlayerFromHandle(entity)))
                    {
                        playersInCollisions.Add(API.getPlayerFromHandle(entity));
                        API.triggerClientEvent(API.getPlayerFromHandle(entity), "triggerUseFunction");
                        API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), "A classic looking barbershop.");
                    }
                }
            }
        }

        // Pull from Database find positions.
        private void API_onResourceStart()
        {
            API.consoleOutput("Started: BarberShop Handler");

            string query = "SELECT ID FROM BarberShops";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            int initializedObjects = 0;

            foreach (DataRow row in result.Rows)
            {
                foreach (DataColumn column in result.Columns)
                {
                    string selectedrow = row[column].ToString();

                    float posX = Convert.ToSingle(db.pullDatabase("BarberShops", "PosX", "ID", selectedrow));
                    float posY = Convert.ToSingle(db.pullDatabase("BarberShops", "PosY", "ID", selectedrow));
                    float posZ = Convert.ToSingle(db.pullDatabase("BarberShops", "PosZ", "ID", selectedrow));

                    positionBlips(new Vector3(posX, posY, posZ));

                    initializedObjects = ++initializedObjects;
                }
            }

            API.consoleOutput("BarberShops Initialized: " + initializedObjects.ToString());
        }

        // Place blips and text labels for interactions.
        public void positionBlips(Vector3 position)
        {
            var newBlip = API.createBlip(new Vector3(position.X, position.Y, position.Z));
            API.setBlipSprite(newBlip, 71);
            API.setBlipColor(newBlip, 9);
            barberShops.Add(new Vector3(position.X, position.Y, position.Z));
            collisionShapes.Add(API.createCylinderColShape(new Vector3(position.X, position.Y, position.Z), 5f, 5f));
            int i = 0;
            ++i;
        }

        public void selectBarberShop(Client player)
        {
            if (db.isPlayerLoggedIn(player))
            {
                if (!player.isInVehicle) // If player is not in Vehicle
                {
                    foreach (Vector3 pos in barberShops)
                    {
                        if (player.position.DistanceTo(pos) <= 5)
                        {
                            if (db.getPlayerMoney(player) >= 30)
                            {
                                playersInBarbershop.Add(player, player.position);
                                Random rand = new Random();
                                int dimension = rand.Next(1, 1000);
                                API.setEntityDimension(player, dimension);
                                API.consoleOutput(player.name + " is in dimension " + API.getEntityDimension(player).ToString());
                                API.triggerClientEvent(player, "openSkinPanel", player.position);
                                API.setEntityPosition(player, new Vector3(-1279.177, -1118.023, 6.990117));
                                API.triggerClientEvent(player, "createCamera", new Vector3(-1281.826, -1118.141, 7.5), player.position);
                                API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@female_arms_crossed@base", "base");
                                player.rotation = new Vector3(0, 0, 88.95126);
                                skinHandler.loadLocalFaceData(player);
                                playersInCollisions.Remove(player);
                                return;
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(player, main.msgPrefix + "Not enough money.");
                                return;
                            }
                        }
                    }
                }
            }
            return;
        }

        public void leaveBarberShop(Client player)
        {
            Vector3 leavePosition = playersInBarbershop[player];
            API.setEntityDimension(player, 0);
            API.setEntityPosition(player, leavePosition);
            playersInBarbershop.Remove(player);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
        }
    }
}
