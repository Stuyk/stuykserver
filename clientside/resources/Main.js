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
            pagePanel = new CefHelper("clientside/resources/regsuccess.html");
            pagePanel.show();
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
        }
    }
	
	if (eventName=="openClothingPanel") {
        if (pagePanel == null)
        {
            pagePanel = new CefHelper("clientside/resources/clothingpanel.html");
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
			API.sendNotification("~y~Bank Account: ~g~" + args[0].toString());
			pagePanel = new CefHelper("clientside/resources/atmpanel.html");
			pagePanel.showBankPanel();
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
	
	// Clothing Panel Display
	if (clothingPanelOpen == true) {
		if (clothingTopNum != null) {
			API.drawText("Top: " + clothingTopNum, resX - 400, resY / 6 + 150, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (clothingTopColorNum != null) {
			API.drawText("Top Color: " + clothingTopColorNum, resX - 400, resY / 6 + 225, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (clothingUndershirtNum != null) {
			API.drawText("Undershirt: " + clothingUndershirtNum, resX - 400, resY / 6 + 300, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
	
		if (clothingUndershirtColorNum != null) {
			API.drawText("Undershirt Color: " + clothingUndershirtColorNum, resX - 400, resY / 6 + 375, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
	
		if (clothingTorsoNum != null) {
			API.drawText("Torso: " + clothingTorsoNum, resX - 400, resY / 6 + 450, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
	
		if (clothingLegsNum != null) {
			API.drawText("Legs: " + clothingLegsNum, resX - 400, resY / 6 + 525, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
	
		if (clothingLegsColorNum != null) {
			API.drawText("Legs Color: " + clothingLegsColorNum, resX - 400, resY / 6 + 600, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
	
	
		if (clothingShoesNum != null) {
			API.drawText("Shoes: " + clothingShoesNum, resX - 400, resY / 6 + 675, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
	
		if (clothingShoesColorNum != null) {
			API.drawText("Shoe Color: " + clothingShoesColorNum, resX - 400, resY / 6 + 750, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
	}
	
	if (facePanelOpen == true) {
		if (faceShapeOne != null) {
			API.drawText("Shape One: " + faceShapeOne, resX - 400, resY / 6 + 0, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (faceShapeTwo != null) {
			API.drawText("Shape Two: " + faceShapeTwo, resX - 400, resY / 6 + 75, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (faceSkinOne != null) {
			API.drawText("Skin One: " + faceSkinOne, resX - 400, resY / 6 + 150, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (faceSkinTwo != null) {
			API.drawText("Skin Two: " + faceSkinTwo, resX - 400, resY / 6 + 225, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (faceShapeMix != null) {
			API.drawText("Shape Mix: " + intToFloat(faceShapeMix, 2), resX - 400, resY / 6 + 300, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (faceSkinMix != null) {
			API.drawText("Skin Mix: " + intToFloat(faceSkinMix, 2), resX - 400, resY / 6 + 375, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (faceHairstyle != null) {
			API.drawText("Hairstyle: " + faceHairstyle, resX - 400, resY / 6 + 450, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (faceHairstyleColor != null) {
			API.drawText("Hairstyle Color: " + faceHairstyleColor, resX - 400, resY / 6 + 525, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
		
		if (faceHairstyleHighlight != null) {
			API.drawText("Hairstyle Color: " + faceHairstyleHighlight, resX - 400, resY / 6 + 600, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}

		if (faceHairstyleTexture != null) {
			API.drawText("Hairstyle Texture: " + faceHairstyleTexture, resX - 400, resY / 6 + 675, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
		}
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
	killPanel();
	if (pagePanel == null) {
		pagePanel = new CefHelper("clientside/resources/loading.html");
		pagePanel.show();
	}
}

function registerHandler(email, password) {
    pagePanel.destroy();
    pagePanel = null;
    API.triggerServerEvent("clientRegistration", email, password);
}

function clientSkin(skinid) {
    pagePanel.destroy();
    pagePanel = null;
    API.triggerServerEvent("clientSkinSelected", skinid);
}

function loadPageContent(page) {
    pagePanel.destroy();
    pagePanel = null;
    loadNextPage(page);
}

function loadNextPage(page) {
    if (pagePanel == null) {
        if (page == "policemale") {
            API.sendNotification(page);
            pagePanel = new CefHelper("clientside/resources/policemale.html");
            pagePanel.show();
        }
        if (page == "policefemale") {
            API.sendNotification(page);
            pagePanel = new CefHelper("clientside/resources/policefemale.html");
            pagePanel.show();
        }
        if (page == "businessmale") {
            API.sendNotification(page);
            pagePanel = new CefHelper("clientside/resources/businessmale.html");
            pagePanel.show();
        }
        if (page == "businessfemale") {
            API.sendNotification(page);
            pagePanel = new CefHelper("clientside/resources/businessfemale.html");
            pagePanel.show();
        }
        if (page == "industryone") {
            API.sendNotification(page);
            pagePanel = new CefHelper("clientside/resources/industryone.html");
            pagePanel.show();
        }
        if (page == "industrytwo") {
            API.sendNotification(page);
            pagePanel = new CefHelper("clientside/resources/industrytwo.html");
            pagePanel.show();
        }
        if (page == "casualmale") {
            API.sendNotification(page);
            pagePanel = new CefHelper("clientside/resources/casualmale.html");
            pagePanel.show();
        }
        if (page == "casualfemale") {
            API.sendNotification(page);
            pagePanel = new CefHelper("clientside/resources/casualfemale.html");
            pagePanel.show();
        }
    } else {
        pagePanel.destroy();
		pagePanel = null;
        API.sendNotification("Your menus have been cleared. Try accessing the menu once more.");
    }
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

function skinGender(gender) {
	API.triggerServerEvent("skinGenderServer", gender);
}

function skinFaceShapeOne(amount) {
	API.triggerServerEvent("skinFaceShapeOneServer", amount);
}

function skinFaceShapeTwo(amount) {
	API.triggerServerEvent("skinFaceShapeTwoServer", amount);
}

function skinSkinFirst(amount) {
	API.triggerServerEvent("skinSkinFirst", amount);
}

function skinSkinSecond(amount) {
	API.triggerServerEvent("skinSkinSecond", amount);
}

function skinShapeMixPositive() {
	API.triggerServerEvent("skinShapeMixPositive");
}

function skinShapeMixNegative() {
	API.triggerServerEvent("skinShapeMixNegative");
}

function skinSkinMixPositive() {
	API.triggerServerEvent("skinSkinMixPositive");
}

function skinSkinMixNegative() {
	API.triggerServerEvent("skinSkinMixNegative");
}

function skinHairstyle(amount) {
	API.triggerServerEvent("skinHairstyle", amount);
}

function skinHairstyleColor(amount) {
	API.triggerServerEvent("skinHairstyleColor", amount);
}

function skinHairstyleHighlight(amount) {
	API.triggerServerEvent("skinHairstyleHighlight", amount);
}

function skinHairstyleTexture(amount) {
	API.triggerServerEvent("skinHairstyleTexture", amount);
}

function skinRotation(amount) {
	API.triggerServerEvent("skinRotation", amount);
}

function skinSave() {
	API.triggerServerEvent("skinSave");
}

// ##########################
// #### MODEL CHANGER    ####
// #### WRITTEN BY STUYK ####
// ##########################
function intToFloat(num) { // Used to create the float numbers.
	return num.toFixed(1);
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
	
	changeUpdateFace();
}

function changeFaceShapeTwo(amount) {
	faceShapeTwo = faceShapeTwo + amount;
	
	if (faceShapeTwo <= -1) {
		faceShapeTwo = 0;
	}
	
	changeUpdateFace();
}

function changeFaceSkinOne(amount) {
	faceSkinOne = faceSkinOne + amount;
	
	if (faceSkinOne <= -1) {
		faceSkinOne = 0;
	}
	
	changeUpdateFace();
}

function changeFaceSkinTwo(amount) {
	faceSkinTwo = faceSkinTwo + amount;
	
	if (faceSkinTwo <= 0) {
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
	}
}

function changeClothingTopColor(amount) { // Top Color Changer
	if (clothingTopColorNum != null) {
		clothingTopColorNum = clothingTopColorNum + amount;
		
		if (clothingTopColorNum <= -1) {
			clothingTopColorNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 11, clothingTopNum, clothingTopColorNum);
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
	}
}

function changeClothingUndershirtColor(amount) { // Undershirt Color Changer
	if (clothingUndershirtColorNum != null) {
		clothingUndershirtColorNum = clothingUndershirtColorNum + amount;
		
		if (clothingUndershirtColorNum <= -1) {
			clothingUndershirtColorNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 8, clothingUndershirtNum, clothingUndershirtColorNum);
	}
}

function changeClothingLegs(amount) { // Legs Changer
	if (clothingLegsNum != null) {
		clothingLegsNum = clothingLegsNum + amount;
		
		if (clothingLegsNum <= -1) {
			clothingLegsNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 4, clothingLegsNum, clothingLegsColorNum);
	}
}

function changeClothingLegsColor(amount) { // Legs Color Changer
	if (clothingLegsColorNum != null) {
		clothingLegsColorNum = clothingLegsColorNum + amount;
		
		if (clothingLegsColorNum <= -1) {
			clothingLegsColorNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 4, clothingLegsNum, clothingLegsColorNum);
	}
}

function changeClothingShoes(amount) { // Shoes Changer
	if (clothingShoesNum != null) {
		clothingShoesNum = clothingShoesNum + amount;
		
		if (clothingShoesNum <= -1) {
			clothingShoesNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 6, clothingShoesNum, clothingShoesColorNum);
	}
}

function changeClothingShoesColor(amount) { // Shoes Color Changer
	if (clothingShoesColorNum != null) {
		clothingShoesColorNum = clothingShoesColorNum + amount;
		
		if (clothingShoesColorNum <= -1) {
			clothingShoesColorNum = 0;
		}
		
		API.setPlayerClothes(API.getLocalPlayer(), 6, clothingShoesNum, clothingShoesColorNum);
	}
}

function changeRotationHandle(amount) {
	var player = API.getLocalPlayer();
	var oldamount = API.getEntityRotation(player);
	API.setEntityRotation(player, new Vector3(oldamount.X, oldamount.Y, oldamount.Z + amount));
	API.playPlayerAnimation("amb@world_human_hang_out_street@female_arms_crossed@base", "base", 0, -1);
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
// #### Paper Boy 		 ####
// #### WRITTEN BY STUYK ####
// ##########################
