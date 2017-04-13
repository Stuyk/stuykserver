var pointStarts = [];
var pointEnds = [];
var textDraws = [];
var rectangles_h = [];
var rectangles = [];
var trigger = false;
var hovered = false;
var i = 0;
API.onUpdate.connect(function () {
    if (!trigger) {
        return;
    }

    var mousePos = API.getCursorPositionMantainRatio();

    if (mousePos.X > pointStarts[i].X && mousePos.X < pointEnds[i].X) {
        if (mousePos.Y > pointStarts[i].Y && mousePos.Y < pointEnds[i].Y) {
            rectangles_h[i];
        }
    }
    else {
        rectangles[i];
    }

    textDraws[i];

    i++;
    if (i > pointStarts.length - 1) {
        i = 0;
    }
});

API.onChatMessage.connect(function(msg) {
    if (msg === "trigger") {
        if (trigger) {
            trigger = false;
            API.showCursor(false);
        } else {
            trigger = true;
            API.showCursor(true);
            createButton(60, 60, 100, 100, "Test", 25, 25, 25, 255, 0, 0, 0, 255);
        }
    }
});

function createButton(x, y, x2, y2, text, r, g, b, alpha, fr, fg, fb, falpha) {
    pointStarts.push(new Point(x, y));
    pointEnds.push(new Point(x + x2, y + y2));
    rectangles.push(API.drawRectangle(x, y, x2, y2, r, g, b, alpha));
    rectangles_h.push(API.drawRectangle(x, y, x2, y2, r, g, b, 100));
    textDraws.push(API.drawText(text, x + (x2 / 2), y + (y2 / 2), 0.5, fr, fg, fb, falpha, 4, 1, false, false, 600));
}