// Resolution Variables
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var res = API.getScreenResolutionMantainRatio();
// Display Variables
var cash = null;
var karma = null;
var fuel = null;
var activeshooter = false;
// Set Functions
function setCash(arg) { cash = arg; }
function setKarma(arg) { karma = arg; }
function setFuel(arg) { fuel = arg; }
function setActiveShooter(arg) { activeshooter = arg; }
// Main OnUpdate Thread
API.onUpdate.connect(function () {
    // If true, return.
    if (API.getEntitySyncedData(API.getLocalPlayer(), "StopDraws")) {
        return;
    }
    // Display all of our shit.
    if (API.isCursorShown()) {
        return;
    }
    cashDisplay();
    karmaDisplay();
    fuelDisplay();
    speedDisplay();
    activeShooterDisplay();
    crosshairDisplay();
});
// Display that $$$ Dawg
function cashDisplay() {
    if (cash === null) {
        return;
    }
    API.drawText("$" + cash, 310, resY - 50, 0.5, 50, 211, 82, 255, 4, 0, false, true, 0);
}
// Display that Karma Dawg
function karmaDisplay() {
    if (karma === null) {
        return;
    }
    API.drawText("~w~Karma: " + karma, 310, resY - 85, 0.5, 244, 244, 66, 255, 4, 0, false, true, 0);
}
// Display that Fuel Dawg
function fuelDisplay() {
    if (fuel === null) {
        return;
    }
    API.drawText("~b~Fuel: ~w~" + Math.round(fuel * 100) / 100, 310, resY - 120, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
}
// Display that MPH Dawg
function speedDisplay() {
    if (!API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
        return;
    }
    // Get the velocity that the player is going.
    var velocity = API.getEntityVelocity(API.getPlayerVehicle(API.getLocalPlayer()));
    // Do some fucking confusing at math. I never passed Algebra I. Fucking kill me.
    var speed = Math.sqrt(velocity.X * velocity.X +
        velocity.Y * velocity.Y +
        velocity.Z * velocity.Z) / 0.44704;
    // Draw the text.
    API.drawText("~b~Speed: ~w~" + Math.round(speed * 100) / 100, 310, resY - 155, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
}
// Display that Active Shooter Dawg
function activeShooterDisplay() {
    if (!activeshooter) {
        return;
    }
    API.drawText("~r~ACTIVE SHOOTER", 310, resY - 190, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
}
// Display that Crosshair Dawg
function crosshairDisplay() {
    API.drawRectangle(resX / 2 - 3, resY / 2 - 3, 6, 6, 0, 0, 0, 255);
    API.drawRectangle(resX / 2 - 2.5, resY / 2 - 2.5, 5, 5, 255, 255, 255, 255);
}
