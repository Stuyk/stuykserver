using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class SurgeryHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();

        public SurgeryHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "saveSurgeryFace")
            {
                string[] varNames = { "skinShapeFirst", "skinShapeSecond", "skinSkinFirst", "skinSkinSecond", "skinShapeMix", "skinSkinMix", "NoseWidth", "NoseHeight", "NoseLength", "NoseBridge", "NoseTip", "NoseBridgeDepth", "EyebrowHeight", "EyebrowDepth", "CheekboneHeight", "CheekboneDepth", "CheekboneWidth", "Eyelids", "Lips", "JawWidth", "JawDepth", "JawLength", "ChinFullness", "ChinWidth", "NeckWidth", "Ageing", "Complexion", "Moles", "skinGender", "Blemishes", "SunDamage" };
                string before = "UPDATE PlayerSkins SET";
                string after = string.Format("WHERE PlayerID='{0}'", Convert.ToString(API.getEntityData(player, "PlayerID")));

                db.compileQuery(before, after, varNames, arguments);
                API.call("SkinHandler", "loadCurrentFace", player);
                actionLeaveSurgery(player);
                API.call("ClothingHandler", "updateClothingForPlayer", player);
            }

            if (eventName == "leaveSurgery")
            {
                API.call("SkinHandler", "loadCurrentFace", player);
                actionLeaveSurgery(player);
                API.call("ClothingHandler", "updateClothingForPlayer", player);
            }
        }

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public void actionSurgery(Client player)
        {
            if (!player.isInVehicle)
            {
                API.requestIpl("RC12B_HospitalInterior");
                API.setEntityData(player, "ReturnPosition", player.position);
                API.triggerClientEvent(player, "setupSurgeryShop");
                API.setEntitySyncedData(player, "StopDraws", true);
                API.delay(3000, true, () =>
                {
                    API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
                    API.setEntityPosition(player, new Vector3(349.3453, -590.2299, 42.3150));
                    API.setEntityRotation(player, new Vector3(0, 0, 43));
                    API.setEntityDimension(player, Convert.ToInt32(API.getEntityData(player, "PlayerID")));
                    API.sendNativeToPlayer(player, (ulong)Hash.TASK_TURN_PED_TO_FACE_COORD, player, 350.3817, -587.4160, 42.3150, 5000);
                });
            }
        }

        public void actionLeaveSurgery(Client player)
        {
            API.freezePlayer(player, false);
            API.setEntitySyncedData(player, "StopDraws", false);
            Vector3 returnPosition = (Vector3)API.getEntityData(player, "ReturnPosition");
            API.setEntityDimension(player, 0);
            API.setEntityData(player, "CHEAT_ALLOW_TELEPORT", true);
            API.setEntityPosition(player, returnPosition);
            API.stopPlayerAnimation(player);
            API.stopPedAnimation(player);
            API.setEntityData(player, "ReturnPosition", null);
        }
    }
}
