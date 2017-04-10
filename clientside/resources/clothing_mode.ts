var x = API.getScreenResolutionMantainRatio().Width;
var y = API.getScreenResolutionMantainRatio().Height;
var camera = null;
var currentIndex = 1;
var currentIndexName = "Mask";
// Main Values
var n_Face = 0;
var n_Mask = 0;
var n_Hair = 0;
var n_Torso = 0;
var n_Legs = 0;
var n_Bags = 0;
var n_Feet = 0;
var n_Accessories = 0;
var n_Undershirt = 0;
var n_BodyArmor = 0;
var n_Decals = 0;
var n_Tops = 0;
// Main Max Values
var n_Face_Max = 0;
var n_Mask_Max = 0;
var n_Hair_Max = 0;
var n_Torso_Max = 0;
var n_Legs_Max = 0;
var n_Bags_Max = 0;
var n_Feet_Max = 0;
var n_Accessories_Max = 0;
var n_Undershirt_Max = 0;
var n_BodyArmor_Max = 0;
var n_Decals_Max = 0;
var n_Tops_Max = 0;
// Texture Values
var t_Face = 0;
var t_Mask = 0;
var t_Hair = 0;
var t_Torso = 0;
var t_Legs = 0;
var t_Bags = 0;
var t_Feet = 0;
var t_Accessories = 0;
var t_Undershirt = 0;
var t_BodyArmor = 0;
var t_Decals = 0;
var t_Tops = 0;
// Texture Max Values
var t_Face_Max = 0;
var t_Mask_Max = 0;
var t_Hair_Max = 0;
var t_Torso_Max = 0;
var t_Legs_Max = 0;
var t_Bags_Max = 0;
var t_Feet_Max = 0;
var t_Accessories_Max = 0;
var t_Undershirt_Max = 0;
var t_BodyArmor_Max = 0;
var t_Decals_Max = 0;
var t_Tops_Max = 0;

API.onChatMessage.connect(function (msg) {
    if (msg !== "clothing") {
        return;
    }

    API.sendChatMessage("CLOTHING TIME MOTHAFUCKA!");
    setupClothingMode();
});

API.onUpdate.connect(function () {
    if (camera === null) {
        return;
    }

    API.disableAllControlsThisFrame();
    displayText();

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

    if (API.isDisabledControlJustPressed(Enums.Controls.MoveUpOnly)) {
        setCurrentIndex(1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.MoveDownOnly)) {
        setCurrentIndex(-1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.ParachuteBrakeLeft)) {
        switchTexture(-1);
    }

    if (API.isDisabledControlJustPressed(Enums.Controls.ParachuteBrakeRight)) {
        switchTexture(1);
    }

    if (API.isDisabledControlPressed(Enums.Controls.LookLeftOnly)) {

    }

    if (API.isDisabledControlJustPressed(Enums.Controls.PhoneCancel)) {
        camera = null;
        API.setActiveCamera(null);
        API.setHudVisible(true);
        API.setChatVisible(true);
        // Save Push Here
    }
});

function displayText() {
    API.drawRectangle(0, 0, x / 4, y, 0, 0, 0, 150);
    API.drawText("~b~Keybinds:~w~~n~Change Index - W or S~n~Change Clothing - A or D~n~Change Texture - Q or E~n~Enter - Save~n~Backspace - Exit~n~~b~CurrentIndex:~w~ " + currentIndexName, 20, 20, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Face: ~w~" + n_Face + "/" + n_Face_Max, 20, 250, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Mask: ~w~" + n_Mask + "/" + n_Mask_Max, 20, 285, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Hair: ~w~" + n_Hair + "/" + n_Hair_Max, 20, 320, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Torso: ~w~" + n_Torso + "/" + n_Torso_Max, 20, 355, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Legs: ~w~" + n_Legs + "/" + n_Legs_Max, 20, 390, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Bags: ~w~" + n_Bags + "/" + n_Bags_Max, 20, 425, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Feet: ~w~" + n_Feet + "/" + n_Feet_Max, 20, 460, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Accessories: ~w~" + n_Accessories + "/" + n_Accessories_Max, 20, 495, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Undershirt: ~w~" + n_Undershirt + "/" + n_Undershirt_Max, 20, 530, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Body Armor: ~w~" + n_BodyArmor + "/" + n_BodyArmor_Max, 20, 565, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Decals: ~w~" + n_Decals + "/" + n_Decals_Max, 20, 600, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Top: ~w~" + n_Tops + "/" + n_Tops_Max, 20, 635, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
}

function setupClothingMode() {
    API.setHudVisible(false);
    API.setChatVisible(false);

    camera = API.createCamera(API.getEntityPosition(API.getLocalPlayer()), new Vector3());
    API.attachCameraToEntity(camera, API.getLocalPlayer(), new Vector3(0, 3, 0));
    API.pointCameraAtEntity(camera, API.getLocalPlayer(), new Vector3(0.5, 0, 0));
    API.setActiveCamera(camera);

    n_Face_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0);
    n_Mask_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 1);
    n_Hair_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 2);
    n_Torso_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 3);
    n_Legs_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 4);
    n_Bags_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 5);
    n_Feet_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 6);
    n_Accessories_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 7);
    n_Undershirt_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 8);
    n_BodyArmor_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 9);
    n_Decals_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 10);
    n_Tops_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 11);
}

function adjustCameraFOV(amount) {
    API.setCameraFov(camera, API.getCameraFov(camera) + amount);
}

function setCurrentIndex(amount) {
    if (currentIndex + amount === -1 || currentIndex + amount === 12) {
        return;
    }

    currentIndex += amount;

    switch (currentIndex) {
        case 0:
            currentIndexName = "Face";
            t_Face = 0;
            break;
        case 1:
            currentIndexName = "Mask";
            t_Mask = 0;
            break;
        case 2:
            currentIndexName = "Hair";
            t_Hair = 0;
            break;
        case 3:
            currentIndexName = "Torso";
            t_Torso = 0;
            break;
        case 4:
            currentIndexName = "Legs";
            t_Legs = 0;
            break;
        case 5:
            currentIndexName = "Bags & Backpacks";
            t_Bags = 0;
            break;
        case 6:
            currentIndexName = "Feet";
            t_Feet = 0;
            break;
        case 7:
            currentIndexName = "Accessories";
            t_Accessories = 0;
            break;
        case 8:
            currentIndexName = "Undershirt";
            t_Undershirt = 0;
            break;
        case 9:
            currentIndexName = "Body Armor";
            t_BodyArmor = 0;
            break;
        case 10:
            currentIndexName = "Decals";
            t_Decals = 0;
            break;
        case 11:
            currentIndexName = "Tops";
            t_Tops = 0;
            break;
    }
}

function switchClothing(amount) {
    switch (currentIndex) {
        case 0:
            n_Face += amount;
            n_Face = checkForZeroOrMax(n_Face, n_Face_Max);
            t_Face_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Face);
            break;
        case 1:
            n_Mask += amount;
            n_Mask = checkForZeroOrMax(n_Mask, n_Mask_Max);
            t_Mask_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Mask);
            break;
        case 2:
            n_Hair += amount;
            n_Hair = checkForZeroOrMax(n_Hair, n_Hair_Max);
            t_Hair_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Hair);
            break;
        case 3:
            n_Torso += amount;
            n_Torso = checkForZeroOrMax(n_Torso, n_Torso_Max);
            t_Torso_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Torso);
            break;
        case 4:
            n_Legs += amount;
            n_Legs = checkForZeroOrMax(n_Legs, n_Legs_Max);
            t_Legs_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Legs);
            break;
        case 5:
            n_Bags += amount;
            n_Bags = checkForZeroOrMax(n_Bags, n_Bags_Max);
            t_Bags_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Bags);
            break;
        case 6:
            n_Feet += amount;
            n_Feet = checkForZeroOrMax(n_Feet, n_Feet_Max);
            t_Feet_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Feet);
            break;
        case 7:
            n_Accessories += amount;
            n_Accessories = checkForZeroOrMax(n_Accessories, n_Accessories_Max);
            t_Accessories_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Accessories);
            break;
        case 8:
            n_Undershirt += amount;
            n_Undershirt = checkForZeroOrMax(n_Undershirt, n_Undershirt_Max);
            t_Undershirt_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Undershirt);
            break;
        case 9:
            n_BodyArmor += amount;
            n_BodyArmor = checkForZeroOrMax(n_BodyArmor, n_BodyArmor_Max);
            t_BodyArmor_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_BodyArmor);
            break;
        case 10:
            n_Decals += amount;
            n_Decals = checkForZeroOrMax(n_Decals, n_Decals_Max);
            t_Decals_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Enums.NativeReturnTypes.Int, API.getLocalPlayer(), 0, n_Decals);
            break;
        case 11:
            n_Tops += amount;
            t_Tops_Max = checkForZeroOrMax(n_Tops, n_Tops_Max);
            break;
    }
    updateClothing();
}

function switchTexture(amount) {
    switch (currentIndex) {
        case 0:
            t_Face += amount;
            t_Face = checkForZeroOrMax(t_Face, t_Face_Max);
            break;
        case 1:
            t_Mask += amount;
            t_Mask = checkForZeroOrMax(t_Mask, t_Mask_Max);
            break;
        case 2:
            t_Hair += amount;
            t_Hair = checkForZeroOrMax(t_Hair, t_Hair_Max);
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
    var player = API.getLocalPlayer();
    API.setPlayerClothes(player, 0, n_Face, t_Face);
    API.setPlayerClothes(player, 1, n_Mask, t_Mask);
    API.setPlayerClothes(player, 2, n_Hair, t_Hair);
    API.setPlayerClothes(player, 3, n_Torso, t_Torso);
    API.setPlayerClothes(player, 4, n_Legs, t_Legs);
    API.setPlayerClothes(player, 5, n_Bags, t_Bags);
    API.setPlayerClothes(player, 6, n_Feet, t_Feet);
    API.setPlayerClothes(player, 7, n_Accessories, t_Accessories);
    API.setPlayerClothes(player, 8, n_Undershirt, t_Undershirt);
    API.setPlayerClothes(player, 9, n_BodyArmor, t_BodyArmor);
    API.setPlayerClothes(player, 10, n_Decals, t_Decals);
    API.setPlayerClothes(player, 11, n_Tops, t_Tops);
}