var x = API.getScreenResolutionMantainRatio().Width;
var y = API.getScreenResolutionMantainRatio().Height;
var position = new Point(x / 2, y / 2);
var targetPos = new Point(getRandomInt(0, 1920), getRandomInt(0, 1080));
var cooldown = new Date().getTime(); // MS
var started = false;
var direction = 0; // 0 = W, A = 1, S = 2, D = 3
var score = 0;
API.onUpdate.connect(function () {
    if (!started) {
        return;
    }
    // Disable our controls for snek mode.
    API.disableAllControlsThisFrame();
    // Update / move our snek.
    if (new Date().getTime() > cooldown) {
        cooldown = new Date().getTime() + 500;
        // Game Movement
        gameMovement();
    }
    // Game Controls
    gameControls();
    gameLogic();
    // Draw our box.
    API.drawRectangle(position.X, position.Y, 20, 20, 255, 0, 0, 255);
    // Draw our target.
    API.drawRectangle(targetPos.X, targetPos.Y, 20, 20, 0, 255, 0, 255);
});
API.onChatMessage.connect(function (msg) {
    if (msg === "snek") {
        if (started) {
            started = false;
        }
        else {
            started = true;
        }
    }
});
function gameControls() {
    // Controls
    // W
    if (API.isDisabledControlPressed(32 /* MoveUpOnly */)) {
        direction = 0;
    }
    // A
    if (API.isDisabledControlPressed(34 /* MoveLeftOnly */)) {
        direction = 1;
    }
    // S
    if (API.isDisabledControlPressed(33 /* MoveDownOnly */)) {
        direction = 2;
    }
    // D
    if (API.isDisabledControlPressed(35 /* MoveRightOnly */)) {
        direction = 3;
    }
}
function gameMovement() {
    switch (direction) {
        case 0:
            position = new Point(position.X, position.Y - 20);
            return;
        case 1:
            position = new Point(position.X - 20, position.Y);
            return;
        case 2:
            position = new Point(position.X, position.Y + 20);
            return;
        case 3:
            position = new Point(position.X + 20, position.Y);
            return;
    }
}
function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}
function gameLogic() {
    if (position.X + 5 <= targetPos.X + 20 && position.X + 5 >= targetPos.X) {
        if (position.Y - 5 >= targetPos.Y - 20 && position.Y - 5 <= targetPos.Y) {
            targetPos = new Point(getRandomInt(0, 1920), getRandomInt(0, 1080));
            score += 1;
            API.sendChatMessage("" + score);
        }
    }
}
