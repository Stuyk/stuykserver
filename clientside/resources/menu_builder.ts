var screenX = API.getScreenResolutionMantainRatio().Width;
var screenY = API.getScreenResolutionMantainRatio().Height;
// Built for 16:9
var panelMinX = (screenX / 32);
var panelMinY = (screenY / 18);
// Menu Elements
var debugTest = true;
var button = null;
var panel = null;
var image = null;
var menuElements = [];
var notification = null;
var notifications = [];
var textnotification = null;
var textnotifications = [];
var currentPage = 0;
var padding = 10;
// Set to True when your menu is ready.
var menuIsReady = false;
var selectedInput = null;
// Animation Stuff
var animationFrames = 0;

class PlayerTextNotification {
    _xPos: number;
    _yPos: number;
    _alpha: number;
    _text: string;
    _increment: number;
    _r: number;
    _g: number;
    _b: number;
    _lastUpdateAlpha: number;
    _lastUpdateTextPosition: number;
    _drawing: boolean;

    constructor(text: string) {
        let playerPos = API.getEntityPosition(API.getLocalPlayer()).Add(new Vector3(0, 0, 1));
        let point = API.worldToScreenMantainRatio(playerPos);
        this._xPos = Point.Round(point).X;
        this._yPos = Point.Round(point).Y;
        this._drawing = true;
        this._alpha = 255;
        this._text = text;
        this._increment = -1;
        this._lastUpdateAlpha = new Date().getTime();
        this._lastUpdateTextPosition = new Date().getTime();
        this._r = 0;
        this._g = 0;
        this._b = 0;
    }

    draw() {
        if (!this._drawing) {
            return;
        }

        if (new Date().getTime() > this._lastUpdateAlpha + 35) { // 60 FPS
            this._lastUpdateAlpha = new Date().getTime();
            this._alpha -= 5;
        }

        if (new Date().getTime() > this._lastUpdateTextPosition + 100) {
            this._yPos -= 0.3;
        }

        API.drawText(this._text, this._xPos, this._yPos, 0.4, this._r, this._g, this._b, this._alpha, 4, 1, true, true, 500);

        if (this._alpha <= 0) {
            this.cleanUpNotification();
        }
    }

    setColor(r, g, b) {
        this._r = r;
        this._g = g;
        this._b = b;
    }

    cleanUpNotification() {
        this._drawing = false;
        textnotification = null;
    }

    returnType() {
        return "PlayerTextNotification";
    }
}

class ProgressBar {
    _xPos: number;
    _yPos: number;
    _width: number;
    _height: number;
    _r: number;
    _g: number;
    _b: number;
    _currentProgress: number;

    constructor(x, y, width, height, currentProgress) {
        this._xPos = x * panelMinX;
        this._yPos = y * panelMinY;
        this._width = width * panelMinX - 10;
        this._height = height * panelMinY - 10;
        this._currentProgress = currentProgress;
        this._r = 0;
        this._g = 0;
        this._b = 0;
    }

    draw() {
        
        API.drawRectangle(this._xPos + 5, this._yPos + 5, ((this._width / 100) * this._currentProgress), this._height, this._r, this._g, this._b, 225);
        API.drawText("" + Math.round(this._currentProgress), this._xPos + (((this._width / 100) * this._currentProgress) / 2), this._yPos, 0.5, 255, 255, 255, 255, 4, 1, false, true, 100);
    }

    setColor(r, g, b) {
        this._r = r;
        this._g = g;
        this._b = b;
    }

    addProgress(value) {
        if (this._currentProgress + value > 100) {
            this._currentProgress = 100;
            return;
        }
        this._currentProgress += value; 
    }

    subtractProgress(value) {
        if (this._currentProgress - value < 0) {
            this._currentProgress = 0;
            return;
        }
        this._currentProgress -= value;
    }

    setProgressAmount(value) {
        if (value >= 100) {
            this._currentProgress = 100;
            return;
        }

        if (value <= 0) {
            this._currentProgress = 0;
            return;
        }

        this._currentProgress = value;
        return;
    }

    returnProgressAmount() {
        return this._currentProgress;
    }

    returnType() {
        return "ProgressBar";
    }
}


class Notification {
    _currentPosX: number;
    _currentPosY: number;
    _targetX: number;
    _targetY: number;
    _width: number;
    _height: number;
    _text: string;
    _r: number;
    _g: number;
    _b: number;
    _alpha: number;
    _textScale: number;
    _offset: number;
    _lastUpdateTime: number;
    _currentPhase: number;
    _displayTime: number;
    _running: boolean;
    _sound: boolean;
    _incrementer: number;

    constructor(text, displayTime) {
        this._currentPosX = 26 * panelMinX; // Starting Position
        this._currentPosY = screenY; // Starting Position Y
        this._targetX = 26 * panelMinX; // Ending Position
        this._targetY = 15 * panelMinY; // Ending Position Y
        this._width = panelMinX * 5;
        this._height = panelMinY * 3;
        // Text Settings
        this._text = text;
        this._r = 255;
        this._g = 165;
        this._b = 0;
        this._offset = 0;
        this._textScale = 0.5;
        // Animation Settings
        this._lastUpdateTime = new Date().getTime(); //ms
        this._alpha = 255;
        this._displayTime = displayTime;
        this._incrementer = 0;
        // Sound Settings
        this._sound = true;
    }

    draw() {
        if (notification !== this) {
            return;
        }

        if (this._sound) {
            this._sound = false;
            API.playSoundFrontEnd("GOLF_NEW_RECORD", "HUD_AWARDS");
        }

        // Starts below max screen.
        API.drawRectangle(this._currentPosX, this._currentPosY - 5, this._width, 5, this._r, this._g, this._b, this._alpha - 30);
        API.drawRectangle(this._currentPosX, this._currentPosY, this._width, this._height, 0, 0, 0, this._alpha - 30);
        API.drawText(this._text, this._offset + this._currentPosX + (this._width / 2), this._currentPosY + (this._height / 4), this._textScale, 255, 255, 255, this._alpha, 4, 1, false, false, this._width - padding);
        this.animate();
    }

    animate() {
        // Did we reach our goal?
        if (this._currentPosY <= this._targetY) {
            this._currentPosY = this._targetY;
            // Ready to fade?
            if (new Date().getTime() > this._lastUpdateTime + this._displayTime) {
                this.fade();
                return;
            }
            return;
        }

        this._lastUpdateTime = new Date().getTime();
        // If not let's reach our goal.
        if (this._currentPosY <= this._targetY + (this._height / 6)) {
            this._currentPosY -= 3;
            return;
        } else {
            this._currentPosY -= 5;
            return;
        }
    }

    fade() {
        if (this._alpha <= 0) {
            this.cleanUpNotification();
            return;
        }

        this._alpha -= 5;
        return;
    }

    cleanUpNotification() {
        animationFrames = 0;
        notification = null;
    }

    setText(value) {
        this._text = value;
    }

    setColor(r, g, b) {
        this._r = r;
        this._g = g;
        this._b = b;
    }

    setTextScale(value) {
        this._textScale = value;
    }

    isHovered() {
        return;
    }

    isClicked() {
        return;
    }

    returnType() {
        return "Notification";
    }
}

API.onResourceStart.connect(function () {
    resource.menu_builder.setupMenu(1);
    let panel: Panel = createPanel(0, 12, 1, 8, 2);
    panel.Header = true;
    panel.Tooltip = "A header tooltip.";
    panel.MainColorR = 187;
    panel.MainColorG = 77;
    panel.MainColorB = 62;
    panel.MainAlpha = 255;

    let textElement: TextElement = panel.addText("Menu Builder");
    textElement.R = 255;
    textElement.G = 255;
    textElement.B = 255;
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.FontScale = 0.6;
    textElement.Font = 1;
    textElement.HoverAlpha = 100;
    
    panel.HoverR = 113;
    panel.HoverG = 47;
    panel.HoverB = 38;
    panel.HoverAlpha = 255;
    panel.Hoverable = true;
    panel.Function = doNothing;
    // Panel 2
    panel = createPanel(0, 12, 3, 8, 10);
    panel.MainColorR = 48;
    panel.MainColorG = 47;
    panel.MainColorB = 47;
    panel.MainAlpha = 255;
    textElement = panel.addText("Beautiful UI made very easy but how easy you may say");
    textElement.Font = 2;
    textElement.FontScale = 0.4;
    textElement.Centered = true;
    textElement = panel.addText("- Create text lines for your panels.");
    textElement.Font = 4;
    textElement.FontScale = 0.4;
    textElement = panel.addText("- Modify each piece of your design with a few lines of code.");
    textElement.Font = 1;
    textElement.FontScale = 0.4;
    textElement.Alpha = 100;
    textElement = panel.addText("- Fast and it doesn't even use CEF.");
    textElement.Font = 0;
    textElement.FontScale = 0.3;
    textElement = panel.addText("- Supports inline ~r~c ~o~o ~y~l ~g~o ~b~r ~p~s ~w~supported by GTANetwork.");
    textElement.Font = 7;
    textElement.FontScale = 0.3;
    textElement = panel.addText("- Can even center per line.");
    textElement.Font = 7;
    textElement.FontScale = 0.3;
    textElement.Centered = true;
    // Panel 3
    panel = createPanel(0, 20, 3, 8, 5);
    panel.MainBackgroundImage = "clientside/resources/images/backgrounds/background_0.jpg";
    panel.MainBackgroundImagePadding = 10;
    panel.MainColorR = 65;
    panel.MainColorG = 64;
    panel.MainColorB = 64;
    panel.MainAlpha = 255;
    panel.Function = doNothing;
    panel.FunctionAudioLib = "Enter_Capture_Zone";
    panel.FunctionAudioName = "DLC_Apartments_Drop_Zone_Sounds";
    resource.menu_builder.openMenu(true, false, false, true, false);
});


function doNothing() {
    API.sendChatMessage("We're doing nothing.");
}





















class TextElement {
    // Positioning
    private _xPos: number;
    private _yPos: number;
    private _width: number;
    private _height: number;
    private _line: number;
    private _padding: number;
    private _hovered: boolean;
    // Main Elements
    private _text: string;
    private _font: number;
    private _fontR: number;
    private _fontG: number;
    private _fontB: number;
    private _fontAlpha: number;
    private _fontScale: number;
    private _shadow: boolean;
    private _outline: boolean;
    // Hover Text
    private _hoverTextR: number;
    private _hoverTextG: number;
    private _hoverTextB: number;
    private _hoverTextAlpha: number;
    // Should we center it?
    private _centered: boolean;
    private _centeredVertically: boolean;
    private _offset: number;
    // Constructor
    constructor(text: string, x: number, y: number, width: number, height: number, line: number) {
        this._xPos = x;
        this._yPos = y + (panelMinY * line);
        this._width = width;
        this._height = height;
        this._text = text;
        this._fontScale = 0.6;
        this._centered = false;
        this._centeredVertically = false;
        this._font = 4;
        this._fontR = 255;
        this._fontG = 255;
        this._fontB = 255;
        this._fontAlpha = 255;
        this._hoverTextAlpha = 255;
        this._hoverTextR = 255;
        this._hoverTextG = 255;
        this._hoverTextB = 255;
        this._offset = 0;
        this._padding = 10;
        this._hovered = false;
        this._shadow = false;
        this._outline = false;
    }

    draw() {
        if (this._centered && this._centeredVertically) {
            this.drawAsCenteredAll();
            return;
        }

        if (this._centered) {
            this.drawAsCentered();
            return;
        }

        if (this._centeredVertically) {
            this.drawAsCenteredVertically();
            return;
        }

        this.drawAsNormal();
    }

    //** Is this text element in a hover state? */
    set Hovered(value: boolean) {
        this._hovered = value;
    }

    get Hovered(): boolean {
        return this._hovered;
    }

    /** Sets the color for RGB of R type. Max of 255 */
    set R(value: number) {
        this._fontR = value;
    }

    /** Gets the color for RGB of R type. */
    get R(): number {
        return this._fontR;
    }

    /** Sets the color for RGB of G type. Max of 255 */
    set G(value: number) {
        this._fontG = value;
    }

    /** Gets the color for RGB of G type. */
    get G(): number {
        return this._fontG;
    }

    /** Sets the color for RGB of B type. Max of 255 */
    set B(value: number) {
        this._fontB = value;
    }

    /** Gets the color for RGB of B type. */
    get B(): number {
        return this._fontB;
    }

    /** Sets the font Alpha property */
    set Alpha(value: number) {
        this._fontAlpha = value;
    }

    get Alpha(): number {
        return this._fontAlpha;
    }

    /** Sets the font Alpha property */
    set HoverAlpha(value: number) {
        this._hoverTextAlpha = value;
    }

    get HoverAlpha(): number {
        return this._hoverTextAlpha;
    }

    /** Sets the hover color for the text RGB of R type. */
    set HoverR(value: number) {
        this._hoverTextR = value;
    }

    get HoverR(): number {
        return this._hoverTextR;
    }

    /** Sets the hover color for the text RGB of G type. */
    set HoverG(value: number) {
        this._hoverTextG = value;
    }

    get HoverG(): number {
        return this._hoverTextG;
    }

    /** Sets the hover color for the text RGB of B type. */
    set HoverB(value: number) {
        this._hoverTextB = value;
    }

    get HoverB(): number {
        return this._hoverTextB;
    }

    /** Set your font type. 0 - 7 
    * 0 Normal
    * 1 Cursive
    * 2 All Caps
    * 3 Squares / Arrows / Etc.
    * 4 Condensed Normal
    * 5 Garbage
    * 6 Condensed Normal
    * 7 Bold GTA Style
    */
    set Font(value: number) {
        this._font = value;
    }

    get Font(): number {
        return this._font;
    }

    /** Sets the size of the text. 0.6 is pretty normal. 1 is quite large. */
    set FontScale(value: number) {
        this._fontScale = value;
    }

    get FontScale(): number {
        return this._fontScale;
    }

    /** Centers the content vertically. Do not use if your box is not very high to begin with */
    set VerticalCentered(value: boolean) {
        this._centeredVertically = value;
    }

    get VerticalCentered(): boolean {
        return this._centeredVertically;
    }

    /** Use this if you want centered content. */
    set Centered(value: boolean) {
        this._centered = value;
    }

    get Centered(): boolean {
        return this._centered;
    }

    private drawAsCenteredAll() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 20, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 1, this._shadow, this._outline, this._width);
            return;
        }

        API.drawText(this._text, this._offset + this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 20, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 1, this._shadow, this._outline, this._width);
    }
    private drawAsCenteredVertically() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos, this._yPos + (this._height / 2) - 20, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 0, this._shadow, this._outline, this._width);
            return;
        }
        API.drawText(this._text, this._offset + this._xPos, this._yPos + (this._height / 2) - 20, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 0, this._shadow, this._outline, this._width);
    }
    private drawAsCentered() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos + (this._width / 2), this._padding + this._yPos, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 1, this._shadow, this._outline, this._width);
            return;
        }
        API.drawText(this._text, this._offset + this._xPos + (this._width / 2), this._padding + this._yPos, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 1, this._shadow, this._outline, this._width);
    }
    private drawAsNormal() {
        if (this._hovered) {
            API.drawText(this._text, this._offset + this._xPos + this._padding, this._yPos + this._padding, this._fontScale, this._hoverTextR, this._hoverTextG, this._hoverTextB, this._hoverTextAlpha, this._font, 0, this._shadow, this._outline, this._width - this._padding);
            return;
        }
        API.drawText(this._text, this._offset + this._xPos + this._padding, this._yPos + this._padding, this._fontScale, this._fontR, this._fontG, this._fontB, this._fontAlpha, this._font, 0, this._shadow, this._outline, this._width - this._padding);
    }
}

class Panel {
    private _xPos: number;
    private _yPos: number;
    private _width: number;
    private _height: number;
    private _line: number;
    private _header: boolean;
    private _offset: number;
    private _r: number;
    private _g: number;
    private _b: number;
    private _textLines: TextElement[];
    private _currentLine: number;
    private _alpha: number;
    private _shadow: boolean;
    private _outline: boolean;
    private _padding: number;
    private _tooltip: string;
    private _hoverable: boolean;
    private _hoverTime: number;
    private _hovered: boolean;
    private _hoverR: number;
    private _hoverG: number;
    private _hoverB: number;
    private _hoverAlpha: number;
    private _hoverAudioLib: string;
    private _hoverAudioName: string;
    private _hoverAudio: boolean;
    private _backgroundImage: string;
    private _backgroundImagePadding: number;
    private _function: any;
    private _functionArgs: any[];
    private _functionAudioLib: string;
    private _functionAudioName: string;
    private _functionClickAudio: boolean;

    /**
     * 
     * @param x - Max of 31. Starts on left side.
     * @param y - Max of 17. Starts at the top.
     * @param width - Max of 31. Each number fills a square.
     * @param height - Max of 17. Each number fills a square.
     */
    constructor(x, y, width, height) {
        this._padding = 10;
        this._xPos = x * panelMinX;
        this._yPos = y * panelMinY;
        this._width = width * panelMinX;
        this._height = height * panelMinY
        this._alpha = 225;
        this._header = false;
        this._offset = 0;
        this._r = 0;
        this._g = 0;
        this._b = 0;
        this._textLines = [];
        this._currentLine = 0;
        this._shadow = false;
        this._outline = false;
        this._tooltip = null;
        this._hovered = false;
        this._hoverTime = 0;
        this._hoverR = 0;
        this._hoverG = 0;
        this._hoverB = 0;
        this._hoverAlpha = 200;
        this._backgroundImage = null;
        this._backgroundImagePadding = 0;
        this._function = null;
        this._functionArgs = [];
        this._functionAudioLib = "Click";
        this._functionAudioName = "DLC_HEIST_HACKING_SNAKE_SOUNDS";
        this._hoverAudioLib = "Cycle_Item";
        this._hoverAudioName = "DLC_Dmod_Prop_Editor_Sounds";
        this._hoverAudio = true;
        this._functionClickAudio = true;
        this._hoverable = false;
        this._line = 0;
    }
    /**
     * Do not call this. It's specifically used for the menu builder file.
     */
    draw() {
        // Only used if using text lines.
        if (this._textLines.length > 0) {
            for (var i = 0; i < this._textLines.length; i++) {
                this._textLines[i].draw();
            }
        }
        this.drawRectangles();
        this.isClicked();
        this.isHovered();
    }
    // Normal Versions
    private drawRectangles() {
        if (this._backgroundImage !== null) {
            this.drawBackgroundImage();
            return;
        }


        if (this._hovered) {
            API.drawRectangle(this._xPos, this._yPos, this._width, this._height, this._hoverR, this._hoverG, this._hoverB, this._hoverAlpha);
            if (this._header) {
                API.drawRectangle(this._xPos, this._yPos + this._height - 5, this._width, 5, 255, 255, 255, 50);
            }
            return;
        }

        API.drawRectangle(this._xPos, this._yPos, this._width, this._height, this._r, this._g, this._b, this._alpha);
        if (this._header) {
            API.drawRectangle(this._xPos, this._yPos + this._height - 5, this._width, 5, 255, 255, 255, 50);
        }
    }
    private drawBackgroundImage() {
        if (this._backgroundImagePadding > 1) {
            API.dxDrawTexture(this._backgroundImage, new Point(this._xPos + this._backgroundImagePadding, this._yPos + this._backgroundImagePadding), new Size(this._width - (this._backgroundImagePadding * 2), this._height - (this._backgroundImagePadding * 2)), 0);
            API.drawRectangle(this._xPos, this._yPos, this._width, this._height, this._r, this._g, this._b, this._alpha);
            return;
        }
        API.dxDrawTexture(this._backgroundImage, new Point(this._xPos, this._yPos), new Size(this._width, this._height), 0);
    }
    // Function Settings
    set Function(value: any) {
        this._function = value;
    }

    /** Add an array or a single value as a function. IMPORTANT! Any function you write must be able to take an array of arguments. */
    public addFunctionArgs(value: any) {
        if (Array.isArray(value)) {
            this._functionArgs = value;
        } else {
            this._functionArgs.push(value);
        }
    }


    // HOVER AUDIO
    /** Sets the hover audio library. Ex: "Cycle_Item" */
    set HoverAudioLib(value: string) {
        this._hoverAudioLib = value;
    }

    get HoverAudioLib(): string {
        return this._hoverAudioLib;
    }

    /** Sets the hover audio name. Ex: "DLC_Dmod_Prop_Editor_Sounds" */
    set HoverAudioName(value: string) {
        this._hoverAudioName = value;
    }

    get HoverAudioName(): string {
        return this._hoverAudioName;
    }

    // FUNCTION AUDIO
    /** Sets the function audio library. Ex: "Cycle_Item" */
    set FunctionAudioLib(value: string) {
        this._functionAudioLib = value;
    }

    get FunctionAudioLib(): string {
        return this._functionAudioLib;
    }

    /** Sets the function audio name. Ex: "DLC_Dmod_Prop_Editor_Sounds" */
    set FunctionAudioName(value: string) {
        this._functionAudioName = value;
    }

    get FunctionAudioName(): string {
        return this._functionAudioName;
    }

    /** Sets if the function audio plays. */
    set FunctionAudio(value: boolean) {
        this._functionClickAudio = value;
    }

    get FunctionAudio(): boolean {
        return this._functionClickAudio;
    }

    // Background Alpha
    /** Sets the background alpha property */
    set MainAlpha(value: number) {
        this._alpha = value;
    }

    get MainAlpha(): number {
        return this._alpha;
    }

    /** Sets the background image padding property */
    set MainBackgroundImagePadding(value: number) {
        this._backgroundImagePadding = value;
    }

    get MainBackgroundImagePadding(): number {
        return this._backgroundImagePadding;
    }

    /** Uses a custom image for your panel background. Must include extension. EX. 'clientside/image.jpg' */
    set MainBackgroundImage(value: string) {
        this._backgroundImage = value;
    }

    get MainBackgroundImage(): string {
        return this._backgroundImage;
    }

    /** Sets the color for RGB of R type. Max of 255 */
    set MainColorR(value: number) {
        this._r = value;
    }

    /** Gets the color for RGB of R type. */
    get MainColorR(): number {
        return this._r;
    }

    /** Sets the color for RGB of G type. Max of 255 */
    set MainColorG(value: number) {
        this._g = value;
    }

    /** Gets the color for RGB of G type. */
    get MainColorG(): number {
        return this._g;
    }

    /** Sets the color for RGB of B type. Max of 255 */
    set MainColorB(value: number) {
        this._b = value;
    }

    /** Gets the color for RGB of B type. */
    get MainColorB(): number {
        return this._b;
    }

    /** Is there a hover state? */
    set Hoverable(value: boolean) {
        this._hoverable = value;
    }

    get Hoverable(): boolean {
        return this._hoverable;
    }

    /** Sets the hover alpha */
    set HoverAlpha(value: number) {
        this._hoverAlpha = value;
    }

    get HoverAlpha(): number {
        return this._hoverAlpha;
    }

    /** Sets the hover color for RGB of R type. */
    set HoverR(value: number) {
        this._hoverR = value;
    }

    get HoverR(): number {
        return this._hoverR;
    }

    /** Sets the hover color for RGB of G type. */
    set HoverG(value: number) {
        this._hoverG = value;
    }

    get HoverG(): number {
        return this._hoverG;
    }

    /** Sets the hover color for RGB of B type. */
    set HoverB(value: number) {
        this._hoverB = value;
    }

    get HoverB(): number {
        return this._hoverB;
    }

    /** Sets the font Outline property */
    set FontOutline(value: boolean) {
        this._outline = value;
    }

    get FontOutline(): boolean {
        return this._outline;
    }

    /** Sets the font Shadow property */
    set FontShadow(value: boolean) {
        this._shadow = value;
    }

    get FontShadow(): boolean {
        return this._shadow;
    }

    /** Sets the Tooltip text for your element. */
    set Tooltip(value: string) {
        this._tooltip = value;
    }

    get Tooltip(): string {
        return this._tooltip;
    }

    /** Adds a stylized line under your your box. */
    set Header(value: boolean) {
        this._header = value;
    }

    get Header(): boolean {
        return this._header;
    }

    /** If your text needs to be pushed in a certain direction either add or remove pixels here. */
    set Offset(value: number) {
        this._offset = value;
    }

    get Offset(): number {
        return this._offset;
    }

    addText(value: string) {
        let textElement: TextElement = new TextElement(value, this._xPos, this._yPos, this._width, this._height, this._line);
        this._textLines.push(textElement);
        this._line += 1;
        return textElement;
    }

    // Hover Action
    isHovered() {
        if (!API.isCursorShown()) {
            return;
        }

        if (!this._hoverable) {
            return;
        }

        let cursorPos = API.getCursorPositionMantainRatio();
        if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + this._width) && cursorPos.Y > this._yPos && cursorPos.Y < this._yPos + this._height) {
            if (!this._hovered) {
                this._hovered = true;
                this.setTextHoverState(true);
                if (this._hoverAudio) {
                    API.playSoundFrontEnd(this._hoverAudioLib, this._hoverAudioName);
                }
            }
            
            this._hoverTime += 1;

            if (this._hoverTime > 50) {
                if (this._tooltip.length > 1) {
                    API.drawText(this._tooltip, cursorPos.X + 25, cursorPos.Y, 0.4, 255, 255, 255, 255, 4, 0, true, true, 200);
                }
            }
            return;
        }

        this._hovered = false;
        this._hoverTime = 0;
        this.setTextHoverState(false);
    }

    // Click Action
    isClicked() {
        // Is there even a cursor?
        if (!API.isCursorShown()) {
            return;
        }

        // Is there a function if they click it?
        if (this._function === null) {
            return;
        }

        // Are they even left clicking?
        if (!API.isControlJustPressed(Enums.Controls.CursorAccept)) {
            return;
        }

        let cursorPos = API.getCursorPositionMantainRatio();
        if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + this._width) && cursorPos.Y > this._yPos && cursorPos.Y < this._yPos + this._height) {
            if (this._functionClickAudio) {
                API.playSoundFrontEnd(this._functionAudioLib, this._functionAudioName);
            }

            if (this._function !== null) {
                if (this._functionArgs.length > 0) {
                    this._function(this._functionArgs);
                    return;
                }

                this._function();
            }
        }
    }

    setTextHoverState(value: boolean) {
        for (var i = 0; i < this._textLines.length; i++) {
            this._textLines[i].Hovered = value;
        }
    }

    // Type
    returnType() {
        return "Panel";
    }
}

class InputPanel {
    _xPos: number;
    _yPos: number;
    _width: number;
    _height: number;
    _input: string;
    _protected: boolean;
    _hovered: boolean;
    _selected: boolean;
    _numeric: boolean;
    _isError: boolean;
    _isTransparent: boolean;

    constructor(x, y, width, height, isPasswordProtected, isSelected) {
        this._xPos = x * panelMinX;
        this._yPos = y * panelMinY;
        this._width = width * panelMinX;
        this._height = height * panelMinY;
        this._protected = isPasswordProtected;
        this._input = "";
        this._hovered = false;
        this._selected = isSelected;
        this._numeric = false;
        this._isError = false;
        this._isTransparent = false;
    }

    draw() {
        if (this._selected) {
            if (!this._isTransparent) {
                API.drawRectangle(this._xPos, this._yPos, this._width, this._height, 0, 0, 0, 225); // Darker Black
            }
            API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, 255, 255, 255, 200);
            if (this._protected) {
                if (this._input.length < 1) {
                    return;
                }
                API.drawText("*".repeat(this._input.length), this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, 0.4, 0, 0, 0, 255, 4, 1, false, false, (panelMinX * this._width));
            } else {
                API.drawText(this._input, this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, 0.4, 0, 0, 0, 255, 4, 1, false, false, (panelMinX * this._width));
            }
            
            return;
        }

        if (this._hovered) { // Hovered
            if (!this._isTransparent) {
                API.drawRectangle(this._xPos, this._yPos, this._width, this._height, 0, 0, 0, 225); // Darker Black
            }
            if (this._isError) {
                API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, 255, 0, 0, 100);
            } else {
                API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, 255, 255, 255, 150);
            }
            
            if (this._protected) {
                if (this._input.length < 1) {
                    return;
                }
                API.drawText("*".repeat(this._input.length), this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, 0.4, 0, 0, 0, 255, 4, 1, false, false, (panelMinX * this._width));
            } else {
                API.drawText(this._input, this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, 0.4, 0, 0, 0, 255, 4, 1, false, false, (panelMinX * this._width));
            }
            //API.drawText(this._input, this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, 0.5, this.r, this.g, this.b, 255, 4, 1, false, false, (panelMinX * this.Width));
        } else { // Not Hovered
            if (!this._isTransparent) {
                API.drawRectangle(this._xPos, this._yPos, this._width, this._height, 0, 0, 0, 225); // Darker Black
            }
            if (this._isError) {
                API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, 255, 0, 0, 100);
            } else {
                API.drawRectangle(this._xPos + 10, this._yPos + 10, this._width - 20, this._height - 20, 255, 255, 255, 100);
            }
            if (this._protected) {
                if (this._input.length < 1) {
                    return;
                }
                API.drawText("*".repeat(this._input.length), this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, 0.4, 0, 0, 0, 255, 4, 1, false, false, (panelMinX * this._width));
            } else {
                API.drawText(this._input, this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, 0.4, 0, 0, 0, 255, 4, 1, false, false, (panelMinX * this._width));
            }
            //API.drawText(this._input, this._xPos + (this._width / 2), this._yPos + (this._height / 2) - 14, 0.5, this.r, this.g, this.b, 50, 4, 1, false, false, (panelMinX * this.Width));
        }
    }

    isHovered() {
        if (API.isCursorShown()) {
            let cursorPos = API.getCursorPositionMantainRatio();
            if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + this._width) && cursorPos.Y > this._yPos && cursorPos.Y < (this._yPos + this._height)) {
                this._hovered = true;
            } else {
                this._hovered = false;
            }
        }
    }

    isError(value) {
        this._isError = value;
    }

    setSelected() {
        selectedInput = this;
        this._selected = true;
    }

    setUnselected() {
        this._selected = false;
    }

    setTransparent() {
        this._isTransparent = true;
    }

    isClicked() {
        let cursorPos = API.getCursorPositionMantainRatio();
        if (cursorPos.X > this._xPos && cursorPos.X < (this._xPos + this._width) && cursorPos.Y > this._yPos && cursorPos.Y < (this._yPos + this._height)) {
            this._selected = true;
            selectedInput = this;
            return;
        } else {
            this._selected = false;
            return;
        }
    }

    addToInput(text: string) {
        if (this._input.length > 2147483647) {
            return;
        }

        if (!this._numeric) {
            this._input += text;
            return this._input;
        } else {
            if (Number.isInteger(+text)) {
                this._input += text;
                return this._input;
            }
        }
    }

    setInput(value) {
        this._input = value;
    }

    removeFromInput() {
        this._input = this._input.substring(0, this._input.length - 1);
    }

    returnInput() {
        return this._input;
    }

    setNumericOnly() {
        this._numeric = true;
    }

    returnType() {
        return "InputPanel";
    }
}

// On-Update Event -- Draws all of our stuff.
API.onUpdate.connect(function () {
    // Notifications can be global.
    drawNotification();
    drawTextNotification();

    if (!menuIsReady) {
        return;
    }

    if (menuElements.length === 0) {
        return;
    }

    if (menuElements[currentPage].length === 0) {
        return;
    }

    drawAllMenuElements();
});

// On-Keydown Event
API.onKeyDown.connect(function (sender, e) {
    if (!menuIsReady) {
        return;
    }

    if (selectedInput === null) {
        return;
    }

    if (e.KeyCode === Keys.Back) {
        selectedInput.removeFromInput();
        return;
    }

    let shiftOn = false;
    if (e.Shift) {
        shiftOn = true;
    }

    let keypress = "";
    switch (e.KeyCode) {
        case Keys.Space:
            keypress = " ";
            break;
        case Keys.A:
            keypress = "a";
            if (shiftOn) {
                keypress = "A";
            }
            break;
        case Keys.B:
            keypress = "b";
            if (shiftOn) {
                keypress = "B";
            }
            break;
        case Keys.C:
            keypress = "c";
            if (shiftOn) {
                keypress = "C";
            }
            break;
        case Keys.D:
            keypress = "d";
            if (shiftOn) {
                keypress = "D";
            }
            break;
        case Keys.E:
            keypress = "e";
            if (shiftOn) {
                keypress = "E";
            }
            break;
        case Keys.F:
            keypress = "f";
            if (shiftOn) {
                keypress = "F";
            }
            break;
        case Keys.G:
            keypress = "g";
            if (shiftOn) {
                keypress = "G";
            }
            break;
        case Keys.H:
            keypress = "h";
            if (shiftOn) {
                keypress = "H";
            }
            break;
        case Keys.I:
            keypress = "i";
            if (shiftOn) {
                keypress = "I";
            }
            break;
        case Keys.J:
            keypress = "j";
            if (shiftOn) {
                keypress = "J";
            }
            break;
        case Keys.K:
            keypress = "k";
            if (shiftOn) {
                keypress = "K";
            }
            break;
        case Keys.L:
            keypress = "l";
            if (shiftOn) {
                keypress = "L";
            }
            break;
        case Keys.M:
            keypress = "m";
            if (shiftOn) {
                keypress = "M";
            }
            break;
        case Keys.N:
            keypress = "n";
            if (shiftOn) {
                keypress = "N";
            }
            break;
        case Keys.O:
            keypress = "o";
            if (shiftOn) {
                keypress = "O";
            }
            break;
        case Keys.P:
            keypress = "p";
            if (shiftOn) {
                keypress = "P";
            }
            break;
        case Keys.Q:
            keypress = "q";
            if (shiftOn) {
                keypress = "Q";
            }
            break;
        case Keys.R:
            keypress = "r";
            if (shiftOn) {
                keypress = "R";
            }
            break;
        case Keys.S:
            keypress = "s";
            if (shiftOn) {
                keypress = "S";
            }
            break;
        case Keys.T:
            keypress = "t";
            if (shiftOn) {
                keypress = "T";
            }
            break;
        case Keys.U:
            keypress = "u";
            if (shiftOn) {
                keypress = "U";
            }
            break;
        case Keys.V:
            keypress = "v";
            if (shiftOn) {
                keypress = "V";
            }
            break;
        case Keys.W:
            keypress = "w";
            if (shiftOn) {
                keypress = "W";
            }
            break;
        case Keys.X:
            keypress = "x";
            if (shiftOn) {
                keypress = "X";
            }
            break;
        case Keys.Y:
            keypress = "y";
            if (shiftOn) {
                keypress = "Y";
            }
            break;
        case Keys.Z:
            keypress = "z";
            if (shiftOn) {
                keypress = "Z";
            }
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
        case Keys.OemMinus:
            keypress = "-";
            if (shiftOn) {
                keypress = "_";
            }
            break;
        case Keys.Oemplus:
            keypress = "=";
            if (shiftOn) {
                keypress = "+";
            }
            break;
        case Keys.OemQuestion:
            keypress = "/";
            if (shiftOn) {
                keypress = "?";
            }
            break;
        case Keys.Oemcomma:
            keypress = ",";
            if (shiftOn) {
                keypress = "<";
            }
            break;
        case Keys.OemPeriod:
            keypress = ".";
            if (shiftOn) {
                keypress = ">";
            }
            break;
        case Keys.OemSemicolon:
            keypress = ";";
            if (shiftOn) {
                keypress = ":";
            }
            break;
        case Keys.OemOpenBrackets:
            keypress = "[";
            if (shiftOn) {
                keypress = "{";
            }
            break;
        case Keys.OemCloseBrackets:
            keypress = "]";
            if (shiftOn) {
                keypress = "}";
            }
            break;
        case Keys.NumPad0:
            keypress = "0";
            break;
        case Keys.NumPad1:
            keypress = "1";
            break;
        case Keys.NumPad2:
            keypress = "2";
            break;
        case Keys.NumPad3:
            keypress = "3";
            break;
        case Keys.NumPad4:
            keypress = "4";
            break;
        case Keys.NumPad5:
            keypress = "5";
            break;
        case Keys.NumPad6:
            keypress = "6";
            break;
        case Keys.NumPad7:
            keypress = "7";
            break;
        case Keys.NumPad8:
            keypress = "8";
            break;
        case Keys.NumPad9:
            keypress = "9";
            break;
    }

    if (keypress === "") {
        return;
    }

    if (keypress.length > 0) {
        selectedInput.addToInput(keypress);
    } else {
        return;
    }
});
// Goes to Next Page
function nextPage() {
    if (currentPage + 1 > menuElements.length - 1) {
        currentPage = 0;
    } else {
        currentPage += 1;
    }
}
// Goes to Previous Page
function prevPage() {
    if (currentPage - 1 < 0) {
        currentPage = menuElements.length - 1;
    } else {
        currentPage -= 1;
    }
}
// Set Page
function setPage(value) {
    currentPage = value;
}
function drawTextNotification() {
    if (textnotification !== null) {
        textnotification.draw();
        return;
    }

    if (textnotifications.length <= 0) {
        return;
    }

    textnotification = textnotifications.shift();
    return;
}
function drawNotification() {
    if (notification !== null) {
        notification.draw();
        return;
    }

    if (notifications.length <= 0) {
        return;
    }

    notification = notifications.shift();
    return;
}
// Draws all elements.
function drawAllMenuElements() {
    if (!menuIsReady) {
        return;
    }

    if (Array.isArray(menuElements[currentPage])) {
        for (var i = 0; i < menuElements[currentPage].length; i++) {
            // This will draw each element.
            menuElements[currentPage][i].draw();
            //menuElements[currentPage][i].isHovered();
            //if (API.isControlJustPressed(Enums.Controls.CursorAccept)) {
            //    menuElements[currentPage][i].isClicked();
            //}
        }
    }
}

// Ready to draw the menu?
function setMenuReady(isReady: boolean) {
    menuIsReady = isReady;
}
// Setup our pages with arrays. This is the first thing we should call.
function setupMenu(numberOfPages: number) {
    for (var i = 0; i < numberOfPages; i++) {
        let emptyArray = [];
        menuElements.push(emptyArray);
    }
}
// Add a page to our pages array.
function createPanel(page: number, xStart: number, yStart: number, xGridWidth: number, yGridHeight: number) {
    panel = new Panel(xStart, yStart, xGridWidth, yGridHeight);
    menuElements[page].push(panel);
    return panel;
}
// Add a button to our pages array.
function createButton(page: number, xStart: number, yStart: number, xGridWidth: number, yGridHeight: number, type: number, text: any) {
    button = new Button(xStart, yStart, xGridWidth, yGridHeight, type, text);
    menuElements[page].push(button);
    return button;
}
// Add a static input to our pages array.
function createInput(page: number, xStart: number, yStart: number, xGridWidth: number, yGridHeight: number, isPasswordProtected: boolean, isSelected: boolean) {
    panel = new InputPanel(xStart, yStart, xGridWidth, yGridHeight, isPasswordProtected, isSelected);
    menuElements[page].push(panel);
    return panel;
}
function createImage(page: number, path: string, x: number, y: number, width: number, height: number) {
    panel = new PanelImage(path, x, y, width, height);
    menuElements[page].push(panel);
    return panel;
}
function createNotification(page: number, text: string, displayTime: number) {
    // Add to queue.
    let notify = new Notification(text, displayTime);
    notifications.push(notify);
    return notify;
}
function createPlayerTextNotification(text: string) {
    let notify = new PlayerTextNotification(text);
    textnotifications.push(notify);
    return notify;
}
function createProgressBar(page: number, x: number, y: number, width: number, height: number, currentProgress: number) {
    let bar = new ProgressBar(x, y, width, height, currentProgress);
    menuElements[page].push(bar);
    return bar;
}   
function getCurrentPage() {
    return currentPage;
}
// Clears the menu entirely.
function exitMenu(cursor: boolean, hud: boolean, chat: boolean, blur: boolean, canOpenChat: boolean) {
    menuIsReady = false;
    
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

    if (canOpenChat) {
        API.setCanOpenChat(true);
    } else {
        API.setCanOpenChat(false);
    }

    menuElements = [[]];
    selectedInput = null;
    currentPage = 0;
}

function killMenu() {
    menuIsReady = false;
    selectedInput = null;
    API.showCursor(false);
    API.setHudVisible(true);
    API.setChatVisible(true);
    API.setCanOpenChat(true);
    API.callNative("_TRANSITION_FROM_BLURRED", 3000);
    menuElements = [[]];
    currentPage = 0;
}

function openMenu(cursor: boolean, hud: boolean, chat: boolean, blur: boolean, canOpenChat: boolean) {
    if (blur === true) {
        API.callNative("_TRANSITION_TO_BLURRED", 3000);
    }

    currentPage = 0;
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

    if (canOpenChat) {
        API.setCanOpenChat(true);
    } else {
        API.setCanOpenChat(false);
    }
}