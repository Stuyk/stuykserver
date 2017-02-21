var useFunction = null;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;

API.onKeyDown.connect(function(player, e) {
	if (!API.isChatOpen() && e.KeyCode == Keys.E) {
		API.triggerServerEvent("useController");
		useFunction = null;
	}
});

API.onServerEventTrigger.connect(function(eventName, args) {
	if (eventName=="stopPlayerControls") {
		API.stopControlOfPlayer(API.getLocalPlayer());
	}
		
	if (eventName=="startPlayerControls") {
		API.requestControlOfPlayer(API.getLocalPlayer());
	}
	
	if (eventName=="triggerUseFunction") {
		useFunction = true;
	}
	
	if (eventName=="removeUseFunction") {
		useFunction = null;
	}
});

API.onUpdate.connect(function() {
	if (useFunction != null) {
		API.dxDrawTexture("clientside/resources/images/presse.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
	}
});