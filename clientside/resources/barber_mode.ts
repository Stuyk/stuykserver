var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
// Variables
var currentIndex = 0;
var n_hair = 0;
var n_hair_max = 0;
var t_hair = 0;
var t_hair_max = 0;
var n_hair_color = 0;
var n_hair_color_max = 0;
var n_highlight = 0;
var n_highlight_max = 0;
var n_beard = 0;
var n_beard_max = 0;
var t_beard = -1;
var t_beard_max = 0;
var n_eyebrows = 0;
var n_eyebrows_max = 0;
var t_eyebrows = 0;
var t_eyebrows_max = 0;
var n_eyes = 0;
var n_eyes_max = 0;
// Camera
var camera = null;
API.onUpdate.connect(function () {
    if (camera === null) {
        return;
    }
    API.disableAllControlsThisFrame();
    drawBarberShopHud();

    // Navigate Left
    if (API.isDisabledControlJustPressed(Enums.Controls.MoveLeftOnly)) {
        subSubIndex();
    }

    // Navigate Right
    if (API.isDisabledControlJustPressed(Enums.Controls.MoveRightOnly)) {
        addSubIndex();
    }

    // Select Hair or Beard - Up
    if (API.isDisabledControlJustPressed(Enums.Controls.MoveUpOnly)) {
        subIndex();
    }

    // Select Hair or Beard - Down
    if (API.isDisabledControlJustPressed(Enums.Controls.MoveDownOnly)) {
        addIndex();
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.CursorScrollDown)) {
        rotateCamera(-0.1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.CursorScrollUp)) {
        rotateCamera(0.1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.Enter)) {
        API.callNative("DO_SCREEN_FADE_OUT", 3000);
        API.sleep(4000);
        camera = null;
        saveAndExit();
        API.setHudVisible(true);
        API.setChatVisible(true);
        API.callNative("DO_SCREEN_FADE_IN", 3000);
        API.setGameplayCameraActive();
        API.stopPlayerAnimation();
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.PhoneCancel)) {
        API.callNative("DO_SCREEN_FADE_OUT", 3000);
        API.sleep(4000);
        camera = null;
        justExit();
        API.setHudVisible(true);
        API.setChatVisible(true);
        API.callNative("DO_SCREEN_FADE_IN", 3000);
        API.setGameplayCameraActive();
        API.stopPlayerAnimation();
    }
});
// Resource Stopper
API.onResourceStop.connect(function () {
    camera = null;
    API.stopPlayerAnimation();
});
// Rotate Camera Around Player
function rotateCamera(value) {
    var angle = API.getCameraRotation(API.getActiveCamera()).Z + value;
    var cosTheta = Math.cos(angle);
    var sinTheta = Math.sin(angle);
    var center = API.getEntityPosition(API.getLocalPlayer());
    var camera = API.getCameraPosition(API.getActiveCamera());
    var x = (cosTheta * (camera.X - center.X) - sinTheta * (camera.Y - center.Y) + center.X);
    var y = (sinTheta * (camera.X - center.X) + cosTheta * (camera.Y - center.Y) + center.Y);
    API.setCameraPosition(API.getActiveCamera(), new Vector3(x, y, 57.8));
}

// Setup Barbershop
function setupBarberShop() {
    setupCamera();
    API.setHudVisible(false);
    API.setChatVisible(false);
    API.callNative("TASK_LOOK_AT_COORD", API.getLocalPlayer(), -36.58, -152.68, 57.8, -1, 0, 0);
    API.playPlayerAnimation("misshair_shop@barbers", "player_base", 1, -1);
    n_hair = API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_HAIRSTYLE");
    n_hair_color = API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_HAIR_HIGHLIGHT_COLOR");
    n_highlight = API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_HAIRSTYLE");
    n_beard = API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_FACIAL_HAIR");
    n_eyes = API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_EYE_COLOR");
    n_eyebrows = API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_EYEBROWS");


}
// Setup Camera
function setupCamera() {
    camera = API.createCamera(new Vector3(-36.58, -152.68, 57.7), new Vector3());
    API.pointCameraAtEntity(camera, API.getLocalPlayer(), new Vector3(0.05, 0, 0.6));
    API.setActiveCamera(camera);
    API.setCameraFov(camera, 15);
}
// Handle Barber HUD
function drawBarberShopHud() {
    // Black Background
    API.drawRectangle(0, 0, (resX / 4), resY, 0, 0, 0, 150);
    // Blue Header + Text
    API.drawRectangle(0, 0, (resX / 4), (resY / 16), 54, 117, 219, 255);
    API.drawText("Barbershop", (resX / 4) / 2, 0, 1, 255, 255, 255, 255, 1, 1, false, false, 500);
    // Hair
    if (currentIndex === 0) {
        API.drawRectangle(0, (resY / 16) * 1, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Hair", 20, ((resY / 16) * 2) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_hair + " )", (resX / 5), ((resY / 16) * 2) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Hair", 20, ((resY / 16) * 2) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_hair + " )", (resX / 5), ((resY / 16) * 2) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Color
    if (currentIndex === 1) {
        API.drawRectangle(0, (resY / 16) * 2, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Color", 20, ((resY / 16) * 3) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_hair_color + " )", (resX / 5), ((resY / 16) * 3) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Color", 20, ((resY / 16) * 3) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_hair_color + " )", (resX / 5), ((resY / 16) * 3) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Highlight
    if (currentIndex === 2) {
        API.drawRectangle(0, (resY / 16) * 3, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Color", 20, ((resY / 16) * 4) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_highlight + " )", (resX / 5), ((resY / 16) * 4) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Color", 20, ((resY / 16) * 4) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_highlight + " )", (resX / 5), ((resY / 16) * 4) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Beard
    if (currentIndex === 3) {
        API.drawRectangle(0, (resY / 16) * 4, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Beard", 20, ((resY / 16) * 5) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_beard + " )", (resX / 5), ((resY / 16) * 5) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Beard", 20, ((resY / 16) * 5) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_beard + " )", (resX / 5), ((resY / 16) * 5) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Eyes
    if (currentIndex === 4) {
        API.drawRectangle(0, (resY / 16) * 5, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Eyes", 20, ((resY / 16) * 6) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_eyes + " )", (resX / 5), ((resY / 16) * 6) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Eyes", 20, ((resY / 16) * 6) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_eyes + " )", (resX / 5), ((resY / 16) * 6) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Eyebrows
    if (currentIndex === 5) {
        API.drawRectangle(0, (resY / 16) * 6, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Eyebrows", 20, ((resY / 16) * 7) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_eyebrows + " )", (resX / 5), ((resY / 16) * 7) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Eyebrows", 20, ((resY / 16) * 7) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_eyebrows + " )", (resX / 5), ((resY / 16) * 7) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } 
}
// Handle Indexes
function addIndex() {
    API.playSoundFrontEnd("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
    currentIndex += 1;
    // Max Value
    if (currentIndex >= 6) {
        currentIndex = 0;
    }
}
function subIndex() {
    API.playSoundFrontEnd("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
    currentIndex -= 1;
    // Lowest Value
    if (currentIndex <= -1) {
        currentIndex = 4;
    }
}
function addSubIndex() {
    API.playSoundFrontEnd("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
    switch (currentIndex) {
        case 0:
            n_hair += 1;
            var n_hair_max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 2);
            if (n_hair > n_hair_max) {
                n_hair = 0;
            }
            break;
        case 1:
            n_hair_color += 1;
            var n_hair_color_max = API.returnNative("_GET_NUM_HAIR_COLORS", Enums.NativeReturnTypes.Int);
            if (n_hair_color > n_hair_color_max) {
                n_hair_color = 0;
            }
            break;
        case 2:
            n_highlight += 1;
            var n_highlight_max = API.returnNative("_GET_NUM_HAIR_COLORS", Enums.NativeReturnTypes.Int);
            if (n_highlight > n_highlight_max) {
                n_highlight = 0;
            }
            break;
        case 3:
            n_beard += 1;
            var n_beard_max = API.returnNative("_GET_NUM_HEAD_OVERLAY_VALUES", Enums.NativeReturnTypes.Int, 1);
            if (n_beard > n_beard_max) {
                n_beard = 0;
            }
            break;
        case 4:
            n_eyes += 1;
            var n_eyes_max = 31;
            if (n_eyes > n_eyes_max) {
                n_eyes = 0;
            }
            break;
        case 5:
            n_eyebrows += 1;
            var n_eyesbrows_max = API.returnNative("_GET_NUM_HEAD_OVERLAY_VALUES", Enums.NativeReturnTypes.Int, 2);
            if (n_eyebrows > n_eyesbrows_max) {
                n_eyebrows = 0;
            }
            break;
    }
    updatePlayer();
}
function subSubIndex() {
    API.playSoundFrontEnd("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
    switch (currentIndex) {
        case 0:
            n_hair -= 1;
            var n_hair_max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 2);
            if (n_hair < 0) {
                n_hair = n_hair_max;
            }
            break;
        case 1:
            n_hair_color -= 1;
            var n_hair_color_max = API.returnNative("_GET_NUM_HAIR_COLORS", Enums.NativeReturnTypes.Int);
            if (n_hair_color < 0) {
                n_hair_color = n_hair_color_max;
            }
            break;
        case 2:
            n_highlight -= 1;
            var n_highlight_max = API.returnNative("_GET_NUM_HAIR_COLORS", Enums.NativeReturnTypes.Int);
            if (n_highlight < 0) {
                n_highlight = n_highlight_max;
            }
            break;
        case 3:
            n_beard -= 1;
            var n_beard_max = API.returnNative("_GET_NUM_HEAD_OVERLAY_VALUES", Enums.NativeReturnTypes.Int, 1);
            if (n_beard < -1) {
                n_beard = n_beard_max;
            }
            break;
        case 4:
            n_eyes -= 1;
            var n_eyes_max = 31;
            if (n_eyes < 0) {
                n_eyes = n_eyes_max;
            }
            break;
        case 5:
            n_eyebrows -= 1;
            var n_eyesbrows_max = API.returnNative("_GET_NUM_HEAD_OVERLAY_VALUES", Enums.NativeReturnTypes.Int, 2);
            if (n_eyebrows < 0) {
                n_eyebrows = n_eyesbrows_max;
            }
            break;
    }
    updatePlayer();
}
// Handle Player Update
function updatePlayer() {
    // Hair + Hair Color
    API.setPlayerClothes(API.getLocalPlayer(), 2, n_hair, 0);
    API.callNative("_SET_PED_HAIR_COLOR", API.getLocalPlayer(), n_hair_color, n_highlight);
    // Beard
    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 1, n_beard, API.f(1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 1, 1, n_hair_color, n_highlight);
    // Eyebrows
    API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 2, n_eyebrows, API.f(1));
    API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", API.getLocalPlayer(), 2, 1, n_hair_color, n_highlight);
    // Eyes
    API.callNative("_SET_PED_EYE_COLOR", API.getLocalPlayer(), n_eyes);
}
// Save and Exit
function saveAndExit() {
    API.triggerServerEvent("barbershopSaveAndExit", n_hair, n_hair_color, n_highlight, 0, n_beard, n_hair_color, n_eyes, n_eyebrows);
}
// Exit without Saving
function justExit() {
    API.triggerServerEvent("barbershopExit");
}