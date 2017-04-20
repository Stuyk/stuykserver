using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class SurgeryHandler : Script
    {
        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }


        [Command("surgery")]
        public void cmdSurgery(Client player)
        {
            API.requestIpl("RC12B_HospitalInterior");
            API.setEntityData(player, "ReturnPosition", player.position);
            API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
            API.setEntityPosition(player, new Vector3(349.3453, -590.2299, 42.3150));
            API.setEntityRotation(player, new Vector3(0, 0, 43));
            API.setEntityDimension(player, Convert.ToInt32(API.getEntityData(player, "PlayerID")));
            API.sendNativeToPlayer(player, (ulong)Hash.TASK_TURN_PED_TO_FACE_COORD, player, 350.3817, -587.4160, 42.3150, 5000);

            //API.triggerClientEvent(player, "setupBarberShop");
            //API.setEntitySyncedData(player, "StopDraws", true);

        }
    }
}
