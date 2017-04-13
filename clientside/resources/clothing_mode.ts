var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var camera = null;
var cameraHeading = 0;
var currentIndex = 0;
var currentIndexName = "Hat";
// Main Values
var n_Face = 0;
var n_Mask = 0;
var n_Glasses = 0;
var n_Torso = 0;
var n_Legs = 0;
var n_Bags = 0;
var n_Feet = 0;
var n_Accessories = 0;
var n_Undershirt = 0;
var n_BodyArmor = 0;
var n_Decals = 0;
var n_Tops = 0;
var n_Hat = 0;
// Main Max Values
var n_Face_Max = 0;
var n_Mask_Max = 0;
var n_Glasses_Max = 0;
var n_Torso_Max = 0;
var n_Legs_Max = 0;
var n_Bags_Max = 0;
var n_Feet_Max = 0;
var n_Accessories_Max = 0;
var n_Undershirt_Max = 0;
var n_BodyArmor_Max = 0;
var n_Decals_Max = 0;
var n_Tops_Max = 0;
var n_Hat_Max = 0;
// Texture Values
var t_Face = 0;
var t_Mask = 0;
var t_Glasses = 0;
var t_Torso = 0;
var t_Legs = 0;
var t_Bags = 0;
var t_Feet = 0;
var t_Accessories = 0;
var t_Undershirt = 0;
var t_BodyArmor = 0;
var t_Decals = 0;
var t_Tops = 0;
var t_Hat = 0;
// Texture Max Values
var t_Face_Max = 0;
var t_Mask_Max = 0;
var t_Glasses_Max = 0;
var t_Torso_Max = 0;
var t_Legs_Max = 0;
var t_Bags_Max = 0;
var t_Feet_Max = 0;
var t_Accessories_Max = 0;
var t_Undershirt_Max = 0;
var t_BodyArmor_Max = 0;
var t_Decals_Max = 0;
var t_Tops_Max = 0;
var t_Hat_Max = 0;

API.onUpdate.connect(function () {
    if (camera === null) {
        return;
    }

    API.disableAllControlsThisFrame();

    if (API.isDisabledControlJustPressed(Enums.Controls.CursorScrollUp)) {
        adjustCameraFOV(-1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.CursorScrollDown)) {
        adjustCameraFOV(1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.MoveLeftOnly)) {
        switchClothing(-1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.MoveRightOnly)) {
        switchClothing(1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.MoveDownOnly)) {
        setCurrentIndex(1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.MoveUpOnly)) {
        setCurrentIndex(-1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.ParachuteBrakeLeft)) {
        switchTexture(-1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.ParachuteBrakeRight)) {
        switchTexture(1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.PhoneCancel)) {
        camera = null;
        API.setActiveCamera(null);
        API.setHudVisible(true);
        API.setChatVisible(true);
        API.callNative("CLEAR_PED_TASKS_IMMEDIATELY", API.getLocalPlayer());
        API.triggerServerEvent("exitClothingShop");
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.Enter)) {
        camera = null;
        API.setActiveCamera(null);
        API.setHudVisible(true);
        API.setChatVisible(true);
        API.callNative("CLEAR_PED_TASKS_IMMEDIATELY", API.getLocalPlayer());
        API.triggerServerEvent("clothingSave", n_Tops, t_Tops, n_Undershirt, t_Undershirt, n_Torso, n_Legs, t_Legs, n_Feet, t_Feet, n_Accessories, n_Glasses, t_Glasses, n_Bags, t_Bags, n_Mask, t_Mask, t_Torso, n_BodyArmor, t_BodyArmor, t_Accessories, n_Decals, t_Decals, n_Hat, t_Hat);
    }

    // Black Background
    API.drawRectangle(0, 0, (resX / 4), resY, 0, 0, 0, 150);
    // Blue Header + Text
    API.drawRectangle(0, 0, (resX / 4), (resY / 16), 54, 117, 219, 255);
    API.drawText("Clothing", (resX / 4) / 2, 0, 1, 255, 255, 255, 255, 1, 1, false, false, 500);
    API.drawText("[F - Save] [Backspace - Exit] ~n~[E & Q - Additional Textures]", resX / 2, 0, 0.5, 255, 255, 255, 255, 4, 1, false, false, 500);
    // Hat
    if (currentIndex === 0) {
        API.drawRectangle(0, (resY / 16) * 1, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Hat", 20, ((resY / 16) * 2) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Hat + " )", (resX / 5) - 20, ((resY / 16) * 2) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Hat", 20, ((resY / 16) * 2) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Hat + " )", (resX / 5) - 20, ((resY / 16) * 2) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Mask
    if (currentIndex === 1) {
        API.drawRectangle(0, (resY / 16) * 2, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Mask", 20, ((resY / 16) * 3) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Mask + " )", (resX / 5) - 20, ((resY / 16) * 3) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Mask", 20, ((resY / 16) * 3) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Mask + " )", (resX / 5) - 20, ((resY / 16) * 3) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Glasses
    if (currentIndex === 2) {
        API.drawRectangle(0, (resY / 16) * 3, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Glasses", 20, ((resY / 16) * 4) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Glasses + " )", (resX / 5) - 20, ((resY / 16) * 4) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Glasses", 20, ((resY / 16) * 4) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Glasses + " )", (resX / 5) - 20, ((resY / 16) * 4) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Torso
    if (currentIndex === 3) {
        API.drawRectangle(0, (resY / 16) * 4, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Torso", 20, ((resY / 16) * 5) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Torso + " )", (resX / 5) - 20, ((resY / 16) * 5) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Torso", 20, ((resY / 16) * 5) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Torso + " )", (resX / 5) - 20, ((resY / 16) * 5) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Legs
    if (currentIndex === 4) {
        API.drawRectangle(0, (resY / 16) * 5, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Pants", 20, ((resY / 16) * 6) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Legs + " )", (resX / 5) - 20, ((resY / 16) * 6) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Pants", 20, ((resY / 16) * 6) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Legs + " )", (resX / 5) - 20, ((resY / 16) * 6) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Bags
    if (currentIndex === 5) {
        API.drawRectangle(0, (resY / 16) * 6, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Bags & Backpacks", 20, ((resY / 16) * 7) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Bags + " )", (resX / 5) - 20, ((resY / 16) * 7) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Bags & Backpacks", 20, ((resY / 16) * 7) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Bags + " )", (resX / 5) - 20, ((resY / 16) * 7) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Feet / Shoes
    if (currentIndex === 6) {
        API.drawRectangle(0, (resY / 16) * 7, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Shoes", 20, ((resY / 16) * 8) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Feet + " )", (resX / 5) - 20, ((resY / 16) * 8) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Shoes", 20, ((resY / 16) * 8) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Feet + " )", (resX / 5) - 20, ((resY / 16) * 8) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Accessories
    if (currentIndex === 7) {
        API.drawRectangle(0, (resY / 16) * 8, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Accessories", 20, ((resY / 16) * 9) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Accessories + " )", (resX / 5) - 20, ((resY / 16) * 9) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Accessories", 20, ((resY / 16) * 9) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Accessories + " )", (resX / 5) - 20, ((resY / 16) * 9) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Undershirt
    if (currentIndex === 8) {
        API.drawRectangle(0, (resY / 16) * 9, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Undershirt", 20, ((resY / 16) * 10) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Undershirt + " )", (resX / 5) - 20, ((resY / 16) * 10) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Undershirt", 20, ((resY / 16) * 10) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Undershirt + " )", (resX / 5) - 20, ((resY / 16) * 10) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Body Armor
    if (currentIndex === 9) {
        API.drawRectangle(0, (resY / 16) * 10, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("BodyArmor", 20, ((resY / 16) * 11) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_BodyArmor + " )", (resX / 5) - 20, ((resY / 16) * 11) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~BodyArmor", 20, ((resY / 16) * 11) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_BodyArmor + " )", (resX / 5) - 20, ((resY / 16) * 11) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Decals
    if (currentIndex === 10) {
        API.drawRectangle(0, (resY / 16) * 11, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("BodyArmor", 20, ((resY / 16) * 12) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Decals + " )", (resX / 5) - 20, ((resY / 16) * 12) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Decals", 20, ((resY / 16) * 12) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Decals + " )", (resX / 5) - 20, ((resY / 16) * 12) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    }
    // Top
    if (currentIndex === 11) {
        API.drawRectangle(0, (resY / 16) * 12, (resX / 4), (resY / 16), 255, 255, 255, 200);
        API.drawText("Top", 20, ((resY / 16) * 13) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("( " + n_Tops + " )", (resX / 5) - 20, ((resY / 16) * 13) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } else {
        API.drawText("~w~Top", 20, ((resY / 16) * 13) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
        API.drawText("~w~( " + n_Tops + " )", (resX / 5) - 20, ((resY / 16) * 13) - (resY / 16), 1, 0, 0, 0, 255, 4, 0, false, false, 500);
    } 
});

function setupClothingMode(cameraPosition) {
    API.setHudVisible(false);
    API.setChatVisible(false);
    camera = API.createCamera(cameraPosition, new Vector3());
    API.pointCameraAtEntity(camera, API.getLocalPlayer(), new Vector3(0.5, 0, 0));
    API.setActiveCamera(camera);
    getPlayerClothing();

    API.callNative("TASK_TURN_PED_TO_FACE_COORD", API.getLocalPlayer(), cameraPosition.X, cameraPosition.Y, cameraPosition.Z, -1);

    n_Hat_Max = API.returnNative("GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0);
    n_Mask_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 1);
    n_Glasses_Max = API.returnNative("GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 1);
    n_Torso_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 3);
    n_Legs_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 4);
    n_Bags_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 5);
    n_Feet_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 6);
    n_Accessories_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 7);
    n_Undershirt_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 8);
    n_BodyArmor_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 9);
    n_Decals_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 10);
    n_Tops_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 11);

    API.callNative("DO_SCREEN_FADE_IN", 3000);
}

function adjustCameraFOV(amount) {
    API.setCameraFov(camera, API.getCameraFov(camera) + amount);
}

function setCurrentIndex(amount) {
    if (currentIndex + amount === -1 || currentIndex + amount === 12) {
        return;
    }

    var lastIndex = currentIndex;
    currentIndex += amount;

    switch (currentIndex) {
        case 0:
            currentIndexName = "Hat";
            break;
        case 1:
            currentIndexName = "Mask";
            break;
        case 2:
            currentIndexName = "Glasses";
            break;
        case 3:
            currentIndexName = "Torso";
            break;
        case 4:
            currentIndexName = "Legs";
            break;
        case 5:
            currentIndexName = "Bags & Backpacks";
            break;
        case 6:
            currentIndexName = "Feet";
            break;
        case 7:
            currentIndexName = "Accessories";
            break;
        case 8:
            currentIndexName = "Undershirt";
            break;
        case 9:
            currentIndexName = "Body Armor";
            break;
        case 10:
            currentIndexName = "Decals";
            break;
        case 11:
            currentIndexName = "Tops";
            break;
    }
}

function switchClothing(amount) {
    switch (currentIndex) {
        case 0:
            n_Hat += amount;
            n_Hat = checkForZeroOrMax(n_Hat, n_Hat_Max);
            t_Hat_Max = API.returnNative("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Hat);
            t_Hat = 0;
            break;
        case 1:
            n_Mask += amount;
            n_Mask = checkForZeroOrMax(n_Mask, n_Mask_Max);
            t_Mask_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 1, n_Mask);
            t_Mask = 0;
            break;
        case 2:
            n_Glasses += amount;
            n_Glasses = checkForZeroOrMax(n_Glasses, n_Glasses_Max);
            t_Glasses_Max = API.returnNative("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 2, n_Glasses);
            t_Glasses = 0;
            break;
        case 3:
            n_Torso += amount;
            n_Torso = checkForZeroOrMax(n_Torso, n_Torso_Max);
            t_Torso_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 3, n_Torso);
            t_Torso = 0;
            break;
        case 4:
            n_Legs += amount;
            n_Legs = checkForZeroOrMax(n_Legs, n_Legs_Max);
            t_Legs_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 4, n_Legs);
            t_Legs = 0;
            break;
        case 5:
            n_Bags += amount;
            n_Bags = checkForZeroOrMax(n_Bags, n_Bags_Max);
            t_Bags_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 5, n_Bags);
            t_Bags = 0;
            break;
        case 6:
            n_Feet += amount;
            n_Feet = checkForZeroOrMax(n_Feet, n_Feet_Max);
            t_Feet_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 6, n_Feet);
            t_Feet = 0;
            break;
        case 7:
            n_Accessories += amount;
            n_Accessories = checkForZeroOrMax(n_Accessories, n_Accessories_Max);
            t_Accessories_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 7, n_Accessories);
            t_Accessories = 0;
            break;
        case 8:
            n_Undershirt += amount;
            n_Undershirt = checkForZeroOrMax(n_Undershirt, n_Undershirt_Max);
            t_Undershirt_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 8, n_Undershirt);
            t_Undershirt = 0;
            break;
        case 9:
            n_BodyArmor += amount;
            n_BodyArmor = checkForZeroOrMax(n_BodyArmor, n_BodyArmor_Max);
            t_BodyArmor_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 9, n_BodyArmor);
            t_BodyArmor = 0;
            break;
        case 10:
            n_Decals += amount;
            n_Decals = checkForZeroOrMax(n_Decals, n_Decals_Max);
            t_Decals_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 10, n_Decals);
            t_Decals = 0;
            break;
        case 11:
            n_Tops += amount;
            t_Tops_Max = checkForZeroOrMax(n_Tops, n_Tops_Max);
            t_Tops_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 11, n_Tops);
            t_Tops = 0;
            break;
    }
    updateClothing();
}

function switchTexture(amount) {
    switch (currentIndex) {
        case 0:
            t_Hat += amount;
            t_Hat = checkForZeroOrMax(t_Hat, t_Hat_Max);
            break;
        case 1:
            t_Mask += amount;
            t_Mask = checkForZeroOrMax(t_Mask, t_Mask_Max);
            break;
        case 2:
            t_Glasses += amount;
            t_Glasses = checkForZeroOrMax(t_Glasses, t_Glasses_Max);
            break;
        case 3:
            t_Torso += amount;
            t_Torso = checkForZeroOrMax(t_Torso, t_Torso_Max);
            break;
        case 4:
            t_Legs += amount;
            t_Legs = checkForZeroOrMax(t_Legs, t_Legs_Max);
            break;
        case 5:
            t_Bags += amount;
            t_Bags = checkForZeroOrMax(t_Bags, t_Bags_Max);
            break;
        case 6:
            t_Feet += amount;
            t_Feet = checkForZeroOrMax(t_Feet, t_Feet_Max);
            break;
        case 7:
            t_Accessories += amount;
            t_Accessories = checkForZeroOrMax(t_Accessories, t_Accessories_Max);
            break;
        case 8:
            t_Undershirt += amount;
            t_Undershirt = checkForZeroOrMax(t_Undershirt, t_Undershirt_Max);
            break;
        case 9:
            t_BodyArmor += amount;
            t_BodyArmor = checkForZeroOrMax(t_BodyArmor, t_BodyArmor_Max);
            break;
        case 10:
            t_Decals += amount;
            t_Decals = checkForZeroOrMax(t_Decals, t_Decals_Max);
            break;
        case 11:
            t_Tops += amount;
            t_Tops = checkForZeroOrMax(t_Tops, t_Tops_Max);
            break;
    }
    updateClothing();
}


function checkForZeroOrMax(current, max) {
    if (current > max) {
        current = 0;
        return current;
    }

    if (current < 0) {
        current = max;
        return current;
    }

    return current;
}

function updateClothing() {
    API.setPlayerAccessory(API.getLocalPlayer(), 0, n_Hat, t_Hat);
    API.setPlayerClothes(API.getLocalPlayer(), 1, n_Mask, t_Mask);
    API.setPlayerAccessory(API.getLocalPlayer(), 1, n_Glasses, t_Glasses);
    API.setPlayerClothes(API.getLocalPlayer(), 3, n_Torso, t_Torso);
    API.setPlayerClothes(API.getLocalPlayer(), 4, n_Legs, t_Legs);
    API.setPlayerClothes(API.getLocalPlayer(), 5, n_Bags, t_Bags);
    API.setPlayerClothes(API.getLocalPlayer(), 6, n_Feet, t_Feet);
    API.setPlayerClothes(API.getLocalPlayer(), 7, n_Accessories, t_Accessories);
    API.setPlayerClothes(API.getLocalPlayer(), 8, n_Undershirt, t_Undershirt);
    API.setPlayerClothes(API.getLocalPlayer(), 9, n_BodyArmor, t_BodyArmor);
    API.setPlayerClothes(API.getLocalPlayer(), 10, n_Decals, t_Decals);
    API.setPlayerClothes(API.getLocalPlayer(), 11, n_Tops, t_Tops);
}

function getPlayerClothing() {
    // Drawables
    n_Hat = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingHat"));
    n_Mask = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingMask"));
    n_Glasses = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingGlasses"));
    n_Torso = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingTorso"));
    n_Legs = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingLegs"));
    n_Bags = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingBag"));
    n_Feet = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingShoes"));
    n_Accessories = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingAccessory"));
    n_Undershirt = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingUndershirt"));
    n_BodyArmor = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingBodyArmor"));
    n_Decals = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingDecal"));
    n_Tops = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingTop"));

    // Textures
    t_Hat = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingHatColor"));
    t_Mask = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingMaskColor"));
    t_Glasses = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingGlassesColor"));
    t_Torso = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingTorsoColor"));
    t_Legs = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingLegsColor"));
    t_Bags = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingBagColor"));
    t_Feet = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingShoesColor"));
    t_Accessories = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingAccessoryColor"));
    t_Undershirt = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingUndershirtColor"));
    t_BodyArmor = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingBodyArmorColor"));
    t_Decals = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingDecalColor"));
    t_Tops = Number(API.getEntitySyncedData(API.getLocalPlayer(), "clothingTopColor"));
    
}