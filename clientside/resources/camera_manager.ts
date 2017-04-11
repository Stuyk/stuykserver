var activeCameras = [];
var lastCamera = null;
var activeCameraBlips = [];
var activeCameraMarkers = [];
var directorMode = false;
var defaultSpeed = 5000; // Used for general Camera Speed when Interopolating.
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var currentCamera = null;
var markersHidden = false;

API.onKeyDown.connect(function (player, e) {
    if (API.isChatOpen()) {
        return;
    }

    // Toggle Director Mode / On & Off
    if (e.KeyCode == Keys.F12) {
        if (directorMode == false) {
            directorMode = true;
            API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
            return;
        }
        else {
            directorMode = false;
            API.playSoundFrontEnd("CANCEL", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            cameraKillActive();
            clearCameraBlips();
            return;
        }
    }

    // Only when Director Mode is on.
    if (directorMode == false) {
        return;
    }

    // GO TO Camera Functions => 0 - 9
    if (e.KeyCode == Keys.D0) {
        cameraGoto(0);
        return;
    }

    if (e.KeyCode == Keys.D1) {
        cameraGoto(1);
        return;
    }

    if (e.KeyCode == Keys.D2) {
        cameraGoto(2);
        return;
    }

    if (e.KeyCode == Keys.D3) {
        cameraGoto(3);
        return;
    }

    if (e.KeyCode == Keys.D4) {
        cameraGoto(4);
        return;
    }

    if (e.KeyCode == Keys.D5) {
        cameraGoto(5);
        return;
    }

    if (e.KeyCode == Keys.D6) {
        cameraGoto(7);
        return;
    }

    if (e.KeyCode == Keys.D7) {
        cameraGoto(7);
        return;
    }

    if (e.KeyCode == Keys.D8) {
        cameraGoto(8);
        return;
    }

    if (e.KeyCode == Keys.D9) {
        cameraGoto(9);
        return;
    }

    // Setup a new Camera = Tab
    if (e.KeyCode == Keys.Tab) {
        cameraSetup(API.getEntityPosition(player), API.getEntityRotation(API.getLocalPlayer()));
        return;
    }

    // Kill Camera = Backspace
    if (e.KeyCode == Keys.Back) {
        cameraKillActive();
        return;
    }

    // Go to last Camera = ~
    if (e.KeyCode == Keys.OemTilde) {
        cameraLastCamera();
        return;
    }

    // Decrease Camera Speed by 500 = ,
    if (e.KeyCode == Keys.OemComma) {
        defaultSpeed -= 500;
        return;
    }

    // Increase Camera Speed by 500 = .
    if (e.KeyCode == Keys.OemPeriod) {
        defaultSpeed += 500;
        return;
    }

    // Move between all camera points setup. = F11
    if (e.KeyCode == Keys.F11) {
        cameraAnimateAllPoints();
        return;
    }

    // Delete last Camera added. = Del
    if (e.KeyCode == Keys.Delete) {
        var number = activeCameraBlips.length - 1;
        var numberMarker = activeCameraMarkers.length - 1;

        if (numberMarker == -1) {
            return;
        }

        if (number == -1) {
            return;
        }

        API.sendNotification("~b~Removing Camera at: ~w~" + number);
        API.deleteEntity(activeCameraBlips[number]);
        API.deleteEntity(activeCameraMarkers[numberMarker]);
        activeCameraBlips.pop();
        activeCameraMarkers.pop();
        activeCameras.pop();
        API.setActiveCamera(null);
        API.playSoundFrontEnd("CANCEL", "HUD_FRONTEND_DEFAULT_SOUNDSET");
        return;
    }

    if (e.KeyCode == Keys.F9) {
        if (markersHidden == true) {
            for (var i = 0; i < activeCameraMarkers.length; i++) {
                API.setMarkerColor(activeCameraMarkers[i], 50, 0, 0, 255);
            }
            markersHidden = false;
            return;
        }
        else {
            for (var i = 0; i < activeCameraMarkers.length; i++) {
                API.setMarkerColor(activeCameraMarkers[i], 0, 0, 0, 255);
            }
            markersHidden = true;
        }
    }

    // Active Camera Functions
    if (API.getActiveCamera() != null) {
        // Increase FOV = LShift + 
        if (e.KeyCode == Keys.OemPlus && e.Shift) {
            var fov = API.getCameraFov(API.getActiveCamera());
            fov += 5;
            API.setCameraFov(API.getActiveCamera(), fov);
            return;
        }
        // Decrease FOV = LShift -
        if (e.KeyCode == Keys.OemMinus && e.Shift) {
            var fov = API.getCameraFov(API.getActiveCamera());
            fov -= 5;
            API.setCameraFov(API.getActiveCamera(), fov);
            return;
        }
        // Change Height by 1 = +
        if (e.KeyCode == Keys.OemPlus) {
            var activePos = API.getCameraPosition(API.getActiveCamera());
            API.setCameraPosition(API.getActiveCamera(), new Vector3(activePos.X, activePos.Y, activePos.Z + 0.5));
            return;
        }
        // Change Height by -1 = -
        if (e.KeyCode == Keys.OemMinus) {
            var activePos = API.getCameraPosition(API.getActiveCamera());
            API.setCameraPosition(API.getActiveCamera(), new Vector3(activePos.X, activePos.Y, activePos.Z - 0.5));
            return;
        }
        // Point at Player = P
        if (e.KeyCode == Keys.P) {
            API.pointCameraAtEntity(API.getActiveCamera(), API.getLocalPlayer(), new Vector3());
            return;
        }
        // Point at Player Position = {
        if (e.KeyCode == Keys.OemOpenBrackets) {
            API.pointCameraAtPosition(API.getActiveCamera(), API.getEntityPosition(API.getLocalPlayer()));
            return;
        }
        // Point at Player Vehicle = }
        if (e.KeyCode == Keys.OemCloseBrackets) {
            if (API.isPlayerInAnyVehicle(player) == false) {
                return;
            }
            API.pointCameraAtEntity(API.getActiveCamera(), API.getPlayerVehicle(API.getLocalPlayer()), new Vector3());
            return;
        }
    }
});

API.onUpdate.connect(function () {
    // If Director Mode is not enabled. Do not go any further.
    if (directorMode == false) {
        return;
    }

    // From Top to Bottom
    API.drawText("~g~Director Mode", 27, resY - 465, 0.6, 50, 211, 82, 255, 1, 0, true, true, 0);
    API.drawText("~y~F11 - Animate Cameras", 27, resY - 425, 0.5, 50, 211, 82, 255, 4, 0, true, true, 0);
    API.drawText("~b~Markers Hidden: ~w~" + markersHidden, 27, resY - 390, 0.5, 50, 211, 82, 255, 4, 0, true, true, 0);

    if (API.getActiveCamera() != null) {
        API.drawText("~y~FOV: ~w~" + API.getCameraFov(API.getActiveCamera()), 27, resY - 355, 0.5, 50, 211, 82, 255, 4, 0, true, true, 0);
    }

    API.drawText("~o~Speed: ~w~" + defaultSpeed, 27, resY - 320, 0.5, 50, 211, 82, 255, 4, 0, true, true, 0);
    API.drawText("~g~Current: ~w~" + currentCamera, 27, resY - 285, 0.5, 50, 211, 82, 255, 4, 0, true, true, 0);
    API.drawText("~b~Cameras: ~w~" + activeCameras.length, 27, resY - 250, 0.5, 50, 211, 82, 255, 4, 0, true, true, 0);
});

function clearCameraBlips() {
    for (var i = 0; i < activeCameraBlips.length; i++) {
        API.deleteEntity(activeCameraBlips[i]);
    }

    activeCameras = [];
    activeCameraBlips = [];
    API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
}

// Sets up a single camera.
function cameraSetup(position, rotation) {
    var camera = API.createCamera(position, rotation);
    API.setActiveCamera(camera);
    activeCameras.push(camera);

    var blip = API.createBlip(position);
    API.setBlipSprite(blip, 135);

    var marker = API.createMarker(28, position, new Vector3(), rotation, new Vector3(0.2, 0.2, 0.2), 0, 0, 255, 50);
    activeCameraMarkers.push(marker);

    activeCameraBlips.push(blip);
    currentCamera = activeCameras.length - 1;
    API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
}

// Setup a camera on the server silently.
function cameraSetupSilent(position, rotation) {
    var camera = API.createCamera(position, rotation);
    activeCameras.push(camera);
    currentCamera = camera;
}
// Point camera at player silently.
function cameraPointAtPlayer() {
    API.pointCameraAtEntity(currentCamera, API.getLocalPlayer(), new Vector3());
}
// Add current camera to array silently.
function cameraActiveCameraToArray() {
    var camera = API.getActiveCamera();
    activeCameras.push(camera);
}

// Use different cameras.
function cameraGoto(camera) {
    if (activeCameras[camera] == null) {
        return;
    }

    currentCamera = camera;
    lastCamera = API.getActiveCamera();
    API.setActiveCamera(activeCameras[camera]);
    API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
}

// Stop looking through a camera perspective.
function cameraKillActive() {
    lastCamera = API.getActiveCamera();
    API.setActiveCamera(null);
    currentCamera = null;
    API.playSoundFrontEnd("CANCEL", "HUD_FRONTEND_DEFAULT_SOUNDSET");
}

// Goto last Camera Active.
function cameraLastCamera() {
    if (lastCamera === null) {
        return;
    }

    API.setActiveCamera(lastCamera);
    API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");

    for (var i = 0; i < activeCameras.length; i++) {
        if (activeCameras[i] == lastCamera) {
            currentCamera = i;
            break;
        }
    }

    return;
}

function cameraAnimateAllPoints() {
    API.playSoundFrontEnd("5s", "MP_MISSION_COUNTDOWN_SOUNDSET");
    for (i = 0; i < activeCameraMarkers.length; i++) {
        API.setMarkerColor(activeCameraMarkers[i], 0, 0, 0, 255);
    }
    API.sleep(5000);

    var i = 0;
    while (i != activeCameras.length) {
        var currentCamera = activeCameras[i];
        var nextCamera = activeCameras[i + 1];

        if (currentCamera == null || nextCamera == null) {
            break;
        }

        API.interpolateCameras(currentCamera, nextCamera, defaultSpeed, true, true);

        API.sleep(defaultSpeed);
        i++;
    }
    API.sendNotification("Route complete");

    for (i = 0; i < activeCameraMarkers.length; i++) {
        API.setMarkerColor(activeCameraMarkers[i], 50, 0, 0, 255);
    }
}
// Silently animate all cameras. Instant.
function cameraSilentAnimate(duration) {
    var i = 0;
    while (i != activeCameras.length) {
        var currentCamera = activeCameras[i];
        var nextCamera = activeCameras[i + 1];

        if (currentCamera == null || nextCamera == null) {
            break;
        }

        API.interpolateCameras(currentCamera, nextCamera, duration, true, true);

        API.sleep(duration);
        i++;
    }
}