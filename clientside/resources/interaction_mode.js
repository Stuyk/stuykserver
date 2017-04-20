// Written by Stuyk.
var x = API.getScreenResolutionMantainRatio().Width;
var y = API.getScreenResolutionMantainRatio().Height;
var target = null;
API.onUpdate.connect(function () {
    // If the chat is open, fuck it.
    if (API.isChatOpen()) {
        return;
    }
    var playerPOS = API.getEntityPosition(API.getLocalPlayer());
    if (target !== null) {
        if (playerPOS.DistanceTo(API.getEntityPosition(target)) >= 10) {
            target = null;
            API.triggerServerEvent("Interaction_Update");
        }
    }
    // Disable the controls we want to use for our interaction mode.
    API.disableControlThisFrame(242 /* CursorScrollDown */); // SCROLL DOWN
    API.disableControlThisFrame(241 /* CursorScrollUp */); // SCROLL UP
    API.disableControlThisFrame(23 /* Enter */); // F
    API.disableControlThisFrame(58 /* ThrowGrenade */); // G
    API.disableControlThisFrame(178 /* PhoneOption */); // DEL Key
    API.callNative("HIDE_HUD_COMPONENT_THIS_FRAME", 19);
    // If the disabled control is pressed... SCROLL WHEEL DOWN
    if (API.isDisabledControlJustPressed(242 /* CursorScrollDown */)) {
        // Lock Vehicle from Inside
        if (API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
            API.triggerServerEvent("useFunction", "Vehicle");
            return;
        }
        drawRayCast();
    }
    // If the disabled control is pressed... SCROLL WHEEL UP
    if (API.isDisabledControlJustPressed(241 /* CursorScrollUp */)) {
        // Vehicle Engine
        if (API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
            API.triggerServerEvent("useFunction", "VehicleEngine");
            return;
        }
        if (target !== null) {
            switch (API.getEntityType(target)) {
                case 1 /* Vehicle */:
                    if (API.hasEntitySyncedData(target, "Type")) {
                        // Vehicle Lock
                        if (playerPOS.DistanceTo(API.getEntityPosition(target)) <= 5) {
                            var type = API.getEntitySyncedData(target, "Type");
                            API.triggerServerEvent("useFunction", type);
                            return;
                        }
                    }
                    break;
                case 6 /* Player */:
                    //
                    break;
                case 2 /* Prop */:
                    //
                    break;
            }
        }
    }
    // MIDDLE MOUSE BUTTON
    if (API.isControlJustPressed(27 /* Phone */)) {
        if (API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
            API.triggerServerEvent("useFunction", "VehicleDoors");
            return;
        }
        target = null;
        API.triggerServerEvent("Interaction_Update");
    }
    // ENTER
    if (API.isDisabledControlJustPressed(23 /* Enter */)) {
        API.sendChatMessage("Pressed");
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
                if (API.hasEntitySyncedData(API.getLocalPlayer(), "Mission_Started")) {
                    if (API.getEntitySyncedData(API.getLocalPlayer(), "Mission_Started")) {
                        API.triggerServerEvent("Mission_Invite", API.getPlayerName(target));
                        return;
                    }
                }
                return;
            case 2 /* Prop */:
                if (playerPOS.DistanceTo(API.getEntityPosition(target)) > 2) {
                    return;
                }
                if (API.hasEntitySyncedData(target, "Type")) {
                    API.triggerServerEvent("useFunction", API.getEntitySyncedData(target, "Type"));
                    target = null;
                    API.triggerServerEvent("Interaction_Update");
                    return;
                }
                return;
        }
    }
    // G
    if (API.isDisabledControlJustPressed(58 /* ThrowGrenade */)) {
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
    // If DEL Key is Pressed. Print target coords.
    if (API.isDisabledControlJustPressed(178 /* PhoneOption */)) {
        var aimPos = API.getPlayerAimCoords(API.getLocalPlayer());
        API.sendChatMessage("[COORDS] - ", aimPos.X.toFixed(4) + " " + aimPos.Y.toFixed(4) + " " + aimPos.Z.toFixed(4));
    }
    // If our target is null, delete the target marker.
    if (target !== null) {
        var pointer = API.worldToScreenMantainRatio(API.getEntityPosition(target));
        var newPointer = Point.Round(pointer);
        switch (API.getEntityType(target)) {
            case 1 /* Vehicle */:
                API.drawText("~b~[~w~Selected~b~] ~w~" + API.getVehicleModelName(API.getEntityModel(target)), newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                break;
            case 6 /* Player */:
                API.drawText("~b~[~w~Selected~b~] ~w~" + API.getPlayerName(target), newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                break;
            case 2 /* Prop */:
                if (API.hasEntitySyncedData(target, "Type")) {
                    var type = API.getEntitySyncedData(target, "Type");
                    API.drawText("~b~[~w~Selected~b~] ~w~" + type, newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                }
                else {
                    API.drawText("~b~[~w~Selected~b~] ~w~Door", newPointer.X, newPointer.Y, 0.3, 0, 0, 255, 255, 4, 1, false, true, 100);
                }
                break;
        }
    }
});
API.onPlayerEnterVehicle.connect(function (entity) {
    target = null;
    API.triggerServerEvent("Interaction_Update", entity);
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
    API.triggerServerEvent("Interaction_Update", target);
}
function taskEnterVehicle(seat) {
    if (API.getVehicleLocked(target)) {
        return;
    }
    if (API.returnNative("GET_VEHICLE_DOORS_LOCKED_FOR_PLAYER", 8 /* Bool */, target, API.getLocalPlayer())) {
        return;
    }
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
