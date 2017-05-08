var blip;
var marker;
// Options
var r = 192;
var g = 217;
var b = 97;
var x = API.getScreenResolutionMantainRatio().Width;
var y = API.getScreenResolutionMantainRatio().Height;
// Mission Variables
var task = null;
var taskType = null; // Used to determine what the current task is.
var taskPosition = null; // Used as a point of reference for all objective types.
var taskPoints = 0; // Used as a display.
var taskTimer = null; // Used as a display.
var taskBar = null; // Used for the progress bar.
var taskMarkers = []; // Used for Markers.
var taskBlips = []; // Used for Blips.
var taskTargets = null; // For Vector3s
var isPlayerInMission = false; // Is the player currently in a mission?
// Mission Types
var taskTypes = ["Waypoint", "Capture", "Destory", "Hack", "DestroyVehicle", "Investigate", "DisableBomb", "TakeVehicle", "DeliverVehicle"];
// Mission Entity Data
API.onEntityDataChange.connect(function (entity, string, oldValue) {
    // Is our player in a mission?
    if (!isPlayerInMission) {
        return;
    }
    // Does the entity changed belong to the player?
    if (entity.Value !== API.getLocalPlayer().Value) {
        return;
    }
    // Update our local data.
    switch (string) {
        case "Mission_Position":
            taskPosition = API.getEntitySyncedData(entity, "Mission_Position");
            return;
        case "Mission_Type":
            taskType = API.getEntitySyncedData(entity, "Mission_Type");
            return;
        case "Mission_Points":
            taskPoints = API.getEntitySyncedData(entity, "Mission_Points");
            return;
        case "Mission_Timer":
            timer = Number(API.getEntitySyncedData(entity, "Mission_Timer"));
            return;
        case "Mission_Started":
            isPlayerInMission = API.getEntitySyncedData(entity, "Mission_Started");
            return;
        case "Mission_Taskbar":
            taskBar = API.getEntitySyncedData(entity, "Mission_Bar");
            return;
    }
});
API.onUpdate.connect(function () {
    if (task === null) {
        return;
    }
    if (!task.getStatus()) {
        return;
    }
    task.draw();
    task.update();
});
// Mission Task Updates
function requestTask() {
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
// Removes Blips, Markers, etc.
function cleanupMarks() {
    if (taskBlips !== null && taskBlips.length > 0) {
        for (var i = 0; i < taskBlips.length; i++) {
            API.deleteEntity(taskBlips[i]);
        }
    }
    if (taskMarkers !== null && taskMarkers.length > 0) {
        for (var i = 0; i < taskMarkers.length; i++) {
            API.deleteEntity(taskMarkers[i]);
        }
    }
}
// Kills the mission entirely.
function cleanupAllData() {
    isPlayerInMission = false;
    task = null;
    taskTimer = null;
    cleanupMarks();
}
// Move to next mission
function switchTask() {
    task = null;
}
// Finish a Mission
function taskWinScreen() {
    API.playSoundFrontEnd("SCREEN_FLASH", "CELEBRATION_SOUNDSET");
    API.showColorShard("Mission Complete", "", 2, 18, 5000);
    cleanupAllData();
}
function taskLoseScreen() {
    cleanup();
    API.playSoundFrontEnd("ScreenFlash", "MissionFailedSounds");
    API.showColorShard("Mission Failed", "", 2, 6, 5000);
    fullCleanup();
}
function taskTieScreen() {
    cleanup();
    API.playSoundFrontEnd("ScreenFlash", "MissionFailedSounds");
    API.showColorShard("Mission Tied", "", 2, 6, 5000);
    fullCleanup();
}
// Task Creation / Setting Functions
function taskCreate(pos) {
    task = new Task(pos);
    return task;
}
// Task Class
class Task {
    // Our main constructor
    constructor(pos) {
        this._taskPosition = pos;
        this._taskType = null;
        this._taskStatus = false;
        this._taskText = "";
    }
    // Start the task?
    setStatus(value) {
        this._taskStatus = value;
    }
    getStatus() {
        return this._taskStatus;
    }
    // Set Task Text
    setText(value) {
        this._taskText = value;
    }
    // Send Notification
    pushTaskNotification() {
        let notification = resource.menu_builder.createNotification(0, this._taskText, 2000);
        notification.setColor(r, g, b);
        notification.setTextScale(0.4);
    }
    // Set the task health.
    setHealth(value) {
        this._taskHealth = value;
    }
    getHealth() {
        return this._taskHealth;
    }
    // Set the task bar.
    setTaskBar(value) {
        this._taskBar = value;
    }
    getTaskBar() {
        return this._taskBar;
    }
    // Set the task timer.
    setTaskTimer(value) {
        this._taskTimer = value;
    }
    getTaskTimer(value) {
        return this._taskTimer;
    }
    // Set the task type.
    setTaskType(value) {
        this._taskType = value;
    }
    getTaskType() {
        return this._taskType;
    }
    runTask() {
        // If our task isn't ready, don't run it.
        if (!this._taskStatus) {
            return;
        }
        this.setupBlip();
        this.setupMarker();
        this.pushTaskNotification();
        API.sendChatMessage("Trying");
    }
    setupBlip() {
        if (this._taskType === null) {
            API.sendNotification("Error: Task Type not set.");
            return;
        }
        switch (this._taskType) {
            case 0:
                createBlip(this._taskPosition, 162);
                break;
            case 1:
                createBlip(this._taskPosition, 164);
                break;
            case 2:
                createBlip(this._taskPosition, 38);
                break;
            case 3:
                createBlip(this._taskPosition, 354);
                break;
            case 4:
                createBlip(this._taskPosition, 380);
                break;
            case 5:
                createBlip(this._taskPosition, 184);
                break;
            case 6:
                createBlip(this._taskPosition, 486);
                break;
            case 7:
                createBlip(this._taskPosition, 490);
                break;
            case 8:
                createBlip(this._taskPosition, 50);
                break;
        }
    }
    setupMarker() {
        if (this._taskType === null) {
            API.sendNotification("Error: Task Type not set.");
            return;
        }
        switch (this._taskType) {
            case 0:
                createMarker(this._taskPosition, 1 /* VerticalCylinder */, new Vector3(5, 5, 0.5));
                break;
            case 1:
                createMarker(this._taskPosition, 1 /* VerticalCylinder */, new Vector3(5, 5, 0.5));
                break;
            case 2:
                createMarker(this._taskPosition.Add(new Vector3(0, 0, 2)), 0 /* UpsideDownCone */, new Vector3(0.5, 0.5, 1));
                break;
            case 3:
                createMarker(this._taskPosition.Add(new Vector3(0, 0, 2)), 0 /* UpsideDownCone */, new Vector3(0.5, 0.5, 1));
                break;
            case 4:
                createMarker(this._taskPosition.Add(new Vector3(0, 0, 5)), 0 /* UpsideDownCone */, new Vector3(0.5, 0.5, 1));
                break;
            case 5:
                createMarker(this._taskPosition, 23 /* HorizontalCircleFlat */, new Vector3(10, 10, 0.5));
                break;
            case 6:
                createMarker(this._taskPosition.Add(new Vector3(0, 0, 2)), 0 /* UpsideDownCone */, new Vector3(0.5, 0.5, 1));
                break;
            case 7:
                createMarker(this._taskPosition.Add(new Vector3(0, 0, 2)), 0 /* UpsideDownCone */, new Vector3(0.5, 0.5, 1));
                break;
            case 8:
                createMarker(this._taskPosition, 23 /* HorizontalCircleFlat */, new Vector3(10, 10, 0.5));
                break;
        }
    }
    draw() {
        if (this._taskType === null) {
            return;
        }
        switch (this._taskType) {
            case 1:
                this.drawTaskBar();
                break;
            case 2:
                this.drawHealthBar();
                break;
            case 3:
                this.drawTaskBar();
                break;
            case 4:
                this.drawHealthBar();
                break;
            case 6:
                this.drawTaskBar();
                break;
        }
    }
    // Main task thread.
    update() {
        if (this._taskType === null) {
            return;
        }
        if (this._taskPosition === null) {
            return;
        }
        let playerPosition = API.getEntityPosition(API.getLocalPlayer());
        // Waypoint
        if (this._taskType === 0) {
            if (playerPosition.DistanceTo(this._taskPosition) <= 2.5) {
                this._taskType = null;
                cleanupMarks();
                switchTask();
                objectiveComplete();
            }
        }
    }
    // Task Bar
    drawTaskBar() {
        if (API.getEntityPosition(API.getLocalPlayer()).DistanceTo(this._taskPosition) > 15) {
            return;
        }
        let point = API.worldToScreenMantainRatio(this._taskPosition.Add(new Vector3(0, 0, 4)));
        let newPoint = Point.Round(point);
        API.drawRectangle(newPoint.X - 200, newPoint.Y - 30, 400, 25, 0, 0, 0, 175);
        API.drawRectangle(newPoint.X - 195, newPoint.Y - 25, (this._taskBar * 4) - 5, 15, r, g, b, 100);
        API.drawText(this._taskBar.toString() + "%", newPoint.X, newPoint.Y - 30, 0.3, 255, 255, 255, 255, 4, 1, false, true, 500);
    }
    // Health Bar
    drawHealthBar() {
        if (API.getEntityPosition(API.getLocalPlayer()).DistanceTo(this._taskPosition) > 15) {
            return;
        }
        let point = API.worldToScreenMantainRatio(this._taskPosition.Add(new Vector3(0, 0, 4)));
        let newPoint = Point.Round(point);
        API.drawRectangle(newPoint.X - 200, newPoint.Y - 30, 400, 25, 0, 0, 0, 175);
        API.drawRectangle(newPoint.X - 195, newPoint.Y - 25, (this._taskHealth * 4) - 5, 15, r, g, b, 100);
        API.drawText(this._taskHealth.toString(), newPoint.X, newPoint.Y - 30, 0.3, 255, 255, 255, 255, 4, 1, false, true, 500);
    }
}
// Create Blip
function createBlip(position, sprite) {
    blip = API.createBlip(position);
    API.setBlipSprite(blip, sprite);
    API.setBlipColor(blip, 24);
    taskBlips.push(blip);
}
// Create Marker
function createMarker(position, type, size) {
    marker = API.createMarker(type, position, new Vector3(), new Vector3(), size, r, g, b, 125);
    taskMarkers.push(marker);
    API.sendChatMessage("" + position.ToString());
}
function objectiveComplete() {
    API.playSoundFrontEnd("Player_Collect", "DLC_PILOT_MP_HUD_SOUNDS");
    var notify = resource.menu_builder.createPlayerTextNotification("Objective Complete");
    notify.setColor(r, g, b);
}
