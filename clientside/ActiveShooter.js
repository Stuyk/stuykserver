started = false;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;

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
	API.sleep(180000);
	API.triggerServerEvent("stopActivity");
	started = false;
	API.drawText("ActiveShooter", resX - 25, 1050, 1, 50, 211, 82, 255, 4, 2, false, true, 0);
}