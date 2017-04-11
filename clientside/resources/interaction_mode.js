// Written by Stuyk.
var x = API.getScreenResolutionMantainRatio().Width;
var y = API.getScreenResolutionMantainRatio().Height;
var target = null;
API.onUpdate.connect(function () {
    // Draw a Cursor
    API.drawRectangle(x / 2 - 3, y / 2 - 3, 6, 6, 0, 0, 0, 255);
    API.drawRectangle(x / 2 - 2.5, y / 2 - 2.5, 5, 5, 255, 255, 255, 255);
    // If the chat is open, fuck it.
    if (API.isChatOpen()) {
        return;
    }
    // Disable the controls we want to use for our interaction mode.
    API.disableControlThisFrame(242 /* CursorScrollDown */);
    API.disableControlThisFrame(241 /* CursorScrollUp */);
    API.disableControlThisFrame(23 /* Enter */);
    API.disableControlThisFrame(58 /* ThrowGrenade */);
    API.callNative("HIDE_HUD_COMPONENT_THIS_FRAME", 19);
    // If the disabled control is pressed... SCROLL WHEEL DOWN
    if (API.isDisabledControlJustPressed(242 /* CursorScrollDown */)) {
        drawRayCast();
    }
    // If the disabled control is pressed... SCROLL WHEEL UP
    if (API.isDisabledControlJustPressed(241 /* CursorScrollUp */)) {
    }
    // MIDDLE MOUSE BUTTON
    if (API.isControlJustPressed(27 /* Phone */)) {
        target = null;
    }
    // ENTER
    if (API.isControlJustPressed(23 /* Enter */)) {
        if (target === null) {
            if (API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
                return;
            }
            API.callNative("CLEAR_PED_TASKS_IMMEDIATELY", API.getLocalPlayer());
            return;
        }
        switch (API.getEntityType(target)) {
            case 1 /* Vehicle */:
                taskEnterVehicle(-1);
                return;
            case 6 /* Player */:
                // PLAYER INTERACTION CRAP
                return;
        }
    }
    // G
    if (API.isControlJustPressed(58 /* ThrowGrenade */)) {
        if (target === null) {
            if (API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
                return;
            }
            API.callNative("CLEAR_PED_TASKS_IMMEDIATELY", API.getLocalPlayer());
            return;
        }
        switch (API.getEntityType(target)) {
            case 1 /* Vehicle */:
                taskEnterVehicle(0);
                return;
            case 6 /* Player */:
                // PLAYER INTERACTION CRAP
                return;
        }
    }
    // Still show weapon wheel with tab.
    if (API.isControlPressed(37 /* SelectWeapon */)) {
        API.callNative("SHOW_HUD_COMPONENT_THIS_FRAME", 19);
    }
    // If our target is null, delete the target marker.
    if (target !== null) {
        var pointer = API.worldToScreenMantainRatio(API.getEntityPosition(target));
        var newPointer = Point.Round(pointer);
        switch (API.getEntityType(target)) {
            case 1 /* Vehicle */:
                API.drawText("~b~[~w~Selected~b~] ~w~" + API.getVehicleModelName(API.getEntityModel(target)), newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                return;
            case 6 /* Player */:
                API.drawText("~b~[~w~Selected~b~] ~w~" + API.getPlayerName(target), newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                return;
            case 2 /* Prop */:
                API.drawText("~b~[~w~Selected~b~] ~w~Marker", newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                return;
        }
    }
});
API.onPlayerEnterVehicle.connect(function (entity) {
    target = null;
});
function drawRayCast() {
    // Get the player Aimed Position, and create a raycast.
    var aimPos = API.getPlayerAimCoords(API.getLocalPlayer());
    var rayCast = API.createRaycast(API.getEntityPosition(API.getLocalPlayer()).Add(new Vector3(0, 0, 3)), aimPos, (10 /* Vehicles */ | 12 /* Peds1 */ | 16 /* Objects */), null);
    // If our raycast hits anything.
    if (rayCast.didHitAnything === false) {
        return;
    }
    if (rayCast.didHitEntity === false) {
        return;
    }
    if (!API.doesEntityExist(rayCast.hitEntity)) {
        return;
    }
    // Cast hit entity to target variable.
    switch (API.getEntityType(rayCast.hitEntity)) {
        case 1 /* Vehicle */:
            target = rayCast.hitEntity;
            break;
        case 6 /* Player */:
            target = rayCast.hitEntity;
            break;
        case 2 /* Prop */:
            target = rayCast.hitEntity;
            break;
        default:
            target = null;
            return;
    }
}
function taskEnterVehicle(seat) {
    if (seat === 0) {
        var maxSeats = API.returnNative("GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS", 0 /* Int */, target);
        API.sendChatMessage("" + maxSeats);
        if (maxSeats < 0) {
            return;
        }
        var isFree = API.returnNative("IS_VEHICLE_SEAT_FREE", 8 /* Bool */, target, 0);
        if (isFree) {
            API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, 0, 1.0, 1, 0);
            return;
        }
        if (maxSeats < 1) {
            return;
        }
        isFree = API.returnNative("IS_VEHICLE_SEAT_FREE", 8 /* Bool */, target, 1);
        if (isFree) {
            API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, 0, 1.0, 1, 1);
            return;
        }
        if (maxSeats < 2) {
            return;
        }
        isFree = API.returnNative("IS_VEHICLE_SEAT_FREE", 8 /* Bool */, target, 2);
        if (isFree) {
            API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, 0, 1.0, 1, 2);
            return;
        }
        if (maxSeats < 3) {
            return;
        }
        isFree = API.returnNative("IS_VEHICLE_SEAT_FREE", 8 /* Bool */, target, 3);
        if (isFree) {
            API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, 0, 1.0, 1, 3);
            return;
        }
    }
    API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, seat, 1.0, 1, 0);
    return;
}
