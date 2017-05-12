var loginUsername: InputPanel;
var loginPassword: InputPanel;

var headerBackgroundPanels = Array<Panel>();
API.onResourceStart.connect(function () {
    let menu: Menu = resource.menu_builder.createMenu(4);
    let panel: Panel;
    let inputPanel: InputPanel;
    let textElement: TextElement;
    // Left Panel / With Image
    panel = menu.createPanel(0, 1, 1, 8, 16);
    panel.MainBackgroundImage = "clientside/resources/images/backgrounds/login_left.jpg";
    panel.MainBackgroundImagePadding = 5;
    headerBackgroundPanels.push(panel);
    // Header Panel / With Login Text
    panel = menu.createPanel(0, 9, 1, 20, 2);
    textElement = panel.addText("Login");
    textElement.Font = 1;
    textElement.FontScale = 0.8
    textElement.Color(169, 176, 179, 255);
    textElement.VerticalCentered = true;
    textElement.Offset = 20;
    headerBackgroundPanels.push(panel);
    // Header Panel / Button
    panel = menu.createPanel(0, 29, 1, 2, 2);
    panel.Hoverable = true;
    panel.HoverBackgroundColor(32, 41, 63, 255);
    panel.Tooltip = "Register a new account.";
    panel.Function = menu.nextPage;
    textElement = panel.addText(">");
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    textElement.Font = 4;
    textElement.Color(169, 176, 179, 255);
    headerBackgroundPanels.push(panel);
    // Main Body Panel
    panel = menu.createPanel(0, 9, 3, 22, 12);
    panel.MainBackgroundColor(32, 41, 64, 255);
    panel = menu.createPanel(0, 13, 3, 14, 12);
    panel.MainBackgroundColor(0, 0, 0, 0); // Blank Panel
    panel.addText(``);
    panel.addText(``);
    panel.addText(``);
    textElement = panel.addText(`USERNAME`);
    textElement.Color(169, 176, 179, 255);
    panel.addText(``);
    panel.addText(``);
    panel.addText(``);
    textElement = panel.addText(`PASSWORD`);
    textElement.Color(169, 176, 179, 255);
    // Set our variables for our Input Panels to use in other functions.
    loginUsername = panel.addInput(0, 4, 14, 2);
    loginPassword = panel.addInput(0, 8, 14, 2);
    loginPassword.Protected = true;
    // Login Panel / Big Ol' Button
    panel = menu.createPanel(0, 9, 15, 22, 2);
    panel.MainBackgroundColor(32, 41, 64, 255);
    panel.Hoverable = true;
    panel.HoverBackgroundColor(3, 12, 34, 255);
    textElement = panel.addText(`Login`);
    textElement.Centered = true;
    textElement.VerticalCentered = true;
    stylizeAll();
    API.showCursor(true);
    menu.Blur = true;
    menu.DisableOverlays = true;
    menu.Ready = true;
    API.setHudVisible(false);
});

function stylizeAll() {
    for (var i = 0; i < headerBackgroundPanels.length; i++) {
        headerBackgroundPanels[i].MainBackgroundColor(3, 12, 34, 255);
    }
}

/**
 *  This is a test function, to help understand how the menu builder handles functions.
 */
function testMessage() {
    API.sendChatMessage("This is a test function.");
}
