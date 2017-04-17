var allyPoints = 0;
var enemyPoints = 0;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var debug = false;
var started = false; // Whether or not the job has started.
var marker = null; // Marker Holder
var blip = null; // Blip Holder
var positionType; // Current OBJECTIVE Type
var currentTaskPosition = null; // Current Position Task
var taskHealth = 25;
var cooldown = Math.round(new Date().getTime());
var timercooldown = Math.round(new Date().getTime());
var timer = 180000;
var inAnimation = false;
var opposition;
var target = null;
var targets = [];
var missionDelay = false;
var deliveryVehicle = null;
// =========================================================
// Entity Data Function
// =========================================================
API.onEntityDataChange.connect(function (entity, string, oldValue) {
    if (entity.Value !== API.getLocalPlayer().Value) {
        return;
    }
    switch (string) {
        case "Mission_Target":
            if (API.getEntitySyncedData(entity, "Mission_Target") === null) {
                return;
            }
            target = API.getEntitySyncedData(entity, "Mission_Target");
            break;
        case "Mission_Opposition":
            opposition = API.getEntitySyncedData(entity, "Mission_Opposition");
            //API.sendChatMessage("New Sync Data" + string + " " + opposition);
            break;
        case "Mission_Position":
            cleanup();
            currentTaskPosition = API.getEntitySyncedData(entity, "Mission_Position");
            //API.sendChatMessage("New Sync Data " + string + " " + currentTaskPosition);
            break;
        case "Mission_Type":
            positionType = API.getEntitySyncedData(entity, "Mission_Type");
            shiftToNextTask();
            //API.sendChatMessage("New Sync Data" + string + " " + positionType);
            break;
        case "Mission_Points_Allies":
            allyPoints = API.getEntitySyncedData(entity, "Mission_Points_Allies");
            //API.sendChatMessage("New Sync Data" + string + " " + allyPoints);
            break;
        case "Mission_Points_Enemies":
            enemyPoints = API.getEntitySyncedData(entity, "Mission_Points_Enemies");
            //API.sendChatMessage("New Sync Data" + string + " " + enemyPoints);
            break;
        case "Mission_Timer":
            timer = Number(API.getEntitySyncedData(entity, "Mission_Timer"));
            break;
        case "Mission_Task_Bar_Allies":
            if (opposition == "Ally") {
                taskHealth = API.getEntitySyncedData(entity, "Mission_Task_Bar_Allies");
            }
            break;
        case "Mission_Task_Bar_Enemies":
            if (opposition == "Enemy") {
                taskHealth = API.getEntitySyncedData(entity, "Mission_Task_Bar_Enemies");
            }
            break;
        case "Mission_Started":
            started = API.getEntitySyncedData(entity, "Mission_Started");
            if (!started) {
                fullCleanup();
            }
            break;
        case "Mission_Delay":
            missionDelay = API.getEntitySyncedData(entity, "Mission_Delay");
            break;
        default:
            break;
    }
});
// =========================================================
// ONVEHICLE Function
// =========================================================
API.onPlayerEnterVehicle.connect(function (entity) {
    if (!started && currentTaskPosition === null) {
        return;
    }
    if (positionType === "TakeVehicle") {
        if (!API.doesEntityExist(target)) {
            return;
        }
        if (entity.Value !== target.Value) {
            return;
        }
        deliveryVehicle = target;
        positionType = null;
        goToNextTaskInherited();
    }
});
// =========================================================
// ONUPDATE Function
// =========================================================
API.onUpdate.connect(function () {
    // If the game has started loop through.
    if (!started && currentTaskPosition === null) {
        return;
    }
    if (missionDelay) {
        return;
    }
    if (API.isChatOpen()) {
        return;
    }
    // Waypoint
    var playerPos = API.getEntityPosition(API.getLocalPlayer());
    if (positionType === "Waypoint" && currentTaskPosition !== null && currentTaskPosition.DistanceTo(playerPos) <= 2) {
        positionType = null;
        goToNextTaskInherited();
        return;
    }
    // Destroy
    if (positionType === "Destroy") {
        if (taskHealth !== null && playerPos.DistanceTo(currentTaskPosition) <= 10) {
            drawProgressBar();
        }
        if (API.isControlJustPressed(24 /* Attack */)) {
            if (Math.round(new Date().getTime()) < cooldown) {
                return;
            }
            if (playerPos.DistanceTo(currentTaskPosition) < 3) {
                var aimPos = API.getPlayerAimCoords(API.getLocalPlayer());
                if (aimPos.DistanceTo(currentTaskPosition) < 1.5) {
                    cooldown = (new Date().getTime() + 1500);
                    taskHealth -= 10;
                    if (taskHealth <= 0) {
                        positionType = null;
                        goToNextTaskInherited();
                    }
                    API.triggerServerEvent("updateMissionBar", taskHealth);
                    return;
                }
            }
        }
        return;
    }
    // Destroy Vehicle
    if (positionType === "DestroyVehicle") {
        if (target !== null && API.doesEntityExist(target)) {
            currentTaskPosition = API.getEntityPosition(target);
        }
        if (taskHealth !== null && playerPos.DistanceTo(currentTaskPosition) <= 10) {
            drawProgressBar();
        }
        if (API.isControlJustPressed(24 /* Attack */)) {
            if (Math.round(new Date().getTime()) < cooldown) {
                return;
            }
            if (playerPos.DistanceTo(currentTaskPosition) < 3) {
                var aimPos = API.getPlayerAimCoords(API.getLocalPlayer());
                if (aimPos.DistanceTo(currentTaskPosition) < 1.5) {
                    cooldown = (new Date().getTime() + 1200);
                    taskHealth -= 5;
                    if (taskHealth <= 0) {
                        positionType = null;
                        goToNextTaskInherited();
                    }
                    API.triggerServerEvent("updateMissionBar", taskHealth);
                    return;
                }
            }
        }
        return;
    }
    // Capture
    if (positionType === "Capture") {
        if (playerPos.DistanceTo(currentTaskPosition) <= 10) {
            drawProgressBar();
        }
        if (playerPos.DistanceTo(currentTaskPosition) <= 4) {
            var point = API.worldToScreenMantainRatio(currentTaskPosition);
            var newPoint = Point.Round(point);
            API.drawText("~w~Capturing...", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);
            if (taskHealth >= 100) {
                positionType = null;
                goToNextTaskInherited();
            }
            if (Math.round(new Date().getTime()) < cooldown) {
                return;
            }
            else {
                cooldown = (new Date().getTime() + 2000);
                taskHealth += 5;
                API.triggerServerEvent("updateMissionBar", taskHealth);
                return;
            }
        }
    }
    // Hack
    if (positionType === "Hack") {
        if (playerPos.DistanceTo(currentTaskPosition) <= 10) {
            drawProgressBar();
        }
        if (playerPos.DistanceTo(currentTaskPosition) <= 3) {
            if (API.isControlJustPressed(23 /* Enter */)) {
                API.triggerServerEvent("playAnimation", "move_crouch_proto", "idle");
            }
            if (API.isControlJustReleased(23 /* Enter */)) {
                API.triggerServerEvent("stopAnimation");
            }
            if (API.isControlPressed(23 /* Enter */)) {
                var point = API.worldToScreenMantainRatio(currentTaskPosition);
                var newPoint = Point.Round(point);
                API.drawText("~w~Hacking...", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);
                if (Math.round(new Date().getTime()) < cooldown) {
                    return;
                }
                else {
                    cooldown = (new Date().getTime() + 5000);
                    taskHealth += 5;
                    API.playSoundFrontEnd("Pin_Good", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                    API.triggerServerEvent("updateMissionBar", taskHealth);
                }
            }
            else {
                var point = API.worldToScreenMantainRatio(currentTaskPosition);
                var newPoint = Point.Round(point);
                API.drawText("~w~Hold [F] to ~b~hack.", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);
            }
            if (taskHealth >= 100) {
                positionType = null;
                goToNextTaskInherited();
            }
        }
    }
    // Investigate
    if (positionType === "Investigate" && currentTaskPosition !== null && currentTaskPosition.DistanceTo(playerPos) < 20) {
        positionType = null;
        goToNextTaskInherited();
        return;
    }
    // DisableBomb
    if (positionType == "DisableBomb" && currentTaskPosition !== null && currentTaskPosition.DistanceTo(playerPos) < 20) {
        if (currentTaskPosition.DistanceTo(playerPos) <= 3) {
            drawTimer();
            drawProgressBar();
            if (API.isControlJustPressed(23 /* Enter */)) {
                API.triggerServerEvent("playAnimation", "move_crouch_proto", "idle");
            }
            if (API.isControlJustReleased(23 /* Enter */)) {
                API.triggerServerEvent("stopAnimation");
            }
            if (API.isControlPressed(23 /* Enter */)) {
                var point = API.worldToScreenMantainRatio(currentTaskPosition);
                var newPoint = Point.Round(point);
                API.drawText("~w~Disarming...", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);
                if (Math.round(new Date().getTime()) < cooldown) {
                }
                else {
                    cooldown = (new Date().getTime() + 2000);
                    taskHealth += 2;
                    API.playSoundFrontEnd("Pin_Bad", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                    API.triggerServerEvent("updateMissionBar", taskHealth);
                }
            }
            else {
                var point = API.worldToScreenMantainRatio(currentTaskPosition);
                var newPoint = Point.Round(point);
                API.drawText("~w~Hold [F] to ~b~disarm.", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);
            }
            if (taskHealth >= 100) {
                positionType = null;
                goToNextTaskInherited();
            }
        }
    }
    // TakeVehicle
    if (positionType == "TakeVehicle" && currentTaskPosition !== null && currentTaskPosition.DistanceTo(playerPos) <= 5) {
        drawTimer();
        if (API.doesEntityExist(target)) {
            if (API.getVehicleLocked(target)) {
                API.setVehicleLocked(target, false);
            }
        }
    }
    // DeliverVehicle
    if (positionType === "DeliverVehicle" && currentTaskPosition !== null && currentTaskPosition.DistanceTo(playerPos) <= 2) {
        if (API.getPlayerVehicle(API.getLocalPlayer()).Value !== deliveryVehicle.Value) {
            return;
        }
        positionType = null;
        goToNextTaskInherited();
        return;
    }
});
// =========================================================
// Draw Functions
// =========================================================
function drawProgressBar() {
    var point = API.worldToScreenMantainRatio(currentTaskPosition);
    var newPoint = Point.Round(point);
    // taskHealth * 3 (100 = 300);
    API.drawRectangle(newPoint.X - 170, newPoint.Y - 10, 320, 40, 0, 0, 0, 100);
    API.drawRectangle(newPoint.X - 160, newPoint.Y, (taskHealth * 3), 20, 0, 255, 0, 100);
}
function drawTimer() {
    var point = API.worldToScreenMantainRatio(currentTaskPosition);
    var newPoint = Point.Round(point);
    API.drawText("" + (timer / 1000), newPoint.X, newPoint.Y - 20, 0.5, 255, 0, 0, 255, 4, 1, false, true, 500);
}
// =========================================================
// Chat Message Command, ONLY FOR DEBUG
// =========================================================
// Start your job
function startMission() {
    if (currentTaskPosition == null) {
        return;
    }
    shiftToNextTask();
}
// Function go to next task, local side.
function goToNextTaskInherited() {
    API.triggerServerEvent("shiftMissionObjectives", currentTaskPosition);
    API.playSoundFrontEnd("Player_Collect", "DLC_PILOT_MP_HUD_SOUNDS");
    API.triggerServerEvent("stopAnimation");
}
// Cleanup any garbage.
function cleanup() {
    taskHealth = 25;
    if (marker !== null) {
        API.deleteEntity(marker);
        marker = null;
    }
    if (blip !== null) {
        API.deleteEntity(blip);
        blip = null;
    }
}
// =========================================================
// Shift to the next Objective / Task.
// =========================================================
function shiftToNextTask() {
    API.triggerServerEvent("stopAnimation");
    cooldown = Math.round(new Date().getTime());
    cleanup();
    switch (positionType) {
        case "Waypoint":
            drawWaypoint();
            return;
        case "Destroy":
            drawDestroy();
            return;
        case "DestroyVehicle":
            drawDestroyVehicle();
            return;
        case "Capture":
            drawCapture();
            return;
        case "Hack":
            drawHack();
            return;
        case "Investigate":
            drawInvestigate();
            return;
        case "DisableBomb":
            drawDisableBomb();
            return;
        case "TakeVehicle":
            drawTakeVehicle();
            return;
        case "DeliverVehicle":
            drawDeliverVehicle();
            return;
    }
}
// =========================================================
// Finish a Job
// =========================================================
function missionWinScreen() {
    cleanup();
    API.playSoundFrontEnd("SCREEN_FLASH", "CELEBRATION_SOUNDSET");
    API.showColorShard("Mission Success", "Great job and good luck on the next one.", 2, 18, 5000);
    fullCleanup();
}
function missionLoseScreen() {
    cleanup();
    API.playSoundFrontEnd("ScreenFlash", "MissionFailedSounds");
    API.showColorShard("Mission Failed", "Next time, try harder.", 2, 6, 5000);
    fullCleanup();
}
function missionTieScreen() {
    cleanup();
    API.playSoundFrontEnd("ScreenFlash", "MissionFailedSounds");
    API.showColorShard("Mission Tied", "Oops. You did a terrible job.", 2, 6, 5000);
    fullCleanup();
}
function fullCleanup() {
    cleanup();
    positionType = null;
    currentTaskPosition = null;
    taskHealth = null;
    started = false;
    allyPoints = 0;
    enemyPoints = 0;
    opposition = null;
}
// =========================================================
// OBJECT TYPES
// =========================================================
// "Waypoint"
function drawWaypoint() {
    marker = API.createMarker(1 /* VerticalCylinder */, currentTaskPosition, new Vector3(), new Vector3(), new Vector3(2, 2, 2), 63, 137, 255, 150);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    API.displaySubtitle("Move to the next ~b~point.", 5000);
}
// "Destroy"
function drawDestroy() {
    taskHealth = 100;
    marker = API.createMarker(0 /* UpsideDownCone */, currentTaskPosition, new Vector3(), new Vector3(), new Vector3(5, 5, 5), 63, 137, 255, 150);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    API.displaySubtitle("Destroy the ~b~target.", 5000);
}
// "Capture"
function drawCapture() {
    taskHealth = 0;
    marker = API.createMarker(1 /* VerticalCylinder */, currentTaskPosition, new Vector3(), new Vector3(), new Vector3(5, 5, 5), 63, 137, 255, 150);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    API.displaySubtitle("Capture the next ~b~point.", 5000);
}
// "Hack"
function drawHack() {
    taskHealth = 0;
    marker = API.createMarker(0 /* UpsideDownCone */, currentTaskPosition, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 63, 137, 255, 150);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    API.displaySubtitle("Hack the next ~b~point.", 5000);
}
// "DestroyVehicle"
function drawDestroyVehicle() {
    taskHealth = 100;
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    API.displaySubtitle("Destroy the ~r~targets ~w~vehicle.", 5000);
}
// "Investigate"
function drawInvestigate() {
    API.displaySubtitle("Investigate the ~b~location.", 5000);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
}
// "DisableBomb"
function drawDisableBomb() {
    cooldown = new Date().getTime();
    timercooldown = new Date().getTime() + 1000;
    taskHealth = 0;
    marker = API.createMarker(0 /* UpsideDownCone */, currentTaskPosition.Add(new Vector3(0, 0, 1.5)), new Vector3(), new Vector3(), new Vector3(0.2, 0.2, 0.7), 63, 137, 255, 150);
    API.displaySubtitle("Defuse the ~b~bomb.", 5000);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    timer = 180000;
}
// "TakeVehicle"
function drawTakeVehicle() {
    marker = API.createMarker(0 /* UpsideDownCone */, currentTaskPosition.Add(new Vector3(0, 0, 5)), new Vector3(), new Vector3(), new Vector3(0.2, 0.2, 0.7), 63, 137, 255, 150);
    API.displaySubtitle("Take the ~g~vehicle.", 5000);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 69);
    timer = 180000;
}
// "DeliverVehicle"
function drawDeliverVehicle() {
    marker = API.createMarker(1 /* VerticalCylinder */, currentTaskPosition, new Vector3(), new Vector3(), new Vector3(2, 2, 2), 63, 137, 255, 150);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    API.displaySubtitle("Deliver the vehicle to the ~b~area~w~.", 5000);
}
