var running = false;
var coinPosition;
var spriteNames = ["0", "1", "2", "3", "4", "5"];
var currentSprite = 0;
var ticks = 0;
var sprite;
API.onChatMessage.connect(function (msg) {
    // Type coinexample in chat to run the example coin.
    if (msg === "coinexample") {
        if (running) {
            running = false;
        } else {
            running = true;
            // Store our player position to the 'coinPosition'
            coinPosition = API.getEntityPosition(API.getLocalPlayer());
        }
    }
});
API.onUpdate.connect(function () {
    // If it's not running, return.
    if (!running) {
        return;
    }
    var worldToScreen = API.worldToScreenMantainRatio(coinPosition);
    var screenPosition = Point.Round(worldToScreen);

    if (ticks % 5 === 0) {
        drawSprite(screenPosition);
    }

    if (ticks % 12 === 0) {
        switchSprite();
    }

    ticks += 1;
    if (ticks > 120) {
        ticks = 0;
    }
});
function drawSprite(screenPos) {
    var playerPos = API.getEntityPosition(API.getLocalPlayer());
    var distance = playerPos.DistanceTo(coinPosition);
    if (distance < 5 && distance > 0) {
        sprite = API.dxDrawTexture("clientside/resources/images/coin/" + spriteNames[currentSprite] + ".png", screenPos, new Size(32, 32), 0);
    }

    if (distance > 5 && distance < 10) {
        sprite = API.dxDrawTexture("clientside/resources/images/coin/" + spriteNames[currentSprite] + ".png", screenPos, new Size(16, 16), 0);
    }

    if (distance > 10 && distance < 15) {
        sprite = API.dxDrawTexture("clientside/resources/images/coin/" + spriteNames[currentSprite] + ".png", screenPos, new Size(8, 8), 0);
    }

    if (distance > 15 && distance < 20) {
        sprite = API.dxDrawTexture("clientside/resources/images/coin/" + spriteNames[currentSprite] + ".png", screenPos, new Size(4, 4), 0);
    }
}
function switchSprite() {
    currentSprite += 1;
    if (currentSprite > spriteNames.length - 1) {
        currentSprite = 0;
    }
}