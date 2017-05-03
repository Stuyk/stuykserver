var panel;
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
            API.callNative("DO_SCREEN_FADE_OUT", 3000);
            API.sleep(5000);
            resource.clothing_mode.setupClothingMode(args[0]);
            return;
        case "setupBarberShop":
            API.callNative("DO_SCREEN_FADE_OUT", 3000);
            API.sleep(5000);
            resource.barber_mode.setupBarberShop();
            return;
        case "setupSurgeryShop":
            API.callNative("DO_SCREEN_FADE_OUT", 3000);
            API.sleep(5000);
            resource.surgery_mode.setupSurgery();
            return;
        //=========================================
        // CEF BROWSER EVENTS - browser_manager.js
        //=========================================
        case "showLogin":
            API.setHudVisible(true);
            API.setChatVisible(true);
            //API.startAudio("clientside/resources/audio/trulyyours.mp3", true);
            //API.setGameVolume(0.1);
            resource.menu_eula.menuEULA();
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
            resource.menu_atm.menuATMPanel();
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
        // MENU CALL EVENTS
        //=========================================
        case "killPanel":
            resource.browser_manager.killPanel();
            return;
        // Strictly Login / Registration
        case "usernameDoesNotExist":
            panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~Username or password does not match.", 3000);
            panel.setColor(255, 0, 0);
            resource.menu_builder.setPage(1);
            return;
        case "passwordDoesNotMatch":
            panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~Username or password does not match.", 3000);
            panel.setColor(255, 0, 0);
            resource.menu_builder.setPage(1);
            return;
        case "usernameExists":
            panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~Username already exists.", 3000);
            panel.setColor(255, 0, 0);
            resource.menu_builder.setPage(2);
            return;
        case "nametagExists":
            panel = resource.menu_builder.createNotification(0, "~r~Error: ~w~Your nametag already exists. Change it in ~b~settings.", 3000);
            panel.setTextScale(0.4);
            panel.setColor(255, 0, 0);
            resource.menu_builder.setPage(2);
            return;
        case "doesNotMatchAccount":
            resource.browser_manager.callCEF("doesNotMatchAccount", null);
            return;
        // ATM
        case "atmSuccess":
            resource.menu_atm.atmSuccess();
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
            API.pauseAudio();
            API.callNative("DO_SCREEN_FADE_OUT", 3000);
            API.sleep(4000);
            resource.menu_builder.killMenu();
            // Panel Message
            resource.menu_login.loginStopActivity();
            panel = resource.menu_builder.createNotification(0, "~g~Successfully Logged In", 3000);
            panel.setColor(0, 255, 0);
            // Transition In
            API.callNative("DO_SCREEN_FADE_IN", 3000);
            API.callNative("_TRANSITION_FROM_BLURRED", 3000);
            API.setActiveCamera(null);
            API.setGameplayCameraActive();
            API.setChatVisible(true);
            API.setCanOpenChat(true);
            API.setHudVisible(true);
            API.setGameVolume(1.0);
            API.stopAudio();
            // Turn on Interaction Mode
            resource.interaction_mode.setInteractionActive(true);
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
        //=========================================
        // MISSION EVENTS
        //=========================================
        case "FishingStart":
            resource.job_fishing.wordIsReady(args[0]);
            return;
        case "FishingUpdate":
            resource.job_fishing.wordMode();
            return;
        case "FishingFinish":
            resource.job_fishing.winEvent();
            return;
        case "FishingNotify":
            let notification = resource.menu_builder.createNotification(0, "~b~You've added the fish to your inventory.", 1500);
            notification.setColor(0, 153, 255);
            return;
        case "FishingFail":
            resource.job_fishing.failEvent();
            return;
        case "FishingBuoy":
            resource.job_fishing.createBuoy();
            return;
        //=========================================
        // MISSION EVENTS
        //=========================================
        // Go to next mission objective, someone else completed it.
        case "startMission":
            resource.mission_handler.startMission();
            return;
        case "missionWinScreen":
            resource.mission_handler.missionWinScreen();
            return;
        case "missionLoseScreen":
            resource.mission_handler.missionLoseScreen();
            return;
        case "missionTieScreen":
            resource.mission_handler.missionTieScreen();
            return;
        //=========================================
        // Object Spooner
        //=========================================
        case "attachObject":
            resource.object_placer.attachObject(args[0]);
            return;
    }
});
