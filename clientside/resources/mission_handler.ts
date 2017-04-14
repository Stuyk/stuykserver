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
var inAnimation = false;
var opposition;
var target = null;
var targets = [];
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
        case "Mission_Position": // Always call position before type.
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
        case "Mission_Task_Bar_Allies":
            if (opposition == "Ally") {
                taskHealth = API.getEntitySyncedData(entity, "Mission_Task_Bar_Allies");
                //API.sendChatMessage("New Sync Data" + string + " " + taskHealth);
            }
            break;
        case "Mission_Task_Bar_Enemies":
            if (opposition == "Enemy") {
                taskHealth = API.getEntitySyncedData(entity, "Mission_Task_Bar_Enemies");
                //API.sendChatMessage("New Sync Data" + string + " " + taskHealth);
            }
            break;
        case "Mission_Started":
            var check = API.getEntitySyncedData(entity, "Mission_Started");
            //API.sendChatMessage("New Sync Data" + string + " " + check);
            if (check) {
                started = true;
                break;
            } else {
                started = false;
                break;
            }
        default:
            break;
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
    API.drawText("" + positionType, 50, 200, 0.5, 0, 0, 0, 255, 4, 0, false, false, 500);
    // Waypoint
    var playerPos = API.getEntityPosition(API.getLocalPlayer());
    if (positionType == "Waypoint" && currentTaskPosition !== null && currentTaskPosition.DistanceTo(playerPos) < 2) {
        positionType = null;
        goToNextTaskInherited();
        return;
    }
    // Destroy
    if (positionType == "Destroy") {
        if (taskHealth !== null && playerPos.DistanceTo(currentTaskPosition) <= 10) {
            drawProgressBar();
        }
        if (API.isControlJustPressed(Enums.Controls.Attack)) {
            if  (Math.round(new Date().getTime()) < cooldown) {
                return;
            }
            if (playerPos.DistanceTo(currentTaskPosition) < 3) {
                var aimPos = API.getPlayerAimCoords(API.getLocalPlayer());
                if (aimPos.DistanceTo(currentTaskPosition) < 1) {
                    cooldown = (new Date().getTime() + 1500)
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
    if (positionType == "DestroyVehicle") {
        if (target != null && API.doesEntityExist(target)) {
            currentTaskPosition = API.getEntityPosition(target);
        }
        
        if (taskHealth !== null && playerPos.DistanceTo(currentTaskPosition) <= 10) {
            drawProgressBar();
        }
        if (API.isControlJustPressed(Enums.Controls.Attack)) {
            if (Math.round(new Date().getTime()) < cooldown) {
                return;
            }
            if (playerPos.DistanceTo(currentTaskPosition) < 3) {
                var aimPos = API.getPlayerAimCoords(API.getLocalPlayer());
                if (aimPos.DistanceTo(currentTaskPosition) < 1) {
                    cooldown = (new Date().getTime() + 1200)
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
    if (positionType == "Capture") {
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
            } else {
                cooldown = (new Date().getTime() + 2000);
                taskHealth += 5;
                API.triggerServerEvent("updateMissionBar", taskHealth);
                return;
            }
        }
    }
    // Hack
    if (positionType == "Hack") {
        if (playerPos.DistanceTo(currentTaskPosition) <= 10) {
            drawProgressBar();
        }

        if (playerPos.DistanceTo(currentTaskPosition) <= 3) {
            if (API.isControlJustPressed(Enums.Controls.Enter)) {
                API.triggerServerEvent("playAnimation", "move_crouch_proto", "idle");
            }

            if (API.isControlJustReleased(Enums.Controls.Enter)) {
                API.triggerServerEvent("stopAnimation");
            }

            if (API.isControlPressed(Enums.Controls.Enter)) {
                var point = API.worldToScreenMantainRatio(currentTaskPosition);
                var newPoint = Point.Round(point);
                API.drawText("~w~Hacking...", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);

                if (Math.round(new Date().getTime()) < cooldown) {
                    return;
                } else {
                    cooldown = (new Date().getTime() + 5000);
                    taskHealth += 50;
                    API.playSoundFrontEnd("Pin_Good", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                    API.triggerServerEvent("updateMissionBar", taskHealth);
                }
            } else {
                var point = API.worldToScreenMantainRatio(currentTaskPosition);
                var newPoint = Point.Round(point);
                API.drawText("~w~Press [F] to ~b~hack.", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);
            }

            if (taskHealth >= 100) {
                positionType = null;
                goToNextTaskInherited();
            }
        }
    }
});
// =========================================================
// Used for Capture, Hack, Destroy
// =========================================================
function drawProgressBar() {
    var point = API.worldToScreenMantainRatio(currentTaskPosition);
    var newPoint = Point.Round(point);
    // taskHealth * 3 (100 = 300);
    API.drawRectangle(newPoint.X - 170, newPoint.Y - 10, 320, 40, 0, 0, 0, 100);
    API.drawRectangle(newPoint.X - 160, newPoint.Y, (taskHealth * 3), 20, 0, 255, 0, 100);
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
    API.playSoundFrontEnd("Player_Collect", "DLC_PILOT_MP_HUD_SOUNDS");
    API.triggerServerEvent("shiftMissionObjectives", currentTaskPosition);
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
    cooldown = Math.round(new Date().getTime());
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
    marker = API.createMarker(Enums.MarkerType.VerticalCylinder, currentTaskPosition, new Vector3(), new Vector3(), new Vector3(2, 2, 2), 63, 137, 255, 150);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    API.displaySubtitle("Move to the next ~b~point.", 5000);
}
// "Destroy"
function drawDestroy() {
    taskHealth = 100;
    marker = API.createMarker(Enums.MarkerType.UpsideDownCone, currentTaskPosition, new Vector3(), new Vector3(), new Vector3(5, 5, 5), 63, 137, 255, 150);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    API.displaySubtitle("Destroy the ~b~target.", 5000);
}
// "Capture"
function drawCapture() {
    taskHealth = 0;
    marker = API.createMarker(Enums.MarkerType.VerticalCylinder, currentTaskPosition, new Vector3(), new Vector3(), new Vector3(5, 5, 5), 63, 137, 255, 150);
    API.setWaypoint(currentTaskPosition.X, currentTaskPosition.Y);
    blip = API.createBlip(currentTaskPosition);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    API.displaySubtitle("Capture the next ~b~point.", 5000);
}
// "Hack"
function drawHack() {
    taskHealth = 0;
    marker = API.createMarker(Enums.MarkerType.UpsideDownCone, currentTaskPosition, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 63, 137, 255, 150);
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