API.onServerEventTrigger.connect(function (event, args) {
    switch (event) {
        //=========================================
        // Misc Functions
        //=========================================
        // login_manager.js
        case "setLoggedIn":
            resource.login_manager.setLoggedIn(true);
            return;
        // ???
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
            API.setHudVisible(false);
            resource.browser_manager.showCEF("clientside/resources/index.html");
            API.startAudio("clientside/resources/audio/trulyyours.mp3", true);
            API.setGameVolume(0.5);
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
            resource.camera_manager.clearCameraBlips();
            return;
        case "serverLoginCamera":
            resource.browser_manager.killPanel();
            API.callNative("DO_SCREEN_FADE_OUT", 3000);
            API.sleep(4000);
            API.callNative("DO_SCREEN_FADE_IN", 3000);
            API.setActiveCamera(null);
            API.setGameplayCameraActive();
            API.setHudVisible(true);
            API.stopAudio();
            API.displaySubtitle("~b~Welcome back ~o~" + API.getPlayerName(API.getLocalPlayer()), 4000);
            API.sendChatMessage("~r~Current Not Working: ~n~Dealerships, ~n~Car Customization, ~n~Player Customization");
            API.sendChatMessage("~b~Come back in a few days after it's fixed.");
            return;
        case "createCamera":
            resource.camera_manager.cameraSetupSilent(args[0], new Vector3());
            resource.camera_manager.cameraPointAtPosition();
            resource.camera_manager.cameraActivate();
            return;
        case "createCameraNoPosition":
            resource.camera_manager.cameraSetupSilent(args[0], new Vector3());
            resource.camera_manager.cameraActivate();
            return;
    }
});
