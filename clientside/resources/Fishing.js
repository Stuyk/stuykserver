var storedword = null;
var currentword = null;
var queuetask = null;
var casting = null;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var resModX = API.getScreenResolution().Height;
var resModY = API.getScreenResolution().Width;
var startTime = 0;
var castTime = 0;
var biteTime = 0;
var secondsLeft = 10;
var hz = 0;
var panelOpen = null;

API.onServerEventTrigger.connect(function (eventName, args) {
	if (eventName == "startFishing") {
		if (currentword == null) {
			castTime = 0;
			API.sendNotification("~b~You cast out your line.");
			API.sendNotification("~b~Wait for the queue to reel in.");
			API.sendNotification("~b~Then type the given word into the chat box.");
			API.playPlayerAnimation("amb@world_human_stand_fishing@idle_a", "idle_c", 1, -1);
			biteTime = Math.floor((Math.random() * 10 * 350) + 1);
			if (biteTime < 500) {
				biteTime += 500;
			}
			startTime = 0;
			secondsLeft = 0;
			storedword = args[0];
			casting = true;
			panelOpen = false;
		}
	}

	if (eventName == "stopFishing") {
		casting = null;
		storedword = null;
		currentword = null;
		secondsleft = 0;
		startTime = 0;
		castTime = 0;
		biteTime = 0;
		API.stopPlayerAnimation();
	}
});

API.onKeyDown.connect(function(player, e) {
	if (panelOpen == false) {
		if (casting != null) {
			if (castTime > 140) {
				if (!API.isChatOpen() && e.KeyCode == Keys.Space) {
					if (queuetask == true) {
						currentword = storedword;
						resource.Main.showFishing();
						panelOpen = true;
						return;
					}
					else
					{
						fishingTimeout();
						return;
					}
				}
				else if (!API.isChatOpen() && e.KeyCode == Keys.Space)
				{
					fishingTimeout();
					return;
				}
			return;
			}
		return;
		}
	}
});

API.onUpdate.connect(function() { //700 HZ is about 10 Seconds, 350HZ is about 5 seconds, 70HZ is about 1 Second.
	if (casting != null) {
		castTime += 1;
		if (currentword == null) {
			if (castTime > biteTime) {
				API.dxDrawTexture("clientside/resources/images/fishing.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
				API.drawText("Press Spacebar to Reel In", resX / 2 + 200, resY - 300, 1, 255, 255, 255, 255, 4, 2, true, true, 0);
				queuetask = true;
			}
		}

		if (biteTime > 5000) {
			fishingTimeout();
		}

		if (currentword != null) {
			if (startTime > 10 && startTime < 13) {
				API.playSoundFrontEnd("CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET");
			}

			if (startTime > 700) {
				fishingTimeout();
			}

			API.drawText(Math.floor(startTime / 70).toString() + "/10", resX / 2 + 200, resY - 200, 1, 255, 255, 255, 255, 4, 2, true, true, 0);

			startTime += 1;
		}
	}
});

function fishingTimeout() {
	casting = null;
	storedword = null;
	currentword = null;
	queuetask = null;
	panelOpen = null;
	secondsLeft = 0;
	startTime = 0;
	API.stopPlayerAnimation();
	API.sendNotification("~r~You ran out of time.");
	API.playSoundFrontEnd("Hack_Failed", "DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
	API.triggerServerEvent("fishingFailed");
}
