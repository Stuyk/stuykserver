var x = API.getScreenResolutionMantainRatio().Width;
var y = API.getScreenResolutionMantainRatio().Height;
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
    displayText();
    if (API.isDisabledControlJustPressed(241 /* CursorScrollUp */)) {
        adjustCameraFOV(-1);
    }
    if (API.isDisabledControlJustPressed(242 /* CursorScrollDown */)) {
        adjustCameraFOV(1);
    }
    if (API.isDisabledControlJustPressed(34 /* MoveLeftOnly */)) {
        switchClothing(-1);
    }
    if (API.isDisabledControlJustPressed(35 /* MoveRightOnly */)) {
        switchClothing(1);
    }
    if (API.isDisabledControlJustPressed(33 /* MoveDownOnly */)) {
        setCurrentIndex(1);
    }
    if (API.isDisabledControlJustPressed(32 /* MoveUpOnly */)) {
        setCurrentIndex(-1);
    }
    if (API.isDisabledControlJustPressed(152 /* ParachuteBrakeLeft */)) {
        switchTexture(-1);
    }
    if (API.isDisabledControlJustPressed(153 /* ParachuteBrakeRight */)) {
        switchTexture(1);
    }
    if (API.isDisabledControlJustPressed(177 /* PhoneCancel */)) {
        camera = null;
        API.setActiveCamera(null);
        API.setHudVisible(true);
        API.setChatVisible(true);
        API.callNative("CLEAR_PED_TASKS_IMMEDIATELY", API.getLocalPlayer());
        API.triggerServerEvent("exitClothingShop");
    }
    if (API.isDisabledControlJustPressed(23 /* Enter */)) {
        camera = null;
        API.setActiveCamera(null);
        API.setHudVisible(true);
        API.setChatVisible(true);
        API.callNative("CLEAR_PED_TASKS_IMMEDIATELY", API.getLocalPlayer());
        API.triggerServerEvent("clothingSave", n_Tops, t_Tops, n_Undershirt, t_Undershirt, n_Torso, n_Legs, t_Legs, n_Feet, t_Feet, n_Accessories, n_Glasses, t_Glasses, n_Bags, t_Bags, n_Mask, t_Mask, t_Torso, n_BodyArmor, t_BodyArmor, t_Accessories, n_Decals, t_Decals, n_Hat, t_Hat);
    }
});
function displayText() {
    API.drawRectangle(0, 0, x / 6, y, 0, 0, 0, 150);
    API.drawText("~b~Keybinds:~w~~n~Change Index - W or S~n~Change Clothing - A or D~n~Change Texture - Q or E~n~F - Save~n~Backspace/Esc - Exit~n~~b~CurrentIndex:~w~ " + currentIndexName, 20, 20, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Hat: ~w~" + n_Hat + "~b~/~w~" + n_Hat_Max + " ~b~||~w~ " + t_Hat + "~b~/~w~" + t_Hat_Max, 20, 250, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Mask: ~w~" + n_Mask + "~b~/~w~" + n_Mask_Max + " ~b~||~w~ " + t_Mask + "~b~/~w~" + t_Mask_Max, 20, 285, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Glasses: ~w~" + n_Glasses + "~b~/~w~" + n_Glasses_Max + " ~b~||~w~ " + t_Glasses + "~b~/~w~" + t_Glasses_Max, 20, 320, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Torso: ~w~" + n_Torso + "~b~/~w~" + n_Torso_Max + " ~b~||~w~ " + t_Torso + "~b~/~w~" + t_Torso_Max, 20, 355, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Legs: ~w~" + n_Legs + "~b~/~w~" + n_Legs_Max + " ~b~||~w~ " + t_Legs + "~b~/~w~" + t_Legs_Max, 20, 390, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Bags: ~w~" + n_Bags + "~b~/~w~" + n_Bags_Max + " ~b~||~w~ " + t_Bags + "~b~/~w~" + t_Bags_Max, 20, 425, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Feet: ~w~" + n_Feet + "~b~/~w~" + n_Feet_Max + " ~b~||~w~ " + t_Feet + "~b~/~w~" + t_Feet_Max, 20, 460, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Accessories: ~w~" + n_Accessories + "~b~/~w~" + n_Accessories_Max + " ~b~||~w~ " + t_Accessories + "~b~/~w~" + t_Accessories_Max, 20, 495, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Undershirt: ~w~" + n_Undershirt + "~b~/~w~" + n_Undershirt_Max + " ~b~||~w~ " + t_Undershirt + "~b~/~w~" + t_Undershirt_Max, 20, 530, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Body Armor: ~w~" + n_BodyArmor + "~b~/~w~" + n_BodyArmor_Max + " ~b~||~w~ " + t_BodyArmor + "~b~/~w~" + t_BodyArmor_Max, 20, 565, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Decals: ~w~" + n_Decals + "~b~/~w~" + n_Decals_Max + " ~b~||~w~ " + t_Decals + "~b~/~w~" + t_Decals_Max, 20, 600, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
    API.drawText("~b~Top: ~w~" + n_Tops + "~b~/~w~" + n_Tops_Max + " ~b~||~w~ " + t_Tops + "~b~/~w~" + t_Tops_Max, 20, 635, 0.5, 0, 0, 0, 255, 4, 0, false, true, x / 4 - 40);
}
function setupClothingMode(cameraPosition) {
    API.setHudVisible(false);
    API.setChatVisible(false);
    camera = API.createCamera(cameraPosition, new Vector3());
    API.pointCameraAtEntity(camera, API.getLocalPlayer(), new Vector3(0.5, 0, 0));
    API.setActiveCamera(camera);
    getPlayerClothing();
    API.callNative("TASK_TURN_PED_TO_FACE_COORD", API.getLocalPlayer(), cameraPosition.X, cameraPosition.Y, cameraPosition.Z, -1);
    n_Hat_Max = API.returnNative("GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 0);
    n_Mask_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 1);
    n_Glasses_Max = API.returnNative("GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 1);
    n_Torso_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 3);
    n_Legs_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 4);
    n_Bags_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 5);
    n_Feet_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 6);
    n_Accessories_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 7);
    n_Undershirt_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 8);
    n_BodyArmor_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 9);
    n_Decals_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 10);
    n_Tops_Max = API.returnNative("GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 11);
}
function adjustCameraFOV(amount) {
    API.setCameraFov(camera, API.getCameraFov(camera) + amount);
}
function setCurrentIndex(amount) {
    if (currentIndex + amount === 0 || currentIndex + amount === 12) {
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
            t_Hat_Max = API.returnNative("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 0, n_Hat);
            t_Hat = 0;
            break;
        case 1:
            n_Mask += amount;
            n_Mask = checkForZeroOrMax(n_Mask, n_Mask_Max);
            t_Mask_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 1, n_Mask);
            t_Mask = 0;
            break;
        case 2:
            n_Glasses += amount;
            n_Glasses = checkForZeroOrMax(n_Glasses, n_Glasses_Max);
            t_Glasses_Max = API.returnNative("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 2, n_Glasses);
            t_Glasses = 0;
            break;
        case 3:
            n_Torso += amount;
            n_Torso = checkForZeroOrMax(n_Torso, n_Torso_Max);
            t_Torso_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 3, n_Torso);
            t_Torso = 0;
            break;
        case 4:
            n_Legs += amount;
            n_Legs = checkForZeroOrMax(n_Legs, n_Legs_Max);
            t_Legs_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 4, n_Legs);
            t_Legs = 0;
            break;
        case 5:
            n_Bags += amount;
            n_Bags = checkForZeroOrMax(n_Bags, n_Bags_Max);
            t_Bags_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 5, n_Bags);
            t_Bags = 0;
            break;
        case 6:
            n_Feet += amount;
            n_Feet = checkForZeroOrMax(n_Feet, n_Feet_Max);
            t_Feet_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 6, n_Feet);
            t_Feet = 0;
            break;
        case 7:
            n_Accessories += amount;
            n_Accessories = checkForZeroOrMax(n_Accessories, n_Accessories_Max);
            t_Accessories_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 7, n_Accessories);
            t_Accessories = 0;
            break;
        case 8:
            n_Undershirt += amount;
            n_Undershirt = checkForZeroOrMax(n_Undershirt, n_Undershirt_Max);
            t_Undershirt_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 8, n_Undershirt);
            t_Undershirt = 0;
            break;
        case 9:
            n_BodyArmor += amount;
            n_BodyArmor = checkForZeroOrMax(n_BodyArmor, n_BodyArmor_Max);
            t_BodyArmor_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 9, n_BodyArmor);
            t_BodyArmor = 0;
            break;
        case 10:
            n_Decals += amount;
            n_Decals = checkForZeroOrMax(n_Decals, n_Decals_Max);
            t_Decals_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 10, n_Decals);
            t_Decals = 0;
            break;
        case 11:
            n_Tops += amount;
            t_Tops_Max = checkForZeroOrMax(n_Tops, n_Tops_Max);
            t_Tops_Max = API.returnNative("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", 0 /* Int */, API.getLocalPlayer(), 11, n_Tops);
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
