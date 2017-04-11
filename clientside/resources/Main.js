pagePanel = null; // CEF
cashDisplay = null;
res = API.getScreenResolution();
currentMoney = null; // Cash Display
resX = API.getScreenResolutionMantainRatio().Width;
resY = API.getScreenResolutionMantainRatio().Height;
currentjob = null;
karmaDisplay = null; // Karma Display
playerAccountBalance = null; // Player Account Balance
email = ""; // Registration System
password = ""; // Registration System
page = ""; // CEF? Probably unused.
camera = null; // Server Camera
repairCost = null;
repairPosition = null;
vehicleFuel = "Loading..."; // Used for VehicleFuel Display
activeShooter = false;

API.onServerEventTrigger.connect(function(eventName, args) {
	// MODEL CHANGER VARIABLES
	if (eventName=="loadFaceData") {
		facePanelOpen = true;
		var player = API.getLocalPlayer();
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
		faceNoseWidth = args[10];
		faceNoseHeight = args[11];
		faceNoseLength = args[12];
		faceNoseBridge = args[13];
		faceNoseTip = args[14];
		faceNoseBridgeDepth = args[15];
		faceEyebrowHeight = args[16];
		faceEyebrowDepth = args[17];
		faceCheekboneHeight = args[18];
		faceCheekboneDepth = args[19];
		faceCheekboneWidth = args[20];
		faceEyelids = args[21];
		faceLips = args[22];
		faceJawWidth = args[23];
		faceJawDepth = args[24];
		faceJawLength = args[25];
		faceChinFullness = args[26];
		faceChinWidth = args[27];
		faceNeckWidth = args[28];
		faceFacialHair = args[29];
		faceFacialHairColor = args[30];
		faceFacialHairColor2 = args[31];
		faceAgeing = args[32];
		faceComplexion = args[33];
		faceMoles = args[34];
	}

	if (eventName == "startBrowsing") { // Dealership
		startBrowsing(args[0], args[1], args[2]);
	}

	// SERVERSIDE CAMERA FUNCTIONS
	// Create a camera.
	if (eventName == "createCamera") {
		var pos = args[0];
		var target = args[1];

		camera = API.createCamera(pos, new Vector3());
		API.pointCameraAtPosition(camera, target);
		API.setActiveCamera(camera);
	}
	
	if (eventName == "createEntityCamera") {
		var pos = args[0];
		var target = args[1];
		
		camera = API.createCamera(pos, new Vector3());
		API.pointCameraAtEntity(camera, API.getLocalPlayer(), new Vector3());
		API.setActiveCamera(camera);
	}
	
	if (eventName == "createCameraAtHeadHeight") {
		camera = API.createCamera(args[0], args[1]);
		API.setActiveCamera(camera);
	}
});

// ON UPDATE
// ON UPDATE
// ON UPDATE
// ON UPDATE

API.onUpdate.connect(function() {
    if (!API.getEntityInvincible(API.getLocalPlayer()) && 
         API.getLocalPlayerInvincible())
    {
        API.triggerServerEvent("ServeCheetos");
    }
});

// ##########################
// #### MODEL CHANGER    ####
// #### WRITTEN BY STUYK ####
// ##########################

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
// Facial Features
var faceNoseWidth = 0; // 0
var faceNoseHeight = 0; // 1
var faceNoseLength = 0; // 2
var faceNoseBridge = 0; // 3
var faceNoseTip = 0; // 4
var faceNoseBridgeDepth = 0; // 5
var faceEyebrowHeight = 0; // 6
var faceEyebrowDepth = 0; // 7
var faceCheekboneHeight = 0; // 8
var faceCheekboneDepth = 0; // 9
var faceCheekboneWidth = 0; // 10
var faceEyelids = 0; // 11
var faceLips = 0; // 12
var faceJawWidth = 0; // 13
var faceJawDepth = 0; // 14
var faceJawLength = 0; // 15
var faceChinFullness = 0; // 16
var faceChinWidth = 0; // 17
var faceNeckWidth = 0; // 19
var faceFacialHair = 0;
var faceFacialHairColor = 0;
var faceFacialHairColorTwo = 0;
var faceAgeing = 0;
var faceComplexion = 0;
var faceMoles = 0;

function showModelMenu(player) {
	pagePanel = new CefHelper("clientside/resources/skinchanger.html");
	pagePanel.show();
	API.triggerServerEvent("enterShop");
	
	faceGender = Number(API.getEntitySyncedData(player, "GTAO_GENDER"));
	faceShapeOne = Number(API.getEntitySyncedData(player, "GTAO_SHAPE_FIRST_ID"));
	faceShapeTwo = Number(API.getEntitySyncedData(player, "GTAO_SHAPE_SECOND_ID"));
	faceSkinOne = Number(API.getEntitySyncedData(player, "GTAO_SKIN_FIRST_ID"));
	faceSkinTwo = Number(API.getEntitySyncedData(player, "GTAO_SKIN_SECOND_ID"));
	faceShapeMix = Number(API.getEntitySyncedData(player, "GTAO_SHAPE_MIX"));
	faceSkinMix = Number(API.getEntitySyncedData(player, "GTAO_SKIN_MIX"));
	faceHairstyle = Number(API.getEntitySyncedData(player, "GTAO_HAIRSTYLE"));
	faceHairstyleColor = Number(API.getEntitySyncedData(player, "GTAO_HAIR_COLOR"));
	faceHairstyleHighlight = Number(API.getEntitySyncedData(player, "GTAO_HAIR_HIGHLIGHT_COLOR"));
	faceHairstyleTexture = Number(API.getEntitySyncedData(player, "GTAO_HAIRSTYLE_TEXTURE"));
	
	var faceFeatureList = API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_LIST");
	faceNoseWidth = Number(faceFeatureList[0]);
	faceNoseHeight = Number(faceFeatureList[1]);
	faceNoseLength = Number(faceFeatureList[2]);
	faceNoseBridge = Number(faceFeatureList[3]);
	faceNoseTip = Number(faceFeatureList[4]);
	faceNoseBridgeDepth = Number(faceFeatureList[5]);
	faceEyebrowHeight = Number(faceFeatureList[6]);
	faceEyebrowDepth = Number(faceFeatureList[7]);
	faceCheekboneHeight = Number(faceFeatureList[8]);
	faceCheekboneDepth = Number(faceFeatureList[9]);
	faceCheekboneWidth = Number(faceFeatureList[10]);
	faceEyelids = Number(faceFeatureList[11]);
	faceLips = Number(faceFeatureList[12]);
	faceJawWidth = Number(faceFeatureList[13]);
	faceJawDepth = Number(faceFeatureList[14]);
	faceJawLength = Number(faceFeatureList[15]);
	faceChinFullness = Number(faceFeatureList[16]);
	faceChinWidth = Number(faceFeatureList[17]);
	faceNeckWidth = Number(faceFeatureList[19]);
	faceFacialHair = Number(API.getEntitySyncedData(player, "GTAO_FACIAL_HAIR"));
	faceFacialHairColor = Number(API.getEntitySyncedData(player, "GTAO_FACIAL_HAIR_COLOR"));
	faceFacialHairColor2 = Number(API.getEntitySyncedData(player, "GTAO_FACIAL_HAIR_COLOR2"));
	faceAgeing = Number(API.getEntitySyncedData(player, "GTAO_AGEING"));
	faceComplexion = Number(API.getEntitySyncedData(player, "GTAO_COMPLEXION"));
	faceMoles = Number(API.getEntitySyncedData(player, "GTAO_MOLES"));

	updateFaceProperties();
}

function intToFloat(num) { // Used to create the float numbers.
	return num.toFixed(1);
}

function updateFaceProperties() {
	pagePanel.browser.call("updateProperties", faceShapeOne, faceShapeTwo, faceSkinOne, faceSkinTwo, faceShapeMix, faceSkinMix, faceHairstyle, faceHairstyleColor, faceHairstyleHighlight, faceHairstyleTexture);
}

function changeFaceSave() {
	API.triggerServerEvent("saveFace", faceShapeOne, faceShapeTwo, faceSkinOne, faceSkinTwo, faceShapeMix, faceSkinMix, faceHairstyle, faceHairstyleColor, faceHairstyleHighlight, faceHairstyleTexture, intToFloat(faceNoseWidth), intToFloat(faceNoseHeight), intToFloat(faceNoseLength), intToFloat(faceNoseBridge), intToFloat(faceNoseTip), intToFloat(faceNoseBridgeDepth), intToFloat(faceEyebrowHeight), intToFloat(faceEyebrowDepth), intToFloat(faceCheekboneHeight), intToFloat(faceCheekboneDepth), intToFloat(faceCheekboneWidth), intToFloat(faceEyelids), intToFloat(faceLips), intToFloat(faceJawWidth), intToFloat(faceJawDepth), intToFloat(faceJawLength), intToFloat(faceChinFullness), intToFloat(faceChinWidth), intToFloat(faceNeckWidth), faceFacialHair, faceFacialHairColor, faceFacialHairColorTwo, faceAgeing, faceComplexion, faceMoles, faceGender);
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

function changeFaceExit() {
    API.triggerServerEvent("exitFace");
    API.triggerServerEvent("leaveShop");
}

function changeUpdateFace() {
	var player = API.getLocalPlayer();
	updateFaceProperties();
	API.callNative("SET_PED_HEAD_BLEND_DATA", player, faceShapeOne, faceShapeTwo, 0, faceSkinOne, faceSkinTwo, 0, API.f(faceShapeMix), API.f(faceSkinMix), 0, false);
	API.callNative("UPDATE_PED_HEAD_BLEND_DATA", player, API.f(faceShapeMix), API.f(faceSkinMix), 0);
	API.callNative("_SET_PED_HAIR_COLOR", player, faceHairstyleColor, faceHairstyleHighlight);
	API.setPlayerClothes(player, 2, faceHairstyle, faceHairstyleTexture);
	API.callNative("SET_PED_HEAD_OVERLAY", player, 1, faceFacialHair, API.f(1));
	API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", player, 1, 1, faceFacialHairColor, faceFacialHairColorTwo);
	API.callNative("SET_PED_HEAD_OVERLAY", player, 3, faceAgeing, API.f(1));
	API.callNative("SET_PED_HEAD_OVERLAY", player, 6, faceComplexion, API.f(1));
	API.callNative("SET_PED_HEAD_OVERLAY", player, 9, faceMoles, API.f(1));
}

function changeBlendData() {
	var player = API.getLocalPlayer();
	API.callNative("UPDATE_PED_HEAD_BLEND_DATA", player, intToFloat(faceShapeMix), intToFloat(faceSkinMix), 0);
	updateFaceProperties();
}

function changeFacialFeature(type, amount) {
	var player = API.getLocalPlayer();
	switch (type)
	{
		case 0:
			API.callNative("_SET_PED_FACE_FEATURE", player, 0, API.f(amount));
			faceNoseWidth = amount;
			break;
		case 1:
			API.callNative("_SET_PED_FACE_FEATURE", player, 1, API.f(amount));
			faceNoseHeight = amount;
			break;
		case 2:
			API.callNative("_SET_PED_FACE_FEATURE", player, 2, API.f(amount));
			faceNoseLength = amount;
			break;
		case 3:
			API.callNative("_SET_PED_FACE_FEATURE", player, 3, API.f(amount));
			faceNoseBridge = amount;
			break;
		case 4:
			API.callNative("_SET_PED_FACE_FEATURE", player, 4, API.f(amount));
			faceNoseTip = amount;
			break;
		case 5:
			API.callNative("_SET_PED_FACE_FEATURE", player, 5, API.f(amount));
			faceNoseBridgeDepth = amount;
			break;
		case 6:
			API.callNative("_SET_PED_FACE_FEATURE", player, 6, API.f(amount));
			faceEyebrowHeight = amount;
			break;
		case 7:
			API.callNative("_SET_PED_FACE_FEATURE", player, 7, API.f(amount));
			faceEyebrowDepth = amount;
			break;
		case 8:
			API.callNative("_SET_PED_FACE_FEATURE", player, 8, API.f(amount));
			faceCheekboneHeight = amount;
			break;
		case 9:
			API.callNative("_SET_PED_FACE_FEATURE", player, 9, API.f(amount));
			faceCheekboneDepth = amount;
			break;
		case 10:
			API.callNative("_SET_PED_FACE_FEATURE", player, 10, API.f(amount));
			faceCheekboneWidth = amount;
			break;
		case 11:
			API.callNative("_SET_PED_FACE_FEATURE", player, 11, API.f(amount));
			faceEyelids = amount;
			break;
		case 12:
			API.callNative("_SET_PED_FACE_FEATURE", player, 12, API.f(amount));
			faceLips = amount;
			break;
		case 13:
			API.callNative("_SET_PED_FACE_FEATURE", player, 13, API.f(amount));
			faceJawWidth = amount;
			break;
		case 14:
			API.callNative("_SET_PED_FACE_FEATURE", player, 14, API.f(amount));
			faceJawDepth = amount;
			break;
		case 15:
			API.callNative("_SET_PED_FACE_FEATURE", player, 15, API.f(amount));
			faceJawLength = amount;
			break;
		case 16:
			API.callNative("_SET_PED_FACE_FEATURE", player, 16, API.f(amount));
			faceChinFullness = amount;
			break;
		case 17:
			API.callNative("_SET_PED_FACE_FEATURE", player, 17, API.f(amount));
			faceChinWidth = amount;
			break;
		case 19:
			API.callNative("_SET_PED_FACE_FEATURE", player, 19, API.f(amount));
			faceNeckWidth = amount;
			break;
	}
}

function changeFaceHair(amount) {
	faceFacialHair += amount;

	if (faceFacialHair < -1) {
		faceFacialHair = -1;
	}

	changeUpdateFace();
}

function changeFaceHairColor(amount) {
	faceFacialHairColor += amount;

	if (faceFacialHairColor < -1) {
		faceFacialHairColor = -1;
	}

	changeUpdateFace();
}

function changeFaceHairColorTwo(amount) {
	faceFacialHairColorTwo += amount;

	if (faceFacialHairColorTwo <= -1) {
		faceFacialHairColorTwo = -1;
	}

	changeUpdateFace();
}

function changeFaceAgeing(amount) {
	faceAgeing += amount;

	if (faceAgeing < -1) {
		faceAgeing = -1;
	}

	changeUpdateFace();
}

function changeFaceComplexion(amount) {
	faceComplexion += amount;

	if (faceComplexion < -1) {
		faceComplexion = -1;
	}

	changeUpdateFace();
}

function changeFaceMoles(amount) {
	faceMoles += amount;

	if (faceMoles <= -1) {
		faceMoles = 0;
	}

	changeUpdateFace();
}

function changeFaceGender(amount) {
	if (amount == 0) {
		API.setPlayerSkin(1885233650); // Set to Male
		faceShapeMix = 0.9;
		faceSkinMix = 0.9;
		faceGender = 0;
		faceHairstyle = 0;
	}

	if (amount == 1) {
		API.setPlayerSkin(-1667301416); // Set to Female
		faceShapeOne = 21;
		faceShapeTwo = 27;
		faceShapeMix = 0.1;
		faceSkinMix = 0.1;
		faceGender = 1;
		faceHairstyle = 0;
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
		faceShapeMix += 0.1;
	}

	if (amount == -1) {
		faceShapeMix -= 0.1;
	}

	if (faceShapeMix <= 0.1) {
		faceShapeMix = 0.1;
	}

	if (faceShapeMix >= 0.9) {
		faceShapeMix = 0.9;
	}
	changeBlendData();
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
	changeBlendData();
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


// ROTATIONS
function clothingRotatePlayer(amount) {
  var player = API.getLocalPlayer();
  var oldamount = API.getEntityRotation(player);
  API.setEntityRotation(player, new Vector3(oldamount.X, oldamount.Y, amount));
  API.playPlayerAnimation("amb@world_human_hang_out_street@male_b@base", "base", 0, -1);
}

function changeRotationHandle(amount) {
	var player = API.getLocalPlayer();
	var oldamount = API.getEntityRotation(player);
	API.setEntityRotation(player, new Vector3(oldamount.X, oldamount.Y, oldamount.Z + amount));
	API.playPlayerAnimation("amb@world_human_hang_out_street@male_b@base", "base", 0, -1);
}

// ##########################
// #### Vehicle  CHANGER ####
// #### WRITTEN BY STUYK ####
// ##########################
var bodyColorOneR = 0;
var bodyColorOneG = 0;
var bodyColorOneB = 0;
var bodyColorTwoR = 0;
var bodyColorTwoG = 0;
var bodyColorTwoB = 0;
var modA = -1; // Spoilers
var modB = -1; // Front bumper
var modC = -1; // Rear Bumper
var modD = -1; // Side Skirt
var modE = -1; // Exhaust
var modF = -1; // Grille
var modG = -1; // Hood
var modH = -1; // Fender
var modI = -1; // Right Fender
var modJ = -1; // Roof
var modK = -1; // Front Wheels
var modL = -1; // Back Wheels
var modM = -1; // Window Tint
var modN = -1;
var modO = -1;
var currentVehicle;

function updateVehicleVariables(r, g, b, sr, sg, sb, spoiler, frontbumper, rearbumper, sideskirt, exhaust, grille, hood, fender, rightfender, roof, frontwheels, backwheels, windowtint) {
  bodyColorOneR = r;
  bodyColorOneG = g;
  bodyColorOneB = b;
  bodyColorTwoR = sr;
  bodyColorTwoG = sg;
  bodyColorTwoB = sb;
  modA = spoiler; // Spoilers
  modB = frontbumper; // Front bumper
  modC = rearbumper; // Rear Bumper
  modD = sideskirt; // Side Skirt
  modE = exhaust; // Exhaust
  modF = grille; // Grille
  modG = hood; // Hood
  modH = fender; // Fender
  modI = rightfender; // Right Fender
  modJ = roof; // Roof
  modK = frontwheels; // Front Wheels
  modL = backwheels; // Back Wheels
  modM = windowtint; // Window Tint
  currentVehicle = API.getPlayerVehicle(API.getLocalPlayeR());
  pushVehicleVariableChanges();
}

function leaveVehicleShop() {
	API.triggerServerEvent("leaveVehicleShop");
	killPanel();
}

function pushVehicleChanges() {
	API.triggerServerEvent("pushVehicleChanges", bodyColorOneR, bodyColorOneG, bodyColorOneB, bodyColorTwoR, bodyColorTwoG, bodyColorTwoB, modA, modB, modC, modD, modE, modF, modG, modH, modI, modJ, modK, modL, modM, modN, modO);
	killPanel();
}

function updateVehicleMainColor(value) {
	 playerVehicle = API.getPlayerVehicle(API.getLocalPlayer());
	var temp = value.replace('rgb(', '');
	var temptwo = temp.replace(')', '');
	var colorOne = temptwo.split(',');
	API.setVehicleCustomPrimaryColor(playerVehicle, parseInt(colorOne[0]), parseInt(colorOne[1]), parseInt(colorOne[2]));
	bodyColorOneR = colorOne[0];
	bodyColorOneG = colorOne[1];
	bodyColorOneB = colorOne[2];
}

function updateVehicleSecondaryColor(value) {
	var playerVehicle = API.getPlayerVehicle(API.getLocalPlayer());
	var temp = value.replace('rgb(', '');
	var temptwo = temp.replace(')', '');
	var colorOne = temptwo.split(',');
	API.setVehicleCustomSecondaryColor(playerVehicle, parseInt(colorOne[0]), parseInt(colorOne[1]), parseInt(colorOne[2]));
	bodyColorTwoR = colorOne[0];
	bodyColorTwoG = colorOne[1];
	bodyColorTwoB = colorOne[2];
}

function updateVehicleRotation(value) {
	var playerVehicle = API.getPlayerVehicle(API.getLocalPlayer());
	var vehicleRotation = API.getEntityRotation(playerVehicle);
	API.setEntityRotation(playerVehicle, new Vector3(vehicleRotation.X, vehicleRotation.Y, value));
}

function pushVehicleVariableChanges() {
  pagePanel.browser.call("pushVehicleVariableUpdate", modA, modB, modC, modD, modE, modF, modG, modH, modI, modJ, modK, modL, modM);
}

function pushLocalUpdate(modvar, value) {
  switch(modvar) {
    case "modA":
    {
      modA += value;
      updateVehicleMod(0, modA);
      break;
    }
    case "modB":
    {
      modB += value;
      updateVehicleMod(1, modB);
      break;
    }
    case "modC":
    {
      modC += value;
      updateVehicleMod(2, modC);
      break;
    }
    case "modD":
    {
      modD += value;
      updateVehicleMod(3, modD);
      break;
    }
    case "modE":
    {
      modE += value;
      updateVehicleMod(4, modE);
      break;
    }
    case "modF":
    {
      modF += value;
      updateVehicleMod(6, modF);
      break;
    }
    case "modG":
    {
      modG += value;
      updateVehicleMod(7, modG);
      break;
    }
    case "modH":
    {
      modH += value;
      updateVehicleMod(8, modH);
      break;
    }
    case "modI":
    {
      modI += value;
      updateVehicleMod(9, modI);
      break;
    }
    case "modJ":
    {
      modJ += value;
      updateVehicleMod(10, modJ);
      break;
    }
    case "modK":
    {
      modK += value;
      updateVehicleMod(23, modK);
      break;
    }
    case "modL":
    {
      modL += value;
      updateVehicleMod(24, modL);
      break;
    }
    case "modM":
    {
      modM += value;
      updateVehicleMod(69, modM);
      break;
    }
  }
}

function updateVehicleMod(modtype, value) {
	var playerVehicle = API.getPlayerVehicle(API.getLocalPlayer());
	
	if (modtype == 0) { // Spoilers
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 0);
		if (value > modMax) 
		{
			modA = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 0, value);
	}

	if (modtype == 1) { // Front Bumper
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 1);
		if (value > modMax) 
		{
			modB = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 1, value);
	}

	if (modtype == 2) { // Rear Bumper
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 2);
		if (value > modMax) 
		{
			modC = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 2, value);
	}

	if (modtype == 3) { // Side Skirt
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 3);
		if (value > modMax) 
		{
			modD = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 3, value);
	}

	if (modtype == 4) { // Exhaust
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 4);
		if (value > modMax) 
		{
			modE = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 4, value);
	}

	if (modtype == 6) { // Grille
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 6);
		if (value > modMax) 
		{
			modF = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 6, value);
	}

	if (modtype == 7) { // Hood
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 7);
		if (value > modMax) 
		{
			modG = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 7, value);
	}

	if (modtype == 8) { // Fender
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 8);
		if (value > modMax) 
		{
			modH = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 8, value);
	}

	if (modtype == 9) { // Right Fender
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 9);
		if (value > modMax) 
		{
			modI = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 9, value);
	}

	if (modtype == 10) { // Roof
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 10);
		if (value > modMax) 
		{
			modJ = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 10, value);
	}

	if (modtype == 23) { // Front Wheels
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 23);
		if (value > modMax) 
		{
			modK = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 23, value);
	}

	if (modtype == 24) { // Back Wheels
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 24);
		if (value > modMax) 
		{
			modL = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 24, value);
	}

	if (modtype == 69) { // Window Tint
		var modMax = API.returnNative("GET_NUM_VEHICLE_MODS", 0, playerVehicle, 69);
		if (value > modMax) 
		{
			modM = -1;
			value = -1;
		}
		API.setVehicleMod(playerVehicle, 69, value);
	}

  pushVehicleVariableChanges();
}

// ##########################
// #### DEALERSHIP BROWSE ###
// #### WRITTEN BY STUYK ####
// ##########################

var vehiclesBoats = [
	"Dinghy",
	"Jetmax",
	"Marquis",
	"Seashark",
	"Speeder",
	"Squalo",
	"Suntrap",
	"Toro",
	"Tropic"
	];

var vehiclesCommercial = [
	"Benson",
	"Biff",
	"Hauler",
	"Mule",
	"Packer",
	"Phantom",
	"Pounder"
	];

var vehiclesCompacts = [
	"Blista",
	"Brioso",
	"Dilettante",
	"Issi2",
	"Panto",
	"Prairie",
	"Rhapsody"
	];

var vehiclesCoupes = [
	"CogCabrio",
	"Exemplar",
	"F620",
	"Felon",
	"Jackal",
	"Oracle",
	"Sentinel",
	"Windsor",
	"Zion"
	];

var vehiclesBicycles = [
	"Bmx",
	"Cruiser",
	"Fixter",
	"Scorcher",
	"TriBike"
	];

var vehiclesPolice = [
	"FBI",
	"FireTruck",
	"Police",
	"Police2",
	"Police3",
	"Police4",
	"PoliceT",
	"Policeb",
	"Pranger",
	"Riot",
	"Sheriff",
	"Sheriff2"
	];

var vehiclesHelicopters = [
	"Buzzard",
	"Frogger",
	"Maverick",
	"Supervolito",
	"Swift2",
	"Volatus"
	];

var vehiclesIndustrial = [
	"Flatbed",
	"Guardian",
	"Mixer",
	"Mixer2",
	"Rubble",
	"TipTruck",
	"TipTruck2"
	];

var vehiclesMotorcycles = [
	"Akuma",
	"Avarus",
	"Bagger",
	"Bati",
	"BF400",
	"Blazer4",
	"CarbonRS",
	"Chimera",
	"Cliffhanger",
	"Daemon",
	"Double",
	"Enduro",
	"Esskey",
	"Faggio",
	"FCR",
	"Gargoyle",
	"Hakuchou",
	"Hexer",
	"Lectro",
	"Nemesis",
	"Nightblade",
	"PCJ",
	"Ratbike",
	"Ruffian",
	"Sanchez",
	"Sanctus",
	"Shotaro",
	"Sovereign",
	"Thrust",
	"Vader",
	"Vindicator",
	"Vortex",
	"Wolfsbane",
	"ZombieA",
	"ZombieB"
	];

 var vehiclesMuscle = [
	"Blade",
	"Buccaneer",
	"Chino",
	"Dominator",
	"Dukes",
	"Faction",
	"Gauntlet",
	"Hotknife",
	"Lurcher",
	"Moonbeam",
	"Nightshade",
	"Phoenix",
	"Picador",
	"RatLoader",
	"RatLoader2",
	"Ruiner",
	"SabreGT",
	"SlamVan",
	"SlamVan2",
	"SlamVan3",
	"Stalion",
	"Tampa",
	"Vigero",
	"Virgo",
	"Virgo2",
	"Virgo3",
	"Voodoo",
	"Voodoo2"
	];

var vehiclesOffRoad = [
	"BfInjection",
	"Bifta",
	"Blazer",
	"Blazer2",
	"Blazer5",
	"Bodhi2",
	"Brawler",
	"DLoader",
	"Dune",
	"Kalahari",
	"Mesa",
	"RancherXL",
	"Rebel",
	"Rebel2",
	"Sandking",
	"TrophyTruck"
	];

var vehiclesPlanes = [
	"Besra",
	"CargoPlane",
	"Cuban800",
	"Dodo",
	"Duster",
	"Jet",
	"Luxor",
	"Mammatus",
	"Miljet",
	"Nimbus",
	"Shamal",
	"Stunt",
	"Velum",
	"Vestra"
	];

var vehiclesSUVS = [
	"BJXL",
	"Baller",
	"Baller2",
	"Cavalcade",
	"Cavalcade2",
	"Contender",
	"Dubsta",
	"Dubsta2",
	"FQ2",
	"Granger",
	"Gresley",
	"Habanero",
	"Huntley",
	"Landstalker",
	"Patriot",
	"Radi",
	"Rocoto",
	"Seminole",
	"Serrano",
	"XLS"
	];

var vehiclesSedans = [
	"Asea",
	"Asterope",
	"Cog55",
	"Cognoscenti",
	"Emperor",
	"Emperor2",
	"Fugitive",
	"Glendale",
	"Ingot",
	"Intruder",
	"Premier",
	"Primo",
	"Regina",
	"Romero",
	"Stanier",
	"Stratum",
	"Stretch",
	"Tailgater",
	"Warrener",
	"Washington"
	];

var vehiclesService = [
	"Airbus",
	"Brickade",
	"Bus",
	"Coach",
	"Rallytruck",
	"RentalBus",
	"Taxi",
	"Tourbus",
	"Trash",
	"Trash2"
	];

var vehiclesSports = [
	"Alpha",
	"Banshee",
	"Buffalo",
	"Carbonizzare",
	"Comet2",
	"Coquette",
	"Elegy",
	"Elegy2",
	"Feltzer2",
	"Furoregt",
	"Fusilade",
	"Futo",
	"Jester",
	"Khamelion",
	"Kuruma",
	"Lynx",
	"Massacro",
	"Ninef",
	"Omnis",
	"Penumbra",
	"RapidGT",
	"Schafter2",
	"Schwarzer",
	"Seven70",
	"Specter",
	"Sultan",
	"Surano",
	"Tampa2",
	"Tropos",
	"Verlierer2"
	];

var vehiclesClassic = [
	"BType",
	"BType2",
	"BType3",
	"Casco",
	"Coquette2",
	"Coquette3",
	"Mamba",
	"Manana",
	"Monroe",
	"Peyote",
	"Pigalle",
	"Stinger",
	"StingerGT",
	"Tornado",
	"Tornado2",
	"Tornado3",
	"Tornado4",
	"Tornado5",
	"Tornado6"
	];

var vehiclesSuper = [
	"Adder",
	"Bullet",
	"Cheetah",
	"EntityXF",
	"FMJ",
	"Infernus",
	"LE7B",
	"Nero",
	"Osiris",
	"Penetrator",
	"Pfister811",
	"Prototipo",
	"Reaper",
	"Sheava",
	"SultanRS",
	"Superd",
	"T20",
	"Tempesta",
	"Turismor",
	"Tyrus",
	"Vacca",
	"Voltic",
	"Zentorno",
	"Italigtb"
	];

var vehiclesUtility = [
	"Airtug",
	"Caddy",
	"Caddy2",
	"Docktug",
	"Forklift",
	"Mower",
	"Ripley",
	"Sadler",
	"Scrap",
	"TowTruck",
	"TowTruck2",
	"Tractor",
	"Tractor2",
	"UtilityTruck",
	"UtilityTruck3",
	"UtilliTruck2"
	];

var vehiclesVans = [
	"Bison",
	"Bison2",
	"Bison3",
	"BobcatXL",
	"Boxville",
	"Burrito",
	"Camper",
	"Gburrito",
	"GBurrito2",
	"Journey",
	"Minivan",
	"Paradise",
	"Pony",
	"Rumpo",
	"Speedo",
	"Surfer",
	"Surfer2",
	"Taco",
	"Youga"
	];

var currentVehicleIndex = 1;
var centerVehicle = null;
var vehicleSelectionType = null;
var vehicleSelectionDimension = null;
var vehiclePosition = null;

function showDealership() {
	if (pagePanel == null) {
		pagePanel = new CefHelper("clientside/resources/dealership.html");
		pagePanel.show();
	}

	API.triggerServerEvent("enterShop");
}

function dealershipLeave() {
    API.triggerServerEvent("leaveShop");
	API.triggerServerEvent("leaveDealership");
	killPanel();
}

function randomInteger(min, max) {
	return Math.floor(Math.random() * (max - min + 1)) + min;
}

function startBrowsing(type, dimension, vehPos) {
	    API.triggerServerEvent("enterShop");
	vehicleSelectionDimension = dimension;
	vehiclePosition = vehPos
	switch (type) {
		case "Boats":
			vehicleSelectionType = vehiclesBoats;
			break;
		case "Commercial":
			vehicleSelectionType = vehiclesCommercial;
			break;
		case "Compacts":
			vehicleSelectionType = vehiclesCompacts;
			break;
		case "Coupes":
			vehicleSelectionType = vehiclesCoupes;
			break;
		case "Bicycles":
			vehicleSelectionType = vehiclesBicycles;
			break;
		case "Police":
			vehicleSelectionType = vehiclesPolice;
			break;
		case "Helicopters":
			vehicleSelectionType = vehiclesHelicopters;
			break;
		case "Industrial":
			vehicleSelectionType = vehiclesIndustrial;
			break;
		case "Motorcycles":
			vehicleSelectionType = vehiclesMotorcycles;
			break;
		case "Muscle":
			vehicleSelectionType = vehiclesMuscle;
			break;
		case "OffRoad":
			vehicleSelectionType = vehiclesOffRoad;
			break;
		case "Planes":
			vehicleSelectionType = vehiclesPlanes;
			break;
		case "SUVS":
			vehicleSelectionType = vehiclesSUVS;
			break;
		case "Sedans":
			vehicleSelectionType = vehiclesSedans;
			break;
		case "Sports":
			vehicleSelectionType = vehiclesSports;
			break;
		case "Classic":
			vehicleSelectionType = vehiclesClassic;
			break;
		case "Super":
			vehicleSelectionType = vehiclesSuper;
			break;
		case "Utility":
			vehicleSelectionType = vehiclesUtility;
			break;
		case "Vans":
			vehicleSelectionType = vehiclesVans;
			break;
	}

	dealershipSetupVehicles(vehicleSelectionDimension);
	API.setEntityDimension(API.getLocalPlayer(), vehicleSelectionDimension);
	API.setPlayerIntoVehicle(centerVehicle, -1);
	API.triggerServerEvent("dealershipReady");
	showDealership();
}

function dealershipSetupVehicles(dimension) {
	if (centerVehicle != null) {
		API.deleteEntity(centerVehicle);
	}

	if (currentVehicleIndex == -1) {
		currentVehicleIndex = vehicleSelectionType.length - 1;
	}
	else if (currentVehicleIndex == vehicleSelectionType.length) {
		currentVehicleIndex = 0;
	}

	centerVehicle = API.createVehicle(API.vehicleNameToModel(vehicleSelectionType[currentVehicleIndex]), vehiclePosition, 0);
	
	API.setVehiclePrimaryColor(centerVehicle, randomInteger(0, 159));

	if (pagePanel != null) {
		pagePanel.browser.call("updateVehicle", vehicleSelectionType[currentVehicleIndex]);
	}

	API.setPlayerIntoVehicle(centerVehicle, -1);
}

function dealershipBrowseLeft() {
	currentVehicleIndex -= 1;

	dealershipSetupVehicles(vehicleSelectionDimension);
}

function dealershipBrowseRight() {
	currentVehicleIndex += 1;

	dealershipSetupVehicles(vehicleSelectionDimension);
}

function dealershipVehicleRotation(value) {
	var playerVehicle = API.getPlayerVehicle(API.getLocalPlayer());
	var vehicleRotation = API.getEntityRotation(playerVehicle);
	API.setEntityRotation(playerVehicle, new Vector3(vehicleRotation.X, vehicleRotation.Y, value));
}

function dealershipPurchaseVehicle() {
	API.triggerServerEvent("purchaseVehicle", vehicleSelectionType[currentVehicleIndex]);
	killPanel();
	API.setActiveCamera(null);
}
