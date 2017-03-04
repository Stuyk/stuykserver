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
                db.updateDatabase("PlayerSkins", "skinShapeFirst", args[0].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerSkins", "skinShapeSecond", args[1].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerSkins", "skinSkinFirst", args[2].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerSkins", "skinSkinSecond", args[3].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerSkins", "skinShapeMix", args[4].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerSkins", "skinSkinMix", args[5].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerSkins", "skinHairstyle", args[6].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerSkins", "skinHairstyleColor", args[7].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerSkins", "skinHairstyleHighlight", args[8].ToString(), "Nametag", player.name);
                db.updateDatabase("PlayerSkins", "skinHairstyleTexture", args[9].ToString(), "Nametag", player.name);
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
            }
        }

        public void loadLocalFaceData(Client player)
        {
            int faceShapeOne = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeFirst", "Nametag", player.name));
            int faceShapeTwo = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeSecond", "Nametag", player.name));
            int faceSkinOne = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinFirst", "Nametag", player.name));
            int faceSkinTwo = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinSecond", "Nametag", player.name));
            float faceShapeMix = Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinShapeMix", "Nametag", player.name));
            float faceSkinMix = Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinSkinMix", "Nametag", player.name));
            int faceHairstyle = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyle", "Nametag", player.name));
            int faceHairstyleColor = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleColor", "Nametag", player.name));
            int faceHairstyleHighlight = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleHighlight", "Nametag", player.name));
            int faceHairstyleTexture = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleTexture", "Nametag", player.name));

            API.triggerClientEvent(player, "loadFaceData", faceShapeOne, faceShapeTwo, faceSkinOne, faceSkinTwo, faceShapeMix, faceSkinMix, faceHairstyle, faceHairstyleColor, faceHairstyleHighlight, faceHairstyleTexture);
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
                API.exported.gtaocharacter.updatePlayerFace(player.handle);
            }
        }
    }
}
