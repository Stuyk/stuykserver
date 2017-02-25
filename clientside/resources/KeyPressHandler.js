var useFunction = null;
var vehicleSpecialFunction = null;
var resX = API.getScreenResolutionMantainRatio().Width;
var resY = API.getScreenResolutionMantainRatio().Height;
var keys = [];
var currentCollisionType = null;

API.onKeyDown.connect(function(player, e) {
	if (!API.isChatOpen() && e.KeyCode == Keys.B) {
		switch (currentCollisionType) {
			case "VehicleModificationShop":
				API.triggerServerEvent("useFunction", "VehicleModificationShop");
				vehicleSpecialFunction = null;
				useFunction = null;
				break;
				
			case "Bank":
				API.triggerServerEvent("useFunction", "Bank");
				vehicleSpecialFunction = null;
				useFunction = null;
				break;
				
			case "FishingSpot":
				API.triggerServerEvent("useFunction", "FishingSpot");
				vehicleSpecialFunction = null;
				useFunction = null;
				break;
				
			case "FishingSaleSpot":
				API.triggerServerEvent("useFunction", "FishingSaleSpot");
				vehicleSpecialFunction = null;
				useFunction = null;
				break;
				
			case "BarberShop":
				API.triggerServerEvent("useFunction", "BarberShop");
				vehicleSpecialFunction = null;
				useFunction = null;
				break;
				
			case "Clothing":
				API.triggerServerEvent("useFunction", "Clothing");
				vehicleSpecialFunction = null;
				useFunction = null;
				break;
				
			case "Dealership":
				API.triggerServerEvent("useFunction", "Dealership");
				vehicleSpecialFunction = null;
				useFunction = null;
				break;
				
			case "VehicleEngine":
				API.triggerServerEvent("useFunction", "VehicleEngine");
				vehicleSpecialFunction = null;
				useFunction = null;
				break;
				
			case "VehicleLock":
				API.triggerServerEvent("useFunction", "VehicleLock");
				vehicleSpecialFunction = null;
				useFunction = null;
				break;
		}	
	}
});

API.onServerEventTrigger.connect(function(eventName, args) {
	switch (eventName) {
		case "triggerUseFunction":
			useFunction = true;
			currentCollisionType = args[0];
			break;
		
		case "removeUseFunction":
			vehicleSpecialFunction = null;
			useFunction = null;
			break;
	}
});

API.onUpdate.connect(function() {
	if (useFunction != null) {
		switch (currentCollisionType) {
		case "VehicleModificationShop":
			API.dxDrawTexture("clientside/resources/images/pressbalt2.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
			break;
			
		case "Bank":
			API.dxDrawTexture("clientside/resources/images/pressb.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
			break;
			
		case "FishingSpot":
			API.dxDrawTexture("clientside/resources/images/pressb.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
			break;
			
		case "FishingSaleSpot":
			API.dxDrawTexture("clientside/resources/images/pressb.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
			break;
			
		case "BarberShop":
			API.dxDrawTexture("clientside/resources/images/pressb.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
			break;
			
		case "Clothing":
			API.dxDrawTexture("clientside/resources/images/pressb.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
			break;
			
		case "Dealership":
			API.dxDrawTexture("clientside/resources/images/pressb.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
			break;
			
		case "VehicleEngine":
			API.dxDrawTexture("clientside/resources/images/pressbalt.png", new Point(resX / 2 - 25, resY / 2 - 75), new Size(200, 125), 1);
			break;
			
		case "VehicleLock":
			API.dxDrawTexture("clientside/resources/images/pressb.png", new Point(resX / 2 - 200, resY / 2 - 125), new Size(200, 125), 1);
			break;
		}
	}
});