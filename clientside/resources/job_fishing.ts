var castPosition = null;
var grow = true;
var buoyUpdate = new Date().getTime();
var wordPanel: TextElement = null;
var inputBox: InputPanel = null;
var increment = 0.000035;
var progressBarPhase = 0; // 0 Add - 1 Subtract
var progressBar: ProgressBar = null;
var buoy = null;
var phase = 0;
var word = null;
var isWordReady = false;
var isFishing = false;
var inWater = false;
var inBoat = false;
var lastTimeCheck = new Date().getTime(); // ms
var fishTimeout = new Date().getTime();
var notificationSentWater = false; // Has our notification been sent yet?
var notificationSentBoat = false;
var posX = API.getScreenResolutionMantainRatio().Width;
var posY = API.getScreenResolutionMantainRatio().Height;
var fishingBugOutTimer = new Date().getTime(); // Used to cancel the event if something isn't happening.
var menu: Menu;
API.onUpdate.connect(function () {
    if (API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
        return;
    }

    if (new Date().getTime() > lastTimeCheck + 5000) {
        lastTimeCheck = new Date().getTime();
        isPlayerNearWater();
        isPlayerNearBoat();
    }

    if (inWater || inBoat) {
        if (isWordReady && word !== null) {
            if (API.isControlJustPressed(Enums.Controls.Enter) || API.isControlJustPressed(Enums.Controls.CursorAccept) && !API.isChatOpen()) {
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
                        buoy = API.createMarker(Enums.MarkerType.DebugSphere, castPosition, new Vector3(), new Vector3(), new Vector3(0.1, 0.1, 0.1), 255, 0, 0, 255);
                    }
                    if (new Date().getTime() > fishTimeout + 8000) {
                        stopFishing();
                        failEvent();
                    }
                    return;
                case 1:
                    let currentValue = progressBar.returnProgressAmount()
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
                    let currentInput: string = inputBox.Input;

                    if (currentInput.length < 1) {
                        return;
                    }

                    if (currentInput === word.substring(0, currentInput.length)) {
                        wordPanel.Text = `Type: ~g~${word.substring(0, currentInput.length)}~w~${word.substring(currentInput.length, word.length)}`;
                    } else {
                        API.triggerServerEvent("FishingFail");
                    }

                    if (currentInput.length === word.length) {
                        phase = 4;
                        API.triggerServerEvent("FishingVerify", inputBox.Input);
                    }
                    return;
            }
            return;
        }

        if (isFishing) {
            if (new Date().getTime() > fishingBugOutTimer + 60000) {
                fishingBugOutTimer = new Date().getTime();
                stopFishing();
                noFishHere();
            }
        }

        if (phase > 0) {
            return;
        }

        if (API.isControlJustPressed(Enums.Controls.Enter) && !API.isChatOpen()) {
            tryToFish();
            return;
        }
    }
});
function isPlayerNearWater() {
    inWater = API.returnNative("IS_ENTITY_IN_WATER", Enums.NativeReturnTypes.Bool, API.getLocalPlayer());

    if (API.returnNative("IS_PED_SWIMMING_UNDER_WATER", Enums.NativeReturnTypes.Bool, API.getLocalPlayer())) {
        inWater = false;
        stopFishing();
        return;
    }

    if (API.returnNative("IS_PED_SWIMMING", Enums.NativeReturnTypes.Bool, API.getLocalPlayer())) {
        inWater = false;
        stopFishing();
        return;
    }

    if (!inWater) {
        notificationSentWater = false;
        return;
    }

    if (!notificationSentWater) {
        notificationSentWater = true;
        let notification = resource.menu_builder.createNotification(0, "You've entered some water. ~n~Press your action key to begin fishing.", 2000);
        notification.setColor(255, 255, 255);
    }
}
function isPlayerNearBoat() {
    var boats = API.getAllVehicles();
    var playerPos = API.getEntityPosition(API.getLocalPlayer());
    for (var i = 0; i < boats.Length; i++) {
        if (API.getEntityPosition(boats[i]).DistanceTo(playerPos) <= 10) {
            inBoat = true;
            if (!notificationSentBoat) {
                notificationSentBoat = true;
                let notification = resource.menu_builder.createNotification(0, "You're near a boat. ~n~Press your action key to begin fishing.", 2000);
                notification.setColor(255, 255, 255);
            }
            return;
        }
    }
    inBoat = false;
    notificationSentBoat = false;
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

    if (inWater) {
        let playerPoint = API.getEntityPosition(API.getLocalPlayer());
        castPosition = new Vector3(aimCoords.X, aimCoords.Y, playerPoint.Z - 0.2);
    }

    if (inBoat) {
        let playerPoint = API.getEntityPosition(API.getLocalPlayer());
        castPosition = new Vector3(aimCoords.X, aimCoords.Y, playerPoint.Z - 1.2);
    }

    API.sleep(2000);
    API.triggerServerEvent("tryFishing");
    isFishing = true;
    fishingBugOutTimer = new Date().getTime();
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
    // Page 1
    menu = resource.menu_builder.createMenu(3);
    let panel: Panel;
    let inputPanel: InputPanel;
    let textElement: TextElement;
    panel = menu.createPanel(0, 12, 4, 7, 1)
    panel.Header = true;
    panel.MainBackgroundColor(0, 0, 0, 175);
    textElement = panel.addText("Fishing");
    textElement.Centered = true;
    textElement.FontScale = 0.6;
    textElement.Font = 7;
    panel = menu.createPanel(0, 12, 5, 7, 2)
    panel.MainBackgroundColor(0, 0, 0, 160);
    textElement = panel.addText("Press ~b~F ~w~when your progress bar is maxed out.");
    textElement.Centered = true;
    textElement.FontScale = 0.45;
    progressBar = panel.addProgressBar(12, 6, 7, 1, 1);
    progressBar.setColor(0, 153, 255);
    progressBar.Alpha = 255;
    progressBar.DrawText = true;
    // Page 2
    panel = menu.createPanel(1, 12, 4, 7, 3)
    panel.MainBackgroundColor(0, 0, 0, 175);
    panel.Header = true;
    textElement = panel.addText("Fishing");
    textElement.Centered = true;
    textElement.Font = 7;
    textElement.FontScale = 0.6;
    wordPanel = panel.addText("Type: " + word);
    wordPanel.Centered = true;
    wordPanel.FontScale = 0.5;
    inputBox = panel.addInput(0, 2, 7, 1);
    menu.Ready = true;
    //menu.DisableOverlays(true);
}
function wordMode() {
    resource.menu_builder.setPage(1);
    inputBox.Selected = true;
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
    //API.showColorShard("~y~Perfect Catch!", "~y~Well Done!", 2, 12, 3000);
    resource.menu_builder.createPlayerTextNotification("~b~+1 Fish");
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
    buoy = API.createMarker(Enums.MarkerType.DebugSphere, castPosition, new Vector3(), new Vector3(), new Vector3(0.1, 0.1, 0.1), 255, 0, 0, 255);
}
function noFishHere() {
    let notification = resource.menu_builder.createNotification(0, "Woops, doesn't seem to be any fish around here.", 2000);
    notification.setColor(255, 255, 255);
}