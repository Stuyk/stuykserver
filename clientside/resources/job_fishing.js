var castPosition = null;
var grow = true;
var buoyUpdate = new Date().getTime();
var wordPanel = null;
var inputBox = null;
var increment = 0.000035;
var progressBarPhase = 0; // 0 Add - 1 Subtract
var progressBar = null;
var buoy = null;
var phase = 0;
var word = null;
var isWordReady = false;
var isFishing = false;
var inWater = false;
var lastTimeCheck = new Date().getTime(); // ms
var fishTimeout = new Date().getTime();
var notificationSent = false; // Has our notification been sent yet?
var posX = API.getScreenResolutionMantainRatio().Width;
var posY = API.getScreenResolutionMantainRatio().Height;
API.onUpdate.connect(function () {
    if (new Date().getTime() > lastTimeCheck + 5000) {
        lastTimeCheck = new Date().getTime();
        isPlayerNearWater();
    }
    if (!inWater) {
        return;
    }
    if (isWordReady && word !== null) {
        if (API.isControlJustPressed(23 /* Enter */) || API.isControlJustPressed(237 /* CursorAccept */) && !API.isChatOpen()) {
            switch (phase) {
                case 0:
                    drawFishingMenus();
                    return;
                case 1:
                    phase = 2;
                    API.triggerServerEvent("requestWord", progressBar.returnProgressAmount());
                    return;
            }
            return;
        }
        switch (phase) {
            case 0:
                if (buoy !== null && new Date().getTime() > buoyUpdate + 60) {
                    API.deleteEntity(buoy);
                    buoyUpdate = new Date().getTime();
                    castPosition = new Vector3(castPosition.X, castPosition.Y, castPosition.Z - 0.01);
                    buoy = API.createMarker(28 /* DebugSphere */, castPosition, new Vector3(), new Vector3(), new Vector3(0.1, 0.1, 0.1), 255, 0, 0, 255);
                }
                if (new Date().getTime() > fishTimeout + 8000) {
                    stopFishing();
                    failEvent();
                }
                return;
            case 1:
                let currentValue = progressBar.returnProgressAmount();
                if (progressBarPhase === 0) {
                    if (currentValue >= 100) {
                        progressBarPhase = 1;
                    }
                    if (currentValue < 100) {
                        if (currentValue <= 0.5) {
                            progressBar.addProgress(1);
                        }
                        progressBar.addProgress(0.05 * currentValue);
                    }
                }
                if (progressBarPhase === 1) {
                    if (currentValue <= 1) {
                        progressBarPhase = 0;
                    }
                    if (currentValue > -5) {
                        progressBar.subtractProgress(0.05 * currentValue);
                    }
                }
                return;
            case 2:
                let currentInput = inputBox.returnInput();
                if (currentInput.length < 1) {
                    return;
                }
                if (currentInput === word.substring(0, currentInput.length)) {
                    wordPanel.setText(`Type: ~g~${word.substring(0, currentInput.length)}~w~${word.substring(currentInput.length, word.length)}`);
                }
                else {
                    API.triggerServerEvent("FishingFail");
                }
                if (currentInput.length === word.length) {
                    phase = 4;
                    API.triggerServerEvent("FishingVerify", inputBox.returnInput());
                }
                return;
        }
        return;
    }
    if (phase > 0) {
        return;
    }
    if (API.isControlJustPressed(23 /* Enter */) && !API.isChatOpen()) {
        tryToFish();
        return;
    }
});
function isPlayerNearWater() {
    inWater = API.returnNative("IS_ENTITY_IN_WATER", 8 /* Bool */, API.getLocalPlayer());
    if (API.returnNative("IS_PED_SWIMMING_UNDER_WATER", 8 /* Bool */, API.getLocalPlayer())) {
        inWater = false;
        stopFishing();
        return;
    }
    if (API.returnNative("IS_PED_SWIMMING", 8 /* Bool */, API.getLocalPlayer())) {
        inWater = false;
        stopFishing();
        return;
    }
    if (!inWater) {
        notificationSent = false;
        return;
    }
    if (!notificationSent) {
        notificationSent = true;
        let notification = resource.menu_builder.createNotification(0, "You've entered some water. ~n~Press your action key to begin fishing.", 2000);
        notification.setColor(255, 255, 255);
    }
}
function tryToFish() {
    let aimCoords = API.getPlayerAimCoords(API.getLocalPlayer());
    if (isFishing) {
        API.triggerServerEvent("stopFishing");
        isFishing = false;
        isWordReady = false;
        word = null;
        phase = 0;
        resource.menu_builder.killMenu();
        if (buoy !== null) {
            API.deleteEntity(buoy);
        }
        return;
    }
    if (aimCoords.Z > API.getEntityPosition(API.getLocalPlayer()).Z - 2) {
        let notification = resource.menu_builder.createNotification(0, "You need to find a deeper spot to fish.", 2000);
        notification.setColor(255, 255, 255);
        return;
    }
    API.callNative("TASK_TURN_PED_TO_FACE_COORD", API.getLocalPlayer(), aimCoords.X, aimCoords.Y, aimCoords.Z, 2000);
    if (buoy !== null) {
        API.deleteEntity(buoy);
    }
    let playerPoint = API.getEntityPosition(API.getLocalPlayer());
    castPosition = new Vector3(aimCoords.X, aimCoords.Y, playerPoint.Z - 0.2);
    API.sleep(2000);
    API.triggerServerEvent("tryFishing");
    isFishing = true;
}
function wordIsReady(value) {
    isWordReady = true;
    word = value;
    fishTimeout = new Date().getTime();
    //let notification = resource.menu_builder.createNotification(0, "~b~You feel a bite. ~n~Press your action key.", 1500);
    //notification.setColor(0, 153, 255);
}
function drawFishingMenus() {
    phase = 1;
    resource.menu_builder.setupMenu(2);
    // Page 1
    let panel = resource.menu_builder.createPanel(0, 12, 4, 7, 1, true, "Fishing");
    panel.setCentered();
    panel = resource.menu_builder.createPanel(0, 12, 5, 7, 2, false, "Press ~b~F ~w~when your progress bar is maxed out.");
    panel.setCentered();
    panel.setFontScale(0.4);
    progressBar = resource.menu_builder.createProgressBar(0, 12, 6, 7, 1, 0);
    progressBar.setColor(0, 153, 255);
    // Page 2
    panel = resource.menu_builder.createPanel(1, 12, 4, 7, 1, true, "Fishing");
    panel.setCentered();
    wordPanel = resource.menu_builder.createPanel(1, 12, 5, 7, 1, false, "Type: " + word);
    wordPanel.setCentered();
    wordPanel.setFontScale(0.6);
    inputBox = resource.menu_builder.createInput(1, 12, 6, 7, 1, false, false);
    resource.menu_builder.openMenu(true, false, false, true, false);
}
function wordMode() {
    resource.menu_builder.setPage(1);
    inputBox.setSelected();
}
function stopFishing() {
    if (isFishing) {
        API.triggerServerEvent("stopFishing");
        isFishing = false;
        isWordReady = false;
        word = null;
        phase = 0;
        resource.menu_builder.killMenu();
        if (buoy !== null) {
            API.deleteEntity(buoy);
            buoy = null;
        }
    }
}
function winEvent() {
    API.triggerServerEvent("stopFishing");
    isFishing = false;
    isWordReady = false;
    word = null;
    phase = 0;
    resource.menu_builder.killMenu();
    API.showColorShard("~y~Perfect Catch!", "~y~Well Done!", 2, 12, 3000);
    API.playSoundFrontEnd("WIN", "HUD_AWARDS");
    if (buoy !== null) {
        API.deleteEntity(buoy);
    }
}
function failEvent() {
    stopFishing();
    API.showColorShard("~r~Failed Event", "~r~The fish swam away.", 2, 12, 3000);
    API.playSoundFrontEnd("ScreenFlash", "MissionFailedSounds");
}
function createBuoy() {
    buoy = API.createMarker(28 /* DebugSphere */, castPosition, new Vector3(), new Vector3(), new Vector3(0.1, 0.1, 0.1), 255, 0, 0, 255);
}
