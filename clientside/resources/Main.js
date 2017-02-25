var pagePanel = null;
var cashDisplay = null;
var res = API.getScreenResolution();
var currentMoney = null;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var currentjob = null;

// Karma Display
var karmaDisplay = null;

// Clothing Variables (IN ORDER)
var clothingTopNum = null;
var clothingTopColorNum = null;
var clothingUndershirtNum = null;
var clothingUndershirtColorNum = null;
var clothingLegsNum = null;
var clothingLegsColorNum = null;
var clothingShoesNum = null;
var clothingShoesColorNum = null;
var clothingTorsoNum = null;
var clothingPanelOpen = null;

// Face Variables (IN ORDER)
var faceGender = null;
var faceShapeOne = null;
var faceShapeTwo = null;
var faceSkinOne = null;
var faceSkinTwo = null;
var faceShapeMix = null;
var faceSkinMix = null;
var faceHairstyle = null;
var faceHairstyleColor = null;
var faceHairstyleHighlight = null;
var faceHairstyleTexture = null;
var facePanelOpen = null;

// Account Balance
var playerAccountBalance = null;

var email = "";
var password = "";
var page = "";
var playerName = "";

class CefHelper {
  constructor (resourcePath)
  {
    this.path = resourcePath;
    this.open = false;
  }

  show () {
    if (this.open == false) {
      this.open = true;
      var resolution = API.getScreenResolution();
      this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
      API.waitUntilCefBrowserInit(this.browser);
      API.setCefBrowserPosition(this.browser, 0, 0);
      API.loadPageCefBrowser(this.browser, this.path);
      API.showCursor(true);
      API.setCanOpenChat(false);
    }
  }
  
  showInv (value) {
    if (this.open == false) {
      this.open = true;
      var resolution = API.getScreenResolution();
      this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
      API.waitUntilCefBrowserInit(this.browser);
      API.setCefBrowserPosition(this.browser, resolution.Width / 2 - 250, 0);
      API.loadPageCefBrowser(this.browser, this.path);
      API.showCursor(true);
      API.setCanOpenChat(false);
    }
  }
  
  showBankPanel () {
    if (this.open == false) {
      this.open = true;
      var resolution = API.getScreenResolution();
      this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
      API.waitUntilCefBrowserInit(this.browser);
      API.setCefBrowserPosition(this.browser, resolution.Width / 2 - 250, resolution.Height / 2 - 120);
      API.loadPageCefBrowser(this.browser, this.path);
      API.showCursor(true);
      API.setCanOpenChat(false);
    }
  }
  
  destroy () {
    this.open = false;
    API.destroyCefBrowser(this.browser);
    API.showCursor(false);
    API.setCanOpenChat(true);
  }

  eval (string) {
    this.browser.eval(string);
  }
}

API.onResourceStart.connect(function() {
  if (pagePanel == null) {
    pagePanel = new CefHelper("clientside/resources/index.html");
    pagePanel.show();
  }
});

API.onResourceStop.connect(function() {
    if (pagePanel != null) {
		pagePanel.destroy();
		pagePanel = null;
	}
});

API.onServerEventTrigger.connect(function(eventName, args) {
    if (eventName=="registerSuccessful") {
        if (pagePanel == null)
        {
            pagePanel = new CefHelper("clientside/resources/index.html");
            pagePanel.show();
        }
		
		if (pagePanel != null) {
			pagePanel.browser.call("showLogin");
		}
    }
	
	if (eventName=="passwordDoesNotMatch") {
		if (pagePanel != null) {
			pagePanel.browser.call("doesNotMatch");
		}
	}
	
	if (eventName=="accountDoesNotExist") {
		if (pagePanel != null) {
			pagePanel.browser.call("doesNotExist");
		}
	}
	
	if (eventName=="doesNotMatchAccount") {
		if (pagePanel != null) {
			pagePanel.browser.call("doesNotMatchAccount");
		}
	}
	
	if (eventName === "update_money_display") {
        currentMoney = args[0];
    }
    
    if (eventName=="openInventory") {
        if (pagePanel == null)
        {
			var value = API.getLocalPlayer();
            pagePanel = new CefHelper("clientside/resources/inventory.html");
        }
    }
    
    if (eventName=="openSkinPanel") {
        if (pagePanel == null)
        {
            pagePanel = new CefHelper("clientside/resources/skinchanger.html");
            pagePanel.show();
			updateFaceProperties();
        }
    }
	
	if (eventName=="openClothingPanel") {
        if (pagePanel == null)
        {
            pagePanel = new CefHelper("clientside/resources/clothingpanel.html");
            pagePanel.show();
			updateClothingProperties();
        }
    }
	
	if (eventName=="openCarPanel") {
		if (pagePanel == null) {
			pagePanel = new CefHelper("clientside/resources/carpanel.html");
			pagePanel.show();
		}
	}
    
    if (eventName=="killPanel") {
        if (pagePanel != null) {
            pagePanel.destroy();
            pagePanel = null;
            API.triggerServerEvent("stopAnimation");
        }
    }
	
	if (eventName=="updateNameVariable") {
		playerName = args[0];
		API.sendNotification(playerName);
	}
	
	if (eventName=="loadATM") {
		if (pagePanel == null) {
			pagePanel = new CefHelper("clientside/resources/atmpanel2.html");
			pagePanel.show();
			playerAccountBalance = args[0].toString();
		}
	}
	
	if (eventName=="updateATM") {
		if (pagePanel != null) {
			playerAccountBalance = args[0].toString();
			playerCashOnHand = args[1].toString();
			pagePanel.browser.call("displayAccountBalance", playerAccountBalance.toString(), playerCashOnHand.toString());
		}
	}
	
	if (eventName=="closeCarDoor") {
		API.setVehicleDoorState(args[0], args[1], false);
	}
	
	if (eventName=="clothingLocalVariableUpdate") {
		clothingPanelOpen = true;
		clothingTorsoNum = args[0];
		clothingTopNum = args[1];
		clothingTopColorNum = args[2];
		clothingUndershirtNum = args[3];
		clothingUndershirtColorNum = args[4];
		clothingLegsNum = args[5];
		clothingLegsColorNum = args[6];
		clothingShoesNum = args[7];
		clothingShoesColorNum = args[8];
	}
	
	if (eventName=="loadFaceData") {
		facePanelOpen = true;
		faceShapeOne = args[0];
		faceShapeTwo = args[1];
		faceSkinOne = args[2];
		faceSkinTwo = args[3];
		faceShapeMix = args[4];
		faceSkinMix = args[5];
		faceHairstyle = args[6];
		faceHairstyleColor = args[7];
		faceHairstyleHighlight = args[8];
		faceHairstyleTexture = args[9];
	}
	
	if (eventName=="updateKarma") {
		karmaDisplay = args[0];
	}
});

API.onUpdate.connect(function() {
    if (currentMoney != null) {
        API.drawText("$" + currentMoney, resX - 25, 25, 1, 50, 211, 82, 255, 4, 2, false, true, 0);
    }
	
	if (karmaDisplay != null) {
		API.drawText(karmaDisplay, resX - 25, resY - 100, 1, 244, 244, 66, 255, 4, 2, false, true, 0);
	}
});

function killPanel() {
	if (pagePanel != null) {
		pagePanel.destroy();
		pagePanel = null;
		API.triggerServerEvent("stopAnimation");
	}
}

function closeInventory() {
    pagePanel.destroy();
    pagePanel = null;
}

function loginHandler(email, password) {
    API.triggerServerEvent("clientLogin", email, password);
}

function requestAccountBalance() {
	pagePanel.browser.call("displayAccountBalance", playerAccountBalance, currentMoney);
}

function registerHandler(email, password) {
    API.triggerServerEvent("clientRegistration", email, password);
}

function getPlayerName() {
	API.triggerServerEvent("localPullName");
	return playerName;
}

function withdrawATM(amount) {
	API.triggerServerEvent("withdrawATM_Server", amount);
}

function depositATM(amount) {
	API.triggerServerEvent("depositATM_Server", amount);
}

function hideATMCash() {
	cashDisplay = null;
}

// ##########################
// #### MODEL CHANGER    ####
// #### WRITTEN BY STUYK ####
// ##########################
function intToFloat(num) { // Used to create the float numbers.
	return num.toFixed(1);
}

function updateFaceProperties() {
	pagePanel.browser.call("updateProperties", faceShapeOne, faceShapeTwo, faceSkinOne, faceSkinTwo, faceShapeMix, faceSkinMix, faceHairstyle, faceHairstyleColor, faceHairstyleHighlight, faceHairstyleTexture);
}

function changeFaceSave() {
	API.triggerServerEvent("saveFace", faceShapeOne, faceShapeTwo, faceSkinOne, faceSkinTwo, faceShapeMix, faceSkinMix, faceHairstyle, faceHairstyleColor, faceHairstyleHighlight, faceHairstyleTexture);
	faceShapeOne = null;
	faceShapeTwo = null;
	faceSkinOne = null;
	faceSkinTwo = null;
	faceShapeMix = null;
	faceSkinMix = null;
	faceHairstyle = null;
	faceHairstyleColor = null;
	faceHairstyleHighlight = null;
	faceHairstyleTexture = null;
	facePanelOpen = null;
	API.stopPlayerAnimation();
}

function changeUpdateFace() {
	var player = API.getLocalPlayer();
	updateFaceProperties();
	API.callNative("SET_PED_HEAD_BLEND_DATA", player, faceShapeOne, faceShapeTwo, 0, faceSkinOne, faceSkinTwo, 0, intToFloat(faceShapeMix), intToFloat(faceSkinMix), 0, false);
	API.callNative("_SET_PED_HAIR_COLOR", player, faceHairstyleColor, faceHairstyleHighlight);
	API.setPlayerClothes(player, 2, faceHairstyle, faceHairstyleTexture);
}

function changeFaceGender(amount) {
	if (amount == 0) { 
		API.setPlayerSkin(1885233650); // Set to Male
		faceShapeMix = 0.9;
		faceSkinMix = 0.9;
	}
	
	if (amount == 1) {
		API.setPlayerSkin(-1667301416); // Set to Female
		faceShapeMix = 0.1;
		faceSkinMix = 0.1;
	}
}

function changeFaceShapeOne(amount) {
	faceShapeOne = faceShapeOne + amount;
	
	if (faceShapeOne <= -1) {
		faceShapeOne = 0;
	}
	
	if (faceShapeOne == 46) {
		faceShapeOne = 0;
	}
	
	changeUpdateFace();
}

function changeFaceShapeTwo(amount) {
	faceShapeTwo = faceShapeTwo + amount;
	
	if (faceShapeTwo <= -1) {
		faceShapeTwo = 0;
	}
	
	if (faceShapeTwo == 46) {
		faceShapeTwo = 0;
	}
	
	changeUpdateFace();
}

function changeFaceSkinOne(amount) {
	faceSkinOne = faceSkinOne + amount;
	
	if (faceSkinOne <= -1) {
		faceSkinOne = 0;
	}
	
	if (faceSkinOne == 46) {
		faceSkinOne = 0;
	}
	
	changeUpdateFace();
}

function changeFaceSkinTwo(amount) {
	faceSkinTwo = faceSkinTwo + amount;
	
	if (faceSkinTwo <= 0) {
		faceSkinTwo = 0;
	}
	
	if (faceSkinTwo == 46) {
		faceSkinTwo = 0;
	}
	
	changeUpdateFace();
}

function changeFaceShapeMix(amount) {
	if (amount == 1) {
		faceShapeMix += 0.1
	}
	
	if (amount == -1) {
		faceShapeMix -= 0.1
	}
	
	if (faceShapeMix <= 0.1) {
		faceShapeMix = 0.1;
	}
	
	if (faceShapeMix >= 0.9) {
		faceShapeMix = 0.9;
	}
	changeUpdateFace();
}

function changeFaceSkinMix(amount) {
	if (amount == 1) {
		faceSkinMix += 0.1;
	}
	
	if (amount == -1) {
		faceSkinMix -= 0.1;
	}
	
	if (faceSkinMix <= 0.1) {
		faceSkinMix = 0.1;
	}
	
	if (faceSkinMix >= 0.9) {
		faceSkinMix = 0.9;
	}
	changeUpdateFace();
}

function changeFaceHairstyle(amount) {
	faceHairstyle += amount;
	
	if (faceHairstyle <= 0) {
		faceHairstyle = 0;
	}
	
	changeUpdateFace();
}

function changeFaceHairstyleColor(amount) {
	faceHairstyleColor += amount;
	
	if (faceHairstyleColor <= 0) {
		faceHairstyleColor = 0;
	}
	
	changeUpdateFace();
}

function changeFaceHairstyleHighlight(amount) {
	faceHairstyleHighlight += amount;
	
	if (faceHairstyleHighlight <= 0) {
		faceHairstyleHighlight = 0;
	}
	
	changeUpdateFace();
}

function changeFaceHairstyleTexture(amount) {
	faceHairstyleTexture =+ amount;
	
	if (faceHairstyleTexture <= 0) {
		faceHairstyleTexture = 0;
	}
	
	if (faceHairstyleTexture == 4) {
		faceHairstyleTexture = 0;
	}
	
	changeUpdateFace();
}
// ##########################
// #### CLOTHING CHANGER ####
// #### WRITTEN BY STUYK ####
// ##########################

function changeClothingTorso(amount) { // Torso Changer
	if (clothingTorsoNum != null) {
		clothingTorsoNum = clothingTorsoNum + amount;
		
		if (clothingTorsoNum <= -1) {
			clothingTorsoNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 3, clothingTorsoNum, 0);
		updateClothingProperties();
	}
}

function changeClothingTop(amount) { // Top Changer
	if (clothingTopNum != null) {
		clothingTopNum = clothingTopNum + amount;
		
		if (clothingTopNum <= -1) {
			clothingTopNum = 0;
		}
		
		if (clothingTopNum >= 206) {
			clothingTopNum = 0;
		}	
		
		API.setPlayerClothes(API.getLocalPlayer(), 11, clothingTopNum, clothingTopColorNum);
		updateClothingProperties();
	}
}

function changeClothingTopColor(amount) { // Top Color Changer
	if (clothingTopColorNum != null) {
		clothingTopColorNum = clothingTopColorNum + amount;
		
		if (clothingTopColorNum <= -1) {
			clothingTopColorNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 11, clothingTopNum, clothingTopColorNum);
		updateClothingProperties();
	}
}

function changeClothingUndershirt(amount) { // Undershirt Changer
	if (clothingUndershirtNum != null) {
		clothingUndershirtNum = clothingUndershirtNum + amount;
		
		if (clothingUndershirtNum <= -1) {
			clothingUndershirtNum = 96;
		}
		
		if (clothingUndershirtNum >= 97) {
			clothingUndershirtNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 8, clothingUndershirtNum, clothingUndershirtColorNum);
		updateClothingProperties();
	}
}

function changeClothingUndershirtColor(amount) { // Undershirt Color Changer
	if (clothingUndershirtColorNum != null) {
		clothingUndershirtColorNum = clothingUndershirtColorNum + amount;
		
		if (clothingUndershirtColorNum <= -1) {
			clothingUndershirtColorNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 8, clothingUndershirtNum, clothingUndershirtColorNum);
		updateClothingProperties();
	}
}

function changeClothingLegs(amount) { // Legs Changer
	if (clothingLegsNum != null) {
		clothingLegsNum = clothingLegsNum + amount;
		
		if (clothingLegsNum <= -1) {
			clothingLegsNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 4, clothingLegsNum, clothingLegsColorNum);
		updateClothingProperties();
	}
}

function changeClothingLegsColor(amount) { // Legs Color Changer
	if (clothingLegsColorNum != null) {
		clothingLegsColorNum = clothingLegsColorNum + amount;
		
		if (clothingLegsColorNum <= -1) {
			clothingLegsColorNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 4, clothingLegsNum, clothingLegsColorNum);
		updateClothingProperties();
	}
}

function changeClothingShoes(amount) { // Shoes Changer
	if (clothingShoesNum != null) {
		clothingShoesNum = clothingShoesNum + amount;
		
		if (clothingShoesNum <= -1) {
			clothingShoesNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 6, clothingShoesNum, clothingShoesColorNum);
		updateClothingProperties();
	}
}

function changeClothingShoesColor(amount) { // Shoes Color Changer
	if (clothingShoesColorNum != null) {
		clothingShoesColorNum = clothingShoesColorNum + amount;
		
		if (clothingShoesColorNum <= -1) {
			clothingShoesColorNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 6, clothingShoesNum, clothingShoesColorNum);
		updateClothingProperties();
	}
}

function changeRotationHandle(amount) {
	var player = API.getLocalPlayer();
	var oldamount = API.getEntityRotation(player);
	API.setEntityRotation(player, new Vector3(oldamount.X, oldamount.Y, oldamount.Z + amount));
	API.playPlayerAnimation("amb@world_human_hang_out_street@male_b@base", "base", 0, -1);
}

function updateClothingProperties() {
	pagePanel.browser.call("updateClothingProperties", clothingTopNum, clothingTopColorNum, clothingUndershirtNum, clothingUndershirtColorNum, clothingTorsoNum, clothingLegsNum, clothingLegsColorNum, clothingShoesNum, clothingShoesColorNum);
}

function changePushClothingChanges() {
	API.triggerServerEvent("clothingSave", clothingTopNum, clothingTopColorNum, clothingUndershirtNum, clothingUndershirtColorNum, clothingTorsoNum, clothingLegsNum, clothingLegsColorNum, clothingShoesNum, clothingShoesColorNum);
	API.stopPlayerAnimation();
	clothingPanelOpen = null;
	clothingTorsoNum = null;
	clothingTopNum = null;
	clothingTopColorNum = null;
	clothingUndershirtNum = null;
	clothingUndershirtColorNum = null;
	clothingLegsNum = null;
	clothingLegsColorNum = null;
	clothingHatNum = null;
	clothingHatColorNum = null;
	clothingShoesNum = null;
	clothingShoesColorNum = null;
}

// ##########################
// #### Vehicle  CHANGER ####
// #### WRITTEN BY STUYK ####
// ##########################
function updateVehicleMainColor(value) {
	var playerVehicle = API.getPlayerVehicle(API.getLocalPlayer());
	var temp = value.replace('rgb(', '');
	var temptwo = temp.replace(')', '');
	var colorOne = temptwo.split(',');
	API.setVehicleCustomPrimaryColor(playerVehicle, parseInt(colorOne[0]), parseInt(colorOne[1]), parseInt(colorOne[2]));
}

function updateVehicleSecondaryColor(value) {
	var playerVehicle = API.getPlayerVehicle(API.getLocalPlayer());
	var temp = value.replace('rgb(', '');
	var temptwo = temp.replace(')', '');
	var colorOne = temptwo.split(',');
	API.setVehicleCustomSecondaryColor(playerVehicle, parseInt(colorOne[0]), parseInt(colorOne[1]), parseInt(colorOne[2]));
}

function updateVehicleRotation(value) {
	var playerVehicle = API.getPlayerVehicle(API.getLocalPlayer());
	var vehicleRotation = API.getEntityRotation(playerVehicle);
	API.setEntityRotation(playerVehicle, new Vector3(vehicleRotation.X, vehicleRotation.Y, value));
}














