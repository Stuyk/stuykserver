var currentmarker = null;

API.onServerEventTrigger.connect(function (eventName, args) {
	if (eventName == "job_create_marker") {
		currentmarker = API.createMarker(
			1,
            args[0],
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(2, 2, 2),
            255, 0, 0, 95
        );
	}
});

API.onUpdate.connect(function(sender, args) {
	
});