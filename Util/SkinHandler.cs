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
                string query = string.Format("UPDATE PlayerSkins SET skinShapeFirst=@skinShapeFirst, skinShapeSecond=@skinShapeSecond, skinSkinFirst=@skinSkinFirst, skinSkinSecond=@skinSkinSecond, skinShapeMix=@skinShapeMix, skinSkinMix=@skinSkinMix, skinHairstyle=@skinHairstyle, skinHairstyleColor=@skinHairstyleColor, skinHairstyleHighlight=@skinHairstyleHighlight, skinHairstyleTexture=@skinHairstyleTexture, NoseWidth=@NoseWidth, NoseHeight=@NoseHeight, NoseLength=@NoseLength, NoseBridge=@NoseBridge, NoseTip=@NoseTip, NoseBridgeDepth=@NoseBridgeDepth, EyebrowHeight=@EyebrowHeight, EyebrowDepth=@EyebrowDepth, CheekboneHeight=@CheekboneHeight, CheekboneDepth=@CheekboneDepth, CheekboneWidth=@CheekboneWidth, Eyelids=@Eyelids, Lips=@Lips, JawWidth=@JawWidth, JawDepth=@JawDepth, JawLength=@JawLength, ChinFullness=@ChinFullness, ChinWidth=@ChinWidth, NeckWidth=@NeckWidth WHERE Nametag='{0}'", player.name);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("@skinShapeFirst", args[0].ToString());
                parameters.Add("@skinShapeSecond", args[1].ToString());
                parameters.Add("@skinSkinFirst", args[2].ToString());
                parameters.Add("@skinSkinSecond", args[3].ToString());
                parameters.Add("@skinShapeMix", args[4].ToString());
                parameters.Add("@skinSkinMix", args[5].ToString());
                parameters.Add("@skinHairstyle", args[6].ToString());
                parameters.Add("@skinHairstyleColor", args[7].ToString());
                parameters.Add("@skinHairstyleHighlight", args[8].ToString());
                parameters.Add("@skinHairstyleTexture", args[9].ToString());
                parameters.Add("@NoseWidth", args[10].ToString());
                parameters.Add("@NoseHeight", args[11].ToString());
                parameters.Add("@NoseLength", args[12].ToString());
                parameters.Add("@NoseBridge", args[13].ToString());
                parameters.Add("@NoseTip", args[14].ToString());
                parameters.Add("@NoseBridgeDepth", args[15].ToString());
                parameters.Add("@EyebrowHeight", args[16].ToString());
                parameters.Add("@EyebrowDepth", args[17].ToString());
                parameters.Add("@CheekboneHeight", args[18].ToString());
                parameters.Add("@CheekboneDepth", args[19].ToString());
                parameters.Add("@CheekboneWidth", args[20].ToString());
                parameters.Add("@Eyelids", args[21].ToString());
                parameters.Add("@Lips", args[22].ToString());
                parameters.Add("@JawWidth", args[23].ToString());
                parameters.Add("@JawDepth", args[24].ToString());
                parameters.Add("@JawLength", args[25].ToString());
                parameters.Add("@ChinFullness", args[26].ToString());
                parameters.Add("@ChinWidth", args[27].ToString());
                parameters.Add("@NeckWidth", args[28].ToString());

                API.exported.database.executePreparedQuery(query, parameters);
                pullCurrentFace(player);
                API.exported.gtaocharacter.updatePlayerFace(player.handle);
                API.stopPlayerAnimation(player);
                API.stopPedAnimation(player);
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

        public void pullCurrentFace(Client player)
        {
            string query = string.Format("SELECT * FROM PlayerSkins WHERE Nametag='{0}'", player.name);
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                API.exported.gtaocharacter.initializePedFace(player.handle);
                API.setEntitySyncedData(player.handle, "GTAO_SHAPE_FIRST_ID", Convert.ToInt32(row["skinShapeFirst"]));
                API.setEntitySyncedData(player.handle, "GTAO_SHAPE_SECOND_ID", Convert.ToInt32(row["skinShapeSecond"]));
                API.setEntitySyncedData(player.handle, "GTAO_SKIN_FIRST_ID", Convert.ToInt32(row["skinSkinFirst"]));
                API.setEntitySyncedData(player.handle, "GTAO_SKIN_SECOND_ID", Convert.ToInt32(row["skinSkinSecond"]));
                API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", Convert.ToSingle(row["skinShapeMix"]));
                API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", Convert.ToSingle(row["skinSkinMix"]));
                player.setClothes(2, Convert.ToInt32(row["skinHairstyle"]), Convert.ToInt32(row["skinHairstyleTexture"]));
                API.setEntitySyncedData(player.handle, "GTAO_HAIR_COLOR", Convert.ToInt32(row["skinHairstyleColor"]));
                API.setEntitySyncedData(player.handle, "GTAO_HAIR_HIGHLIGHT_COLOR", Convert.ToInt32(row["skinHairstyleHighlight"]));

                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR", 10);
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR", 5);
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR2", 6);

                API.setEntitySyncedData(player.handle, "GTAO_AGEING", 5);
                API.setEntitySyncedData(player.handle, "GTAO_COMPLEXION", 6);
                API.setEntitySyncedData(player.handle, "GTAO_MOLES", 5);

                var list = new float[21];
                list[0] = Convert.ToSingle(row["NoseWidth"]);
                list[1] = Convert.ToSingle(row["NoseHeight"]);
                list[2] = Convert.ToSingle(row["NoseLength"]);
                list[3] = Convert.ToSingle(row["NoseBridge"]);
                list[4] = Convert.ToSingle(row["NoseTip"]);
                list[5] = Convert.ToSingle(row["NoseBridgeDepth"]);
                list[6] = Convert.ToSingle(row["EyebrowHeight"]);
                list[7] = Convert.ToSingle(row["EyebrowDepth"]);
                list[8] = Convert.ToSingle(row["CheekboneHeight"]);
                list[9] = Convert.ToSingle(row["CheekboneDepth"]);
                list[10] = Convert.ToSingle(row["CheekboneWidth"]);
                list[11] = Convert.ToSingle(row["Eyelids"]);
                list[12] = Convert.ToSingle(row["Lips"]);
                list[13] = Convert.ToSingle(row["JawWidth"]);
                list[14] = Convert.ToSingle(row["JawDepth"]);
                list[15] = Convert.ToSingle(row["JawLength"]);
                list[16] = Convert.ToSingle(row["ChinFullness"]);
                list[17] = Convert.ToSingle(row["ChinWidth"]);
                list[18] = 0;
                list[19] = Convert.ToSingle(row["NeckWidth"]);
                list[20] = 0;

                API.setEntitySyncedData(player.handle, "GTAO_FACE_FEATURES_LIST", list);
            }
        }

        public void loadLocalFaceData(Client player)
        {
            string query = string.Format("SELECT * FROM PlayerSkins WHERE Nametag='{0}'", player.name);
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                int faceShapeOne = Convert.ToInt32(row["skinShapeFirst"]);
                int faceShapeTwo = Convert.ToInt32(row["skinShapeSecond"]);
                int faceSkinOne = Convert.ToInt32(row["skinSkinFirst"]);
                int faceSkinTwo = Convert.ToInt32(row["skinSkinSecond"]);
                float faceShapeMix = Convert.ToSingle(row["skinShapeMix"]);
                float faceSkinMix = Convert.ToSingle(row["skinSkinMix"]);
                int faceHairstyle = Convert.ToInt32(row["skinHairstyle"]);
                int faceHairstyleTexture = Convert.ToInt32(row["skinHairstyleTexture"]);
                int faceHairstyleColor = Convert.ToInt32(row["skinHairstyleColor"]);
                int faceHairstyleHighlight = Convert.ToInt32(row["skinHairstyleHighlight"]);

                float noseWidth = Convert.ToSingle(row["NoseWidth"]);
                float noseHeight = Convert.ToSingle(row["NoseHeight"]);
                float noseLength = Convert.ToSingle(row["NoseLength"]);
                float noseBridge = Convert.ToSingle(row["NoseBridge"]);
                float noseTip = Convert.ToSingle(row["NoseTip"]);
                float noseBridgeDepth = Convert.ToSingle(row["NoseBridgeDepth"]);
                float eyeBrowHeight= Convert.ToSingle(row["EyebrowHeight"]);
                float eyeBrowDepth = Convert.ToSingle(row["EyebrowDepth"]);
                float cheekboneHeight = Convert.ToSingle(row["CheekboneHeight"]);
                float cheekboneDepth = Convert.ToSingle(row["CheekboneDepth"]);
                float cheekboneWidth = Convert.ToSingle(row["CheekboneWidth"]);
                float eyeLids = Convert.ToSingle(row["Eyelids"]);
                float lips = Convert.ToSingle(row["Lips"]);
                float jawWidth = Convert.ToSingle(row["JawWidth"]);
                float jawDepth = Convert.ToSingle(row["JawDepth"]);
                float jawLength = Convert.ToSingle(row["JawLength"]);
                float chinFullness = Convert.ToSingle(row["ChinFullness"]);
                float chinWidth = Convert.ToSingle(row["ChinWidth"]);
                float neckWidth = Convert.ToSingle(row["NeckWidth"]);

                API.triggerClientEvent(player, "loadFaceData", faceShapeOne, faceShapeTwo, faceSkinOne, faceSkinTwo, faceShapeMix, faceSkinMix, faceHairstyle, faceHairstyleColor, faceHairstyleHighlight, faceHairstyleTexture, noseWidth, noseHeight, noseLength, noseBridge, noseTip, noseBridgeDepth, eyeBrowHeight, eyeBrowDepth, cheekboneHeight, cheekboneDepth, cheekboneWidth, eyeLids, lips, jawWidth, jawDepth, jawLength, chinFullness, chinWidth, neckWidth);
            }
        }

        public void loadCurrentFace(Client player)
        {
            int gender = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinGender", "Nametag", player.name));

            if (gender == 0)
            {
                API.setPlayerSkin(player, PedHash.FreemodeMale01);
            }
            else
            {
                API.setPlayerSkin(player, PedHash.FreemodeFemale01);
            }

            string query = string.Format("SELECT * FROM PlayerSkins WHERE Nametag='{0}'", player.name);
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                API.exported.gtaocharacter.initializePedFace(player.handle);
                API.setEntitySyncedData(player.handle, "GTAO_SHAPE_FIRST_ID", Convert.ToInt32(row["skinShapeFirst"]));
                API.setEntitySyncedData(player.handle, "GTAO_SHAPE_SECOND_ID", Convert.ToInt32(row["skinShapeSecond"]));
                API.setEntitySyncedData(player.handle, "GTAO_SKIN_FIRST_ID", Convert.ToInt32(row["skinSkinFirst"]));
                API.setEntitySyncedData(player.handle, "GTAO_SKIN_SECOND_ID", Convert.ToInt32(row["skinSkinSecond"]));
                API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", Convert.ToSingle(row["skinShapeMix"]));
                API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", Convert.ToSingle(row["skinSkinMix"]));
                player.setClothes(2, Convert.ToInt32(row["skinHairstyle"]), Convert.ToInt32(row["skinHairstyleTexture"]));
                API.setEntitySyncedData(player.handle, "GTAO_HAIR_COLOR", Convert.ToInt32(row["skinHairstyleColor"]));
                API.setEntitySyncedData(player.handle, "GTAO_HAIR_HIGHLIGHT_COLOR", Convert.ToInt32(row["skinHairstyleHighlight"]));

                API.setEntitySyncedData(player.handle, "GTAO_EYEBROWS_COLOR", 3);
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR", 10);
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR", 5);
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR2", 6);

                API.setEntitySyncedData(player.handle, "GTAO_AGEING", 5);

                API.setEntitySyncedData(player.handle, "GTAO_COMPLEXION", 6);

                API.setEntitySyncedData(player.handle, "GTAO_MOLES", 5);

                var list = new float[21];
                list[0] = Convert.ToSingle(row["NoseWidth"]);
                list[1] = Convert.ToSingle(row["NoseHeight"]);
                list[2] = Convert.ToSingle(row["NoseLength"]);
                list[3] = Convert.ToSingle(row["NoseBridge"]);
                list[4] = Convert.ToSingle(row["NoseTip"]);
                list[5] = Convert.ToSingle(row["NoseBridgeDepth"]);
                list[6] = Convert.ToSingle(row["EyebrowHeight"]);
                list[7] = Convert.ToSingle(row["EyebrowDepth"]);
                list[8] = Convert.ToSingle(row["CheekboneHeight"]);
                list[9] = Convert.ToSingle(row["CheekboneDepth"]);
                list[10] = Convert.ToSingle(row["CheekboneWidth"]);
                list[11] = Convert.ToSingle(row["Eyelids"]);
                list[12] = Convert.ToSingle(row["Lips"]);
                list[13] = Convert.ToSingle(row["JawWidth"]);
                list[14] = Convert.ToSingle(row["JawDepth"]);
                list[15] = Convert.ToSingle(row["JawLength"]);
                list[16] = Convert.ToSingle(row["ChinFullness"]);
                list[17] = Convert.ToSingle(row["ChinWidth"]);
                list[18] = 0;
                list[19] = Convert.ToSingle(row["NeckWidth"]);
                list[20] = 0;

                API.setEntitySyncedData(player.handle, "GTAO_FACE_FEATURES_LIST", list);

                API.exported.gtaocharacter.updatePlayerFace(player.handle);
            }
        }
    }
}
