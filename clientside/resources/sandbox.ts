var menu: Menu = null;
API.onResourceStart.connect(function () {
    menu = resource.menu_builder.createMenu(1);
    let panel: Panel;
    let inputPanel: InputPanel;
    let textElement: TextElement;

    
    
    panel = menu.createPanel(0, 12, 1, 8, 2);
    panel.MainBackgroundColor(255, 255, 255, 50);
    menu.Ready = true;


    /*let panel: Panel = createPanel(0, 12, 1, 8, 2);
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

    let inputPanel = panel.addInput(0, 12, 8, 1);
    inputPanel.Input = "test";
    inputPanel = panel.addInput(0, 13, 8, 1);
    inputPanel.Input = "test2";
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
    */
});