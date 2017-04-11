var activeMarkers = [];
var marker = null;
// Marker Handler Class
class markerHandler {
    constructor(type, position, scale, alpha, r, g, b, rotation) {
        marker = API.createMarker(type, position, new Vector3(), rotation, scale, r, g, b, alpha);
    }

    pushToActive() {
        activeMarkers.push(marker);
    }
}
// Push / Create a marker and push it up to the array.
function pushMarker(type, position, scale, alpha, r, g, b, rotation) {
    var markerHandle = new markerHandler(type, position, scale, alpha, r, g, b, rotation);
    markerHandle.pushToActive();
}
// Remove any Active Markers
function removeMarkers() {
    for (var i = 0; i < activeMarkers.length; i++) {
        API.deleteEntity(activeMarkers[i]);
    }
    activeMarkers = [];
}