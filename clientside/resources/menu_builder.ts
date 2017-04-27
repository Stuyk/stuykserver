var screenX = API.getScreenResolutionMantainRatio().Width;
var screenY = API.getScreenResolutionMantainRatio().Height;
// Built for 16:9
var panelMinX = (screenX / 32); // 1920 = 128
var panelMinY = (screenY / 18); // 1080 = 120
// Menu Elements
var debugTest = true;
var button = null;
var panel = null;
var image = null;
var buttons = [];
var pages = []; // Pages you can loop through.
var panes = []; // Static Panels
var currentPage = 0;
var images = [];
var padding = 10;
// Set to True when your menu is ready.
var menuIsReady = false;

class PanelImage {
    _xPos: number;
    _yPos: number;
    _width: number;
    _height: number;
    _path: string;

    constructor(path, x, y, width, height) {
        this._path = path;
        this._xPos = x * panelMinX;
        this._yPos = y * panelMinY;
        this._width = width * panelMinX;
        this._height = height * panelMinY
    }

    draw() {
        API.dxDrawTexture(this._path, new Point(this._xPos, this._yPos), new Size(this._width, this._height), 0);
    }

    isHovered() {
        return;
    }

    isClicked() {
        return;
    }

    returnType() {
        return "PanelImage";
    }
}

class Panel {
    _xPos: number;
    _yPos: number;
    _width: number;
    _height: number;
    _text: string;
    _header: boolean;
    _textScale: number;
    _centered: boolean;
    _fontScale: number;

    constructor(x, y, width, height, isHeader, text) {
        this._xPos = x * panelMinX;
        this._yPos = y * panelMinY;
        this._width = width * panelMinX;
        this._height = height * panelMinY
        this._text = text;
        this._header = isHeader;
        this._textScale = (panelMinY / (panelMinY * 10)) * height;
        this._fontScale = (panelMinY / (panelMinY * 10)) * height + 0.2;
        this._centered = false;

        if (this._textScale > 0.6) {
            this._textScale = 0.6;
        }

        if (this._fontScale > 0.6) {
            this._fontScale = 0.6;
        }
    }

    draw() {
        if (this._header) {
            if (this._centered) {
                API.drawText(this._text, this._xPos + (this._width / 2), this._yPos + (this._height / 4), this._textScale * 5, 255, 255, 255, 255, 1, 1, false, false, this._width - padding);
            } else {
                API.drawText(this._text, this._xPos + padding, this._yPos + (this._height / 4), this._textScale * 5, 255, 255, 255, 255, 1, 0, false, false, this._width - padding);
            }
            API.drawRectangle(this._xPos, this._yPos, this._width, this._height, 0, 0, 0, 225);
            API.drawRectangle(this._xPos, this._yPos + this._height - 5, this._width, 5, 255, 255, 255, 50);
        } else {
            if (this._centered) {
                API.drawText(this._text, this._xPos + (this._width / 2), this._yPos + padding, this._fontScale - (this._fontScale / 4), 255, 255, 255, 255, 4, 1, false, false, this._width - padding);
            } else {
                API.drawText(this._text, this._xPos + padding, this._yPos + padding, this._fontScale - (this._fontScale / 4), 255, 255, 255, 255, 4, 0, false, false, this._width - padding);
            }
            API.drawRectangle(this._xPos, this._yPos, this._width, this._height, 0, 0, 0, 200);
        }
    }

    setCentered() {
        this._centered = true;
    }

    isHovered() {
        return;
    }

    isClicked() {
        return;
    }

    returnType() {
        return "Panel";
    }
}

class Button {
    xPos: number;
    yPos: number;
    Width: number;
    Height: number;
    text: string;
    hovered: boolean;
    thisFunction: any;
    type: number; // 0 - Regular / 1 - Success(Green) / 2 - Danger(Orange) / 3 - Alert(Red)
    r: number;
    g: number;
    b: number;

    constructor(x, y, width, height, type, t) {
        this.xPos = x * panelMinX;
        this.yPos = y * panelMinY;
        this.Width = width * panelMinX;
        this.Height = height * panelMinY;
        this.text = t;
        this.hovered = false;
        this.type = type;

        switch (type) {
            case 0: // Regular
                this.r = 255;
                this.g = 255;
                this.b = 255;
                return;
            case 1: // Success
                this.r = 0;
                this.g = 255;
                this.b = 0;
                return;
            case 2: // Danger
                this.r = 255;
                this.g = 165;
                this.b = 0;
                return;
            case 3: // Alert
                this.r = 255;
                this.g = 0;
                this.b = 0;
                return;
        }
    }

    function(obj) {
        this.thisFunction = obj;
    }

    draw() {
        if (this.hovered) { // Hovered
            API.drawRectangle(this.xPos, this.yPos, this.Width, this.Height, 0, 0, 0, 175); // Darker Black
            API.drawRectangle(this.xPos, this.yPos + this.Height - 5, this.Width, 5, this.r, this.g, this.b, 200);
            API.drawText(this.text, this.xPos + (this.Width / 2), this.yPos + (this.Height / 2) - 14, 0.5, this.r, this.g, this.b, 255, 4, 1, false, false, (panelMinX * this.Width));
        } else { // Not Hovered
            API.drawRectangle(this.xPos, this.yPos, this.Width, this.Height, 0, 0, 0, 225); // Black
            API.drawRectangle(this.xPos, this.yPos + this.Height - 5, this.Width, 5, this.r, this.g, this.b, 50);
            API.drawText(this.text, this.xPos + (this.Width / 2), this.yPos + (this.Height / 2) - 14, 0.5, this.r, this.g, this.b, 50, 4, 1, false, false, (panelMinX * this.Width));
        }
    }

    isHovered() {
        if (API.isCursorShown()) {
            let cursorPos = API.getCursorPositionMantainRatio();
            if (cursorPos.X > this.xPos && cursorPos.X < (this.xPos + this.Width) && cursorPos.Y > this.yPos && cursorPos.Y < this.yPos + this.Height) {
                this.hovered = true;
            } else {
                this.hovered = false;
            }
        }
    }

    isClicked() {
        if (!API.isCursorShown()) {
            return;
        }

        let cursorPos = API.getCursorPositionMantainRatio();
        if (cursorPos.X > this.xPos && cursorPos.X < (this.xPos + this.Width) && cursorPos.Y > this.yPos && cursorPos.Y < this.yPos + this.Height) {
            API.playSoundFrontEnd("Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS");
            if (this.thisFunction !== null) {
                this.thisFunction();
            }
        }
    }

    returnType() {
        return "Button";
    }
}

// On-Update Event -- Draws all of our stuff.
var inputText = "!";
API.onUpdate.connect(function () {
    API.drawText(inputText, 50, 50, 1.0, 255, 255, 255, 255, 4, 0, false, false, 1920);
    if (!menuIsReady) {
        return;
    }
    drawAllImages();
    drawAllButtons();
    if (API.isCursorShown()) {
        updateAllButtons();
        if (API.isControlJustPressed(Enums.Controls.CursorAccept)) {
            isButtonClicked();
        }
    }
    drawAllPages();
    drawAllStaticPanes();
});
// On-Keydown Event
API.onKeyDown.connect(function (sender, e) {
    if (e.KeyCode === Keys.Back) {
        inputText = inputText.substring(0, inputText.length - 1);
        return;
    }


    let shiftOn = false;
    if (e.Shift) {
        shiftOn = true;
    }

    let keypress;
    switch (e.KeyCode) {
        case Keys.A:
            keypress = "A";
            break;
        case Keys.B:
            keypress = "B";
            break;
        case Keys.C:
            keypress = "C";
            break;
        case Keys.D:
            keypress = "D";
            break;
        case Keys.E:
            keypress = "E";
            break;
        case Keys.F:
            keypress = "F";
            break;
        case Keys.G:
            keypress = "G";
            break;
        case Keys.H:
            keypress = "H";
            break;
        case Keys.I:
            keypress = "I";
            break;
        case Keys.J:
            keypress = "J";
            break;
        case Keys.K:
            keypress = "K";
            break;
        case Keys.L:
            keypress = "L";
            break;
        case Keys.M:
            keypress = "M";
            break;
        case Keys.N:
            keypress = "N";
            break;
        case Keys.O:
            keypress = "O";
            break;
        case Keys.P:
            keypress = "P";
            break;
        case Keys.Q:
            keypress = "Q";
            break;
        case Keys.R:
            keypress = "R";
            break;
        case Keys.S:
            keypress = "S";
            break;
        case Keys.T:
            keypress = "T";
            break;
        case Keys.U:
            keypress = "U";
            break;
        case Keys.V:
            keypress = "V";
            break;
        case Keys.W:
            keypress = "W";
            break;
        case Keys.X:
            keypress = "X";
            break;
        case Keys.Y:
            keypress = "Y";
            break;
        case Keys.Z:
            keypress = "Z";
            break;
        case Keys.D0:
            keypress = "0";
            if (shiftOn) {
                keypress = ")";
            }
            break;
        case Keys.D1:
            keypress = "1";
            if (shiftOn) {
                keypress = "!";
            }
            break;
        case Keys.D2:
            keypress = "2";
            if (shiftOn) {
                keypress = "@";
            }
            break;
        case Keys.D3:
            keypress = "3";
            if (shiftOn) {
                keypress = "#";
            }
            break;
        case Keys.D4:
            keypress = "4";
            if (shiftOn) {
                keypress = "$";
            }
            break;
        case Keys.D5:
            keypress = "5";
            if (shiftOn) {
                keypress = "%";
            }
            break;
        case Keys.D6:
            keypress = "6";
            if (shiftOn) {
                keypress = "^";
            }
            break;
        case Keys.D7:
            keypress = "7";
            if (shiftOn) {
                keypress = "&";
            }
            break;
        case Keys.D8:
            keypress = "8";
            if (shiftOn) {
                keypress = "*";
            }
            break;
        case Keys.D9:
            keypress = "9";
            if (shiftOn) {
                keypress = "(";
            }
            break;
    }

    if (shiftOn) {
        inputText += keypress.toUpperCase();
    } else {
        inputText += keypress.toLowerCase();
    }
});
// Goes to Next Page
function nextPage() {
    if (currentPage + 1 > pages.length - 1) {
        currentPage = 0;
    } else {
        currentPage += 1;
    }
}
// Goes to Previous Page
function prevPage() {
    if (currentPage - 1 < 0) {
        currentPage = pages.length - 1;
    } else {
        currentPage -= 1;
    }
}
// Draws all panels.
function drawAllPages() {
    if (pages.length > 0) {
        if (Array.isArray(pages[currentPage])) {
            for (var i = 0; i < pages[currentPage].length; i++) {
                pages[currentPage][i].draw();
                let type = pages[currentPage][i].returnType();
                if (type === "Button") {
                    pages[currentPage][i].isHovered();
                }
            }
        } else {
            pages[currentPage].draw();
            let type = pages[currentPage].returnType();
            if (type === "Button") {
                pages[currentPage].isHovered();
            }
        }
    }
}
// Draws all static panes.
function drawAllStaticPanes() {
    if (panes.length > 0) {
        for (var i = 0; i < panes.length; i++) {
            panes[i].draw();
        }
    }
}
// Draws all buttons.
function drawAllButtons() {
    if (buttons.length > 0) {
        for (var i = 0; i < buttons.length; i++) {
            buttons[i].draw();
        }
    }
}
// Draws all images.
function drawAllImages() {
    if (images.length > 0) {
        for (var i = 0; i < images.length; i++) {
            images[i].draw();
        }
    }
}
// Determine if our mouse is hovered.
function updateAllButtons() {
    if (buttons.length > 0) {
        for (var i = 0; i < buttons.length; i++) {
            buttons[i].isHovered();
        }
    }
}
// Determines if the mouse is clicked.
function isButtonClicked() {
    if (buttons.length > 0) {
        for (var i = 0; i < buttons.length; i++) {
            buttons[i].isClicked();
        }
    }

    if (pages.length > 0) {
        if (Array.isArray(pages[currentPage])) {
            for (var i = 0; i < pages[currentPage].length; i++) {
                let type = pages[currentPage][i].returnType();
                if (type === "Button") {
                    pages[currentPage][i].isClicked();
                }
            }
        } else {
            let type = pages[currentPage].returnType();
            if (type === "Button") {
                pages[currentPage].isClicked();
            }
        }
    }
}
// Ready to draw the menu?
function setMenuReady(isReady: boolean) {
    menuIsReady = isReady;
}
// Add a page to our pages array.
function addPage(xStart: number, yStart: number, xGridWidth: number, yGridHeight: number, isHeaderType: boolean, text: string) {
    panel = new Panel(xStart, yStart, xGridWidth, yGridHeight, isHeaderType, text);
    pages.push(panel);
}
// Add a page array to our pages array.
function pushPageElements(pageElements: Array<Panel>) {
    pages.push(pageElements);
}
// Created a page that can be used to push to the 'column' page type.
function createPage(xStart: number, yStart: number, xGridWidth: number, yGridHeight: number, isHeaderType: boolean, text: string) {
    panel = new Panel(xStart, yStart, xGridWidth, yGridHeight, isHeaderType, text);
    return panel;
}
// Create a static button.
function createButton(xStart: number, yStart: number, xGridWidth: number, yGridHeight: number, type: number, text: any) {
    button = new Button(xStart, yStart, xGridWidth, yGridHeight, type, text);
    buttons.push(button);
    return button;
}
// Create and Return a button.
function createPageButton(xStart: number, yStart: number, xGridWidth: number, yGridHeight: number, type: number, text: any) {
    button = new Button(xStart, yStart, xGridWidth, yGridHeight, type, text);
    return button;
}
// Add a static pane to our grid. Use this for fillers / spacers / column spacers / etc.
function addPane(xStart: number, yStart: number, xGridWidth: number, yGridHeight: number, isHeaderType: boolean, text: string) {
    panel = new Panel(xStart, yStart, xGridWidth, yGridHeight, isHeaderType, text);
    panes.push(panel);
    return panel;
}
// Clears the menu entirely.
function exitMenu(cursor: boolean, hud: boolean, chat: boolean, blur: boolean) {
    menuIsReady = false;
    buttons = [];
    panes = [];
    pages = [];
    
    if (cursor) {
        API.showCursor(true);
    } else {
        API.showCursor(false);
    }

    if (hud) {
        API.setHudVisible(true);
    } else {
        API.setHudVisible(false);
    }
    
    if (chat) {
        API.setChatVisible(true);
    } else {
        API.setChatVisible(false);
    }

    if (blur) {
        API.callNative("_TRANSITION_FROM_BLURRED", 3000);
    }
}

function openMenu(cursor, hud, chat, blur) {
    if (blur === true) {
        API.callNative("_TRANSITION_TO_BLURRED", 3000);
    }

    menuIsReady = true;

    if (cursor) {
        API.showCursor(true);
    } else {
        API.showCursor(false);
    }

    if (hud) {
        API.setHudVisible(true);
    } else {
        API.setHudVisible(false);
    }

    if (chat) {
        API.setChatVisible(true);
    } else {
        API.setChatVisible(false);
    }
}