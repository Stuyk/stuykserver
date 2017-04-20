var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var camera = null;
var cameraHeading = 0;
var currentIndex = 0;
var currentPage = 0;
var currentIndexName = "Sex";
// Index Listing
var faceGender = 0; // 0 - Index // 0 Male - 1 Female
var faceShapeOne = 0; // 1- Index
var faceShapeTwo = 0; // 2 - Index
var faceSkinOne = 0; // 3 - Index
var faceSkinTwo = 0; // 4 - Index
var faceShapeMix = 0; // 5 - Index
var faceSkinMix = 0; // 6 - Index
// Facial Features
var faceNoseWidth = 0; // 0 // 1  - Index
var faceNoseHeight = 0; // 1 // 2  - Index
var faceNoseLength = 0; // 2 // 3  - Index
var faceNoseBridge = 0; // 3 // 4  - Index
var faceNoseTip = 0; // 4 // 5 - Index
var faceNoseBridgeDepth = 0; // 5 // 6 - Index
var faceEyebrowHeight = 0; // 6 // 7 - Index
var faceEyebrowDepth = 0; // 7 // 9 - Index
var faceCheekboneHeight = 0; // 8 // 9 - Index
var faceCheekboneDepth = 0; // 9 // 16 - Index
var faceCheekboneWidth = 0; // 10 // 17 - Index
var faceEyelids = 0; // 11 // 18 - Index
var faceLips = 0; // 12 // 20 - Index
var faceJawWidth = 0; // 13 // 21 - Index
var faceJawDepth = 0; // 14 // 22 - Index
var faceJawLength = 0; // 15 // 23 - Index
var faceChinFullness = 0; // 16 // 24 - Index
var faceChinWidth = 0; // 17 // 25 - Index
var faceNeckWidth = 0; // 19 // 26 - Index
var faceAgeing = 0; // 20 // 27 - Index
var faceComplexion = 0; //  // 22 - Index
var faceMoles = 0; //   // 23 - Index
var faceBlemishes = 0;
var faceSunDamage = 0;
// Menu Reference Math
var startPointY = resY / 16;
var menuWidth = resX / 4;
var menuHeight = resY / 16;
var menuTextHeight = resY / 16;
API.onChatMessage.connect(function (msg) {
    if (msg == "su") {
        if (camera === null) {
            setupSurgery();
        }
        else {
            API.setActiveCamera(null);
            camera = null;
        }
    }
});
// On Update
API.onUpdate.connect(function () {
    if (camera === null) {
        return;
    }
    API.disableAllControlsThisFrame();
    showSurgeryMenu();
    if (API.isDisabledControlJustPressed(242 /* CursorScrollDown */)) {
        rotateSurgeryCamera(-0.1);
    }
    if (API.isDisabledControlJustPressed(241 /* CursorScrollUp */)) {
        rotateSurgeryCamera(0.1);
    }
    if (API.isDisabledControlJustPressed(34 /* MoveLeftOnly */)) {
        API.playSoundFrontEnd("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
        if (currentIndex === 0) {
            if (currentPage <= 0) {
                currentPage = 0;
                return;
            }
            currentPage -= 1;
            return;
        }
    }
    if (API.isDisabledControlJustPressed(35 /* MoveRightOnly */)) {
        API.playSoundFrontEnd("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
        if (currentIndex === 0) {
            if (currentPage >= 2) {
                currentPage = 2;
                return;
            }
            currentPage += 1;
            return;
        }
        updateCurrentIndex(currentPage);
    }
    if (API.isDisabledControlJustPressed(33 /* MoveDownOnly */)) {
        API.playSoundFrontEnd("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
        if (currentPage === 0) {
            if (currentIndex > 12) {
                currentIndex = 0;
                return;
            }
            currentIndex += 1;
            return;
        }
        if (currentPage === 1) {
            if (currentIndex > 8) {
                currentIndex = 0;
                return;
            }
            currentIndex += 1;
            return;
        }
        if (currentPage === 2) {
            if (currentIndex > 11) {
                currentIndex = 0;
                return;
            }
            currentIndex += 1;
            return;
        }
    }
    if (API.isDisabledControlJustPressed(32 /* MoveUpOnly */)) {
        API.playSoundFrontEnd("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
        if (currentPage === 0) {
            if (currentIndex < 0) {
                currentIndex = 12;
                return;
            }
            currentIndex -= 1;
            return;
        }
        if (currentPage === 1) {
            if (currentIndex < 0) {
                currentIndex = 8;
                return;
            }
            currentIndex -= 1;
            return;
        }
        if (currentPage === 2) {
            if (currentIndex < 0) {
                currentIndex = 11;
                return;
            }
            currentIndex -= 1;
            return;
        }
    }
});
function updateCurrentIndex(currentPage) {
    if (currentPage === 0) {
        switch (currentIndex) {
            case 1:
                if (faceGender >= 1) {
                    faceGender = 1;
                    return;
                }
                faceGender += 1;
                API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), faceShapeOne, faceShapeTwo, 0, faceSkinOne, faceSkinTwo, 0, API.f(faceShapeMix), API.f(faceSkinMix), 0, false);
                API.callNative("UPDATE_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), API.f(faceShapeMix), API.f(faceSkinMix), 0);
                return;
            case 2:
                faceShapeOne += 1;
                if (faceShapeOne > 45) {
                    faceShapeOne = 0;
                }
                API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), faceShapeOne, faceShapeTwo, 0, faceSkinOne, faceSkinTwo, 0, API.f(faceShapeMix), API.f(faceSkinMix), 0, false);
                API.callNative("UPDATE_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), API.f(faceShapeMix), API.f(faceSkinMix), 0);
                return;
            case 3:
                faceShapeTwo += 1;
                if (faceShapeTwo > 45) {
                    faceShapeTwo = 0;
                }
                API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), faceShapeOne, faceShapeTwo, 0, faceSkinOne, faceSkinTwo, 0, API.f(faceShapeMix), API.f(faceSkinMix), 0, false);
                API.callNative("UPDATE_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), API.f(faceShapeMix), API.f(faceSkinMix), 0);
                return;
            case 4:
                faceSkinOne += 1;
                if (faceSkinOne > 45) {
                    faceSkinOne = 0;
                }
                API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), faceShapeOne, faceShapeTwo, 0, faceSkinOne, faceSkinTwo, 0, API.f(faceShapeMix), API.f(faceSkinMix), 0, false);
                API.callNative("UPDATE_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), API.f(faceShapeMix), API.f(faceSkinMix), 0);
                return;
            case 5:
                faceSkinTwo += 1;
                if (faceSkinTwo > 45) {
                    faceSkinTwo = 0;
                }
                API.callNative("SET_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), faceShapeOne, faceShapeTwo, 0, faceSkinOne, faceSkinTwo, 0, API.f(faceShapeMix), API.f(faceSkinMix), 0, false);
                API.callNative("UPDATE_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), API.f(faceShapeMix), API.f(faceSkinMix), 0);
                return;
            case 6:
                faceShapeMix += 0.1;
                if (faceShapeMix > 1) {
                    faceShapeMix = 1;
                }
                faceShapeMix = (Math.round(faceShapeMix * 100) / 100);
                API.callNative("UPDATE_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), API.f(faceShapeMix), API.f(faceSkinMix), 0);
                return;
            case 7:
                faceSkinMix += 0.1;
                if (faceSkinMix > 1) {
                    faceSkinMix = 1;
                }
                faceSkinMix = (Math.round(faceSkinMix * 100) / 100);
                API.callNative("UPDATE_PED_HEAD_BLEND_DATA", API.getLocalPlayer(), API.f(faceShapeMix), API.f(faceSkinMix), 0);
                return;
            case 8:
                if (faceAgeing > 14) {
                    faceAgeing = 0;
                    return;
                }
                faceAgeing += 1;
                API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 3, faceAgeing, API.f(1));
                return;
            case 9:
                if (faceComplexion > 11) {
                    faceComplexion = 0;
                    return;
                }
                faceComplexion += 1;
                API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 6, faceComplexion, API.f(1));
                return;
            case 10:
                if (faceMoles > 17) {
                    faceMoles = 0;
                    return;
                }
                faceMoles += 1;
                API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 9, faceMoles, API.f(1));
                return;
            case 11:
                if (faceBlemishes > 23) {
                    faceBlemishes = 0;
                    return;
                }
                faceBlemishes += 1;
                API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 0, faceBlemishes, API.f(1));
                return;
            case 12:
                if (faceSunDamage > 10) {
                    faceSunDamage = 0;
                    return;
                }
                faceSunDamage += 1;
                API.callNative("SET_PED_HEAD_OVERLAY", API.getLocalPlayer(), 7, faceSunDamage, API.f(1));
                return;
        }
    }
    if (currentPage === 1) {
        switch (currentIndex) {
            case 1:
                faceNoseWidth = adjustSelection(faceNoseWidth, 0.1, 0);
                return;
            case 2:
                faceNoseHeight = adjustSelection(faceNoseHeight, 0.1, 1);
                return;
            case 3:
                faceNoseLength = adjustSelection(faceNoseLength, 0.1, 2);
                return;
            case 4:
                faceNoseBridge = adjustSelection(faceNoseBridge, 0.1, 3);
                return;
            case 5:
                faceNoseTip = adjustSelection(faceNoseTip, 0.1, 4);
                return;
            case 6:
                faceNoseBridgeDepth = adjustSelection(faceNoseBridgeDepth, 0.1, 5);
                return;
            case 7:
                faceEyebrowHeight = adjustSelection(faceEyebrowHeight, 0.1, 6);
                return;
            case 8:
                faceEyebrowDepth = adjustSelection(faceEyebrowDepth, 0.1, 7);
                return;
        }
    }
    if (currentPage === 2) {
        switch (currentIndex) {
            case 1:
                faceCheekboneHeight = adjustSelection(faceCheekboneHeight, 0.1, 8);
                return;
            case 2:
                faceCheekboneDepth = adjustSelection(faceCheekboneDepth, 0.1, 9);
                return;
            case 3:
                faceCheekboneWidth = adjustSelection(faceCheekboneWidth, 0.1, 10);
                return;
            case 4:
                faceEyelids = adjustSelection(faceEyelids, 0.1, 11);
                return;
            case 5:
                faceLips = adjustSelection(faceLips, 0.1, 12);
                return;
            case 6:
                faceJawWidth = adjustSelection(faceJawWidth, 0.1, 13);
                return;
            case 7:
                faceJawDepth = adjustSelection(faceJawDepth, 0.1, 14);
                return;
            case 8:
                faceJawLength = adjustSelection(faceJawLength, 0.1, 15);
                return;
            case 9:
                faceChinFullness = adjustSelection(faceChinFullness, 0.1, 16);
                return;
            case 10:
                faceChinWidth = adjustSelection(faceChinWidth, 0.1, 17);
                return;
            case 11:
                faceNeckWidth = adjustSelection(faceNeckWidth, 0.1, 19);
                return;
        }
    }
}
function roundToFixed(_float, _digits) {
    var rounder = Math.pow(10, _digits);
    return (Math.round(_float * rounder) / rounder).toFixed(_digits);
}
function adjustSelection(type, amount, feature) {
    if (type > 1) {
        type = 1;
        return type;
    }
    type += amount;
    type = (Math.round(type * 100) / 100);
    API.callNative("_SET_PED_FACE_FEATURE", API.getLocalPlayer(), feature, API.f(type));
    return type;
}
// Surgery Menu
function showSurgeryMenu() {
    // Black Background
    API.drawRectangle(0, 0, (resX / 4), resY, 0, 0, 0, 150);
    // Blue Header + Text
    API.drawRectangle(0, 0, (resX / 4), (resY / 16), 112, 33, 33, 255);
    API.drawText("Surgery", (resX / 4) / 2, 0, 1, 255, 255, 255, 255, 1, 1, false, false, 500);
    API.drawText("[F - Save] [Backspace - Exit]", resX / 2, 0, 0.5, 255, 255, 255, 255, 4, 1, false, false, 500);
    // Gender
    if (currentPage === 0) {
        drawTextMenuText(0, "Current Page", currentPage);
        drawTextMenuText(1, "Gender", faceGender);
        drawTextMenuText(2, "Face #1", faceShapeOne);
        drawTextMenuText(3, "Face #2", faceShapeTwo);
        drawTextMenuText(4, "Skin #1", faceSkinOne);
        drawTextMenuText(5, "Skin #2", faceSkinTwo);
        drawTextMenuText(6, "Shape Mix", faceShapeMix);
        drawTextMenuText(7, "Skin Mix", faceSkinMix);
        drawTextMenuText(8, "Ageing", faceAgeing);
        drawTextMenuText(9, "Complexion", faceComplexion);
        drawTextMenuText(10, "Moles", faceMoles);
        drawTextMenuText(11, "Blemishes", faceBlemishes);
        drawTextMenuText(12, "Sun Damage", faceSunDamage);
    }
    if (currentPage === 1) {
        drawTextMenuText(0, "Current Page", currentPage);
        drawTextMenuText(1, "Nose Width", faceNoseWidth);
        drawTextMenuText(2, "Nose Height", faceNoseHeight);
        drawTextMenuText(3, "Nose Length", faceNoseLength);
        drawTextMenuText(4, "Nose Bridge", faceNoseBridge);
        drawTextMenuText(5, "Nose Tip", faceNoseTip);
        drawTextMenuText(6, "Nose Bridge Depth", faceNoseBridgeDepth);
        drawTextMenuText(7, "Eyebrow Height", faceEyebrowHeight);
        drawTextMenuText(8, "Eyebrow Depth", faceEyebrowDepth);
    }
    if (currentPage === 2) {
        drawTextMenuText(0, "Current Page", currentPage);
        drawTextMenuText(1, "Cheekbone Height", faceCheekboneHeight);
        drawTextMenuText(2, "Cheekbone Depth", faceCheekboneDepth);
        drawTextMenuText(3, "Cheekbone Width", faceCheekboneWidth);
        drawTextMenuText(4, "Eyelids", faceEyelids);
        drawTextMenuText(5, "Lips", faceLips);
        drawTextMenuText(6, "Jaw Width", faceJawWidth);
        drawTextMenuText(7, "Jaw Depth", faceJawDepth);
        drawTextMenuText(8, "Jaw Length", faceJawLength);
        drawTextMenuText(9, "Chin Fullness", faceChinFullness);
        drawTextMenuText(10, "Chin Width", faceChinWidth);
        drawTextMenuText(11, "Neck Width", faceNeckWidth);
    }
}
function drawTextMenuText(index, text, parameter) {
    if (currentIndex === index) {
        API.drawRectangle(0, startPointY * (index + 1), menuWidth, menuHeight, 255, 255, 255, 200);
        API.drawText("~u~" + text, 20, menuTextHeight * (index + 1), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~u~&lt; " + parameter + " &gt;", (resX / 5), menuTextHeight * (index + 1), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    else {
        API.drawText("~w~" + text, 20, menuTextHeight * (index + 1), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~&lt; " + parameter + " &gt;", (resX / 5), menuTextHeight * (index + 1), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
}
// Setup Surgery Mode
function setupSurgery() {
    setupSurgeryCamera();
    API.setHudVisible(false);
    API.setChatVisible(false);
    API.callNative("TASK_LOOK_AT_COORD", API.getLocalPlayer(), -36.58, -152.68, 57.8, -1, 0, 0);
    API.callNative("DO_SCREEN_FADE_IN", 3000);
}
// Setup Camera
function setupSurgeryCamera() {
    camera = API.createCamera(API.getEntityPosition(API.getLocalPlayer()).Add(new Vector3(2, 0, 0.85)), new Vector3());
    API.pointCameraAtEntity(camera, API.getLocalPlayer(), new Vector3(0, 0, 0.6));
    API.setActiveCamera(camera);
    API.setCameraFov(camera, 15);
}
// Rotate Camera Around Player
function rotateSurgeryCamera(value) {
    var angle = API.getCameraRotation(API.getActiveCamera()).Z + value;
    var cosTheta = Math.cos(angle);
    var sinTheta = Math.sin(angle);
    var center = API.getEntityPosition(API.getLocalPlayer());
    var camera = API.getCameraPosition(API.getActiveCamera());
    var x = (cosTheta * (camera.X - center.X) - sinTheta * (camera.Y - center.Y) + center.X);
    var y = (sinTheta * (camera.X - center.X) + cosTheta * (camera.Y - center.Y) + center.Y);
    API.setCameraPosition(API.getActiveCamera(), new Vector3(x, y, camera.Z));
}
