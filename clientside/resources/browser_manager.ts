var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var res = API.getScreenResolutionMantainRatio();
var cef = null; // Main CEF Page
// Main CEFHelper Class.
class CefHelper {
    path: string;
    open: boolean;
    browser: any;
    // Main Constructor - Requires the Path of the CEF File or HTML File or whatever.
    constructor(resourcePath) {
        this.path = resourcePath;
        this.open = false;
    }
    // Displays the HTML File we pushed up.
    show() {
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
    // Destroys the CEF Browser.
    destroy() {
        this.open = false;
        API.destroyCefBrowser(this.browser);
        API.showCursor(false);
        API.setCanOpenChat(true);
    }
    // No idea what the fuck this does.
    eval(string) {
        this.browser.eval(string);
    }
}

// Destroy the CEF Panel if the user disconnects. That way we don't fucking destroy their game and aliens invade and shit.
API.onResourceStop.connect(function () {
    if (cef !== null) {
        cef.destroy();
        cef = null;
    }
});
// Destroy any active CEF Panel.
function killPanel() {
    if (cef !== null) {
        cef.destroy();
        cef = null;
    }
}
// Show page, literally any fuggin path.
function showCEF(path: string) {
    if (cef !== null) {
        cef.destroy();
    }
    cef = null;
    cef = new CefHelper(path);
    cef.show();
}
// Browser Function Handler
function callCEF(func: string, args: Array<any>) {
    if (cef === null) {
        return;
    }
    cef.browser.call(func, args);
}
//=========================================
// LOGIN / REGISTRATION EVENTS - LoginHandler.cs
//=========================================
function loginHandler(email, password) { API.triggerServerEvent("clientLogin", email, password); }
function registerHandler(email, password) { API.triggerServerEvent("clientRegistration", email, password); }
//=========================================
// FISHING EVENTS - Jobs/Fishing.cs
//=========================================
function fishingGetWord() { API.triggerServerEvent("pushWordToPanel"); }
function fishingPushWord(value) { API.triggerServerEvent("submitWord", value); }
//=========================================
// ATM EVENTS - BankHandler.cs
//=========================================
function withdrawATM(amount) { API.triggerServerEvent("withdrawATM_Server", amount); }
function depositATM(amount) { API.triggerServerEvent("depositATM_Server", amount); }
function requestAccountBalance() { API.triggerServerEvent("balanceNotDisplayed"); }
//=========================================
// VEHICLE EVENTS - VehicleHandler.cs
//=========================================
function vehicleOpenHood() { API.triggerServerEvent("vehicleHood"); }
function vehicleOpenTrunk() { API.triggerServerEvent("vehicleTrunk"); }
//=========================================
// HOUSE EVENTS - HouseHandler.cs
//=========================================
function housePurchase() { API.triggerServerEvent("housePurchase"); }
function housePriceGet() { API.triggerServerEvent("housePricePoint"); }
function setHouseLock(value) { API.triggerServerEvent("setHouseLock", value); }
function housePropertyChanges(forSale, price) {
    if (forSale == true) {
        API.triggerServerEvent("setHouseProperties", true, price);
    }

    if (forSale == false) {
        API.triggerServerEvent("setHouseProperties", false, null);
    }
}
