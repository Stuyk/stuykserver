var mainLoginPanel = null;
var regSuccessPanel = null;
var inventoryPanel = null;

var email = "";
var password = "";

class CefHelper {
  constructor (resourcePath) {
    this.path = resourcePath;
    this.open = false;
  }

  show () {
    if (this.open === false) {
      this.open = true
      var resolution = API.getScreenResolution();
      this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
      API.waitUntilCefBrowserInit(this.browser);
      API.setCefBrowserPosition(this.browser, 0, 0);
      API.loadPageCefBrowser(this.browser, this.path);
      API.showCursor(true);
	  API.setCanOpenChat(false);
	  API.sendNotification("~g~Login panel started, you may now login.");
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
  if (mainLoginPanel == null) {
	mainLoginPanel = new CefHelper('clientside/resources/index.html');
	mainLoginPanel.show();
	API.sendNotification("~g~Login panel started, you may now login.")
  }
});

API.onResourceStop.connect(function() {
	mainLoginPanel.destroy();
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName=="registerSuccessful") {
		if (regSuccessPanel == null)
		{
			regSuccessPanel = new CefHelper('clientside/resources/regsuccess.html');
			regSuccessPanel.show();
		}
    }
	
	
});

function loginHandler(email, password)
{
	mainLoginPanel.destroy();
	API.triggerServerEvent("clientLogin", email, password);
}

function registerHandler(email, password)
{
	mainLoginPanel.destroy();
	API.triggerServerEvent("clientRegistration", email, password);
}