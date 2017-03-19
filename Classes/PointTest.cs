using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class PointTest : Script
    {
        TextLabel capturePercent;
        Vector3 point = new Vector3(399.3882, -132.6692, 64);
        ColShape whatever;
        Marker whatmarker;

        int currentAmount = 0;
        bool playerInMarker;

        DateTime updateTime;

        public PointTest()
        {
            API.onResourceStart += API_onResourceStart;
            API.onUpdate += API_onUpdate;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onEntityExitColShape += API_onEntityExitColShape;
        }

        private void API_onEntityExitColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client player = API.getPlayerFromHandle(entity);
                API.sendChatMessageToPlayer(player, "Out of point.");
                playerInMarker = false;
            }
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                Client player = API.getPlayerFromHandle(entity);
                playerInMarker = true;
                updateTime = DateTime.Now;
                API.sendChatMessageToPlayer(player, "In Point");
            }
        }

        private void API_onUpdate()
        {
            if (playerInMarker)
            {
                if (DateTime.Now >= updateTime.AddSeconds(10))
                {
                    updateTime = DateTime.Now;
                    currentAmount += 1;
                    API.setTextLabelText(capturePercent, string.Format("{0}%", currentAmount));
                }
            }
        }

        private void API_onResourceStart()
        {
            capturePercent = API.createTextLabel(string.Format("{0}%", currentAmount), new Vector3(point.X, point.Y, point.Z + 1), 10f, 1f);
            whatever = API.createCylinderColShape(point, 10f, 10f);
            whatmarker = API.createMarker(1, point, new Vector3(), new Vector3(), new Vector3(10f, 10f, 10f), 25, 0, 0, 200);
        }
    }
}
