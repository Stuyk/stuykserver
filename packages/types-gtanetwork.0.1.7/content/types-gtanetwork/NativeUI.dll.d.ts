//****************************************************************
//  (Partially) Generated by:  ToTypeScriptD
//  Website:       http://github.com/ToTypeScriptD/ToTypeScriptD
//  Version:       0.0.0.0
//  Date:          12/31/2016 7:08:00 PM
//
//  Assemblies:
//    NativeUI.dll
//
//****************************************************************



declare namespace NativeUI {

    class BarTimerBar {
        Text: string;
        Percentage: number;
        BackgroundColor: System.Drawing.Color;
        ForegroundColor: System.Drawing.Color;
        constructor(label: string);
        Draw(interval: number, offset: System.Drawing.Size): void;
    }

    class BigMessageHandler {
        constructor();
        Load(): void;
        Dispose(): void;
        ShowMissionPassedMessage(msg: string, time: number): void;
        ShowColoredShard(msg: string, desc: string, textColor: NativeUI.HudColor, bgColor: NativeUI.HudColor, time: number): void;
        ShowOldMessage(msg: string, time: number): void;
        ShowSimpleShard(title: string, subtitle: string, time: number): void;
        ShowRankupMessage(msg: string, subtitle: string, rank: number, time: number): void;
        ShowWeaponPurchasedMessage(bigMessage: string, weaponName: string, weapon: GTA.WeaponHash, time: number): void;
        ShowMpMessageLarge(msg: string, time: number): void;
        ShowCustomShard(funcName: string, ...paremeters: any[]): void;
    }

    class BigMessageThread {
        MessageInstance: NativeUI.BigMessageHandler;
        constructor();
    }

    class CheckboxChangeEvent {
        constructor(object: any, method: number);
        Invoke(sender: NativeUI.UIMenu, checkboxItem: NativeUI.UIMenuCheckboxItem, Checked: boolean): void;
        //BeginInvoke(sender: NativeUI.UIMenu, checkboxItem: NativeUI.UIMenuCheckboxItem, Checked: boolean, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        BeginInvoke(sender: NativeUI.UIMenu, checkboxItem: NativeUI.UIMenuCheckboxItem, Checked: boolean, callback: any, object: any): any;
        //EndInvoke(result: System.IAsyncResult): void;
        EndInvoke(result: any): void;
    }

    enum HudColor {
        HUD_COLOUR_PURE_WHITE = 0,
        HUD_COLOUR_WHITE = 1,
        HUD_COLOUR_BLACK = 2,
        HUD_COLOUR_GREY = 3,
        HUD_COLOUR_GREYLIGHT = 4,
        HUD_COLOUR_GREYDARK = 5,
        HUD_COLOUR_RED = 6,
        HUD_COLOUR_REDLIGHT = 7,
        HUD_COLOUR_REDDARK = 8,
        HUD_COLOUR_BLUE = 9,
        HUD_COLOUR_BLUELIGHT = 10,
        HUD_COLOUR_BLUEDARK = 11,
        HUD_COLOUR_YELLOW = 12,
        HUD_COLOUR_YELLOWLIGHT = 13,
        HUD_COLOUR_YELLOWDARK = 14,
        HUD_COLOUR_ORANGE = 15,
        HUD_COLOUR_ORANGELIGHT = 16,
        HUD_COLOUR_ORANGEDARK = 17,
        HUD_COLOUR_GREEN = 18,
        HUD_COLOUR_GREENLIGHT = 19,
        HUD_COLOUR_GREENDARK = 20,
        HUD_COLOUR_PURPLE = 21,
        HUD_COLOUR_PURPLELIGHT = 22,
        HUD_COLOUR_PURPLEDARK = 23,
        HUD_COLOUR_PINK = 24,
        HUD_COLOUR_RADAR_HEALTH = 25,
        HUD_COLOUR_RADAR_ARMOUR = 26,
        HUD_COLOUR_RADAR_DAMAGE = 27,
        HUD_COLOUR_NET_PLAYER1 = 28,
        HUD_COLOUR_NET_PLAYER2 = 29,
        HUD_COLOUR_NET_PLAYER3 = 30,
        HUD_COLOUR_NET_PLAYER4 = 31,
        HUD_COLOUR_NET_PLAYER5 = 32,
        HUD_COLOUR_NET_PLAYER6 = 33,
        HUD_COLOUR_NET_PLAYER7 = 34,
        HUD_COLOUR_NET_PLAYER8 = 35,
        HUD_COLOUR_NET_PLAYER9 = 36,
        HUD_COLOUR_NET_PLAYER10 = 37,
        HUD_COLOUR_NET_PLAYER11 = 38,
        HUD_COLOUR_NET_PLAYER12 = 39,
        HUD_COLOUR_NET_PLAYER13 = 40,
        HUD_COLOUR_NET_PLAYER14 = 41,
        HUD_COLOUR_NET_PLAYER15 = 42,
        HUD_COLOUR_NET_PLAYER16 = 43,
        HUD_COLOUR_NET_PLAYER17 = 44,
        HUD_COLOUR_NET_PLAYER18 = 45,
        HUD_COLOUR_NET_PLAYER19 = 46,
        HUD_COLOUR_NET_PLAYER20 = 47,
        HUD_COLOUR_NET_PLAYER21 = 48,
        HUD_COLOUR_NET_PLAYER22 = 49,
        HUD_COLOUR_NET_PLAYER23 = 50,
        HUD_COLOUR_NET_PLAYER24 = 51,
        HUD_COLOUR_NET_PLAYER25 = 52,
        HUD_COLOUR_NET_PLAYER26 = 53,
        HUD_COLOUR_NET_PLAYER27 = 54,
        HUD_COLOUR_NET_PLAYER28 = 55,
        HUD_COLOUR_NET_PLAYER29 = 56,
        HUD_COLOUR_NET_PLAYER30 = 57,
        HUD_COLOUR_NET_PLAYER31 = 58,
        HUD_COLOUR_NET_PLAYER32 = 59,
        HUD_COLOUR_SIMPLEBLIP_DEFAULT = 60,
        HUD_COLOUR_MENU_BLUE = 61,
        HUD_COLOUR_MENU_GREY_LIGHT = 62,
        HUD_COLOUR_MENU_BLUE_EXTRA_DARK = 63,
        HUD_COLOUR_MENU_YELLOW = 64,
        HUD_COLOUR_MENU_YELLOW_DARK = 65,
        HUD_COLOUR_MENU_GREEN = 66,
        HUD_COLOUR_MENU_GREY = 67,
        HUD_COLOUR_MENU_GREY_DARK = 68,
        HUD_COLOUR_MENU_HIGHLIGHT = 69,
        HUD_COLOUR_MENU_STANDARD = 70,
        HUD_COLOUR_MENU_DIMMED = 71,
        HUD_COLOUR_MENU_EXTRA_DIMMED = 72,
        HUD_COLOUR_BRIEF_TITLE = 73,
        HUD_COLOUR_MID_GREY_MP = 74,
        HUD_COLOUR_NET_PLAYER1_DARK = 75,
        HUD_COLOUR_NET_PLAYER2_DARK = 76,
        HUD_COLOUR_NET_PLAYER3_DARK = 77,
        HUD_COLOUR_NET_PLAYER4_DARK = 78,
        HUD_COLOUR_NET_PLAYER5_DARK = 79,
        HUD_COLOUR_NET_PLAYER6_DARK = 80,
        HUD_COLOUR_NET_PLAYER7_DARK = 81,
        HUD_COLOUR_NET_PLAYER8_DARK = 82,
        HUD_COLOUR_NET_PLAYER9_DARK = 83,
        HUD_COLOUR_NET_PLAYER10_DARK = 84,
        HUD_COLOUR_NET_PLAYER11_DARK = 85,
        HUD_COLOUR_NET_PLAYER12_DARK = 86,
        HUD_COLOUR_NET_PLAYER13_DARK = 87,
        HUD_COLOUR_NET_PLAYER14_DARK = 88,
        HUD_COLOUR_NET_PLAYER15_DARK = 89,
        HUD_COLOUR_NET_PLAYER16_DARK = 90,
        HUD_COLOUR_NET_PLAYER17_DARK = 91,
        HUD_COLOUR_NET_PLAYER18_DARK = 92,
        HUD_COLOUR_NET_PLAYER19_DARK = 93,
        HUD_COLOUR_NET_PLAYER20_DARK = 94,
        HUD_COLOUR_NET_PLAYER21_DARK = 95,
        HUD_COLOUR_NET_PLAYER22_DARK = 96,
        HUD_COLOUR_NET_PLAYER23_DARK = 97,
        HUD_COLOUR_NET_PLAYER24_DARK = 98,
        HUD_COLOUR_NET_PLAYER25_DARK = 99,
        HUD_COLOUR_NET_PLAYER26_DARK = 100,
        HUD_COLOUR_NET_PLAYER27_DARK = 101,
        HUD_COLOUR_NET_PLAYER28_DARK = 102,
        HUD_COLOUR_NET_PLAYER29_DARK = 103,
        HUD_COLOUR_NET_PLAYER30_DARK = 104,
        HUD_COLOUR_NET_PLAYER31_DARK = 105,
        HUD_COLOUR_NET_PLAYER32_DARK = 106,
        HUD_COLOUR_BRONZE = 107,
        HUD_COLOUR_SILVER = 108,
        HUD_COLOUR_GOLD = 109,
        HUD_COLOUR_PLATINUM = 110,
        HUD_COLOUR_GANG1 = 111,
        HUD_COLOUR_GANG2 = 112,
        HUD_COLOUR_GANG3 = 113,
        HUD_COLOUR_GANG4 = 114,
        HUD_COLOUR_SAME_CREW = 115,
        HUD_COLOUR_FREEMODE = 116,
        HUD_COLOUR_PAUSE_BG = 117,
        HUD_COLOUR_FRIENDLY = 118,
        HUD_COLOUR_ENEMY = 119,
        HUD_COLOUR_LOCATION = 120,
        HUD_COLOUR_PICKUP = 121,
        HUD_COLOUR_PAUSE_SINGLEPLAYER = 122,
        HUD_COLOUR_FREEMODE_DARK = 123,
        HUD_COLOUR_INACTIVE_MISSION = 124,
        HUD_COLOUR_DAMAGE = 125,
        HUD_COLOUR_PINKLIGHT = 126,
        HUD_COLOUR_PM_MITEM_HIGHLIGHT = 127,
        HUD_COLOUR_SCRIPT_VARIABLE = 128,
        HUD_COLOUR_YOGA = 129,
        HUD_COLOUR_TENNIS = 130,
        HUD_COLOUR_GOLF = 131,
        HUD_COLOUR_SHOOTING_RANGE = 132,
        HUD_COLOUR_FLIGHT_SCHOOL = 133,
        HUD_COLOUR_NORTH_BLUE = 134,
        HUD_COLOUR_SOCIAL_CLUB = 135,
        HUD_COLOUR_PLATFORM_BLUE = 136,
        HUD_COLOUR_PLATFORM_GREEN = 137,
        HUD_COLOUR_PLATFORM_GREY = 138,
        HUD_COLOUR_FACEBOOK_BLUE = 139,
        HUD_COLOUR_INGAME_BG = 140,
        HUD_COLOUR_DARTS = 141,
        HUD_COLOUR_WAYPOINT = 142,
        HUD_COLOUR_MICHAEL = 143,
        HUD_COLOUR_FRANKLIN = 144,
        HUD_COLOUR_TREVOR = 145,
        HUD_COLOUR_GOLF_P1 = 146,
        HUD_COLOUR_GOLF_P2 = 147,
        HUD_COLOUR_GOLF_P3 = 148,
        HUD_COLOUR_GOLF_P4 = 149,
        HUD_COLOUR_WAYPOINTLIGHT = 150,
        HUD_COLOUR_WAYPOINTDARK = 151,
        HUD_COLOUR_PANEL_LIGHT = 152,
        HUD_COLOUR_MICHAEL_DARK = 153,
        HUD_COLOUR_FRANKLIN_DARK = 154,
        HUD_COLOUR_TREVOR_DARK = 155,
        HUD_COLOUR_OBJECTIVE_ROUTE = 156,
        HUD_COLOUR_PAUSEMAP_TINT = 157,
        HUD_COLOUR_PAUSE_DESELECT = 158,
        HUD_COLOUR_PM_WEAPONS_PURCHASABLE = 159,
        HUD_COLOUR_PM_WEAPONS_LOCKED = 160,
        HUD_COLOUR_END_SCREEN_BG = 161,
        HUD_COLOUR_CHOP = 162,
        HUD_COLOUR_PAUSEMAP_TINT_HALF = 163,
        HUD_COLOUR_NORTH_BLUE_OFFICIAL = 164,
        HUD_COLOUR_SCRIPT_VARIABLE_2 = 165,
        HUD_COLOUR_H = 166,
        HUD_COLOUR_HDARK = 167,
        HUD_COLOUR_T = 168,
        HUD_COLOUR_TDARK = 169,
        HUD_COLOUR_HSHARD = 170,
        HUD_COLOUR_CONTROLLER_MICHAEL = 171,
        HUD_COLOUR_CONTROLLER_FRANKLIN = 172,
        HUD_COLOUR_CONTROLLER_TREVOR = 173,
        HUD_COLOUR_CONTROLLER_CHOP = 174,
        HUD_COLOUR_VIDEO_EDITOR_VIDEO = 175,
        HUD_COLOUR_VIDEO_EDITOR_AUDIO = 176,
        HUD_COLOUR_VIDEO_EDITOR_TEXT = 177,
        HUD_COLOUR_HB_BLUE = 178,
        HUD_COLOUR_HB_YELLOW = 179
    }

    class IndexChangedEvent {
        constructor(object: any, method: number);
        Invoke(sender: NativeUI.UIMenu, newIndex: number): void;
        BeginInvoke(sender: NativeUI.UIMenu, newIndex: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        EndInvoke(result: System.IAsyncResult): void;
    }

    class InstructionalButton {
        Text: string;
        ItemBind: NativeUI.UIMenuItem;
        constructor(control: GTA.Control, text: string);
        constructor(keystring: string, text: string);
        BindToItem(item: NativeUI.UIMenuItem): void;
        GetButtonId(): string;
    }

    class ItemActivatedEvent {
        constructor(object: any, method: number);
        Invoke(sender: NativeUI.UIMenu, selectedItem: NativeUI.UIMenuItem): void;
        BeginInvoke(sender: NativeUI.UIMenu, selectedItem: NativeUI.UIMenuItem, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        EndInvoke(result: System.IAsyncResult): void;
    }

    class ItemCheckboxEvent {
        constructor(object: any, method: number);
        Invoke(sender: NativeUI.UIMenuCheckboxItem, Checked: boolean): void;
        BeginInvoke(sender: NativeUI.UIMenuCheckboxItem, Checked: boolean, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        EndInvoke(result: System.IAsyncResult): void;
    }

    class ItemListEvent {
        constructor(object: any, method: number);
        Invoke(sender: NativeUI.UIMenuListItem, newIndex: number): void;
        BeginInvoke(sender: NativeUI.UIMenuListItem, newIndex: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        EndInvoke(result: System.IAsyncResult): void;
    }

    class ItemSelectEvent {
        constructor(object: any, method: number);
        Invoke(sender: NativeUI.UIMenu, selectedItem: NativeUI.UIMenuItem, index: number): void;
        BeginInvoke(sender: NativeUI.UIMenu, selectedItem: NativeUI.UIMenuItem, index: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        EndInvoke(result: System.IAsyncResult): void;
    }

    class ListChangedEvent {
        constructor(object: any, method: number);
        Invoke(sender: NativeUI.UIMenu, listItem: NativeUI.UIMenuListItem, newIndex: number): void;
        BeginInvoke(sender: NativeUI.UIMenu, listItem: NativeUI.UIMenuListItem, newIndex: number, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        EndInvoke(result: System.IAsyncResult): void;
    }

    class MenuChangeEvent {
        constructor(object: any, method: number);
        Invoke(oldMenu: NativeUI.UIMenu, newMenu: NativeUI.UIMenu, forward: boolean): void;
        BeginInvoke(oldMenu: NativeUI.UIMenu, newMenu: NativeUI.UIMenu, forward: boolean, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        EndInvoke(result: System.IAsyncResult): void;
    }

    class MenuCloseEvent {
        constructor(object: any, method: number);
        Invoke(sender: NativeUI.UIMenu): void;
        BeginInvoke(sender: NativeUI.UIMenu, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        EndInvoke(result: System.IAsyncResult): void;
    }

    class MenuPool {
        MouseEdgeEnabled: boolean;
        ControlDisablingEnabled: boolean;
        ResetCursorOnOpen: boolean;
        FormatDescriptions: boolean;
        AUDIO_LIBRARY: string;
        AUDIO_UPDOWN: string;
        AUDIO_SELECT: string;
        AUDIO_BACK: string;
        AUDIO_ERROR: string;
        WidthOffset: number;
        CounterPretext: string;
        DisableInstructionalButtons: boolean;
        constructor();
        Add(menu: NativeUI.UIMenu): void;
        AddSubMenu(menu: NativeUI.UIMenu, text: string): NativeUI.UIMenu;
        AddSubMenu(menu: NativeUI.UIMenu, text: string, description: string): NativeUI.UIMenu;
        RefreshIndex(): void;
        ToList(): NativeUI.UIMenu[];
        ProcessControl(): void;
        ProcessKey(key: System.Windows.Forms.Keys): void;
        ProcessMouse(): void;
        Draw(): void;
        IsAnyMenuOpen(): boolean;
        ProcessMenus(): void;
        CloseAllMenus(): void;
        SetBannerType(bannerType: NativeUI.Sprite): void;
        SetBannerType(bannerType: NativeUI.UIResRectangle): void;
        SetBannerType(bannerPath: string): void;
        SetKey(menuControl: NativeUI.UIMenu_MenuControls, control: GTA.Control): void;
        SetKey(menuControl: NativeUI.UIMenu_MenuControls, control: GTA.Control, controllerIndex: number): void;
        SetKey(menuControl: NativeUI.UIMenu_MenuControls, control: System.Windows.Forms.Keys): void;
        ResetKey(menuControl: NativeUI.UIMenu_MenuControls): void;
    }

    class MiscExtensions {
        //SharedRandom: System.Random;
        SharedRandom: any;
        static AddPoints(left: System.Drawing.Point, right: System.Drawing.Point): System.Drawing.Point;
        static SubtractPoints(left: System.Drawing.Point, right: System.Drawing.Point): System.Drawing.Point;
        static Clamp(val: number, min: number, max: number): number;
        static LinearVectorLerp(start: GTA.Math.Vector3, end: GTA.Math.Vector3, currentTime: number, duration: number): GTA.Math.Vector3;
        static VectorLerp(start: GTA.Math.Vector3, end: GTA.Math.Vector3, currentTime: number, duration: number, easingFunc: number): GTA.Math.Vector3;
        static LinearFloatLerp(start: number, end: number, currentTime: number, duration: number): number;
        static QuadraticEasingLerp(start: number, end: number, currentTime: number, duration: number): number;
    }

    class Sprite {
        Position: System.Drawing.Point;
        Size: System.Drawing.Size;
        Color: System.Drawing.Color;
        Visible: boolean;
        Heading: number;
        TextureName: string;
        TextureDict: string;
        constructor(textureDict: string, textureName: string, position: System.Drawing.Point, size: System.Drawing.Size, heading: number, color: System.Drawing.Color);
        constructor(textureDict: string, textureName: string, position: System.Drawing.Point, size: System.Drawing.Size);
        Draw(): void;
        static DrawTexture(path: string, position: System.Drawing.Point, size: System.Drawing.Size, rotation: number, color: System.Drawing.Color): void;
        static DrawTexture(path: string, position: System.Drawing.Point, size: System.Drawing.Size): void;
        //static WriteFileFromResources(yourAssembly: System.Reflection.Assembly, fullResourceName: string): string;
        static WriteFileFromResources(yourAssembly: any, fullResourceName: string): string;
        //static WriteFileFromResources(yourAssembly: System.Reflection.Assembly, fullResourceName: string, savePath: string): string;
        static WriteFileFromResources(yourAssembly: any, fullResourceName: string, savePath: string): string;
    }

    class StringMeasurer {
        static MeasureString(input: string): number;
    }

    class TextTimerBar {
        Text: string;
        constructor(label: string, text: string);
        Draw(interval: number, offset: System.Drawing.Size): void;
    }

    class TimerBarBase {
        Label: string;
        constructor(label: string);
        Draw(interval: number, offset: System.Drawing.Size): void;
    }

    class TimerBarPool {
        Offset: System.Drawing.Size;
        constructor();
        ToList(): NativeUI.TimerBarBase[];
        Add(timer: NativeUI.TimerBarBase): void;
        Remove(timer: NativeUI.TimerBarBase): void;
        Draw(): void;
    }

    class UIMenu {
        AUDIO_LIBRARY: string;
        AUDIO_UPDOWN: string;
        AUDIO_LEFTRIGHT: string;
        AUDIO_SELECT: string;
        AUDIO_BACK: string;
        AUDIO_ERROR: string;
        MenuItems: NativeUI.UIMenuItem[];
        MouseEdgeEnabled: boolean;
        ControlDisablingEnabled: boolean;
        ResetCursorOnOpen: boolean;
        FormatDescriptions: boolean;
        MouseControlsEnabled: boolean;
        ScaleWithSafezone: boolean;
        Children: NativeUI.UIMenuItem[];
        WidthOffset: number;
        Visible: boolean;
        CurrentSelection: number;
        IsUsingController: boolean;
        Size: number;
        Title: NativeUI.UIResText;
        Subtitle: NativeUI.UIResText;
        CounterPretext: string;
        ParentMenu: NativeUI.UIMenu;
        ParentItem: NativeUI.UIMenuItem;
        constructor(title: string, subtitle: string);
        constructor(title: string, subtitle: string, offset: System.Drawing.Point);
        constructor(title: string, subtitle: string, offset: System.Drawing.Point, customBanner: string);
        constructor(title: string, subtitle: string, offset: System.Drawing.Point, spriteLibrary: string, spriteName: string);
        SetMenuWidthOffset(widthOffset: number): void;
        static DisEnableControls(enable: boolean): void;
        DisableInstructionalButtons(disable: boolean): void;
        SetBannerType(spriteBanner: NativeUI.Sprite): void;
        SetBannerType(rectangle: NativeUI.UIResRectangle): void;
        SetBannerType(pathToCustomSprite: string): void;
        AddItem(item: NativeUI.UIMenuItem): void;
        InsertItem(item: NativeUI.UIMenuItem, position: number): void;
        RemoveItemAt(index: number): void;
        RefreshIndex(): void;
        Clear(): void;
        Draw(): void;
        static GetScreenResolutionMantainRatio(): System.Drawing.SizeF;
        static IsMouseInBounds(topLeft: System.Drawing.Point, boxSize: System.Drawing.Size): boolean;
        IsMouseInListItemArrows(item: NativeUI.UIMenuListItem, topLeft: System.Drawing.Point, safezone: System.Drawing.Point): number;
        static GetSafezoneBounds(): System.Drawing.Point;
        GoUpOverflow(): void;
        GoUp(): void;
        GoDownOverflow(): void;
        GoDown(): void;
        GoLeft(): void;
        GoRight(): void;
        SelectItem(): void;
        GoBack(): void;
        BindMenuToItem(menuToBind: NativeUI.UIMenu, itemToBindTo: NativeUI.UIMenuItem): void;
        ReleaseMenuFromItem(releaseFrom: NativeUI.UIMenuItem): boolean;
        ProcessMouse(): void;
        SetKey(control: NativeUI.UIMenu_MenuControls, keyToSet: System.Windows.Forms.Keys): void;
        SetKey(control: NativeUI.UIMenu_MenuControls, gtaControl: GTA.Control): void;
        SetKey(control: NativeUI.UIMenu_MenuControls, gtaControl: GTA.Control, controlIndex: number): void;
        ResetKey(control: NativeUI.UIMenu_MenuControls): void;
        HasControlJustBeenPressed(control: NativeUI.UIMenu_MenuControls, key: System.Windows.Forms.Keys): boolean;
        HasControlJustBeenReleased(control: NativeUI.UIMenu_MenuControls, key: System.Windows.Forms.Keys): boolean;
        IsControlBeingPressed(control: NativeUI.UIMenu_MenuControls, key: System.Windows.Forms.Keys): boolean;
        ProcessControl(key: System.Windows.Forms.Keys): void;
        ProcessKey(key: System.Windows.Forms.Keys): void;
        AddInstructionalButton(button: NativeUI.InstructionalButton): void;
        RemoveInstructionalButton(button: NativeUI.InstructionalButton): void;
        UpdateScaleform(): void;
        OnIndexChange: IEvent<(sender: NativeUI.UIMenu, newIndex: number) => void>;
        OnListChange: IEvent<(sender: NativeUI.UIMenu, listItem: NativeUI.UIMenuListItem, newIndex: number) => void>;
        OnCheckboxChange: IEvent<(sender: NativeUI.UIMenu, checkboxItem: NativeUI.UIMenuCheckboxItem, Checked: boolean) => void>;
        OnItemSelect: IEvent<(sender: NativeUI.UIMenu, selectedItem: NativeUI.UIMenuItem, index: number) => void>;
        OnMenuClose: IEvent<(sender: NativeUI.UIMenu) => void>;
        OnMenuChange: IEvent<(oldMenu: NativeUI.UIMenu, newMenu: NativeUI.UIMenu, forward: boolean) => void>;
    }

    enum UIMenu_MenuControls {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        Select = 4,
        Back = 5
    }

    class UIMenuItem {
        Selected: boolean;
        Hovered: boolean;
        Description: string;
        Enabled: boolean;
        Offset: System.Drawing.Point;
        Text: string;
        RightLabel: string;
        LeftBadge: NativeUI.UIMenuItem_BadgeStyle;
        RightBadge: NativeUI.UIMenuItem_BadgeStyle;
        Parent: NativeUI.UIMenu;
        constructor(text: string);
        constructor(text: string, description: string);
        Position(y: number): void;
        ProcessControl(control: NativeUI.UIMenu_MenuControls): boolean;
        Draw(): void;
        SetLeftBadge(badge: NativeUI.UIMenuItem_BadgeStyle): void;
        SetRightBadge(badge: NativeUI.UIMenuItem_BadgeStyle): void;
        SetRightLabel(text: string): void;
        Activated: IEvent<(sender: NativeUI.UIMenu, selectedItem: NativeUI.UIMenuItem) => void>;
    }

    class UIMenuCheckboxItem extends UIMenuItem {
        Checked: boolean;
        constructor(text: string, check: boolean);
        constructor(text: string, check: boolean, description: string);
        Position(y: number): void;
        ProcessControl(control: NativeUI.UIMenu_MenuControls): boolean;
        Draw(): void;
        CheckboxEventTrigger(): void;
        SetRightBadge(badge: NativeUI.UIMenuItem_BadgeStyle): void;
        SetRightLabel(text: string): void;
        CheckboxEvent: IEvent<(sender: NativeUI.UIMenuCheckboxItem, Checked: boolean) => void>;
    }

    class UIMenuColoredItem extends UIMenuItem {
        MainColor: System.Drawing.Color;
        HighlightColor: System.Drawing.Color;
        TextColor: System.Drawing.Color;
        HighlightedTextColor: System.Drawing.Color;
        constructor(label: string, color: System.Drawing.Color, highlightColor: System.Drawing.Color);
        constructor(label: string, description: string, color: System.Drawing.Color, highlightColor: System.Drawing.Color);
        Draw(): void;
    }

    enum UIMenuItem_BadgeStyle {
        None = 0,
        BronzeMedal = 1,
        GoldMedal = 2,
        SilverMedal = 3,
        Alert = 4,
        Crown = 5,
        Ammo = 6,
        Armour = 7,
        Barber = 8,
        Clothes = 9,
        Franklin = 10,
        Bike = 11,
        Car = 12,
        Gun = 13,
        Heart = 14,
        Makeup = 15,
        Mask = 16,
        Michael = 17,
        Star = 18,
        Tatoo = 19,
        Trevor = 20,
        Lock = 21,
        Tick = 22
    }

    class UIMenuListItem extends UIMenuItem {
        Index: number;
        constructor(text: string, items: any, index: number);
        constructor(text: string, items: any, index: number, description: string);
        Position(y: number): void;
        ItemToIndex(item: any): number;
        IndexToItem(index: number): any;
        ProcessControl(control: NativeUI.UIMenu_MenuControls): boolean;
        Draw(): void;
        SetRightBadge(badge: NativeUI.UIMenuItem_BadgeStyle): void;
        SetRightLabel(text: string): void;
        OnListChanged: IEvent<(sender: NativeUI.UIMenuListItem, newIndex: number) => void>;
    }

    class UIResRectangle extends UIMenuItem {
        constructor();
        constructor(pos: System.Drawing.Point, size: System.Drawing.Size);
        constructor(pos: System.Drawing.Point, size: System.Drawing.Size, color: System.Drawing.Color);
        Draw(): void;
        Draw(offset: System.Drawing.SizeF): void;
    }

    class UIResText {
        TextAlignment: NativeUI.UIResText_Alignment;
        DropShadow: boolean;
        Outline: boolean;
        WordWrap: System.Drawing.Size;
        constructor(caption: string, position: System.Drawing.Point, scale: number);
        constructor(caption: string, position: System.Drawing.Point, scale: number, color: System.Drawing.Color);
        constructor(caption: string, position: System.Drawing.Point, scale: number, color: System.Drawing.Color, font: GTA.UI.Font, justify: NativeUI.UIResText_Alignment);
        static AddLongString(str: string): void;
        static MeasureStringWidth(str: string, font: GTA.UI.Font, scale: number): number;
        static MeasureStringWidthNoConvert(str: string, font: GTA.UI.Font, scale: number): number;
        Draw(offset: System.Drawing.SizeF): void;
    }

    enum UIResText_Alignment {
        Left = 0,
        Centered = 1,
        Right = 2
    }

}
declare namespace NativeUI.PauseMenu {

    class MissionInformation {
        Name: string;
        Description: string;
        Logo: NativeUI.PauseMenu.MissionLogo;
        //ValueList: System.Tuple<string,string>[];
        ValueList: any;
        //constructor(name: string, info: System.Tuple<string,string>);
        constructor(name: string, info: any);
        //constructor(name: string, description: string, info: System.Tuple<string,string>);
        constructor(name: string, description: string, info: any);
    }

    class MissionLogo {
        FileName: string;
        DictionaryName: string;
        constructor(filepath: string);
        constructor(textureDict: string, textureName: string);
    }

    class OnItemSelect {
        constructor(object: any, method: number);
        Invoke(selectedItem: NativeUI.PauseMenu.MissionInformation): void;
        BeginInvoke(selectedItem: NativeUI.PauseMenu.MissionInformation, callback: System.AsyncCallback, object: any): System.IAsyncResult;
        EndInvoke(result: System.IAsyncResult): void;
    }

    class TabInteractiveListItem {
        Items: NativeUI.UIMenuItem[];
        Index: number;
        IsInList: boolean;
        constructor(name: string, items: NativeUI.UIMenuItem);
        MoveDown(): void;
        MoveUp(): void;
        RefreshIndex(): void;
        ProcessControls(): void;
        Draw(): void;
    }

    class TabItem {
        DrawBg: boolean;
        Visible: boolean;
        Focused: boolean;
        Title: string;
        Active: boolean;
        JustOpened: boolean;
        CanBeFocused: boolean;
        TopLeft: System.Drawing.Point;
        BottomRight: System.Drawing.Point;
        SafeSize: System.Drawing.Point;
        UseDynamicPositionment: boolean;
        Parent: NativeUI.PauseMenu.TabView;
        FadeInWhenFocused: boolean;
        constructor(name: string);
        OnActivated(): void;
        ProcessControls(): void;
        Draw(): void;
        Activated: IEvent<(sender: any, e: System.EventArgs) => void>;
        DrawInstructionalButtons: IEvent<(sender: any, e: System.EventArgs) => void>;
    }

    class TabItemSimpleList {
        Dictionary: string[];
        constructor(title: string, dict: string);
        Draw(): void;
    }

    class TabMissionSelectItem {
        Heists: NativeUI.PauseMenu.MissionInformation[];
        Index: number;
        _noLogo: NativeUI.Sprite;
        constructor(name: string, list: NativeUI.PauseMenu.MissionInformation);
        ProcessControls(): void;
        Draw(): void;
        OnItemSelect: IEvent<(selectedItem: NativeUI.PauseMenu.MissionInformation) => void>;
    }

    class TabSubmenuItem {
        Items: NativeUI.PauseMenu.TabItem[];
        Index: number;
        IsInList: boolean;
        Focused: boolean;
        constructor(name: string, items: NativeUI.PauseMenu.TabItem);
        RefreshIndex(): void;
        ProcessControls(): void;
        Draw(): void;
    }

    class TabTextItem {
        TextTitle: string;
        Text: string;
        WordWrap: number;
        constructor(name: string, title: string);
        constructor(name: string, title: string, text: string);
        Draw(): void;
    }

    class TabView {
        Index: number;
        Title: string;
        Photo: NativeUI.Sprite;
        Name: string;
        Money: string;
        MoneySubtitle: string;
        Tabs: NativeUI.PauseMenu.TabItem[];
        FocusLevel: number;
        TemporarilyHidden: boolean;
        CanLeave: boolean;
        HideTabs: boolean;
        Visible: boolean;
        constructor(title: string);
        AddTab(item: NativeUI.PauseMenu.TabItem): void;
        ShowInstructionalButtons(): void;
        DrawInstructionalButton(slot: number, control: GTA.Control, text: string): void;
        ProcessControls(): void;
        RefreshIndex(): void;
        Update(): void;
        OnMenuClose: IEvent<(sender: any, e: System.EventArgs) => void>;
    }

}

