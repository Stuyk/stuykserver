var activeShooterBlips = [];
var blip = null;
// Main Blip Handler Class
class activeShooterBlipHandler {
    constructor(position, color, sprite) {
        blip = API.createBlip(position);
        API.setBlipSprite(blip, sprite);
        API.setBlipColor(blip, color);
    }
    // Pushes blip to the shooter array.
    pushToActive() {
        activeShooterBlips.push(blip);
    }
}
// Main Update
API.onUpdate.connect(function () {
    if (API.returnNative("IS_PED_SHOOTING", 8, API.getLocalPlayer())) {
        API.triggerServerEvent("PedIsShooting");
    }
});
// Push an active shooter blip to the Array and display it to the player.
function pushShooterBlip(position, color, sprite) {
    var blipHandle = new activeShooterBlipHandler(position, color, sprite);
    blipHandle.pushToActive();
}
// Pop any active shooter blips.
function popShooterBlip() {
    for (var i = 0; i < activeShooterBlips.length; i++) {
        API.deleteEntity(activeShooterBlips[i]);
    }
}
