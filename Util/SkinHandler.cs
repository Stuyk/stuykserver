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
        SpawnPoints sp = new SpawnPoints();

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        int skinFaceShapeOne;

        public SkinHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            skinFaceShapeOne = 0;
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
        }

        public void pullCurrentFace(Client player)
        {
            API.exported.gtaocharacter.initializePedFace(player.handle);
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_FIRST_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeFirst", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_SECOND_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeSecond", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_FIRST_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinFirst", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_SECOND_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinSecond", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinShapeMix", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinSkinMix", "Nametag", player.name)));
            player.setClothes(2, Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyle", "Nametag", player.name)), Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleTexture", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_HAIR_COLOR", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleColor", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_HAIR_HIGHLIGHT_COLOR", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleHighlight", "Nametag", player.name)));
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

            API.exported.gtaocharacter.initializePedFace(player.handle);
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_FIRST_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeFirst", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_SECOND_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeSecond", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_FIRST_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinFirst", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_SECOND_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinSecond", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinShapeMix", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinSkinMix", "Nametag", player.name)));
            player.setClothes(2, Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyle", "Nametag", player.name)), Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleTexture", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_HAIR_COLOR", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleColor", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_HAIR_HIGHLIGHT_COLOR", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleHighlight", "Nametag", player.name)));
            API.exported.gtaocharacter.updatePlayerFace(player.handle);
        }
    }
}
