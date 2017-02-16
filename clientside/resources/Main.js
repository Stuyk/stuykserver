var pagePanel = null;
var cashDisplay = null;
var res = API.getScreenResolution();
var currentMoney = null;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;

// Clothing Variables (IN ORDER)
var clothingTopNum = null;
var clothingTopColorNum = null;
var clothingUndershirtNum = null;
var clothingUndershirtColorNum = null;
var clothingLegsNum = null;
var clothingLegsColorNum = null;
var clothingHatNum = null;
var clothingHatColorNum = null;
var clothingShoesNum = null;
var clothingShoesColorNum = null;
var clothingTorsoNum = null;

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
	
	if (eventName=="loadLogin") {
		if (pagePanel == null) {
			pagePanel = new CefHelper("clientside/resources/loading.html");
			pagePanel.show();
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
		clothingTorsoNum = args[0];
		clothingTopNum = args[1];
		clothingTopColorNum = args[2];
		clothingUndershirtNum = args[3];
		clothingUndershirtColorNum = args[4];
		clothingLegsNum = args[5];
		clothingLegsColorNum = args[6];
		clothingHatNum = args[7];
		clothingHatColorNum = args[8];
		clothingShoesNum = args[9];
		clothingShoesColorNum = args[10];
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

function clothingTorso(amount) {
	API.triggerServerEvent("clothingTorsoChange", amount);
}

function clothingTopColor(amount) {
	API.triggerServerEvent("clothingTopColorChange", amount);
}

function clothingTop(amount) {
	API.triggerServerEvent("clothingTopChange", amount);
}

function clothingUnderShirt(amount) {
	API.triggerServerEvent("clothingUndershirtChange", amount);
}

function clothingUnderShirtColor(amount) {
	API.triggerServerEvent("clothingUndershirtColorChange", amount);
}

function clothingLegs(amount) {
	API.triggerServerEvent("clothingLegsChange", amount);
}

function clothingLegsColor(amount) {
	API.triggerServerEvent("clothingLegsColorChange", amount);
}

function clothingHat(amount) {
	API.triggerServerEvent("clothingHatChange", amount);
}

function clothingHatColor(amount) {
	API.triggerServerEvent("clothingHatColorChange", amount);
}

function clothingShoes(amount) {
	API.triggerServerEvent("clothingShoesChange", amount);
}

function clothingShoesColor(amount) {
	API.triggerServerEvent("clothingShoesColorChange", amount);
}

function clothingSave() {
	API.triggerServerEvent("clothingSave");
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

API.onUpdate.connect(function() {
    if (currentMoney != null) {
        API.drawText("$" + currentMoney, resX - 25, 25, 1, 50, 211, 82, 255, 4, 2, false, true, 0);
    }
	
	if (clothingHatNum != null) {
		API.drawText("Hat: " + clothingHatNum, resX - 400, resY / 6, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
	}
	
	if (clothingHatColorNum != null) {
		API.drawText("Hat Color: " + clothingHatColorNum, resX - 400, resY / 6 + 75, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
	}
	
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
	
	
});