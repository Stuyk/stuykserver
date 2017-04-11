API.onServerEventTrigger.connect(function (event, args) {
    switch (event) {
        case "closeCarDoor":
            //API.setVehicleDoorState(args[0], args[1], false);
            return;
        //=========================================
        // HUD FUNCTION EVENTS - hud_manager.js
        //=========================================
        case "updateKarma":
            resource.hud_manager.setKarma(args[0]);
            return;
        case "updateFuel":
            resource.hud_manager.setFuel(args[0]);
            return;
        case "update_money_display":
            resource.hud_manager.setCash(args[0]);
            return;
        case "setActiveShooter":
            resource.hud_manager.setActiveShooter(args[0]);
            return;
        //=========================================
        // USE FUNCTION EVENTS - function_manager.js
        //=========================================
        case "triggerUseFunction":
            resource.function_manager.setUseFunction(args[0]);
            return;
        case "triggerSilentUseFunction":
            resource.function_manager.setUseFunctionSilently(args[0]);
            return;
        case "removeUseFunction":
            resource.function_manager.removeUseFunction();
            return;
        //=========================================
        // SHOP EVENTS - ???
        //=========================================
        case "setupClothingMode":
            resource.clothing_mode.setupClothingMode(args[0]);
            return;
        //=========================================
        // CEF BROWSER EVENTS - browser_manager.js
        //=========================================
        case "showLogin":
            API.setHudVisible(true);
            resource.browser_manager.showCEF("clientside/resources/index.html");
            return;
        case "showInvalidName":
            resource.browser_manager.showCEF("clientside/resources/invalidname.html");
            return;
        case "loadFishing":
            resource.browser_manager.showCEF("clientside/resources/fishing.html");
            return;
        case "openInventory":
            resource.browser_manager.showCEF("clientside/resources/inventory.html");
            return;
        case "openSkinPanel":
            resource.browser_manager.showCEF("clientside/resources/skinchanger.html");
            //showModelMenu(args[0]); GOTTA FIX THIS SHIT YO
            return;
        case "openCarPanel":
            resource.browser_manager.showCEF("clientside/resources/carpanel.html");
            return;
        case "loadATM":
            resource.browser_manager.showCEF("clientside/resources/atmpanel.html");
            return;
        case "showBuyHousing":
            resource.browser_manager.showCEF("clientside/resources/buyhousing.html");
            return;
        case "ShowHousePropertyPanel":
            resource.browser_manager.showCEF("clientside/resources/housing.html");
            return;
        case "showRadialMenu":
            resource.browser_manager.showCEF("clientside/resources/menu_vehiclecontrols.html");
            return;
        case "showDealership":
            resource.browser_manager.showCEF("clientside/resources/dealership.html");
            return;
        //=========================================
        // CEF BROWSER CALL EVENTS - browser_manager.js
        //=========================================
        case "killPanel":
            resource.browser_manager.killPanel();
            return;
        // Strictly Login / Registration
        case "registerSuccessful":
            resource.browser_manager.callCEF("showLogin", null);
            return;
        case "passwordDoesNotMatch":
            resource.browser_manager.callCEF("doesNotMatch", null);
            return;
        case "alreadyLoggedIn":
            resource.browser_manager.callCEF("alreadyLoggedIn", null);
            return;
        case "accountDoesNotExist":
            resource.browser_manager.callCEF("doesNotExist", null);
            return;
        case "doesNotMatchAccount":
            resource.browser_manager.callCEF("doesNotMatchAccount", null);
            return;
        // ATM
        case "refreshATM":
            resource.browser_manager.callCEF("displayAccountBalance", args);
            return;
        case "depositAlertSuccess":
            resource.browser_manager.callCEF("displayDepositSuccess", null);
            return;
        case "displayWithdrawSuccess":
            resource.browser_manager.callCEF("displayWithdrawSuccess", null);
            return;
        case "displayNotThatMuch":
            resource.browser_manager.callCEF("displayNotThatMuch");
            return;
        // FISHING
        case "fishingPushWord":
            resource.browser_manager.callCEF("displayWord", args);
            return;
        // VEHICLE MODIFICATION PANEL
        case "passVehicleModification":
            resource.browser_manager.callCEF("passVehicleModifications", args);
            //updateVehicleVariables(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15], args[16], args[17], args[18]);
            return;
        // HOUSING
        case "passHousePrice":
            resource.browser_manager.callCEF("passHousePrice", args);
            return;
        //=========================================
        // MARKER EVENTS - marker_manager.js
        //=========================================
        case "pushMarker":
            resource.marker_manager.pushMarker(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
            return;
        case "removeMarkers":
            resource.marker_manager.removeMarkers();
            return;
        //=========================================
        // BLIP EVENTS - blip_manager.js
        //=========================================
        case "pushBlip":
            resource.blip_manager.pushBlip(args[0], args[1], args[2]);
            return;
        case "removeBlips":
            resource.blip_manager.popActiveBlips();
            return;
        // Active Shooter
        case "pushShooterBlip":
            resource.active_shooter_manager.pushShooterBlip(args[0], args[1], args[2]);
            return;
        case "popShooterBlip":
            resource.active_shooter_manager.popShooterBlip();
            return;
        //=========================================
        // CAMERA EVENTS
        //=========================================
        case "endCamera":
            API.setActiveCamera(null);
            API.setGameplayCameraActive();
            return;
        case "serverLoginCamera":
            resource.camera_manager.cameraActiveCameraToArray();
            resource.camera_manager.cameraSetupSilent(API.getEntityPosition(API.getLocalPlayer()).Add(new Vector3(0, 0, 1500)), API.getEntityRotation(API.getLocalPlayer()));
            resource.camera_manager.cameraPointAtPlayer();
            resource.camera_manager.cameraSetupSilent(API.getEntityPosition(API.getLocalPlayer()).Add(new Vector3(0, 0, 5)), new Vector3());
            resource.camera_manager.cameraPointAtPlayer();
            resource.camera_manager.cameraSilentAnimate(3000);
            API.setActiveCamera(null);
            API.setGameplayCameraActive();
            return;
    }
});
