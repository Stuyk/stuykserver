var sprites = Array();
class Sprite {
    constructor() {
        this._paths = [];
        this._rotation = 0;
        this._ticksPerFrame = 4;
        this._currentFrame = 0;
        this._width = 0;
        this._height = 0;
        this._size = new Size(0, 0);
        this._point = new Point(0, 0);
        this._loops = 0;
        this._currentLoop = 0;
    }
    // Height
    set Height(value) {
        this._height = value;
        this._size = new Size(this._width, this._height);
    }
    get Height() {
        return this._height;
    }
    // Width
    set Width(value) {
        this._width = value;
        this._size = new Size(this._width, this._height);
    }
    get Width() {
        return this._width;
    }
    // X Position
    set XPos(value) {
        this._x = value;
        this._point = new Point(this._x, this._y);
    }
    get XPos() {
        return this._x;
    }
    // Y Position
    set YPos(value) {
        this._y = value;
        this._point = new Point(this._x, this._y);
    }
    get YPos() {
        return this._y;
    }
    // Ticks Per Frame
    set TicksPerFrame(value) {
        this._ticksPerFrame = value;
    }
    // Add a String Path
    set AddFrame(value) {
        this._paths.push(value);
    }
    // Set how many times it should play.
    set LoopAmount(value) {
        this._loops = value;
    }
    render() {
        if (this._paths.length < 1) {
            return;
        }
        if (this._currentFrame > this._paths.length - 1) {
            this._currentFrame = 0;
            if (this._loops < 1) {
                return;
            }
            if (this._currentLoop < this._loops) {
                this._currentLoop++;
                if (this._currentLoop >= this._loops) {
                    this.delete();
                    return;
                }
            }
        }
        this._tick++;
        if (this._tick < this._ticksPerFrame) {
            return;
        }
        this._tick = 0;
        API.dxDrawTexture(this._paths[this._currentFrame], this._point, this._size, this._rotation);
        this._currentFrame++;
    }
    delete() {
        if (sprites.length < 1) {
            return;
        }
        let removalIndex;
        for (var i = 0; i < sprites.length; i++) {
            if (sprites[i] === this) {
                removalIndex = i;
                break;
            }
        }
        sprites.splice(removalIndex, 1);
    }
    ready() {
        sprites.push(this);
    }
}
// Update Function
API.onUpdate.connect(function () {
    if (sprites.length < 1) {
        return;
    }
    for (var i = 0; i < sprites.length; i++) {
        sprites[i].render();
    }
});
// Create / Return Sprite
function newSprite() {
    let sprite = new Sprite();
    return sprite;
}
