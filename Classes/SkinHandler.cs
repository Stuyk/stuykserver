using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class SkinHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Main main = new Main();

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public SkinHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        public void API_onClientEventTrigger(Client player, string eventName, params object[] args)
        {
            if (eventName == "saveFace")
            {
                actionSaveFace(player, args);
                loadCurrentFace(player);
                API.stopPlayerAnimation(player);
                API.stopPedAnimation(player);
                API.call("ClothingHandler", "updateClothingForPlayer", player);
                API.call("BarberShopHandler", "leaveBarberShop", player);
                API.triggerClientEvent(player, "killPanel");
                API.triggerClientEvent(player, "endCamera");
            }

            if (eventName == "exitFace")
            {
                API.call("BarberShopHandler", "leaveBarberShop", player);
                API.triggerClientEvent(player, "killPanel");
                API.triggerClientEvent(player, "endCamera");
                loadCurrentFace(player);
                API.call("ClothingHandler", "updateClothingForPlayer", player);
            }
        }

        public void actionSaveFace(Client player, params object[] args)
        {
            string[] varNames = { "skinShapeFirst", "skinShapeSecond", "skinSkinFirst", "skinSkinSecond", "skinShapeMix", "skinSkinMix", "skinHairstyle", "skinHairstyleColor",
                    "skinHairstyleHighlight", "skinHairstyleTexture", "NoseWidth", "NoseHeight", "NoseLength", "NoseBridge", "NoseTip", "NoseBridgeDepth", "EyebrowHeight", "EyebrowDepth",
                    "CheekboneHeight", "CheekboneDepth", "CheekboneWidth", "Eyelids", "Lips", "JawWidth", "JawDepth", "JawLength", "ChinFullness", "ChinWidth", "NeckWidth", "FacialHair",
                    "FacialHairColor", "FacialHairColor2", "Ageing", "Complexion", "Moles" };
            string before = "UPDATE PlayerSkins SET";
            string after = string.Format("WHERE PlayerID='{0}'", Convert.ToString(API.getEntitySyncedData(player, "PlayerID")));

            db.compileQuery(before, after, varNames, args);
        }

        public void loadCurrentFace(Client player)
        {
            string[] varNames = { "PlayerID" };
            string before = "SELECT * FROM PlayerSkins WHERE";
            object[] data = { Convert.ToInt32(API.getEntitySyncedData(player, "PlayerID")) };
            DataTable result = db.compileSelectQuery(before, varNames, data);

            int gender = Convert.ToInt32(result.Rows[0]["skinGender"]);
            if (gender == 0)
            {
                API.setPlayerSkin(player, PedHash.FreemodeMale01);
            }
            else
            {
                API.setPlayerSkin(player, PedHash.FreemodeFemale01);
            }

            API.exported.gtaocharacter.initializePedFace(player.handle);
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_FIRST_ID", Convert.ToInt32(result.Rows[0]["skinShapeFirst"]));
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_SECOND_ID", Convert.ToInt32(result.Rows[0]["skinShapeSecond"]));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_FIRST_ID", Convert.ToInt32(result.Rows[0]["skinSkinFirst"]));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_SECOND_ID", Convert.ToInt32(result.Rows[0]["skinSkinSecond"]));
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", Convert.ToSingle(result.Rows[0]["skinShapeMix"]));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", Convert.ToSingle(result.Rows[0]["skinSkinMix"]));
            API.setEntitySyncedData(player.handle, "GTAO_HAIRSTYLE", Convert.ToInt32(result.Rows[0]["skinHairstyle"])); // Not used by GTAO HANDLE
            API.setEntitySyncedData(player.handle, "GTAO_HAIRSTYLE_TEXTURE", Convert.ToInt32(result.Rows[0]["skinHairstyleTexture"])); // Not used by GTAO HANDLE
            player.setClothes(2, Convert.ToInt32(result.Rows[0]["skinHairstyle"]), Convert.ToInt32(result.Rows[0]["skinHairstyleTexture"]));
            API.setEntitySyncedData(player.handle, "GTAO_HAIR_COLOR", Convert.ToInt32(result.Rows[0]["skinHairstyleColor"]));
            API.setEntitySyncedData(player.handle, "GTAO_HAIR_HIGHLIGHT_COLOR", Convert.ToInt32(result.Rows[0]["skinHairstyleHighlight"]));
            API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR", Convert.ToInt32(result.Rows[0]["FacialHair"]));
            API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR", Convert.ToInt32(result.Rows[0]["FacialHairColor"]));
            API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR2", Convert.ToInt32(result.Rows[0]["FacialHairColor2"]));
            API.setEntitySyncedData(player.handle, "GTAO_AGEING", Convert.ToInt32(result.Rows[0]["Ageing"]));
            API.setEntitySyncedData(player.handle, "GTAO_COMPLEXION", Convert.ToInt32(result.Rows[0]["Complexion"]));
            API.setEntitySyncedData(player.handle, "GTAO_MOLES", Convert.ToInt32(result.Rows[0]["Moles"]));

            var list = new float[21];
            list[0] = Convert.ToSingle(result.Rows[0]["NoseWidth"]);
            list[1] = Convert.ToSingle(result.Rows[0]["NoseHeight"]);
            list[2] = Convert.ToSingle(result.Rows[0]["NoseLength"]);
            list[3] = Convert.ToSingle(result.Rows[0]["NoseBridge"]);
            list[4] = Convert.ToSingle(result.Rows[0]["NoseTip"]);
            list[5] = Convert.ToSingle(result.Rows[0]["NoseBridgeDepth"]);
            list[6] = Convert.ToSingle(result.Rows[0]["EyebrowHeight"]);
            list[7] = Convert.ToSingle(result.Rows[0]["EyebrowDepth"]);
            list[8] = Convert.ToSingle(result.Rows[0]["CheekboneHeight"]);
            list[9] = Convert.ToSingle(result.Rows[0]["CheekboneDepth"]);
            list[10] = Convert.ToSingle(result.Rows[0]["CheekboneWidth"]);
            list[11] = Convert.ToSingle(result.Rows[0]["Eyelids"]);
            list[12] = Convert.ToSingle(result.Rows[0]["Lips"]);
            list[13] = Convert.ToSingle(result.Rows[0]["JawWidth"]);
            list[14] = Convert.ToSingle(result.Rows[0]["JawDepth"]);
            list[15] = Convert.ToSingle(result.Rows[0]["JawLength"]);
            list[16] = Convert.ToSingle(result.Rows[0]["ChinFullness"]);
            list[17] = Convert.ToSingle(result.Rows[0]["ChinWidth"]);
            list[18] = 0;
            list[19] = Convert.ToSingle(result.Rows[0]["NeckWidth"]);
            list[20] = 0;

            API.setEntitySyncedData(player.handle, "GTAO_FACE_FEATURES_LIST", list);
            API.exported.gtaocharacter.updatePlayerFace(player.handle);
        }
    }
}
