var markers = [];
var textlabels = [];

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName=="markonmap") {

        API.setWaypoint(args[0], args[1]);
		markers.push(API.createMarker(1, new Vector3(args[0], args[1], args[2] - 4), new Vector3(), new Vector3(), new Vector3(2, 2, 10), 0, 255, 0, 100));
    }
	
	if (eventName=="createTextLabel") {
		
		textlabels.push(API.createTextLabel(args[0], new Vector3(args[1], args[2], args[3]), 15, 1));
	}
	
	if (eventName=="clearMarkers") {
		for (var marker of markers) {
			API.deleteEntity(marker);
		}
		
		for (var label of textlabels) {
			API.deleteEntity(label);
		}
	}
});

