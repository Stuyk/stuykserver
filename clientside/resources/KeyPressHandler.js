var useFunction = null;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;

API.onKeyDown.connect(function(player, e) {
	if (!API.isChatOpen() && e.KeyCode == Keys.E) {
		if (API.isPlayerInAnyVehicle(API.getLocalPlayer()) == false) {
			API.triggerServerEvent("useController");
			useFunction = null;
		}
		else
		{
			API.triggerServerEvent("vehicleController");
			useFunction = null;
		}
	}
});

API.onServerEventTrigger.connect(function(eventName, args) {
	if (eventName=="triggerUseFunction") {
		useFunction = true;
	}
	
	if (eventName=="removeUseFunction") {
		useFunction = null;
	}
});

API.onUpdate.connect(function() {
	if (API.isPlayerInAnyVehicle(API.getLocalPlayer()) == false) {
		if (useFunction != null) {
			API.dxDrawTexture("clientside/resources/images/presse.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
		}
	}
	
	if (API.isPlayerInAnyVehicle(API.getLocalPlayer()) == true) {
		if (useFunction != null) {
			API.dxDrawTexture("clientside/resources/images/pressevehicle.png", new Point(resX / 2 - 25, resY / 2 - 75), new Size(200, 125), 1);
		}
	}
});