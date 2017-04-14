var allyPoints = 0;
var enemyPoints = 0;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var debug = false;
var started = false; // Whether or not the job has started.
var marker = null; // Marker Holder
var blip = null; // Blip Holder
var positions = []; // Objective Positions
var positionsTypes = []; // Position Types. SEE BELOW
var currentTaskPosition = null; // Current Position Task
var taskHealth = null;
var cooldown = Math.round(new Date().getTime());
var inAnimation = false;
// =========================================================
// ONUPDATE Function
// =========================================================
API.onUpdate.connect(function () {
    // If the game has started loop through.
    if (!started && currentTaskPosition === null) {
        return;
    }
    // Waypoint
    var playerPos = API.getEntityPosition(API.getLocalPlayer());
    if (positionsTypes[0] == "Waypoint" && currentTaskPosition !== null && currentTaskPosition.DistanceTo(playerPos) < 3) {
        API.deleteEntity(marker);
        currentTaskPosition = null;
        // If this was our last position / objective or not.
        if (positions.length > 1) {
            // Continue to next objective.
            positions.shift();
            positionsTypes.shift();
            shiftToNextTask();
            API.playSoundFrontEnd("Player_Collect", "DLC_PILOT_MP_HUD_SOUNDS");
            API.triggerServerEvent("updateMissionBar", null);
            API.triggerServerEvent("shiftMissionObjectives");
        }
        else {
            // Finish objective.
            finish();
            API.playSoundFrontEnd("WIN", "HUD_AWARDS");
        }
        return;
    }
    // Destroy
    if (positionsTypes[0] == "Destroy") {
        if (taskHealth !== null && playerPos.DistanceTo(positions[0]) <= 10) {
            drawProgressBar();
        }
        if (API.isControlJustPressed(24 /* Attack */)) {
            if (Math.round(new Date().getTime()) < cooldown) {
                if (debug) {
                    API.sendChatMessage("[DEBUG] COOLDOWN");
                }
                return;
            }
            if (debug) {
                API.sendChatMessage("[DEBUG] SWINGING");
            }
            if (playerPos.DistanceTo(positions[0]) < 3) {
                var aimPos = API.getPlayerAimCoords(API.getLocalPlayer());
                if (aimPos.DistanceTo(positions[0]) < 1) {
                    cooldown = (new Date().getTime() + 1500);
                    taskHealth -= 10;
                    if (taskHealth <= 0) {
                        if (positions.length > 1) {
                            positions.shift();
                            positionsTypes.shift();
                            shiftToNextTask();
                            API.playSoundFrontEnd("Player_Collect", "DLC_PILOT_MP_HUD_SOUNDS");
                            API.triggerServerEvent("updateMissionBar", null);
                            API.triggerServerEvent("shiftMissionObjectives");
                            return;
                        }
                        else {
                            finish();
                            API.playSoundFrontEnd("WIN", "HUD_AWARDS");
                            return;
                        }
                    }
                    API.triggerServerEvent("updateMissionBar", taskHealth);
                    return;
                }
            }
        }
        return;
    }
    // Capture
    if (positionsTypes[0] == "Capture") {
        if (playerPos.DistanceTo(positions[0]) <= 10) {
            drawProgressBar();
        }
        if (playerPos.DistanceTo(positions[0]) <= 4) {
            var point = API.worldToScreenMantainRatio(positions[0]);
            var newPoint = Point.Round(point);
            API.drawText("~w~Capturing...", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);
            if (taskHealth >= 100) {
                if (positions.length > 1) {
                    positions.shift();
                    positionsTypes.shift();
                    shiftToNextTask();
                    API.playSoundFrontEnd("Player_Collect", "DLC_PILOT_MP_HUD_SOUNDS");
                    API.triggerServerEvent("updateMissionBar", null);
                    API.triggerServerEvent("shiftMissionObjectives");
                    return;
                }
                else {
                    finish();
                    API.playSoundFrontEnd("WIN", "HUD_AWARDS");
                    return;
                }
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
    if (positionsTypes[0] == "Hack") {
        if (playerPos.DistanceTo(positions[0]) <= 10) {
            drawProgressBar();
        }
        if (playerPos.DistanceTo(positions[0]) <= 4) {
            if (API.isControlJustPressed(23 /* Enter */)) {
                API.triggerServerEvent("playAnimation", "move_crouch_proto", "idle");
            }
            if (API.isControlJustReleased(23 /* Enter */)) {
                API.triggerServerEvent("stopAnimation");
            }
            if (API.isControlPressed(23 /* Enter */)) {
                var point = API.worldToScreenMantainRatio(positions[0]);
                var newPoint = Point.Round(point);
                API.drawText("~w~Hacking...", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);
                if (Math.round(new Date().getTime()) < cooldown) {
                    return;
                }
                else {
                    cooldown = (new Date().getTime() + 5000);
                    taskHealth += 10;
                    API.playSoundFrontEnd("Pin_Good", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                    API.triggerServerEvent("updateMissionBar", taskHealth);
                }
            }
            else {
                var point = API.worldToScreenMantainRatio(positions[0]);
                var newPoint = Point.Round(point);
                API.drawText("~w~Press [F] to ~b~hack.", newPoint.X, newPoint.Y, 0.5, 0, 0, 0, 255, 4, 1, false, true, 600);
            }
            if (taskHealth >= 100) {
                if (positions.length > 1) {
                    positions.shift();
                    positionsTypes.shift();
                    shiftToNextTask();
                    API.playSoundFrontEnd("Hack_Success", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                    API.triggerServerEvent("stopAnimation");
                    API.triggerServerEvent("updateMissionBar", null);
                    API.triggerServerEvent("shiftMissionObjectives");
                    return;
                }
                else {
                    finish();
                    return;
                }
            }
        }
    }
});
// =========================================================
// Used for Capture, Hack, Destroy
// =========================================================
function drawProgressBar() {
    var point = API.worldToScreenMantainRatio(positions[0]);
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
    if (positions.length < 1) {
        return;
    }
    shiftToNextTask();
}
// Add positions to our job array.
function addJobPosition(position, type) {
    positions.push(position);
    positionsTypes.push(type);
}
// Shift next task
function goToNextTask(allPoints, enmPoints) {
    allyPoints = allPoints;
    enemyPoints = enmPoints;
    if (positions.length > 1) {
        positions.shift();
        positionsTypes.shift();
        shiftToNextTask();
        API.playSoundFrontEnd("Hack_Success", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
        API.triggerServerEvent("stopAnimation");
        return;
    }
    else {
        finish();
        API.playSoundFrontEnd("WIN", "HUD_AWARDS");
        API.triggerServerEvent("stopAnimation");
        return;
    }
}
// Sync Mission Bars Up
function syncMissionBar(amount, allPoints, enmPoints) {
    allyPoints = allPoints;
    enemyPoints = enmPoints;
    taskHealth = amount;
}
// Cleanup any garbage.
function cleanup() {
    if (marker !== null) {
        API.deleteEntity(marker);
        marker = null;
    }
    if (blip !== null) {
        API.deleteEntity(blip);
        blip = null;
    }
    positions = [];
    positionsTypes = [];
    taskHealth = null;
    currentTaskPosition = null;
    if (debug) {
        API.sendChatMessage("[DEBUG] CLEANED");
    }
}
// =========================================================
// Shift to the next Objective / Task.
// =========================================================
function shiftToNextTask() {
    if (debug) {
        API.sendChatMessage("[DEBUG] SHIFTED TO NEW OBJECTIVE");
    }
    if (positions[0] === null) {
        if (marker !== null) {
            cleanup();
        }
        return;
    }
    if (marker !== null) {
        API.deleteEntity(marker);
    }
    if (blip !== null) {
        API.deleteEntity(blip);
    }
    switch (positionsTypes[0]) {
        case "Waypoint":
            drawWaypoint();
            return;
        case "Destroy":
            drawDestroy();
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
function finish() {
    API.triggerServerEvent("stopAnimation");
    var opposition = API.getEntitySyncedData(API.getLocalPlayer(), "Opposition");
    switch (opposition) {
        case "Ally":
            if (allyPoints > enemyPoints) {
                API.showColorShard("Mission Complete", "Your team won the mission.", 2, 10, 5000);
            }
            else {
                API.showColorShard("Mission Failed", "Your team lost the mission.", 2, 10, 5000);
            }
            break;
        case "Enemy":
            if (enemyPoints > allyPoints) {
                API.showColorShard("Mission Complete", "Your team won the mission.", 2, 10, 5000);
            }
            else {
                API.showColorShard("Mission Failed", "Your team lost the mission.", 2, 10, 5000);
            }
            break;
    }
    API.triggerServerEvent("EndMission"); // Not implemented yet.
    cleanup();
    started = false;
}
// =========================================================
// OBJECT TYPES
// =========================================================
// "Waypoint"
function drawWaypoint() {
    if (debug) {
        API.sendChatMessage("[DEBUG] WAYPOINT OBJECTIVE ADDED");
    }
    marker = API.createMarker(1 /* VerticalCylinder */, positions[0], new Vector3(), new Vector3(), new Vector3(2, 2, 2), 63, 137, 255, 150);
    API.setWaypoint(positions[0].X, positions[0].Y);
    blip = API.createBlip(positions[0]);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    currentTaskPosition = positions[0];
    API.displaySubtitle("Move to the next ~b~point.", 5000);
}
// "Destroy"
function drawDestroy() {
    if (debug) {
        API.sendChatMessage("[DEBUG] DESTROY OBJECTIVE ADDED");
    }
    taskHealth = 100;
    API.displaySubtitle("Destroy the ~b~target.", 5000);
}
// "Capture"
function drawCapture() {
    if (debug) {
        API.sendChatMessage("[DEBUG] CAPTURE OBJECTIVE ADDED");
    }
    marker = API.createMarker(1 /* VerticalCylinder */, positions[0], new Vector3(), new Vector3(), new Vector3(5, 5, 5), 63, 137, 255, 150);
    blip = API.createBlip(positions[0]);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    currentTaskPosition = positions[0];
    API.displaySubtitle("Capture the next ~b~point.", 5000);
    taskHealth = 0;
}
// "Hack"
function drawHack() {
    if (debug) {
        API.sendChatMessage("[DEBUG] CAPTURE OBJECTIVE ADDED");
    }
    marker = API.createMarker(0 /* UpsideDownCone */, positions[0], new Vector3(), new Vector3(), new Vector3(1, 1, 1), 63, 137, 255, 150);
    blip = API.createBlip(positions[0]);
    API.setBlipSprite(blip, 1);
    API.setBlipColor(blip, 67);
    currentTaskPosition = positions[0];
    API.displaySubtitle("Hack the next ~b~point.", 5000);
    taskHealth = 0;
}
