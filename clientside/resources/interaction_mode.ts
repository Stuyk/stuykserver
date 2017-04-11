// Written by Stuyk.
var x = API.getScreenResolutionMantainRatio().Width;
var y = API.getScreenResolutionMantainRatio().Height;
var target = null;
API.onUpdate.connect(function () {
    if (API.getEntitySyncedData(API.getLocalPlayer(), "StopDraws")) {
        return;
    }
    // Draw a Cursor
    API.drawRectangle(x / 2 - 3, y / 2 - 3, 6, 6, 0, 0, 0, 255);
    API.drawRectangle(x / 2 - 2.5, y / 2 - 2.5, 5, 5, 255, 255, 255, 255);
    // If the chat is open, fuck it.
    if (API.isChatOpen()) {
        return;
    }
    // Disable the controls we want to use for our interaction mode.
    API.disableControlThisFrame(Enums.Controls.CursorScrollDown);
    API.disableControlThisFrame(Enums.Controls.CursorScrollUp);
    API.disableControlThisFrame(Enums.Controls.Enter);
    API.disableControlThisFrame(Enums.Controls.ThrowGrenade);
    API.callNative("HIDE_HUD_COMPONENT_THIS_FRAME", 19);
    // If the disabled control is pressed... SCROLL WHEEL DOWN
    if (API.isDisabledControlJustPressed(Enums.Controls.CursorScrollDown)) {
        drawRayCast();
    }
    // If the disabled control is pressed... SCROLL WHEEL UP
    if (API.isDisabledControlJustPressed(Enums.Controls.CursorScrollUp)) {
        //
    }
    // MIDDLE MOUSE BUTTON
    if (API.isControlJustPressed(Enums.Controls.Phone)) {
        target = null;
    }
    // ENTER
    if (API.isControlJustPressed(Enums.Controls.Enter)) {
        if (target === null) {
            if (API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
                return;
            }
            API.callNative("CLEAR_PED_TASKS_IMMEDIATELY", API.getLocalPlayer());
            return;
        }
        switch (API.getEntityType(target)) {
            case Enums.EntityType.Vehicle:
                taskEnterVehicle(-1);
                return;
            case Enums.EntityType.Player:
                // PLAYER INTERACTION CRAP
                return;
        }
    }
    // G
    if (API.isControlJustPressed(Enums.Controls.ThrowGrenade)) {
        if (target === null) {
            if (API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
                return;
            }
            API.callNative("CLEAR_PED_TASKS_IMMEDIATELY", API.getLocalPlayer());
            return;
        }
        switch (API.getEntityType(target)) {
            case Enums.EntityType.Vehicle:
                taskEnterVehicle(0);
                return;
            case Enums.EntityType.Player:
                // PLAYER INTERACTION CRAP
                return;
        }
    }
    // Still show weapon wheel with tab.
    if (API.isControlPressed(Enums.Controls.SelectWeapon)) {
        API.callNative("SHOW_HUD_COMPONENT_THIS_FRAME", 19);
    }
    // If our target is null, delete the target marker.
    if (target !== null) {
        var pointer = API.worldToScreenMantainRatio(API.getEntityPosition(target));
        var newPointer = Point.Round(pointer);
        switch (API.getEntityType(target)) {
            case Enums.EntityType.Vehicle:
                API.drawText("~b~[~w~Selected~b~] ~w~" + API.getVehicleModelName(API.getEntityModel(target)), newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                return;
            case Enums.EntityType.Player:
                API.drawText("~b~[~w~Selected~b~] ~w~" + API.getPlayerName(target), newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                return;
            case Enums.EntityType.Prop:
                API.drawText("~b~[~w~Selected~b~] ~w~Marker", newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                return;
        }
    }
});

API.onPlayerEnterVehicle.connect(function(entity) {
    target = null;
});

function drawRayCast() {
    // Get the player Aimed Position, and create a raycast.
    var aimPos = API.getPlayerAimCoords(API.getLocalPlayer());
    var rayCast = API.createRaycast(API.getEntityPosition(API.getLocalPlayer()).Add(new Vector3(0, 0, 3)), aimPos, (Enums.IntersectOptions.Vehicles | Enums.IntersectOptions.Peds1 | Enums.IntersectOptions.Objects), null);
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
        case Enums.EntityType.Vehicle:
            target = rayCast.hitEntity;
            break;
        case Enums.EntityType.Player:
            target = rayCast.hitEntity;
            break;
        case Enums.EntityType.Prop:
            target = rayCast.hitEntity;
            break;
        default:
            target = null;
            return;
    }
}

function taskEnterVehicle(seat) {
    if (API.getVehicleLocked(target)) {
        return;
    }

    if (API.returnNative("GET_VEHICLE_DOORS_LOCKED_FOR_PLAYER", Enums.NativeReturnTypes.Bool, target, API.getLocalPlayer())) {
        return;
    }

    if (seat === 0) {
        var maxSeats = API.returnNative("GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS", Enums.NativeReturnTypes.Int, target);
        API.sendChatMessage("" + maxSeats);

        if (maxSeats < 0) {
            return;
        }

        var isFree = API.returnNative("IS_VEHICLE_SEAT_FREE", Enums.NativeReturnTypes.Bool, target, 0);
        if (isFree) {
            API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, 0, 1.0, 1, 0);
            return;
        }

        if (maxSeats < 1) {
            return;
        }

        isFree = API.returnNative("IS_VEHICLE_SEAT_FREE", Enums.NativeReturnTypes.Bool, target, 1);
        if (isFree) {
            API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, 0, 1.0, 1, 1);
            return;
        }

        if (maxSeats < 2) {
            return;
        }

        isFree = API.returnNative("IS_VEHICLE_SEAT_FREE", Enums.NativeReturnTypes.Bool, target, 2);
        if (isFree) {
            API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, 0, 1.0, 1, 2);
            return;
        }

        if (maxSeats < 3) {
            return;
        }

        isFree = API.returnNative("IS_VEHICLE_SEAT_FREE", Enums.NativeReturnTypes.Bool, target, 3);
        if (isFree) {
            API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, 0, 1.0, 1, 3);
            return;
        }
    }
    API.callNative("TASK_ENTER_VEHICLE", API.getLocalPlayer(), target, 3000, seat, 1.0, 1, 0);
    return;
}
