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

        public void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "clientSkinSelected")
            {
                API.sendNotificationToPlayer(player, "~g~Your model has been changed to: " + arguments[0]);
                API.setPlayerSkin(player, API.pedNameToModel(arguments[0].ToString()));
                db.updateDatabase("Players", "CurrentSkin", arguments[0].ToString(), "Nametag", player.name);
                db.setPlayerMoney(player, -30);
            }

            if (eventName == "skinGenderServer")
            {
                if (Convert.ToInt32(arguments[0]) == 0)
                {
                    API.setPlayerSkin(player, PedHash.FreemodeMale01);
                    pullCurrentFace(player);
                    API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", 1);
                    API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", 1);
                    API.exported.gtaocharacter.updatePlayerFace(player.handle);
                    db.updateDatabase("PlayerSkins", "skinGender", "0", "Nametag", player.name);
                    db.updateDatabase("PlayerSkins", "skinShapeMix", "1", "Nametag", player.name);
                    db.updateDatabase("PlayerSkins", "skinSkinMix", "1", "Nametag", player.name);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@female_arms_crossed@base", "base");
                }

                if (Convert.ToInt32(arguments[0]) == 1)
                {
                    API.setPlayerSkin(player, PedHash.FreemodeFemale01);
                    pullCurrentFace(player);
                    API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", 1);
                    API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", 1);
                    API.exported.gtaocharacter.updatePlayerFace(player.handle);
                    db.updateDatabase("PlayerSkins", "skinGender", "1", "Nametag", player.name);
                    db.updateDatabase("PlayerSkins", "skinShapeMix", "0", "Nametag", player.name);
                    db.updateDatabase("PlayerSkins", "skinSkinMix", "0", "Nametag", player.name);
                    API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@female_arms_crossed@base", "base");
                }
            }

            if (eventName == "skinFaceShapeOneServer")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeFirst", "Nametag", player.name)); // Pull the ID we're using from the database.
                if (amount < 0 || amount >= 45) // If the amount is not within what is available. Set it to zero. Else add or subtract.
                {
                    amount = 0;
                }
                else
                {
                    amount = amount + Convert.ToInt32(arguments[0]);
                }

                // Initialize, change, sync changes.
                pullCurrentFace(player);
                API.setEntitySyncedData(player.handle, "GTAO_SHAPE_FIRST_ID", amount);
                API.exported.gtaocharacter.updatePlayerFace(player.handle);

                // Save change to database.
                db.updateDatabase("PlayerSkins", "skinShapeFirst", amount.ToString(), "Nametag", player.name);
            }

            if (eventName == "skinFaceShapeTwoServer")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeSecond", "Nametag", player.name)); // Pull the ID we're using from the database.
                if (amount < 0 || amount >= 45) // If the amount is not within what is available. Set it to zero. Else add or subtract.
                {
                    amount = 0;
                }
                else
                {
                    amount = amount + Convert.ToInt32(arguments[0]);
                }

                // Initialize, change, sync changes.
                pullCurrentFace(player);
                API.setEntitySyncedData(player.handle, "GTAO_SHAPE_SECOND_ID", amount);
                API.exported.gtaocharacter.updatePlayerFace(player.handle);

                // Save change to database.
                db.updateDatabase("PlayerSkins", "skinShapeSecond", amount.ToString(), "Nametag", player.name);
            }

            if (eventName == "skinSkinFirst")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinFirst", "Nametag", player.name)); // Pull the ID we're using from the database.
                if (amount < 0 || amount >= 45) // If the amount is not within what is available. Set it to zero. Else add or subtract.
                {
                    amount = 0;
                }
                else
                {
                    amount = amount + Convert.ToInt32(arguments[0]);
                }

                // Initialize, change, sync changes.
                pullCurrentFace(player);
                API.setEntitySyncedData(player.handle, "GTAO_SKIN_FIRST_ID", amount);
                API.exported.gtaocharacter.updatePlayerFace(player.handle);

                // Save change to database.
                db.updateDatabase("PlayerSkins", "skinSkinFirst", amount.ToString(), "Nametag", player.name);
            }

            if (eventName == "skinSkinSecond")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinSecond", "Nametag", player.name)); // Pull the ID we're using from the database.
                if (amount < 0 || amount >= 45) // If the amount is not within what is available. Set it to zero. Else add or subtract.
                {
                    amount = 0;
                }
                else
                {
                    amount = amount + Convert.ToInt32(arguments[0]);
                }

                // Initialize, change, sync changes.
                pullCurrentFace(player);
                API.setEntitySyncedData(player.handle, "GTAO_SKIN_SECOND_ID", amount);
                API.exported.gtaocharacter.updatePlayerFace(player.handle);

                // Save change to database.
                db.updateDatabase("PlayerSkins", "skinSkinSecond", amount.ToString(), "Nametag", player.name);
            }

            if (eventName == "skinShapeMixPositive")
            {
                float amount = Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinShapeMix", "Nametag", player.name)); // Pull the ID we're using from the database.
                
                if (amount != 1f)
                {
                    amount = amount + 0.1f;

                    // Initialize, change, sync changes.
                    pullCurrentFace(player);
                    API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", amount);
                    API.exported.gtaocharacter.updatePlayerFace(player.handle);

                    // Save change to database.
                    db.updateDatabase("PlayerSkins", "skinShapeMix", amount.ToString(), "Nametag", player.name);
                }
            }

            if (eventName == "skinShapeMixNegative")
            {
                float amount = Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinShapeMix", "Nametag", player.name)); // Pull the ID we're using from the database.

                if (amount != 0f)
                {
                    amount = amount - 0.1f;

                    // Initialize, change, sync changes.
                    pullCurrentFace(player);
                    API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", amount);
                    API.exported.gtaocharacter.updatePlayerFace(player.handle);

                    // Save change to database.
                    db.updateDatabase("PlayerSkins", "skinShapeMix", amount.ToString(), "Nametag", player.name);
                }
            }

            if (eventName == "skinSkinMixPositive")
            {
                float amount = Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinSkinMix", "Nametag", player.name)); // Pull the ID we're using from the database.

                if (amount != 1f)
                {
                    amount = amount + 0.1f;

                    // Initialize, change, sync changes.
                    pullCurrentFace(player);
                    API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", amount);
                    API.exported.gtaocharacter.updatePlayerFace(player.handle);

                    // Save change to database.
                    db.updateDatabase("PlayerSkins", "skinSkinMix", amount.ToString(), "Nametag", player.name);
                }
            }

            if (eventName == "skinSkinMixNegative")
            {
                float amount = Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinSkinMix", "Nametag", player.name)); // Pull the ID we're using from the database.

                if (amount != 0f)
                {
                    amount = amount - 0.1f;

                    // Initialize, change, sync changes.
                    pullCurrentFace(player);
                    API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", amount);
                    API.exported.gtaocharacter.updatePlayerFace(player.handle);

                    // Save change to database.
                    db.updateDatabase("PlayerSkins", "skinSkinMix", amount.ToString(), "Nametag", player.name);
                }
            }

            if (eventName == "skinHairstyle")
            {
                int gender = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinGender", "Nametag", player.name));
                int amount = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyle", "Nametag", player.name));
                int texture = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleTexture", "Nametag", player.name));

                if (gender == 0) // Male
                {
                    if (amount < 0 || amount >= 36)
                    {
                        amount = 0;
                    }
                    else if (amount == 23) // Skip stupid goggles hairstyle.
                    {
                        amount = amount + Convert.ToInt32(arguments[0]) + Convert.ToInt32(arguments[0]);
                    }
                    else
                    {
                        amount = amount + Convert.ToInt32(arguments[0]);
                    }
                }

                if (gender == 1) // Female
                {
                    if (amount < 0 || amount >= 38)
                    {
                        amount = 0;
                    }
                    else if (amount == 24) // Skip stupid goggles hairstyle.
                    {
                        amount = amount + Convert.ToInt32(arguments[0]) + Convert.ToInt32(arguments[0]);
                    }
                    else
                    {
                        amount = amount + Convert.ToInt32(arguments[0]);
                    }
                }
                // Initialize, change, sync changes.
                pullCurrentFace(player);
                player.setClothes(2, amount, texture);
                API.exported.gtaocharacter.updatePlayerFace(player.handle);

                // Save change to database.
                db.updateDatabase("PlayerSkins", "skinHairstyle", amount.ToString(), "Nametag", player.name);
            }

            if (eventName == "skinHairstyleColor")
            {
                int gender = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinGender", "Nametag", player.name));
                int amount = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleColor", "Nametag", player.name));
                int hairstyle = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyle", "Nametag", player.name));

                if (amount < 0 || amount >= 63)
                {
                    amount = 0;
                }
                else
                {
                    amount = amount + Convert.ToInt32(arguments[0]);
                }
                // Initialize, change, sync changes.
                pullCurrentFace(player);
                API.setEntitySyncedData(player.handle, "GTAO_HAIR_COLOR", amount);
                API.exported.gtaocharacter.updatePlayerFace(player.handle);

                // Save change to database.
                db.updateDatabase("PlayerSkins", "skinHairstyleColor", amount.ToString(), "Nametag", player.name);
            }

            if (eventName == "skinHairstyleHighlight")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleHighlight", "Nametag", player.name));

                if (amount < 0 || amount >= 63)
                {
                    amount = 0;
                }
                else
                {
                    amount = amount + Convert.ToInt32(arguments[0]);
                }
                // Initialize, change, sync changes.
                pullCurrentFace(player);
                API.setEntitySyncedData(player.handle, "GTAO_HAIR_HIGHLIGHT_COLOR", amount);
                API.exported.gtaocharacter.updatePlayerFace(player.handle);

                // Save change to database.
                db.updateDatabase("PlayerSkins", "skinHairstyleHighlight", amount.ToString(), "Nametag", player.name);
            }

            if (eventName == "skinHairstyleTexture")
            {
                int amount = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleTexture", "Nametag", player.name));
                int hairstyle = Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyle", "Nametag", player.name));
                if (amount < 0 || amount >= 3)
                {
                    amount = 0;
                }
                else
                {
                    amount = amount + Convert.ToInt32(arguments[0]);
                }

                // Initialize, change, sync changes.
                pullCurrentFace(player);
                player.setClothes(2, hairstyle, amount);
                API.exported.gtaocharacter.updatePlayerFace(player.handle);

                // Save change to database.
                db.updateDatabase("PlayerSkins", "skinHairstyleTexture", amount.ToString(), "Nametag", player.name);

            }

            if (eventName == "skinRotation")
            {
                int amount = Convert.ToInt32(arguments[0]);
                Vector3 oldRotation = API.getEntityRotation(player);
                API.setEntityRotation(player, new Vector3(oldRotation.X, oldRotation.Y, oldRotation.Z + amount));
                API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_hang_out_street@female_arms_crossed@base", "base");
            }

            if (eventName == "skinSave")
            {
                API.triggerClientEvent(player, "killPanel");
                API.triggerClientEvent(player, "endCamera");
                API.call("BarberShopHandler", "leaveBarberShop", player);
            }
        }

        public void pullCurrentFace(Client player)
        {
            API.exported.gtaocharacter.initializePedFace(player.handle);
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_MIX", Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinShapeMix", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_MIX", Convert.ToSingle(db.pullDatabase("PlayerSkins", "skinSkinMix", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_FIRST_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeFirst", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SHAPE_SECOND_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinShapeSecond", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_FIRST_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinFirst", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_SKIN_SECOND_ID", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinSkinSecond", "Nametag", player.name)));
            player.setClothes(2, Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyle", "Nametag", player.name)), Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleTexture", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_HAIR_COLOR", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleColor", "Nametag", player.name)));
            API.setEntitySyncedData(player.handle, "GTAO_HAIR_HIGHLIGHT_COLOR", Convert.ToInt32(db.pullDatabase("PlayerSkins", "skinHairstyleHighlight", "Nametag", player.name)));
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
