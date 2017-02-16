API.onKeyDown.connect(function(player, e) {
	if (!API.isChatOpen() && e.KeyCode == Keys.F) {
		API.triggerServerEvent("useController");
	}
});

API.onServerEventTrigger.connect(function(eventName, args) {
	if (eventName=="stopPlayerControls") {
		API.stopControlOfPlayer(API.getLocalPlayer());
	}
		
	if (eventName=="startPlayerControls") {
		API.requestControlOfPlayer(API.getLocalPlayer());
	}
});