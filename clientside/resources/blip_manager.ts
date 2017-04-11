var activeBlips = [];
var activeShooterBlips = [];
var blip = null;
// Main Blip Handler Class
class blipHandler {
    constructor(position, color, sprite) {
        blip = API.createBlip(position);
        API.setBlipSprite(blip, sprite);
        API.setBlipColor(blip, color);
    }
    // Pushes blip to the array.
    pushToActive() {
        activeBlips.push(blip);
    }
    // Pushes blip to the shooter array.
    pushToShooter() {
        activeShooterBlips.push(blip);
    }
}
// Push a blip to the Array and display it to the player.
function pushBlip(position, color, sprite) {
    var blipHandle = new blipHandler(position, color, sprite);
    blipHandle.pushToActive();
}
// Push an active shooter blip to the Array and display it to the player.
function pushShooterBlip(position, color, sprite) {
    var blipHandle = new blipHandler(position, color, sprite);
    blipHandle.pushToShooter();
}
// Pop any active shooter blips.
function popShooterBlip() {
    for (var i = 0; i < activeShooterBlips.length; i++) {
        API.deleteEntity(activeShooterBlips[i]);
    }
}
// Remove any active blips.
function popActiveBlips() {
    for (var i = 0; i < activeBlips.length; i++) {
        API.deleteEntity(activeBlips[i]);
    }

    activeBlips = [];
}