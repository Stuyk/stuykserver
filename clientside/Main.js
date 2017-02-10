var pagePanel = null;
var cashDisplay = null;
var res = API.getScreenResolution();
var atmopen = null;
var currentatmcash = null;


var email = "";
var skinid = "";
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
  
  showInv () {
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
    
    if (eventName=="openInventory") {
        if (pagePanel == null)
        {
            pagePanel = new CefHelper("clientside/resources/inventory.html");
            pagePanel.showInv();
        }
    }
    
    if (eventName=="openSkinPanel") {
        if (pagePanel == null)
        {
            pagePanel = new CefHelper("clientside/resources/skinselector.html");
            pagePanel.show();
        }
    }
    
    if (eventName=="killPanel") {
        if (pagePanel != null) {
            pagePanel.destroy();
			pagePanel = null;
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
});

function killPanel() {
	if (pagePanel != null) {
		pagePanel.destroy();
		pagePanel = null;
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
	return;
}

function depositATM(amount) {
	API.triggerServerEvent("depositATM_Server", amount);
	return;
}

function hideATMCash() {
	cashDisplay = null;
	return;
}

