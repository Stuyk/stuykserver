started = false;

API.onUpdate.connect(function() {
    if (API.isPlayerShooting(API.getLocalPlayer())) {
		if (started == false) {
			API.triggerServerEvent("startActivity");
		}
	}
});

API.onServerEventTrigger.connect(function(eventName, args) {
	if (eventName=="startActiveShooter") {
		started = true;
		startShooter();
	}
});

function startShooter()
{
	API.sendNotification("~r~You are an active shooter.")
	API.sleep(180000);
	API.triggerServerEvent("stopActivity");
	started = false;
	API.sendNotification("No longer an active shooter.")
	API.drawText("ActiveShooter", resX - 25, 1050, 1, 50, 211, 82, 255, 4, 2, false, true, 0);
}