var storedword = null;
var currentword = null;
var casting = null;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var startTime = 0;
var castTime = 0;
var biteTime = 0;

API.onServerEventTrigger.connect(function (eventName, args) {
	if (eventName == "startFishing") {
		if (currentword == null) {
			castTime = 0;
			API.sendNotification("~g~You cast your line.");
			API.sendNotification("~y~Type the word that appears.");
			API.playPlayerAnimation("amb@world_human_stand_fishing@idle_a", "idle_c", 1, -1);
			biteTime = Math.floor((Math.random() * 20) * 100);
			startTime = 0;
			storedword = args[0];
			casting = true;
		}
	}
	
	if (eventName == "stopFishing") {
		casting = null;
		storedword = null;
		currentword = null;
		startTime = 0;
		castTime = 0;
		biteTime = 0;
		API.stopPlayerAnimation();
	}
});

API.onUpdate.connect(function() {
	if (casting != null) {
		castTime += 1;
		
		if (castTime > biteTime) {
			currentword = storedword;
		}
		
		if (currentword != null) {
			API.drawText(currentword, resX / 2 + 75, 300, 1, 255, 0, 0, 255, 4, 2, true, true, 0);
		
			if (startTime > 10 && startTime < 13) {
				API.playSoundFrontEnd("CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET");
			}
		
			if (startTime > 500) {
				fishingTimeout();
			}
			startTime += 1;
		}
	}
    
});

function fishingTimeout() {
	casting = null;
	storedword = null;
	currentword = null;
	startTime = 0;
	API.sendNotification("~r~You ran out of time.");
	API.playSoundFrontEnd("Hack_Failed", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
	API.triggerServerEvent("fishingFailed");
}