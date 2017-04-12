var useFunction = null;
var currentCollisionType = null;
var vehicleSpecialFunction = null;
// Image Paths
var pressB = "clientside/resources/images/pressb.png";
var pressBAlt1 = "clientside/resources/images/pressbalt.png";
var pressBAlt2 = "clientside/resources/images/pressbalt2.png";
var pressBAlt3 = "clientside/resources/images/pressbalt3.png";
// Main KeyDown Functions
API.onKeyDown.connect(function (player, e) {
    if (API.isChatOpen()) {
        return;
    }
    if (currentCollisionType === null) {
        return;
    }
    if (e.KeyCode === Keys.B) {
        if (e.Shift) {
            if (currentCollisionType == "Vehicle") {
                resource.browser_manager.showCEF("clientside/resources/menu_vehiclecontrols.html");
                vehicleSpecialFunction = null;
                useFunction = null;
                API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
                return;
            }
            API.triggerServerEvent("useSpecial", currentCollisionType);
            vehicleSpecialFunction = null;
            useFunction = null;
            API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
            return;
        }
        API.sendChatMessage("B Pressed in collision: " + currentCollisionType);
        API.triggerServerEvent("useFunction", currentCollisionType);
        vehicleSpecialFunction = null;
        useFunction = null;
        API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
        return;
    }
});
// Use Function Display
API.onUpdate.connect(function () {
    if (useFunction === null || currentCollisionType === null) {
        return;
    }
    // Get the drawing point where we would like to display.
    var pointOfDraw = Point.Round(API.worldToScreen(API.getEntityPosition(API.getLocalPlayer())));
    var playerHeadPoint = new Point(pointOfDraw.X, pointOfDraw.Y - 300);
    // Let's display some stuff.
    switch (currentCollisionType) {
        case "Modification":
            API.dxDrawTexture(pressBAlt2, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "FuelPump":
            API.dxDrawTexture(pressBAlt2, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Repair":
            API.dxDrawTexture(pressBAlt2, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Atm":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Fishing":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "FishingSale":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Barbershop":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Clothing":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "VehicleEngine":
            API.dxDrawTexture(pressBAlt1, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Vehicle":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "House":
            API.dxDrawTexture(pressBAlt3, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "ForSale":
            API.dxDrawTexture(pressBAlt3, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Boats":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Classic":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Commercial":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Compacts":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Coupes":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Bicycles":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Helicopters":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Industrial":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Motorcycles":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "OffRoad":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Muscle":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Planes":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Police":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "SUVS":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Sedans":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Sports":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Super":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Utility":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
        case "Vans":
            API.dxDrawTexture(pressB, playerHeadPoint, new Size(200, 125), 1);
            return;
    }
});
// Set the Use Function and Collision Type
function setUseFunction(argument) {
    useFunction = true;
    currentCollisionType = argument;
    API.playSoundFrontEnd("Click_Special", "WEB_NAVIGATION_SOUNDS_PHONE");
}
// Set the Use Function and Collision Type Silently.
function setUseFunctionSilently(argument) {
    useFunction = true;
    currentCollisionType = argument;
    useFunction = null;
}
// Remove any active Use Function.
function removeUseFunction() {
    useFunction = null;
    currentCollisionType = null;
    vehicleSpecialFunction = null;
    API.playSoundFrontEnd("CLICK_BACK", "WEB_NAVIGATION_SOUNDS_PHONE");
}
