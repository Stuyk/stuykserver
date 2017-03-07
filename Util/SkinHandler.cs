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
                // Gather all our data
                string[] varNames = { "skinShapeFirst", "skinShapeSecond", "skinSkinFirst", "skinSkinSecond", "skinShapeMix", "skinSkinMix", "skinHairstyle", "skinHairstyleColor", "skinHairstyleHighlight", "skinHairstyleTexture", "NoseWidth", "NoseHeight", "NoseLength", "NoseBridge", "NoseTip", "NoseBridgeDepth", "EyebrowHeight", "EyebrowDepth", "CheekboneHeight", "CheekboneDepth", "CheekboneWidth", "Eyelids", "Lips", "JawWidth", "JawDepth", "JawLength", "ChinFullness", "ChinWidth", "NeckWidth", "FacialHair", "FacialHairColor", "FacialHairColor2", "Ageing", "Complexion", "Moles" };
                string before = "UPDATE PlayerSkins SET";
                string after = string.Format("WHERE Nametag='{0}'", player.name);

                // Send all our data to generate the query and run it
                this.db.compileQuery(before, after, varNames, args);

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

                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR", Convert.ToInt32(row["FacialHair"]));
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR", Convert.ToInt32(row["FacialHairColor"]));
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR2", Convert.ToInt32(row["FacialHairColor2"]));
                API.setEntitySyncedData(player.handle, "GTAO_AGEING", Convert.ToInt32(row["Ageing"]));
                API.setEntitySyncedData(player.handle, "GTAO_COMPLEXION", Convert.ToInt32(row["Complexion"]));
                API.setEntitySyncedData(player.handle, "GTAO_MOLES", Convert.ToInt32(row["Moles"]));

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

                int facialHair = Convert.ToInt32(row["FacialHair"]);
                int facialHairColor = Convert.ToInt32(row["FacialHairColor"]);
                int facialHairColorTwo = Convert.ToInt32(row["FacialHairColor2"]);
                int facialAgeing = Convert.ToInt32(row["Ageing"]);
                int facialComplexion = Convert.ToInt32(row["Complexion"]);
                int facialMoles = Convert.ToInt32(row["Moles"]);

                API.triggerClientEvent(player, "loadFaceData", faceShapeOne, faceShapeTwo, faceSkinOne, faceSkinTwo, faceShapeMix, faceSkinMix, faceHairstyle, faceHairstyleColor, faceHairstyleHighlight, faceHairstyleTexture, noseWidth, noseHeight, noseLength, noseBridge, noseTip, noseBridgeDepth, eyeBrowHeight, eyeBrowDepth, cheekboneHeight, cheekboneDepth, cheekboneWidth, eyeLids, lips, jawWidth, jawDepth, jawLength, chinFullness, chinWidth, neckWidth, facialHair, facialHairColor, facialHairColorTwo, facialAgeing, facialComplexion, facialMoles);
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
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR", Convert.ToInt32(row["FacialHair"]));
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR", Convert.ToInt32(row["FacialHairColor"]));
                API.setEntitySyncedData(player.handle, "GTAO_FACIAL_HAIR_COLOR2", Convert.ToInt32(row["FacialHairColor2"]));
                API.setEntitySyncedData(player.handle, "GTAO_AGEING", Convert.ToInt32(row["Ageing"]));
                API.setEntitySyncedData(player.handle, "GTAO_COMPLEXION", Convert.ToInt32(row["Complexion"]));
                API.setEntitySyncedData(player.handle, "GTAO_MOLES", Convert.ToInt32(row["Moles"]));

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
